using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Resources;
using Gaming.Models;

namespace Gaming.Services;

/// <summary>
/// Azure VM service
/// </summary>
public class AzureVmService
{
    private const string Location = "France Central";
    private const string ResourceGroupName = "rg-gaming-001";
    private string _vmName = "vm-";
    private string _ipName = "ip-";
    private string _nicName = "nic-";
    private const string VnetName = "vnet-gaming-001";
    private ResourceGroupResource _resourceGroup;

    /// <summary>
    /// Init a resource group
    /// </summary>
    /// <param name="subscription"></param>
    private async Task InitResourceGroup(SubscriptionResource subscription)
    {
        bool isExist = await subscription.GetResourceGroups().ExistsAsync(ResourceGroupName);
        if (!isExist)
        {
            await Console.Out.WriteLineAsync("Creating resource group...");
            _resourceGroup = (await subscription.GetResourceGroups()
                .CreateOrUpdateAsync(
                    WaitUntil.Completed,
                    ResourceGroupName, 
                    new ResourceGroupData(Location), 
                    cancellationToken: default
                )).Value;
        }
    }
    
    /// <summary>
    /// Get a IP address
    /// </summary>
    /// <param name="resourceGroup"></param>
    /// <returns></returns>
    private string GetIpAdress(ResourceGroupResource resourceGroup)
    {
        var publicIps = resourceGroup.GetPublicIPAddresses();
        var ipResource = publicIps.Get(_ipName);
        return ipResource.Value.Data.IPAddress;
    }
    
    /// <summary>
    /// Init a public ip
    /// </summary>
    /// <param name="resourceGroup"></param>
    /// <returns></returns>
    private PublicIPAddressResource InitIp(ResourceGroupResource resourceGroup)
    {
        var publicIps = resourceGroup.GetPublicIPAddresses();
        var ipResource = publicIps.
            CreateOrUpdate(
            WaitUntil.Completed,
            _ipName,
            new PublicIPAddressData()
            {
                PublicIPAddressVersion = NetworkIPVersion.IPv4,
                PublicIPAllocationMethod = NetworkIPAllocationMethod.Dynamic,
                Location = Location
            }).Value;

        return ipResource;
    }
    
    /// <summary>
    /// Init a virtual network
    /// </summary>
    /// <param name="resourceGroupResource"></param>
    /// <returns></returns>
    private static async Task<VirtualNetworkResource> InitVnet(ResourceGroupResource resourceGroupResource)
    {
        var vns = resourceGroupResource.GetVirtualNetworks();
        var vnetResource = (await vns.CreateOrUpdateAsync(
            WaitUntil.Completed,
            VnetName,
            new VirtualNetworkData
            {
                Location = Location,
                Subnets =
                {
                    new SubnetData()
                    {
                        Name = "SubNet",
                        AddressPrefix = "10.0.0.0/24"
                    }
                },
                AddressPrefixes =
                {
                    "10.0.0.0/16"
                }
            })).Value;
        
        return vnetResource;
    }
    
    /// <summary>
    /// Init a network interface
    /// </summary>
    /// <param name="resourceGroupResource"></param>
    /// <returns></returns>
    private NetworkInterfaceResource InitNetwork(ResourceGroupResource resourceGroupResource)
    {
        var nics = resourceGroupResource.GetNetworkInterfaces();
        var nicResource = nics.CreateOrUpdate(
            WaitUntil.Completed,
            _nicName,
            new NetworkInterfaceData()
            {
                Location = Location,
                IPConfigurations =
                {
                    new NetworkInterfaceIPConfigurationData()
                    {
                        Name = "Primary",
                        Primary = true,
                        Subnet = new SubnetData { Id = InitVnet(resourceGroupResource).Result.Data.Subnets.First().Id },
                        PrivateIPAllocationMethod = NetworkIPAllocationMethod.Dynamic,
                        PublicIPAddress = new PublicIPAddressData { Id = InitIp(resourceGroupResource).Data.Id }
                    }
                }
            }).Value;

        return nicResource;
    }
    
    /// <summary>
    /// Create a new Azure VM
    /// </summary>
    /// <param name="customVm"></param>
    public async Task CreateAzureVm(CustomVm customVm)
    {
        _vmName += customVm.Login;
        _ipName += customVm.Login;
        _nicName += customVm.Login;
        // First we construct our armClient
        var armClient = new ArmClient(new DefaultAzureCredential());

        // Next we get a resource group object
        // ResourceGroup is a {ResourceName}Resource object from above
        var subscription = await armClient.GetDefaultSubscriptionAsync();
        
        await InitResourceGroup(subscription);

        var resourceGroups = subscription.GetResourceGroups();
        ResourceGroupResource resourceGroup = await resourceGroups.GetAsync(ResourceGroupName);

        await resourceGroup.GetVirtualMachines()
            .CreateOrUpdateAsync(
                WaitUntil.Completed,
                _vmName,
                new VirtualMachineData(Location)
                {
                    HardwareProfile = new VirtualMachineHardwareProfile()
                    {
                        VmSize = VirtualMachineSizeType.StandardD2V3
                    },
                    OSProfile = new VirtualMachineOSProfile()
                    {
                        ComputerName = "Lulu",
                        AdminUsername = customVm.Login,
                        AdminPassword = customVm.Password,
                        LinuxConfiguration = new LinuxConfiguration()
                        {
                            DisablePasswordAuthentication = false,
                            ProvisionVmAgent = true
                        }
                    },
                    StorageProfile = new VirtualMachineStorageProfile()
                    {
                        OSDisk = new VirtualMachineOSDisk(DiskCreateOptionType.FromImage),
                        ImageReference = new ImageReference()
                        {
                            Offer = "UbuntuServer",
                            Publisher = "Canonical",
                            Sku = "18.04-LTS",
                            Version = "latest"
                        }
                    },
                    NetworkProfile = new VirtualMachineNetworkProfile()
                    {
                        NetworkInterfaces =
                        {
                            new VirtualMachineNetworkInterfaceReference()
                            {
                                Id = InitNetwork(resourceGroup).Id
                            }
                        }
                    },
                }
            );
        
        customVm.Name = _vmName;
        customVm.IsStarted = true;
        customVm.Ip = GetIpAdress(resourceGroup);
    }

    /// <summary>
    /// Retrieve resource group
    /// </summary>
    private void RetrieveResourceGroup()
    {
        var armClient = new ArmClient(new DefaultAzureCredential());

        var subscription = armClient.GetDefaultSubscriptionAsync().Result;
        
        var resourceGroups = subscription.GetResourceGroups();
        _resourceGroup = resourceGroups.GetAsync(ResourceGroupName).Result;
    }
    
    /// <summary>
    /// Start Azure VM
    /// </summary>
    /// <param name="name"></param>
    public void StartAzureVm(string name)
    {
        RetrieveResourceGroup();
        var virtualMachines = _resourceGroup.GetVirtualMachines();
        var virtualMachine = virtualMachines.GetAsync(name).Result.Value;
        virtualMachine.PowerOn(WaitUntil.Completed);
    }
    
    /// <summary>
    /// Stop Azure VM
    /// </summary>
    /// <param name="name"></param>
    public void StopAzureVm(string name)
    {
        RetrieveResourceGroup();
        var virtualMachines = _resourceGroup.GetVirtualMachines();
        var virtualMachine = virtualMachines.GetAsync(name).Result.Value;
        virtualMachine.PowerOff(WaitUntil.Completed);
    }
    
    /// <summary>
    /// Delete Azure VM
    /// </summary>
    /// <param name="login"></param>
    public async Task DeleteAzureVm(string login)
    {
        // Retrieve
        RetrieveResourceGroup();
        
        // Get
        VirtualMachineResource vm = await _resourceGroup.GetVirtualMachines().GetAsync(_vmName + login);
        NetworkInterfaceResource nic = await _resourceGroup.GetNetworkInterfaces().GetAsync(_nicName + login);
        PublicIPAddressResource publicIp = await _resourceGroup.GetPublicIPAddresses().GetAsync(_ipName + login);
        VirtualNetworkResource vn = await _resourceGroup.GetVirtualNetworks().GetAsync(VnetName);
        var diskName = (await vm.InstanceViewAsync(CancellationToken.None)).Value.Disks.ToString();
        //DiskAccessResource diskAccess = await _resourceGroup.GetDiskAccesses().GetAsync(diskName);

        // Delete
        await vm.DeleteAsync(WaitUntil.Completed);
        //await diskAccess.DeleteAsync(WaitUntil.Completed);
        await nic.DeleteAsync(WaitUntil.Completed);
        await vn.DeleteAsync(WaitUntil.Completed);
        await publicIp.DeleteAsync(WaitUntil.Completed);
    }
}
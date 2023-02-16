using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Resources;
using Gaming.Models;
using Gaming.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gaming.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private const string Location = "France Central";
    private const string ResourceGroupName = "rg-gaming-vm-001";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
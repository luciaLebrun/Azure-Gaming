#! main.tf
# DÃ©finition des ressources Azure
terraform {
  required_version = "1.3.8"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.43.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~>3.0"
    }
    tls = {
      source  = "hashicorp/tls"
      version = "~>4.0"
    }
  }
}

provider "azurerm" {
  features {
    # azurerm features
  }
  subscription_id = "927cea4c-145c-49f0-b5bc-f71022884d33"
  tenant_id       = "b7b023b8-7c32-4c02-92a6-c8cdaa1d189c"
}

resource "azurerm_resource_group" "rg" {
  name     = var.rg_name
  location = var.rg_location
}

resource "azurerm_service_plan" "service_plan" {
  name                = var.asp_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = var.asp_sku_name
}

resource "azurerm_linux_web_app" "web_app" {
  name                = var.app_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.service_plan.id

  site_config {
    application_stack {
      dotnet_version = var.dotnet_version
    }
  }
}

resource "azurerm_mssql_server" "db_server" {
  name                         = var.db_server_name
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.db_login
  administrator_login_password = var.db_password

  tags = {
    environment = "production"
  }
}

resource "azurerm_storage_account" "db_account" {
  name                     = var.db_account_name
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_mssql_database" "db_mssql" {
  name           = var.db_name
  server_id      = azurerm_mssql_server.db_server.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 1
  read_scale     = false
  sku_name       = "S0"
  zone_redundant = false
}

# The Azure feature Allow access to Azure services can be enabled by setting start_ip_address and end_ip_address to 0.0.0.0
resource "azurerm_sql_firewall_rule" "sql_firewall" {
  name                = "FirewallRule1"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_mssql_server.db_server.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}


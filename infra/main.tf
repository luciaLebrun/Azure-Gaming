/*
*   Define ressources for Azure
*/

# Define the Microsoft Azure Provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.43.0"
    }
  }
  required_version = "1.3.8"
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
  subscription_id = "927cea4c-145c-49f0-b5bc-f71022884d33"
  tenant_id       = "b7b023b8-7c32-4c02-92a6-c8cdaa1d189c"
}

# Ressource group
resource "azurerm_resource_group" "gaming_1" {
  name     = var.rg_name
  location = var.location
}

# App Service Plan
resource "azurerm_service_plan" "gaming_1" {
  name                = var.asp_name
  location            = azurerm_resource_group.gaming_1.location
  resource_group_name = azurerm_resource_group.gaming_1.name
  os_type             = "Linux"
  sku_name            = var.asp_sku
}

# Web App
resource "azurerm_linux_web_app" "gaming_1" {
  name                = var.wa_name
  location            = azurerm_resource_group.gaming_1.location
  resource_group_name = azurerm_resource_group.gaming_1.name
  service_plan_id = azurerm_service_plan.gaming_1.id
  https_only          = true
  site_config {
    application_stack {
      dotnet_version = "7.0"
    }
    
  }
}
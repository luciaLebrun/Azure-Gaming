# Définition des variables d'entrée
variable "rg_name" {
  type        = string
  description = "Nom du resource group\n-- Exemple : test2"
}

variable "rg_location" {
  type        = string
  description = "Location du resource group\n-- Exemple : westeurope"
}

variable "asp_name" {
  type        = string
  description = "The name of the app service plan."
}

variable "asp_sku_name" {
  type        = string
  description = "Sku name of the app service plan."
}

variable "app_name" {
  type        = string
  description = "Name of the app."
}

variable "dotnet_version" {
  type        = string
  description = "The version of .NET to be used"
}

variable "db_server_name" {
  type        = string
  description = "The name of the database server."
}

variable "db_login" {
  type        = string
  description = "The login of the database server."
}

variable "db_password" {
  type        = string
  description = "The password of the database server."
}

variable "db_account_name" {
  type        = string
  description = "The name of the database account."
}

variable "db_name" {
  type        = string
  description = "The name of the database."
}


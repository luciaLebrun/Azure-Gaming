// Def variables
variable "rg_name" {
  description = "name of the resource group"
  type = string
}

variable "location" {
  description = "value of the location"
  type = string
}

variable "asp_name" {
  description = "name of the app service plan"
  type = string
}

variable "asp_sku" {
  description = "sku of the app service plan"
  type = string
}

variable "wa_name" {
  description = "name of the web app"
  type = string
}
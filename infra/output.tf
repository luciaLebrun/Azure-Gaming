// define the output
output "url" {
  value = "https://${azurerm_linux_web_app.gaming_1.default_hostname}/"
}
param appName string
param location string = resourceGroup().location
param sku string = 'F1' // Free tier, change as needed

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${appName}-plan'
  location: location
  sku: {
    name: sku
    tier: sku == 'F1' ? 'Free' : 'Standard'
  }
  kind: 'app'
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
  }
}

output webAppUrl string = 'https://${webApp.name}.azurewebsites.net'

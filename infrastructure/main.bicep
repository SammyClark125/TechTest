@description('Base name for the app (e.g., "techtest")')
param appName string

@description('The deployment environment (e.g., "dev", "test", "prod")')
@allowed([
  'dev'
  'test'
  'prod'
])
param environment string = 'dev'

@description('Azure region to deploy resources in')
param location string = 'uksouth'

@description('App Service pricing tier')
@allowed([
  'F1'
  'B1'
  'S1'
])
param sku string = 'F1'

// Name variables
var webAppName = '${appName}-${environment}'
var planName = '${webAppName}-plan'
var appInsightsName = '${webAppName}-ai'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: planName
  location: location
  sku: {
    name: sku
    tier: sku == 'F1' ? 'Free' : (sku == 'B1' ? 'Basic' : 'Standard')
  }
  kind: 'app'
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Web App
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: webAppName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'ApplicationInsights__InstrumentationKey'
          value: appInsights.properties.InstrumentationKey
        }
      ]
    }
  }
  dependsOn: [
    appServicePlan
    appInsights
  ]
}

// Outputs
output webAppUrl string = 'https://${webApp.name}.azurewebsites.net'
output instrumentationKey string = appInsights.properties.InstrumentationKey

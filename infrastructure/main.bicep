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
  'F1' // Free
  'B1' // Basic
  'S1' // Standard
])
param sku string = 'F1'

// Resource name variables
var webAppName = '${appName}-${environment}'
var planName = '${webAppName}-plan'
var appInsightsName = '${webAppName}-ai'

// Reuse existing App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' existing = {
  name: planName
}

// Reuse existing Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

// Reuse existing Web App and update settings
resource webApp 'Microsoft.Web/sites@2022-03-01' existing = {
  name: webAppName
}

// Update Web App Configuration
resource config 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: webApp
  name: 'web'
  properties: {
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

// Outputs
output webAppUrl string = 'https://${webApp.name}.azurewebsites.net'
output instrumentationKey string = appInsights.properties.InstrumentationKey

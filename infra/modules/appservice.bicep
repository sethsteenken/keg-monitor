param location string = resourceGroup().location
param appServicePlanName string
param appServiceName string
param containerRegistry string = 'index.docker.io'
param containerImageName string = 'sethsteenken/kegmonitor'
param containerImageTag string = 'latest'
param dockerHubUsername string

@secure()
param dockerHubPassword string

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'P1v2'
    tier: 'PremiumV2'
    size: 'P1v2'
    capacity: 1
  }
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2021-02-01' = {
  name: appServiceName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${containerRegistry}'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: dockerHubUsername
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: dockerHubPassword
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
      linuxFxVersion: 'DOCKER|${containerRegistry}/${containerImageName}:${containerImageTag}'
      numberOfWorkers: 1
      alwaysOn: false
      functionAppScaleLimit: 0
      minimumElasticInstanceCount: 0
    }
  }
  kind: 'app,linux,container'
}

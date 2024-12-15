targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@description('Resource group name, defaults to rg-env')
param resourceGroupName string = 'rg-${environmentName}'

@minLength(1)
@description('Primary location for all resources')
param location string

var tags = {
  environment: environmentName
}

var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module logging 'modules/logging.bicep' = {
  name: 'logging-${environmentName}-deployment'
  scope: rg
  params: {
    appInsightsName: 'appi-${resourceToken}'
    logAnalyticsWorkspaceName: 'law-${resourceToken}'
    location: location
    tags: tags
  }
}

module keyVault 'modules/keyvault.bicep' = {
  name: 'key-vault-${environmentName}-deployment'
  scope: rg
  params: {
    keyVaultName: 'kv${resourceToken}'
    tenantId: subscription().tenantId
    location: location
    tags: tags
    logAnalyticsWorkspaceName: logging.outputs.logAnalyticsWorkspaceName
  }
}

module storageAccount 'modules/storage.bicep' = {
  name: 'storage-account-${environmentName}-deployment'
  scope: rg
  params: {
    name: 'sa${resourceToken}'
    location: location
    tags: tags
  }
}

module sql 'modules/sql.bicep' = {
  name: 'sql-${environmentName}-deployment'
  scope: rg
  params: {
    location: location
    name: 'sql-${resourceToken}'
    databaseName: 'kegmonitor'
    tenantId: subscription().tenantId
    tags: tags
  }
}

module appService 'modules/appservice.bicep' = {
  name: 'app-service-${environmentName}-deployment'
  scope: rg
  params: {
    location: location
    appServicePlanName: 'asp-${resourceToken}'
    appServiceName: 'as-${resourceToken}'
    containerRegistry: 'index.docker.io'
    containerImageName: 'sethsteenken/kegmonitor'
    containerImageTag: 'latest'
    dockerHubUsername: ''
    dockerHubPassword: ''
  }
}

output AZURE_RESOURCE_GROUP_NAME string = rg.name
output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = subscription().tenantId
output AZURE_SUBSCRIPTION_ID string = subscription().subscriptionId
output AZURE_STORAGE_ACCOUNT_NAME string = storageAccount.outputs.storageAccountName
output AZURE_STORAGE_ACCOUNT_FILE_SHARE_NAME string = storageAccount.outputs.fileShareName
output AZURE_STORAGE_ACCOUNT_BLOB_CONTAINER_NAME string = storageAccount.outputs.blobContainerName
output AZURE_KEY_VAULT_NAME string = keyVault.outputs.keyVaultName
output AZURE_APP_INSIGHTS_NAME string = logging.outputs.appInsightsName
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = logging.outputs.logAnalyticsWorkspaceName


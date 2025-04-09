targetScope = 'resourceGroup'

@minLength(3)
@maxLength(12)
@description('Name of the the application environment which is used to generate a short unique hash used in all resources.')
param name string

@minLength(1)
@description('Primary location for all Azure resources.')
param location string = resourceGroup().location

@description('Optional. Specifies the resource tags for all the resoources. Tag "zad-env-name" is automatically added to all resources.')
param tags object = {}

@description('Specifies the object id of a Microsoft Entra ID user. In general, this the object id of the system administrator who deploys the Azure resources. This defaults to the deploying user.')
param userObjectId string = deployer().objectId

@description('Specifies the client id of the Microsoft Entra ID application. This is used to authenticate the application with Microsoft Entra ID.')
param azureEntraClientId string

@secure()
@description('Specifies the client secret of the Microsoft Entra ID application. This is used to authenticate the application with Microsoft Entra ID.')
param azureEntraClientSecret string

@description('Specifies the Azure Entra tenant id. This is used to authenticate the application with Microsoft Entra ID.')
param azureEntraTenantId string = subscription().tenantId

@description('Specifies the Azure Entra domain. This is used to authenticate the application with Microsoft Entra ID.')
param azureEntraDomain string

@description('Specifies the image tag of the Docker image. Defaults to latest.')
param imageTag string = 'latest'

var defaultTags = {
  'azd-env-name': name
}

var allTags = union(defaultTags, tags)
var resourceToken = substring(uniqueString(subscription().id, location, name), 0, 5)

module logAnalyticsWorkspace 'br/public:avm/res/operational-insights/workspace:0.11.1' = {
  name: take('${name}-log-analytics-deployment', 64)
  params: {
    name: toLower('log-${name}')
    location: location
    tags: allTags
    dataRetention: 60
  }
}

module applicationInsights 'br/public:avm/res/insights/component:0.6.0' = {
  name: take('${name}-app-insights-deployment', 64)
  params: {
    name: toLower('appi-${name}')
    location: location
    tags: allTags
    workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
  }
}

module appIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.4.0' = {
  name: take('${name}-app-identity-deployment', 64)
  params: {
    name: toLower('id-app-${name}')
    location: location
    tags: allTags
    roleAssignments: empty(userObjectId) ? [] : [
      {
        principalId: userObjectId
        principalType: 'User'
        roleDefinitionIdOrName: 'Owner'
      }
    ]
  }
}

var keyVaultName = take(toLower('kv${name}${resourceToken}'), 24)

module keyVault 'br/public:avm/res/key-vault/vault:0.11.0' = {
  name: take('${name}-keyvault-deployment', 64)
  params: {
    name: keyVaultName
    location: location
    tags: allTags
    sku: 'standard'
    publicNetworkAccess: 'Enabled'
    networkAcls: {
     defaultAction: 'Allow'
    }
    enablePurgeProtection: true
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    diagnosticSettings:[
      {
        storageAccountResourceId: storageAccount.outputs.resourceId
        workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
      } 
    ]
    roleAssignments: concat(empty(userObjectId) ? [] : [
      {
        principalId: userObjectId
        principalType: 'User'
        roleDefinitionIdOrName: 'Key Vault Secrets User'
      }
    ], [
      {
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Key Vault Secrets User'
      }
    ])
  }
}

module storageAccount 'br/public:avm/res/storage/storage-account:0.17.0' = {
  name: take('${name}-storage-account-deployment', 64)
  params: {
    name: take(toLower('st${name}${resourceToken}'), 24)
    location: location
    tags: allTags
    publicNetworkAccess: 'Enabled'
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: false
    allowCrossTenantReplication: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    blobServices: {
      containers: [
        {
          name: 'appdata'
          publicAccess: 'None'
        }
      ]
    }
    fileServices: {
      shares: [
        {
          accessTier: 'Hot'
          name: 'appfiles'
          shareQuota: 5120
        }
      ]
    }
    diagnosticSettings: [
      {
        workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
      }
    ]
    roleAssignments: concat(empty(userObjectId) ? [] : [
      {
        principalId: userObjectId
        principalType: 'User'
        roleDefinitionIdOrName: 'Storage Blob Data Contributor'
      }
    ], [
      {
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Storage Blob Data Contributor'
      }
    ])
  }
}

var databaseName = 'kegmonitor'

module postgreSQL 'br/public:avm/res/db-for-postgre-sql/flexible-server:0.11.0' = {
  name: take('${name}-postgresql-deployment', 64)
  params: {
    name: take(toLower('psql${name}${resourceToken}'), 63)
    skuName: 'Standard_D2s_v3'
    tier: 'GeneralPurpose'
    geoRedundantBackup: 'Disabled'
    highAvailability: 'Disabled'
    location: location
    publicNetworkAccess: 'Enabled'
    version: '15'
    tags: allTags
    storageSizeGB: 32
    databases: [
      {
        name: databaseName
      }
    ]
    roleAssignments: empty(userObjectId) ? [] : [
      {
        principalId: userObjectId
        principalType: 'User'
        roleDefinitionIdOrName: 'Owner'
      }
    ]
    administrators: concat(empty(userObjectId) ? [] : [
      {
        objectId: userObjectId
        principalName: userObjectId
        principalType: 'User'
      }
    ], [
      {
        objectId: appIdentity.outputs.principalId
        principalName: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
      }
    ])
    diagnosticSettings: [
      {
        metricCategories: [
          {
            category: 'AllMetrics'
          }
        ]
        name: 'customSetting'
        storageAccountResourceId: storageAccount.outputs.resourceId
        workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
      }
    ]
  }
}

module appServicePlan 'br/public:avm/res/web/serverfarm:0.4.1' = {
  name: take('${name}-app-service-plan-deployment', 64)
  params: {
    name: toLower('asp-${name}')
    location: location
    tags: allTags
    kind: 'app'
    skuName: 'B2'
    skuCapacity: 1
    reserved: true
    diagnosticSettings: [
      {
        metricCategories: [
          {
            category: 'AllMetrics'
          }
        ]
        name: 'customSetting'
        storageAccountResourceId: storageAccount.outputs.resourceId
        workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
      }
    ]
  }
}

module eventGridNamespace 'br/public:avm/res/event-grid/namespace:0.7.1' = {
  name:  take('${name}-event-grid-deployment', 64)
  params: {
    name: take(toLower('events${name}${resourceToken}'), 63)
    diagnosticSettings: [
      {
        storageAccountResourceId: storageAccount.outputs.resourceId
        workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
      }
    ]
    location: location
    tags: allTags
    roleAssignments: union(empty(userObjectId) ? [] : [
      {
        principalId: userObjectId
        principalType: 'User'
        roleDefinitionIdOrName: 'EventGrid Contributor'
      }
    ], [
      {
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'EventGrid EventSubscription Reader'
      }
    ])
  }
}

var mqttPasswordSecretName = 'mqtt-password'
var azureEntraClientSecretName = 'azure-entra-client-secret'
var postgreSqlConnectionStringSecretName = 'postgres-connection-string'

resource mqttPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2024-11-01' = {
  name: '${keyVaultName}/${mqttPasswordSecretName}'
  dependsOn: [
    keyVault
  ]
  properties: {
    value: uniqueString(subscription().id, location, name, resourceToken, mqttPasswordSecretName)
  }
}

resource azureEntraClientSecretSecret 'Microsoft.KeyVault/vaults/secrets@2024-11-01' = {
  name: '${keyVaultName}/${azureEntraClientSecretName}'
  dependsOn: [
    keyVault
  ]
  properties: {
    value: azureEntraClientSecret
  }
}

resource postgreSqlConnStringSecret 'Microsoft.KeyVault/vaults/secrets@2024-11-01' = {
  name: '${keyVaultName}/${postgreSqlConnectionStringSecretName}'
  dependsOn:[
    keyVault
  ]
  properties: {
    value: 'Host=${postgreSQL.outputs.fqdn};Database=${databaseName};Username=${appIdentity.outputs.principalId}'
  }
}

var appServiceName = toLower('app-${name}${resourceToken}')
 
module appService 'br/public:avm/res/web/site:0.15.1' = {
  name: take('${name}-app-service-deployment', 64)
  params: {
    name: appServiceName
    location: location
    tags: allTags
    kind: 'app,linux,container'
    serverFarmResourceId: appServicePlan.outputs.resourceId
    appInsightResourceId: applicationInsights.outputs.resourceId
    storageAccountResourceId: storageAccount.outputs.resourceId
    storageAccountUseIdentityAuthentication: true
    managedIdentities: {
      userAssignedResourceIds: [
        appIdentity.outputs.resourceId
      ]
    }
    logsConfiguration: {
      applicationLogs: {
        fileSystem: {
          level: 'Information'
        }
      }
      detailedErrorMessages: {
        enabled: true
      }
      failedRequestsTracing: {
        enabled: true
      }
      httpLogs: {
        fileSystem: {
          enabled: true
          retentionInDays: 1
          retentionInMb: 35
        }
      }
    }
    diagnosticSettings: [
      {
        storageAccountResourceId: storageAccount.outputs.resourceId
        workspaceResourceId: logAnalyticsWorkspace.outputs.resourceId
        metricCategories: [
          {
            category: 'AllMetrics'
          }
        ]
      }
    ]
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    appSettingsKeyValuePairs: {
      ASPNETCORE_ENVIRONMENT: 'Production'
      ConnectionStrings__DefaultConnection: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=${postgreSqlConnectionStringSecretName})'
      WebDomain: 'https://${appServiceName}.azurewebsites.net'
      MigrateDatabaseToLatest: 'true'
      Mqtt__ClientId: 'KM_Web_Sub'
      Mqtt__IpAddress: ''
      Mqtt__Port: '1883'
      Mqtt__Username: 'keg_monitor_web_user'
      Mqtt__Password: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=${mqttPasswordSecretName})'
      RequireAuthentication: 'true'
      AzureAd__TenantId: azureEntraTenantId
      AzureAd__ClientId: azureEntraClientId
      AzureAd__Domain: azureEntraDomain
      AzureAd__ClientCredentials__0__ClientSecret: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=${azureEntraClientSecretName})'
      WEBSITES_ENABLE_APP_SERVICE_STORAGE: 'false'
    }
    siteConfig: {
      alwaysOn: true
      ftpsState: 'FtpsOnly'
      healthCheckPath: '/health'
      linuxFxVersion: 'DOCKER|index.docker.io/sethsteenken/kegmonitor:${imageTag}'
    }
  }
}

//

// output AZURE_RESOURCE_GROUP_NAME string = rg.name
// output AZURE_LOCATION string = location
// output AZURE_TENANT_ID string = subscription().tenantId
// output AZURE_SUBSCRIPTION_ID string = subscription().subscriptionId
// output AZURE_STORAGE_ACCOUNT_NAME string = storageAccount.outputs.storageAccountName
// output AZURE_STORAGE_ACCOUNT_FILE_SHARE_NAME string = storageAccount.outputs.fileShareName
// output AZURE_STORAGE_ACCOUNT_BLOB_CONTAINER_NAME string = storageAccount.outputs.blobContainerName
// output AZURE_KEY_VAULT_NAME string = keyVault.outputs.keyVaultName
// output AZURE_APP_INSIGHTS_NAME string = logging.outputs.appInsightsName
// output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = logging.outputs.logAnalyticsWorkspaceName


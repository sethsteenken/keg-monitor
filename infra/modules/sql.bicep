param location string
param name string
param databaseName string
param tenantId string
param tags object 


resource server 'Microsoft.DBforPostgreSQL/flexibleServers@2024-11-01-preview' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Standard_B2s'
    tier: 'Burstable'
  }
  properties: {
    version: '15'
    authConfig: {
      activeDirectoryAuth: 'Enabled'
      passwordAuth: 'Disabled'
      tenantId: tenantId
    }
    storage: {
      storageSizeGB: 64
    }
    availabilityZone: '1'
    backup: {
        backupRetentionDays: 7
        geoRedundantBackup: 'Disabled'
    }
    highAvailability: {
        mode: 'Disabled'
    }
    maintenanceWindow: {
        customWindow: 'Disabled'
        dayOfWeek: 0
        startHour: 0
        startMinute: 0
    }
    replicationRole: 'Primary'
  }
}

resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2021-06-01' = {
  name: databaseName
  parent: server
  properties: {}
}



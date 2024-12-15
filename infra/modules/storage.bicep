param location string
param name string
param tags object

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {}
}

resource fileServices 'Microsoft.Storage/storageAccounts/fileServices@2023-01-01' = {
  name: 'default'
  parent: storageAccount
  properties: {
  }
}

resource fileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2022-09-01' = {
  parent: fileServices
  name: 'kegmonitorfileshare'
  properties: {}
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  name: 'default'
  parent: storageAccount
  properties: {}
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  parent: blobServices
  name: 'kegmonitorcontainer'
  properties: {}
}


output id string = storageAccount.id
output storageAccountName string = storageAccount.name
output fileShareName string = fileShare.name
output blobContainerName string = blobContainer.name
output endpoint string = 'https://${storageAccount.name}.blob.${environment().suffixes.storage}'

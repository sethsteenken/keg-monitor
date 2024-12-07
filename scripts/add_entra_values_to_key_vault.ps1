
# Add app registration details to existing Key Vault
Write-Host "Adding app registration details to Key Vault '$keyVaultName'"

$clientAppKeySecret = ConvertTo-SecureString $clientAppKey -AsPlainText -Force
$currentAppIdSecret = ConvertTo-SecureString $currentAppId -AsPlainText -Force
$tenantAuthorityUriSecret = ConvertTo-SecureString $tenantAuthorityUri -AsPlainText -Force

Write-Host "Adding App Registration Client Secret to Key Vault..."
Set-AzKeyVaultSecret -VaultName $keyVaultName -Name "entra-app-client-secret" -SecretValue $clientAppKeySecret

Write-Host "Adding App Registration Client ID to Key Vault..."
Set-AzKeyVaultSecret -VaultName $keyVaultName -Name "entra-app-client-id" -SecretValue $currentAppIdSecret

Write-Host "Adding Tenant Name to Key Vault..."
Set-AzKeyVaultSecret -VaultName $keyVaultName -Name "entra-authority-uri" -SecretValue $tenantAuthorityUriSecret
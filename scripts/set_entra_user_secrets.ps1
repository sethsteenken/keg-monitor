param (
    [string]$clientSecret,
    [string]$currentAppId,
    [string]$tenantAuthorityUri
)

Write-Host "Adding app registration details to local secrets..."
$projectPath = "./../src/KegMonitor.Web/KegMonitor.Web.csproj"
dotnet user-secrets set "AzureAD__ClientCredentials__ClientSecret" $clientAppKey -p $projectPath
dotnet user-secrets set "AzureAD__ClientId" $currentAppId -p $projectPath
dotnet user-secrets set "AzureAD__Authority" $tenantAuthorityUri -p $projectPath


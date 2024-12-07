#Requires -Version 7
 
[CmdletBinding()]
param(
    [Parameter(Mandatory=$False, HelpMessage='Tenant ID (This is a GUID which represents the "Directory ID" of the AzureAD tenant into which you want to create the apps')]
    [string] $tenantId,
    [Parameter(Mandatory=$False, HelpMessage='Azure environment to use while running the script. Default = Global')]
    [string] $azureEnvironmentName,
    [Parameter(Mandatory=$False, HelpMessage='Application name to register. Defaults to environment variable AZURE_ENV_NAME')]
    [string] $appName,
    [Parameter(Mandatory=$False, HelpMessage='URL of the application. Default to environment variable AZURE_APP_URL')]
    [string] $appUrl #https://localhost:7274
)

<#
 This script creates the Azure AD applications needed for this sample and updates the configuration files
 for the visual Studio projects from the data in the Azure AD applications.

 In case you don't have Microsoft.Graph.Applications already installed, the script will automatically install it for the current user
 
 There are two ways to run this script. For more information, read the AppCreationScripts.md file in the same folder as this script.
#>

# Create an application key
# See https://www.sabin.io/blog/adding-an-azure-active-directory-application-and-key-using-powershell/
Function CreateAppKey([DateTime] $fromDate, [double] $durationInMonths, [string] $displayName)
{
    $key = New-Object Microsoft.Graph.PowerShell.Models.MicrosoftGraphPasswordCredential

    $key.StartDateTime = $fromDate
    $key.EndDateTime = $fromDate.AddMonths($durationInMonths)
    $key.KeyId = (New-Guid).ToString()
    $key.DisplayName = $displayName

    return $key
}

# Adds the requiredAccesses (expressed as a pipe separated string) to the requiredAccess structure
# The exposed permissions are in the $exposedPermissions collection, and the type of permission (Scope | Role) is 
# described in $permissionType
Function AddResourcePermission($requiredAccess, `
                               $exposedPermissions, [string]$requiredAccesses, [string]$permissionType)
{
    foreach($permission in $requiredAccesses.Trim().Split("|"))
    {
        foreach($exposedPermission in $exposedPermissions)
        {
            if ($exposedPermission.Value -eq $permission)
                {
                $resourceAccess = New-Object Microsoft.Graph.PowerShell.Models.MicrosoftGraphResourceAccess
                $resourceAccess.Type = $permissionType # Scope = Delegated permissions | Role = Application permissions
                $resourceAccess.Id = $exposedPermission.Id # Read directory data
                $requiredAccess.ResourceAccess += $resourceAccess
                }
        }
    }
}

#
# Example: GetRequiredPermissions "Microsoft Graph"  "Graph.Read|User.Read"
# See also: http://stackoverflow.com/questions/42164581/how-to-configure-a-new-azure-ad-application-through-powershell
Function GetRequiredPermissions([string] $applicationDisplayName, [string] $requiredDelegatedPermissions, [string]$requiredApplicationPermissions, $servicePrincipal)
{
    # If we are passed the service principal we use it directly, otherwise we find it from the display name (which might not be unique)
    if ($servicePrincipal)
    {
        $sp = $servicePrincipal
    }
    else
    {
        $sp = Get-MgServicePrincipal -Filter "DisplayName eq '$applicationDisplayName'"
    }
    $appid = $sp.AppId
    $requiredAccess = New-Object Microsoft.Graph.PowerShell.Models.MicrosoftGraphRequiredResourceAccess
    $requiredAccess.ResourceAppId = $appid 
    $requiredAccess.ResourceAccess = New-Object System.Collections.Generic.List[Microsoft.Graph.PowerShell.Models.MicrosoftGraphResourceAccess]

    # $sp.Oauth2Permissions | Select Id,AdminConsentDisplayName,Value: To see the list of all the Delegated permissions for the application:
    if ($requiredDelegatedPermissions)
    {
        AddResourcePermission $requiredAccess -exposedPermissions $sp.Oauth2PermissionScopes -requiredAccesses $requiredDelegatedPermissions -permissionType "Scope"
    }
    
    # $sp.AppRoles | Select Id,AdminConsentDisplayName,Value: To see the list of all the Application permissions for the application
    if ($requiredApplicationPermissions)
    {
        AddResourcePermission $requiredAccess -exposedPermissions $sp.AppRoles -requiredAccesses $requiredApplicationPermissions -permissionType "Role"
    }
    return $requiredAccess
}

<#.Description
   Primary entry method to create and configure app registrations
#> 
Function ConfigureApplications
{
    Write-Host "azureEnvironmentName: $azureEnvironmentName"
    Write-Host "tenantId: $tenantId"
    Write-Host "appName: $appName"
    Write-Host "appUri: $appUrl"

    # Connect to the Microsoft Graph API, non-interactive is not supported for the moment (Oct 2021)
    Write-Host "Connecting to Microsoft Graph"
    Connect-MgGraph -TenantId $tenantId -Scopes "User.Read.All Organization.Read.All Application.ReadWrite.All" -Environment $azureEnvironmentName
    
    $context = Get-MgContext
    $tenantId = $context.TenantId

    $existing = Get-MgApplication -ConsistencyLevel eventual -Count appCount -Filter "DisplayName eq $appName"
    if ($existing.Count -gt 0) 
    { 
        Write-Host "Application '$appName' already exists. Skipping creation." -ForegroundColor Yellow 
        return
    }

    # Get the user running the script
    $currentUserPrincipalName = $context.Account
    $user = Get-MgUser -Filter "UserPrincipalName eq '$($context.Account)'"

    # get the tenant we signed in to
    $tenant = Get-MgOrganization
    
    $verifiedDomain = $tenant.VerifiedDomains | where {$_.Isdefault -eq $true}
    $verifiedDomainName = $verifiedDomain.Name
    $tenantId = $tenant.Id

    Write-Host ("Connected to Tenant {0} ({1}) as account '{2}'. Domain is '{3}'" -f  $tenant.DisplayName, $tenant.Id, $currentUserPrincipalName, $verifiedDomainName)

    # Create the client Entra application
    Write-Host "Creating the Entra application ($appName)"

    
    # create the application 
    $clientAadApplication = New-MgApplication -DisplayName $appName `
                                                        -Web `
                                                        @{ `
                                                            RedirectUris = $appUrl, "${appUrl}/signin-oidc"; `
                                                            HomePageUrl = $appUrl; `
                                                            LogoutUrl = "${appUrl}/signout-callback-oidc"; `
                                                            } `
                                                        -SignInAudience AzureADMyOrg `

    Write-Host "Created the client application ($appName)"                                                    

    $currentAppId = $clientAadApplication.AppId
    $currentAppObjectId = $clientAadApplication.Id
    $tenantName = (Get-MgApplication -ApplicationId $currentAppObjectId).PublisherDomain
    $tenantAuthorityUri = "https://" + $tenantName.Split(".onmicrosoft.com")[0] + ".ciamlogin.com/"

    Write-Host "Current app ID: $currentAppId"
    Write-Host "Current app object ID: $currentAppObjectId"
    Write-Host "Tenant name: $tenantName"

    #add a secret to the application
    Write-Host "Adding password credential secret to the application..."

    $fromDate = [DateTime]::Now;
    $key = CreateAppKey -fromDate $fromDate -durationInMonths 6 -displayName "app secret"

    $pwdCredential = Add-MgApplicationPassword -ApplicationId $currentAppObjectId -PasswordCredential $key
    $clientAppKey = $pwdCredential.SecretText            
    
    Write-Host "Setting local environment variables for the app registration..."
    [System.Environment]::SetEnvironmentVariable('AZURE_KEGMONITOR_TENANT_ID', $tenantId)
    [System.Environment]::SetEnvironmentVariable('AZURE_KEGMONITOR_CLIENT_ID', $currentAppId)
    [System.Environment]::SetEnvironmentVariable('AZURE_KEGMONITOR_CLIENT_SECRET', $clientAppKey)

    Write-Host "Adding Service Principal for the application..."

    # create the service principal of the newly created application     
    $clientServicePrincipal = New-MgServicePrincipal -AppId $currentAppId -Tags {WindowsAzureActiveDirectoryIntegratedApp}

    # add the user running the script as an app owner if needed
    $owner = Get-MgApplicationOwner -ApplicationId $currentAppObjectId
    if ($owner -eq $null)
    { 
        $ownerObjectId = $user.Id
        Write-Host "Adding the user running the script as an application owner to app..."
        Write-Host "Owner object ID: $ownerObjectId"
        
        New-MgApplicationOwnerByRef -ApplicationId $currentAppObjectId -BodyParameter @{"@odata.id" = "https://graph.microsoft.com/v1.0/directoryObjects/{$ownerObjectId}"}
        Write-Host "'$($user.UserPrincipalName)' added as an application owner to app '$($clientServicePrincipal.DisplayName)'"
    }
    Write-Host "Done creating the client application ($appName)"

    # URL of the Entra application in the Azure portal\
    $clientPortalUrl = "https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/"+$currentAppId+"/isMSAApp~/false"

    # Declare a list to hold RRA items    
    $requiredResourcesAccess = New-Object System.Collections.Generic.List[Microsoft.Graph.PowerShell.Models.MicrosoftGraphRequiredResourceAccess]

    # Add Required Resources Access (from 'client' to 'Microsoft Graph')
    Write-Host "Getting access from 'client' to 'Microsoft Graph'"
    $requiredPermission = GetRequiredPermissions -applicationDisplayName "Microsoft Graph"`
        -requiredDelegatedPermissions "openid|offline_access"

    $requiredResourcesAccess.Add($requiredPermission)
    Write-Host "Added 'Microsoft Graph' to the RRA list."
    
    Update-MgApplication -ApplicationId $currentAppObjectId -RequiredResourceAccess $requiredResourcesAccess
    Write-Host "Granted permissions."

    Write-Host "Updating application to allow Id_Token..."

    Update-MgApplication -ApplicationId $currentAppObjectId -BodyParameter @{
        Web = @{
            ImplicitGrantSettings = @{
                EnableIdTokenIssuance = $true
            }
        }
    }
    Write-Host "Enabled ID token issuance."

    # print the registered app portal URL for any further navigation
    Write-Host "Successfully registered and configured that app registration for '$appName' at `n $clientPortalUrl" -ForegroundColor Green 
        
    Write-Host -ForegroundColor Green "Entra applicaiton registered successfully!"
} # end of ConfigureApplications function

# Pre-requisites

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph")) {
    Install-Module "Microsoft.Graph" -Scope CurrentUser 
}

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph.Authentication")) {
    Install-Module "Microsoft.Graph.Authentication" -Scope CurrentUser 
}

Import-Module Microsoft.Graph.Authentication

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph.Identity.DirectoryManagement")) {
    Install-Module "Microsoft.Graph.Identity.DirectoryManagement" -Scope CurrentUser 
}

Import-Module Microsoft.Graph.Identity.DirectoryManagement

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph.Applications")) {
    Install-Module "Microsoft.Graph.Applications" -Scope CurrentUser 
}

Import-Module Microsoft.Graph.Applications

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph.Groups")) {
    Install-Module "Microsoft.Graph.Groups" -Scope CurrentUser 
}

Import-Module Microsoft.Graph.Groups

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph.Users")) {
    Install-Module "Microsoft.Graph.Users" -Scope CurrentUser 
}

Import-Module Microsoft.Graph.Users

$ErrorActionPreference = "Stop"

try
{
    if ([string]::IsNullOrEmpty($tenantId)) {
        $tenantId = $env:AZURE_TENANT_ID
    }
    
    if ([string]::IsNullOrEmpty($azureEnvironmentName)) {
        $azureEnvironmentName = 'Global'
    }

    if ([string]::IsNullOrEmpty($appName)) {
        $appName = $env:AZURE_ENV_NAME
    }

    if ([string]::IsNullOrEmpty($appUrl)) {
        $appUrl = $env:AZURE_WEB_APP_URL
    }

    ConfigureApplications
}
catch
{
    $_.Exception.ToString() | out-host
    $message = $_
    Write-Warning $Error[0]    
    Write-Host "Unable to register apps. Error: $message" -ForegroundColor White -BackgroundColor Red
}
Write-Host "Disconnecting from tenant"
Disconnect-MgGraph
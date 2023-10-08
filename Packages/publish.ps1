# require Install-Package System.Management.Automation

param
(
    [Parameter(Mandatory = $true)]
    [string]$pat,

    [string]$version = $null
)

$cwd = Get-Location

$root = "$PSScriptRoot/.."
$directory = "$PSScriptRoot/output"

Set-Location $cwd

function Increment-ProjectVersion
{
    param (
        [string]$path = ".",
        [int]$increment = 1
    )

    # Get a list of all .csproj files in the specified directory
    $csprojFiles = Get-ChildItem -Path $path -Filter *.csproj -Recurse | ForEach-Object {
        $_.FullName
    }


    # Loop through each .csproj file and increment the Version attribute
    foreach ($csprojFile in $csprojFiles)
    {
        $xml = [xml](Get-Content $csprojFile)

        # Find the Version attribute within a PropertyGroup element
        $propertyGroup = $xml.SelectNodes('//PropertyGroup')[0]
        $versionNode = $propertyGroup.SelectSingleNode('Version')


        if ($null -ne $versionNode)
        {
            # Increment the current version
            $currentVersion = [Version]$versionNode.InnerText

            if ($null -ne $version -and $version -ne "")
            {
                $newVersion = [Version]$version
            }
            else {
                $newVersion = [Version]::new($currentVersion.Major,$currentVersion.Minor,$currentVersion.Build + 1)
            }

            # Update the Version attribute
            $versionNode.InnerText = $newVersion.ToString()

            # Save the modified XML back to the file
            $xml.Save($csprojFile)

            Write-Host "Updated $( $csprojFile ) to version $( $newVersion )"
        }
        else
        {
            Write-Host "No Version attribute found in $( $csprojFile )"
        }
    }

    Write-Host "Version increment complete."
}


function Push-Packages
{
    Remove-Item -Recurse -Force $directory
    New-Item -Path $directory -ItemType Directory


    Set-Location $root

    Get-ChildItem -Path $root -Include bin, obj -Recurse -Directory | Remove-Item -Recurse -Force

    dotnet build

    Get-ChildItem -Path $root -Filter *.nupkg -Recurse | Move-Item -Destination $directory

    Set-Location $directory
    dotnet nuget push * -k $pat --source https://api.nuget.org/v3/index.json --skip-duplicate
}





Increment-ProjectVersion -path $PSScriptRoot -increment 1
Push-Packages

Set-Location $cwd
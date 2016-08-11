<#
    .SYNOPSIS
    Adds the class library and NuGet projects necessary to support a new API set
    to the solution.

    .PARAMETER LibraryName
    The name of the API set you are introducing support for

    .EXAMPLE
    AddNewLibrary -LibraryName HighDPI
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true,Position=0)]
    [string]$LibraryName
)
function Replace-Placeholders {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path,
        [Parameter(Mandatory=$true)]
        [string]$LibraryName,
        [Parameter(Mandatory=$true)]
        $Replacements
    )

    Get-ChildItem -Recurse $Path -File |% {
        $content = Get-Content -Path $_.FullName
        $Replacements.GetEnumerator() |% { $content = $content -replace $_.Key,$_.Value }
        Set-Content -Path $_.FullName -Value $content -Encoding UTF8

        if ($_.Name -match 'LIBNAME') {
            $NewLeafName = $_.Name -Replace 'LIBNAME',$LibraryName
            Rename-Item $_.FullName $NewLeafName
        }
    }

    # Rename directories
    Get-ChildItem -Recurse $Path -Directory |% {
        if ($_.Name -match 'LIBNAME') {
            $NewLeafName = $_.Name -Replace 'LIBNAME',$LibraryName
            Rename-Item $_.FullName $NewLeafName
        }
    }

    # Finally, rename $Path itself
    $NewLeafName = (Split-Path -Leaf $Path) -Replace 'LIBNAME',$LibraryName
    Rename-Item $Path $NewLeafName
}

$Src = Resolve-Path "$PSScriptRoot\..\src"

$Directories = 'WinApi.LIBNAME','WinApi.LIBNAME.Tests','WinApi.LIBNAME.NuGet'
$TemplateDirectories = @()
$SrcDirectories = @()
foreach($dir in $Directories) {
    $SrcDirectory = "$Src\$dir"
    $TemplateDirectory = "$PSScriptRoot\$dir"
    $SrcDirectory_Substituted = $SrcDirectory.Replace('LIBNAME', $LibraryName)
    If (-not (Test-Path $SrcDirectory_Substituted)) {
        $TemplateDirectories += $TemplateDirectory
        $SrcDirectories += $SrcDirectory
    }
}

$Replacements = @{
    '\$guid1\$' = [Guid]::NewGuid().ToString('b').ToUpper();
    '\$guid2\$' = [Guid]::NewGuid().ToString('b').ToUpper();
    '\$guid3\$' = [Guid]::NewGuid().ToString('b').ToUpper();
    '\$guid4\$' = [Guid]::NewGuid().ToString('b').ToUpper();
    '\$guid5\$' = [Guid]::NewGuid().ToString('b').ToUpper();
    '\$guid6\$' = [Guid]::NewGuid().ToString('b').ToUpper();
    'LIBNAME' = $LibraryName;
}

Copy-Item -Recurse -Path $TemplateDirectories -Destination $Src
$SrcDirectories |% { Replace-Placeholders -LibraryName $LibraryName -Replacements $Replacements -Path $_ }

Write-Output "Great. Your new projects have been created. Please also perform a few more manual steps:"
Write-Output "1. Add these new projects to your solution file:"
Write-Output "    $Src\WinApi.$LibraryName\WinApi.$LibraryName.csproj"
Write-Output "    $Src\WinApi.$LibraryName.Tests\WinApi.$LibraryName.Tests.csproj"
Write-Output "    $Src\WinApi.$LibraryName.NuGet\WinApi.$LibraryName.NuGet.nuproj"
Write-Output "2. Add your library to the README.md file."
Write-Output "3. Add a project references to $Src\WinApi.$LibraryName.NuGet\WinApi.$LibraryName.NuGet.nuproj"


param($installPath, $toolsPath, $package, $project)

$hostConfig = $project.ProjectItems.Item("Medseek.Util.MicroServices.Host.exe.config")
$servicesXml = $project.ProjectItems.Item("services.xml")

# 'Copy To Output Directory' = 'Copy if newer'
$copyToOutput = $hostConfig.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 2
$copyToOutput = $servicesXml.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 2

# 'Build Action' = 'Content'
$buildAction = $hostConfig.Properties.Item("BuildAction")
$buildAction.Value = 2
$buildAction = $servicesXml.Properties.Item("BuildAction")
$buildAction.Value = 2

$assemblyName = $project.Properties.Item("AssemblyName").Value
$projectFilePath = $project.Properties.Item("FullPath").Value
$projectName = $project.FullName

# Create nuspec from template, if not present
$nuspecName = "$assemblyName.nuspec"
$nuspecPath = $projectFilePath

$destinationPath = "$nuspecPath$nuspecName"

if (!(Test-Path -Path $destinationPath)) 
{
	Write-Host "Copying project nuspec template to $destinationPath"
	Copy-Item -Path "$toolsPath\nuspec.template" -Destination "$destinationPath" | Out-Null

	$project.ProjectItems.AddFromFileCopy("$destinationPath");	
}

$project.Save();

# Update services.xml
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($project.FullName) | Select-Object -First 1
$servicesXmlFilePath = $servicesXml.Properties.Item("FullPath").Value

$servicesXmlText0 = [System.IO.File]::ReadAllText($servicesXmlFilePath)
Write-Verbose $servicesXmlText0
$servicesXmlText1 = $servicesXmlText0.Replace("{ASSEMBLYNAME}", $assemblyName);
Write-Verbose $servicesXmlText1
[System.IO.File]::WriteAllText($servicesXmlFilePath, $servicesXmlText1);

# Update project startup settings
$startAction = $msbuild.GetProperty("StartAction")

if($startAction -ne $null)
{
	return
}
	
$msbuild.SetProperty("StartAction" , "Program")
$msbuild.SetProperty("StartProgram" , "`$(MSBuildProjectDirectory)\`$(OutputPath)Medseek.Util.MicroServices.Host.exe")
$msbuild.SetProperty("StartArguments" , "`$(AssemblyName).dll")
$msbuild.Save()

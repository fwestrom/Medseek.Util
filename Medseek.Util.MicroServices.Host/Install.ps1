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
$servicesXmlFilePath = $servicesXml.Properties.Item("FullPath").Value

$servicesXmlText0 = [System.IO.File]::ReadAllText($servicesXmlFilePath)
Write-Verbose $servicesXmlText0
$servicesXmlText1 = $servicesXmlText0.Replace("{ASSEMBLYNAME}", $assemblyName);
Write-Verbose $servicesXmlText1
[System.IO.File]::WriteAllText($servicesXmlFilePath, $servicesXmlText1);

# Update project startup settings
	
[xml] $prjXml = Get-Content $project.FullName
$xmlNs = $prjXml.Project.GetAttribute("xmlns");

[System.Xml.XmlNamespaceManager] $nsmgr = $prjXml.NameTable
$nsmgr.AddNamespace('projNs',$xmlNs);

$startAction = $prjXml.SelectSingleNode("//projNs:PropertyGroup/projNs:StartAction", $nsmgr);
if($startAction -ne $null)
{
	return
}

$propertyGroupElement = $prjXml.CreateElement("PropertyGroup", $xmlNs);

$startActionElement = $prjXml.CreateElement("StartAction", $xmlNs);
$startActionElement.InnerText = "Program";
$propertyGroupElement.AppendChild($startActionElement) | Out-Null

$startProgramElement = $prjXml.CreateElement("StartProgram", $xmlNs);
$startProgramElement.InnerText = "`$(MSBuildProjectDirectory)\`$(OutputPath)Medseek.Util.MicroServices.Host.exe";
$propertyGroupElement.AppendChild($startProgramElement) | Out-Null

$startArgumentsElement = $prjXml.CreateElement("StartArguments", $xmlNs);
$startArgumentsElement.InnerText = "`$(AssemblyName).dll";
$propertyGroupElement.AppendChild($startArgumentsElement) | Out-Null

$prjXml.project.AppendChild($propertyGroupElement) | Out-Null

$prjXml.Save($project.FullName);

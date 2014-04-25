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

$project.Save();

# Update services.xml
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject($project.FullName) | Select-Object -First 1
$assemblyName = $msbuild.GetProperty("AssemblyName").Xml.Value
$projectFilePath = Resolve-Path -Path $project.FullName
$servicesXmlFilePath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($projectFilePath), "services.xml")
$servicesXmlText0 = [System.IO.File]::ReadAllText($servicesXmlFilePath)
Write-Verbose $servicesXmlText0
$servicesXmlText1 = $servicesXmlText0.Replace("{ASSEMBLYNAME}", $assemblyName);
Write-Verbose $servicesXmlText1
[System.IO.File]::WriteAllText($servicesXmlFilePath, $servicesXmlText1);

# Update project startup settings
[xml] $prjXml = Get-Content $project.FullName
foreach($PropertyGroup in $prjXml.project.ChildNodes)
{
	if($PropertyGroup.StartAction -ne $null)
	{
		return
	}
}

$propertyGroupElement = $prjXml.CreateElement("PropertyGroup", $prjXml.Project.GetAttribute("xmlns"));
$startActionElement = $prjXml.CreateElement("StartAction", $prjXml.Project.GetAttribute("xmlns"));
$propertyGroupElement.AppendChild($startActionElement) | Out-Null
$propertyGroupElement.StartAction = "Program"
$startProgramElement = $prjXml.CreateElement("StartProgram", $prjXml.Project.GetAttribute("xmlns"));
$propertyGroupElement.AppendChild($startProgramElement) | Out-Null
$propertyGroupElement.StartProgram = "`$(MSBuildProjectDirectory)\`$(OutputPath)Medseek.Util.MicroServices.Host.exe"
$prjXml.project.AppendChild($propertyGroupElement) | Out-Null
$startArgumentsElement = $prjXml.CreateElement("StartArguments", $prjXml.Project.GetAttribute("xmlns"));
$propertyGroupElement.AppendChild($startArgumentsElement) | Out-Null
$propertyGroupElement.StartArguments = "`$(AssemblyName).dll"
$writerSettings = new-object System.Xml.XmlWriterSettings
$writerSettings.OmitXmlDeclaration = $false
$writerSettings.NewLineOnAttributes = $false
$writerSettings.Indent = $true
$writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
$prjXml.WriteTo($writer)
$writer.Flush()
$writer.Close()

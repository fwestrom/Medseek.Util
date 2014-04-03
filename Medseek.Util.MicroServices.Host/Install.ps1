param($installPath, $toolsPath, $package, $project)

$hostConfig = $project.ProjectItems.Item("Medseek.Util.MicroServices.Host.exe.config")

# 'Copy To Output Directory' = 'Copy if newer'
$copyToOutput = $hostConfig.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 2

# 'Build Action' = 'Content'
$buildAction = $hostConfig.Properties.Item("BuildAction")
$buildAction.Value = 2

$project.Save();

[xml] $prjXml = Get-Content $project.FullName
foreach($PropertyGroup in $prjXml.project.ChildNodes)
{
	if($PropertyGroup.StartAction -ne $null)
	{
		return
	}
}

$exeName = "Medseek.Util.MicroServices.Host.exe"

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
$projectFilePath = Resolve-Path -Path $project.FullName
$writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
$prjXml.WriteTo($writer)
$writer.Flush()
$writer.Close()
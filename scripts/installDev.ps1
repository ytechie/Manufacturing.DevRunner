#To run this script - first execute "Set-ExecutionPolicy Unrestricted"

#TODO: How can we share this with the application configuration?
$DataCollectorPort = 1234

Write-Host Adding security exception for DataCollector API host...
netsh http add urlacl http://+:$DataCollectorPort/ user=$env:USERDOMAIN\$env:USERNAME

Write-Host Installing MSMQ if needed...
Enable-WindowsOptionalFeature -Online -FeatureName MSMQ-Server -all

#CheckNetIsolation.exe LoopbackExempt –a –p=89acdcc2-c3e1-47f2-9bc3-d6080a93480d
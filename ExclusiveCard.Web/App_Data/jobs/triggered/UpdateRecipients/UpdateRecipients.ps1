$Logfile = "D:\home\site\wwwroot\Logs\WebJob.log"

Function LogWrite
{
   Param ([string]$Message)

    $Stamp = (Get-Date).toString("yyyy/MM/dd HH:mm:ss")
    $Line = "$Stamp $Message"

   Add-content $Logfile -value $Line
}

try
{

& "D:\home\site\wwwroot\App_Data\Exclusive.jobs\netcoreapp3.1\Exclusive.jobs.exe" All


}
catch
{
	LogWrite("UpdateRecipients Issue: $PSItem")
}

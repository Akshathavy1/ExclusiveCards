[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$TheDate = Get-date
$DateFrom = ($TheDate).AddDays(-90).ToString("yyyy-MM-dd")
$DateTo = $TheDate.ToString("yyyy-MM-dd")

#Live URLs
$reportURL = "https://admin.exclusive.cards/api/Cashback/GetTransactionReport?DateFrom=" + $DateFrom + "&DateTo=" + $DateTo
$migrateURL = "https://admin.exclusive.cards/api/Cashback/MigrateStagingData"

#Test URLs
#$reportURL = "https://exclusivecards-testadmin.azurewebsites.net/api/Cashback/GetTransactionReport?DateFrom=" + $DateFrom + "&DateTo=" + $DateTo
#$migrateURL = "https://exclusivecards-testadmin.azurewebsites.net/api/Cashback/MigrateStagingData"

"Fetching Transaction Report"
$response = Invoke-RestMethod -Uri $reportURL -Method Get -ContentType "application/json" 
$response

"Migrating Staging Data"
$response = Invoke-RestMethod -Uri $migrateURL -Method Post  -ContentType "application/json" 
$response


[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$TheDate = Get-date
$TheDuration = -90

if ($TheDate.Day -eq 1) {
	#1st of the month so pick up the whole previous year
	$TheDuration = -365
	"Running full year to " + ($TheDate).AddDays($TheDuration).ToString("yyyy-MM-dd")
}

$DateFrom = ($TheDate).AddDays($TheDuration).ToString("yyyy-MM-dd")
$DateTo = $TheDate.ToString("yyyy-MM-dd")

#Live URLs
$reportURL = "https://admin.exclusiverewards.co.uk/api/Cashback/GetTransactionReport?DateFrom=" + $DateFrom + "&DateTo=" + $DateTo
$migrateURL = "https://admin.exclusiverewards.co.uk/api/Cashback/MigrateStagingData"

#Live Seychelles URLs
#$reportURL = "https://admin.exclusive.cards/api/Cashback/GetTransactionReport?DateFrom=" + $DateFrom + "&DateTo=" + $DateTo
#$migrateURL = "https://admin.exclusive.cards/api/Cashback/MigrateStagingData"

#Test URLs
#$reportURL = "https://exclusivecards-testadmin.azurewebsites.net/api/Cashback/GetTransactionReport?DateFrom=" + $DateFrom + "&DateTo=" + $DateTo
#$migrateURL = "https://exclusivecards-testadmin.azurewebsites.net/api/Cashback/MigrateStagingData"

#$reportURL = "https://localhost:44315/api/Cashback/GetTransactionReport?DateFrom=" + $DateFrom + "&DateTo=" + $DateTo
#$migrateURL = "https://localhost:44315/api/Cashback/MigrateStagingData"

"Fetching Transaction Report"
$response = Invoke-RestMethod -Uri $reportURL -Method Get -ContentType "application/json" 
$response

"Migrating Staging Data"
$response = Invoke-RestMethod -Uri $migrateURL -Method Post  -ContentType "application/json" 
$response


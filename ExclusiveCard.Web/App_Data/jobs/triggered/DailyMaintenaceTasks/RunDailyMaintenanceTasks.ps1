[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12


# Live 
$maintURL = "https://admin.exclusiverewards.co.uk/api/Maintenance/Daily"

# Live Seyshelles
#$maintURL = "https://admin.exclusive.cards/api/Maintenance/Daily"

# Test
#$maintURL = "https://exclusivecards-testadmin.azurewebsites.net/api/Maintenance/Daily"

"Executing Maintenance Tasks"
$response = Invoke-RestMethod -Uri $maintURL -Method Post -ContentType "application/json" 
$response
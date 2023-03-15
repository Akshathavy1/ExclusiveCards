[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

#Live URLs
$generateReportURL = "https://admin.exclusiverewards.co.uk/api/PartnerReport/SendPartnerReport?partnerId=1"
$generateWithdrawalReportURL = "https://admin.exclusiverewards.co.uk/api/PartnerReport/SendPartnerWithdrawalReport"
$processReportURL = "https://admin.exclusiverewards.co.uk/api/PartnerReport/ProcessPartnerReport?partnerId=1"
$processPartnerPositionFileURL = "https://admin.exclusiverewards.co.uk/api/PartnerReport/ProcessPartnerPositionFile"

#Live Seychelles URLs
#$generateReportURL = "https://admin.exclusive.cards/api/PartnerReport/SendPartnerReport?partnerId=1"
#$generateWithdrawalReportURL = "https://admin.exclusive.cards/api/PartnerReport/SendPartnerWithdrawalReport"
#$processReportURL = "https://admin.exclusive.cards/api/PartnerReport/ProcessPartnerReport?partnerId=1"
#$processPartnerPositionFileURL = "https://admin.exclusive.cards/api/PartnerReport/ProcessPartnerPositionFile"


#TEST URLs
#$generateReportURL = "https://exclusivecards-testadmin.azurewebsites.net/api/PartnerReport/SendPartnerReport?partnerId=1"
#$generateWithdrawalReportURL = "https://exclusivecards-testadmin.azurewebsites.net/api/PartnerReport/SendPartnerWithdrawalReport"
#$processReportURL = "https://exclusivecards-testadmin.azurewebsites.net/api/PartnerReport/ProcessPartnerReport?partnerId=1"
#$processPartnerPositionFileURL = "https://exclusivecards-testadmin.azurewebsites.net/api/PartnerReport/ProcessPartnerPositionFile"


"Send Partner Transactions Report"
$response = Invoke-RestMethod -Uri $generateReportURL -Method Post -ContentType "application/json" 
$response

"Send Partner Withdrawal Report"
$response = Invoke-RestMethod -Uri $generateWithdrawalReportURL -Method Post  -ContentType "application/json" 
$response


"Process Partner Transactions Report"
$response = Invoke-RestMethod -Uri $processReportURL -Method Post -ContentType "application/json" 
$response

"Process position file"
$response = Invoke-RestMethod -Uri $processPartnerPositionFileURL -Method Post  -ContentType "application/json" 
$response

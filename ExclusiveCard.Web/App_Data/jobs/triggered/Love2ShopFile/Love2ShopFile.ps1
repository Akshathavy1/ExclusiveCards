[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

#Live URLs
$generateLove2Shop = "https://admin.exclusiverewards.co.uk/api/Offer/GenerateOfferRedemptionFile"

#Live Seychelles URLs
#$generateLove2Shop = "https://admin.exclusive.cards/api/Offer/GenerateOfferRedemptionFile"

#Test URLs
#$generateLove2Shop = "https://exclusivecards-testadmin.azurewebsites.net/api/Offer/GenerateOfferRedemptionFile"



"Send Partner Withdrawal Report"
$response = Invoke-RestMethod -Uri $generateLove2Shop -Method Post  -ContentType "application/json" 
$response

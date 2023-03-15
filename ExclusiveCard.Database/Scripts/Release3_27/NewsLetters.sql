DECLARE @ListName nvarchar(50)
--DECLARE @Description nvarchar(500)
DECLARE @NewOfferListId int
DECLARE @NewNewsLtrId int
DECLARE @NewsLetterId int
DECLARE @Name nvarchar(255)
DECLARE @Description nvarchar(512)
DECLARE @EmailTemplateId int
DECLARE @EmailName nvarchar(200)
DECLARE @NewsletterEmailType int = 3

SET @EmailName='Email New Letter'
SET @ListName = 'Newsletter Hub' -- Newsletter offer list name
SET @Description = 'Newsletter Hub' -- Newsletter offer list description
SET @Name='Newsletter' -- Newsletter name
SET @Description='Main newsletter' -- Newsletter description

  -- Offer List
  IF NOT EXISTS(Select Id FROM [CMS].[OfferList] WHERE [ListName] = @ListName)
  BEGIN
      INSERT INTO [CMS].[OfferList]
      (
        [ListName],[Description],[MaxSize],[IsActive],[IncludeShowAllLink],[ShowAllLinkCaption],[PermissionLevel]
      )
      VALUES
      (
        @ListName, @Description, 0, 1, 1,'Show All', 0
      )

      SELECT @NewOfferListId = SCOPE_IDENTITY()
  END
  ELSE
  BEGIN 
      SELECT @NewOfferListId = [Id] FROM [CMS].[OfferList] WHERE [ListName] = @ListName
  END

  -- Add Email Template
  IF NOT EXISTS(Select Id FROM [Exclusive].[EmailTemplates] Where EmailName = @EmailName)
  BEGIN
      INSERT INTO [Exclusive].[EmailTemplates]
      (
        [EmailName], [Subject], [HeaderText], [HeaderHtml], [BodyHtml], [FooterHtml], [BodyText], [FooterText], [IsDeleted], [TemplateTypeId]
      )
      VALUES
      (
        'Newsletter Email', 'Latest Offers', '[whitelabelName]'
        , 
  '<!doctype html>
<html lang="en" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office" style="background-color: #f6f6f6; height: 100%; margin: 0; min-width: 100%; padding: 0; width: 100%;">
<head>
    <meta charset="utf-8">
    <meta http-equiv="x-ua-compatible" content="ie=edge">
    <meta name="viewport" content="width=device-width,initial-scale=1">
    <meta name="x-apple-disable-message-reformatting">
    <title>Our Latest Top Offers</title>
</head>
<body bgcolor="#f6f6f6" style="-moz-box-sizing: border-box; -ms-text-size-adjust: 100%; -webkit-box-sizing: border-box; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: 100%; background-color: #f6f6f6; box-sizing: border-box; height: 100%; margin: 0; min-width: 100%; padding: 0; width: 100% !important; word-break: break-word;">
<!--header start-->
<br>
   <table class="c_email" bgcolor="#ffffff" align="center" border="0" cellpadding="0" cellspacing="0" width="700" style="background-color: #fff; border: none; border-collapse: collapse; border-spacing: 0; margin: 0 auto; max-width: 640px; padding: 0; table-layout: fixed; text-align: left; vertical-align: top; width: 640px;">
        <!--header-->
        <tr style="padding: 0; text-align: left; vertical-align: top;">
            <td class="c_banner" valign="middle" style="-moz-hyphens: auto; -webkit-hyphens: auto; background-color: #fff; border-bottom: 1px dashed #e3e3e3; border-collapse: collapse !important; color: #616161; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 50px 50px 40px; text-align: left; vertical-align: top; word-wrap: break-word;">
                <table align="left" border="0" cellpadding="0" cellspacing="0" width="100%" style="border: none; border-collapse: collapse; border-spacing: 0; padding: 0; table-layout: fixed; text-align: left; vertical-align: top;">
                    <tr style="padding: 0; text-align: left; vertical-align: top;">
                        <td class="c_banner__logo" valign="middle" style="-moz-hyphens: auto; -webkit-hyphens: auto; border-collapse: collapse !important; color: #616161; display: inline-block; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; height: 40px; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 0; text-align: left; vertical-align: top; width: 136.5px; word-wrap: break-word;">
                            <a href="[url]" style="color: #6b6b6b; text-decoration: none;">
                                <img src="[logo]" width="137" height="40" style="-ms-interpolation-mode: bicubic; border: none; clear: both; display: inline-block; height: 40px; line-height: 100%; margin: 0; max-width: 100%; outline: none; padding: 0; text-decoration: none; width: 136.5px;">
                            </a>
                        </td>
						<td class="c_banner" valign="middle"><h1>[whitelabelName]</h1></td>
                    </tr>
                </table>
            </td>
        </tr>
   </table>		
<!--header end-->'
,
'<!--body start-->
    <table class="c_email" bgcolor="#ffffff" align="center" border="0" cellpadding="0" cellspacing="0" width="700" style="background-color: #fff; border: none; border-collapse: collapse; border-spacing: 0; margin: 0 auto; max-width: 640px; padding: 0; table-layout: fixed; text-align: left; vertical-align: top; width: 640px;">
        <tr style="padding: 0; text-align: left; vertical-align: top;">
            <td class="c_content" valign="top" style="-moz-hyphens: auto; -webkit-hyphens: auto; background-color: #fff; border-collapse: collapse !important; color: #616161; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 50px; text-align: left; vertical-align: top; word-wrap: break-word;">	<h1 style="color: #2b2b2b; display: block; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 31.581px; font-style: normal; font-weight: 700; letter-spacing: normal; line-height: 120%; margin: 0; margin-bottom: 31.581px; text-align: left;">Our Latest Top Offers</h1>
				<p style="color: #616161; display: block; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; letter-spacing: normal; line-height: 170%; margin: 0 0 16px; text-align: left;">Hi {{first_name}} we found these offers that we thought you might like</p><hr style="background-color: #e3e3e3; border: none; height: 1px; margin: 40px auto 40px 0; text-align: left; width: 100px;">
				<table class="c_offers-loop" align="left" border="0" cellpadding="0" cellspacing="0" width="100%" style="border: none; border-collapse: collapse; border-spacing: 0; padding: 0; table-layout: fixed; text-align: left; vertical-align: top;">
					<tr style="padding: 0; text-align: left; vertical-align: top;">
						<td class="c_offers-loop__item" style="-moz-hyphens: auto; -webkit-hyphens: auto; border-collapse: collapse !important; color: #616161; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 0; padding-bottom: 30px; text-align: left; vertical-align: top; word-wrap: break-word;">
							<table class="c_offer" align="left" border="0" cellpadding="0" cellspacing="0" width="100%" style="border: none; border-collapse: collapse; border-spacing: 0; padding: 0; table-layout: fixed; text-align: left; vertical-align: top;">
								[offerList]
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
<!--body end-->'
,
'<!--footer start-->
    <table class="c_email" bgcolor="#ffffff" align="center" border="0" cellpadding="0" cellspacing="0" width="700" style="background-color: #fff; border: none; border-collapse: collapse; border-spacing: 0; margin: 0 auto; max-width: 640px; padding: 0; table-layout: fixed; text-align: left; vertical-align: top; width: 640px;">
        <tr style="padding: 0; text-align: left; vertical-align: top;">
            <td class="c_footer" valign="top" style="-moz-hyphens: auto; -webkit-hyphens: auto; background-color: #fff; border-collapse: collapse !important; border-top: 1px dashed #e3e3e3; color: #616161; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 16px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 40px 50px; text-align: left; vertical-align: top; word-wrap: break-word;">
                <table align="left" border="0" cellpadding="0" cellspacing="0" width="100%" style="border: none; border-collapse: collapse; border-spacing: 0; padding: 0; table-layout: fixed; text-align: left; vertical-align: top;">
                    <tr style="padding: 0; text-align: left; vertical-align: top;">
                        <td class="c_footer__links" valign="middle" style="-moz-hyphens: auto; -webkit-hyphens: auto; border-collapse: collapse !important; color: #949494; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 12.755px; font-style: normal; font-weight: 400; hyphens: auto; letter-spacing: normal; line-height: 170%; padding: 0; padding-bottom: 10px; text-align: left; vertical-align: top; word-wrap: break-word;">
                            <a href="[url]Account/Unsubscribe?email={{email}}" style="color: #6b6b6b; text-decoration: none;">Unsubscribe.</a> &bull;
                            <a href="[url]Account/PrivacyPolicy?country=GB" style="color: #6b6b6b; text-decoration: none;">Privacy Policy</a> &bull;
                            <a href="[url]Account/Terms?country=GB" style="color: #6b6b6b; text-decoration: none;">Terms & Conditions</a> &bull; <a href="[url]Account/ContactUs?country=GB" style="color: #6b6b6b; text-decoration: none;">Contact Us</a>
                        </td>
                    </tr>
                    <tr style="padding: 0; text-align: left; vertical-align: top;">
                        <td class="c_footer__smallprint" valign="middle" style="-moz-hyphens: auto; -webkit-hyphens: auto; border-collapse: collapse !important; color: #949494; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif; font-size: 12.755px; font-style: normal; font-weight: 400; hyphens: none; letter-spacing: normal; line-height: 170%; padding: 0; text-align: left; vertical-align: top; word-wrap: normal;">[whitelabelName] is powered by Exclusive Media Ltd. &copy; Copyright 2020 Exclusive Media Ltd. Registered Address: 15 Hoghton Street, Southport, United Kingdom, PR9 0NS</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
	<!--footer end-->'
,
'( [url] )

*********************
Our Latest Top Offers
*********************

Hi {{fisrt_name}} we found these offers that we thought you might like

[offerList]

 '
,
'Unsubscribe. ( [url]/Account/Unsubscribe?email={{email}} ) Privacy Policy ( [url]/Account/PrivacyPolicy ) Terms & Conditions ( =
[url]/Account/Terms )  Contact Us ( [url]/Account/ContactUs ) [whitelabelName] is powered by Exclusive Media Ltd.  Copyright 2020 Exclusive Media Ltd. Registered Address: 15 Hoghton Street, Southport, United Kingdom, PR9 0NS' 
, 0, @NewsletterEmailType
      )

      SELECT @EmailTemplateId = SCOPE_IDENTITY()
  END
  ELSE
  BEGIN
    SELECT @EmailTemplateId = Id FROM [Exclusive].[EmailTemplates] WHERE [EmailName] = @EmailName
  END

  -- Add  NewsLetter
  IF NOT EXISTS(SELECT * FROM [Marketing].[Newsletter] Where Name = @Name)
  BEGIN
       INSERT INTO [Marketing].[Newsletter]
       (
         [NewsLetterId], [Name], [Description], [Schedule],[Enabled] ,[EmailTemplateId],[OfferListId]
       )
      VALUES
      (
        1, @Name, @Description, NULL, 1, @EmailTemplateId, @NewOfferListId
      )

      SELECT @NewNewsLtrId = SCOPE_IDENTITY() 
  END
  ELSE
  BEGIN
    SELECT @NewNewsLtrId  = Id FROM [Marketing].[Newsletter] Where Name = @Name
  END

  -- Add Campaigns Data
  IF NOT EXISTS(Select * FROM [Marketing].[Campaigns])
  BEGIN
    INSERT INTO [Marketing].[Campaigns]
    (
        [WhiteLabelId], [ContactListId], [ContactListName], [CampaignId], [CampaignName], [SenderId], [Enabled]
    )
    SELECT [Id] as WhiteLabelId, 
           NULL as ContactListId, 
           REPLACE([Name], ' ', '') + '_ContactList' as ContactListName,
           NULL as CampaignId, 
           REPLACE([Name], ' ', '') + '_Campaign' as CampaignName, 
           1068141 as SenderId, 
           1 as Enabled
      FROM [CMS].[WhiteLabelSettings]
  END

  -- ADD Link Data for Each NewsLetter & Campaign Link
  IF NOT EXISTS(Select * FROM [Marketing].[NewsletterCampaignLink] WHERE NewsletterId = @NewsLetterId)
  BEGIN
        INSERT INTO [Marketing].[NewsletterCampaignLink]
        (
            [NewsletterId], [CampaignId], [Enabled]
        )
        SELECT @NewNewsLtrId as NewsletterId,
               Id as CampaignId,
               1 as Enabled
        FROM [Marketing].[Campaigns]
  END
  ELSE
  BEGIN
        SELECT * FROM [Marketing].[NewsletterCampaignLink] Where NewsletterId = @NewsLetterId
  END
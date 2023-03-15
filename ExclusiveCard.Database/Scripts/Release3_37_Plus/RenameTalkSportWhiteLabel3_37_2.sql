  
  BEGIN TRAN
  
  UPDATE [CMS].[WhiteLabelSettings]
  SET 
  [Logo] = 'logo.png',
  [Name] = 'Exclusive talkSPORT Rewards'
  WHERE 
  [Slug] = 'talksport'

  --COMMIT TRAN
  --ROLLBACK TRAN

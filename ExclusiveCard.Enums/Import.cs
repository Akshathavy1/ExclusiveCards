namespace ExclusiveCard.Enums
{
    public enum Import
    {
        /// <summary>File identified and initial details added to Staging.OfferImportFile</summary>
        New = 24,
        /// <summary>File records upload to Staging table complete</summary>
        Uploaded = 66,
        //Migrated = 23,
        /// <summary>Record transfer to offer table complete</summary>
        Processed = 23,

        /// <summary>Records are currently being transfered to staging or offer table</summary>
        Processing = 42,
        /// <summary>Confirmation from user on admin site that they are finished with a file</summary>
        Complete = 22,
        /// <summary>File import or processing failed, not records processed</summary>
        Failed = 67
    }


}
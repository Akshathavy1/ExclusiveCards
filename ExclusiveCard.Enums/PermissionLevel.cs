namespace ExclusiveCard.Enums
{
    public enum PermissionLevel : int
    {
        Everyone = 0,
        LoggedIn = 1,
        PendingAndActiveCards = 2,
        ActiveCards = 3
    }
}
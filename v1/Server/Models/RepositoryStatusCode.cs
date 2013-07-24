namespace Server.Models
{
    public enum RepositoryStatusCode
    {
        OK,
        Failed,
        Unauthorized,
        NotFound,
        UpgradeRequired,
        BadInput
    }
}
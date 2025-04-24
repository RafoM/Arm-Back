namespace TransactionCore.Models.RequestModels
{
    public interface UpdateUserRole
    {
        Guid UserId { get; }
        int RoleId { get; }
    }
}

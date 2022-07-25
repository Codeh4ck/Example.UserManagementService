namespace Example.UserManagementService.Common.Models
{
    public enum UpdateUserEmailResult
    {
        Success,
        InvalidPassword,
        EmailInUse,
        InternalServiceError
    }
}

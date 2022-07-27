using Codelux.Mappers;
using Example.UserManagementService.Common.Models;

namespace Example.UserManagementService.Internal.Mappers
{
    public class UserToBasicUserMapper : MapperBase<User, BasicUser>
    {
        public override BasicUser Map(User model)
        {
            return new()
            {
                Id = model.Id,
                Username = model.Username,
                Email = model.Email,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}

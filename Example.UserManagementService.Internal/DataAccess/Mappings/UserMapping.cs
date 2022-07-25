using Codelux.ServiceStack.OrmLite;
using Example.UserManagementService.Common.Models;

namespace Example.UserManagementService.Internal.DataAccess.Mappings
{
    public class UserMapping : OrmLiteMapping<User>
    {
        public UserMapping()
        {
            MapToSchema("dbo");
            MapToTable("users");

            MapToColumn(x => x.Id, "id");
            MapToColumn(x => x.Username, "username");
            MapToColumn(x => x.Password, "password");
            MapToColumn(x => x.Email, "email");
            MapToColumn(x => x.CreatedAt, "created_at");
            MapToColumn(x => x.UpdatedAt, "updated_at");
        }
    }
}

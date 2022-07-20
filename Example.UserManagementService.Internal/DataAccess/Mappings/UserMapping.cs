using Codelux.ServiceStack.OrmLite;
using Example.UserManagementService.Common.Model;

namespace Example.UserManagementService.Internal.DataAccess.Mappings
{
    public class UserMapping : OrmLiteMapping<User>
    {
        public UserMapping()
        {
            MapToSchema("dbo");
            MapToTable("users");

            MapToColumn(x => x.Id, "id");
            MapToColumn(x => x.Username, "id");
            MapToColumn(x => x.Password, "id");
            MapToColumn(x => x.Email, "id");
            MapToColumn(x => x.CreatedAt, "id");
            MapToColumn(x => x.UpdatedAt, "id");
        }
    }
}

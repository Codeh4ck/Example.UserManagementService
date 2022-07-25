using ServiceStack;
using Codelux.Mappers;
using Codelux.ServiceStack.Utilities;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Internal.Mappers;

namespace Example.UserManagementService.Dependencies
{
    public class MappersModule : DependencyModuleBase
    {
        public MappersModule(ServiceStackHost appHost) : base(appHost) { }

        public override void RegisterDependencies()
        {
            AppHost.Container.RegisterAutoWiredAs<RegisterUserRequestToUserMapper, IMapper<RegisterUserRequest, User>>();
        }
    }
}

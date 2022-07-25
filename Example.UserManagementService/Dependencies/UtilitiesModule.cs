using ServiceStack;
using Codelux.Utilities;
using Codelux.Utilities.Crypto;
using Codelux.ServiceStack.Utilities;
using Example.UserManagementService.Validators.ValidationRules;

namespace Example.UserManagementService.Dependencies
{
    public class UtilitiesModule : DependencyModuleBase
    {
        public UtilitiesModule(ServiceStackHost appHost) : base(appHost) { }

        public override void RegisterDependencies()
        {

            AppHost.Container.RegisterAutoWiredAs<EmailUniqueValidationRule, IEmailUniqueValidationRule>();
            AppHost.Container.RegisterAutoWiredAs<UsernameUniqueValidationRule, IUsernameUniqueValidationRule>();

            AppHost.Container.RegisterAutoWiredAs<ClockService, IClockService>();
            AppHost.Container.RegisterAutoWiredAs<Md5PasswordEncryptor, IPasswordEncryptor>();
        }
    }
}

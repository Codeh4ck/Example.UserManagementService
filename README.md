# Example.UserManagementService

**Example.UserManagementService** is an example microservice based on [ServiceStack](https://github.com/ServiceStack/ServiceStack) that enables user creation, authentication and basic user functionality such as updating a user's e-mail or password. The microservice is split in different projects which are all loaded in the main Web project and exposed on the internet. A description of how and why the microservice is split is below.


## How is the Microservice split?

The microservice is split in different projects, each being a DLL except for the Web one. Every project is named accordingly to represent its intent accurately. The microservice consists of the following projects:

 - **Example.UserManagementService**

This project contains the main functionality of the microservice. This includes defining routes, handinling requests, registering dependencies to the IoC container and validation. This DLL is directly exposed to the internet through the Web project.

 -  **Example.UserManagementService.Client**

This project enables third-party services to consume our microservice. It is basically an HTTP client that sends requests to our microservice and returns the relevant response. Normally, this would be uploaded to a NuGet server to be fed to potential consumers. The .Common project must also be published to a NuGet server as it contains the microservice's models (explained below).

- **Example.UserManagementService.Common**

This project contains all objects that are shared between each project. For example, models, standard errors and constants are defined in the Common project. This allows us to reuse objects and ensure continuity within the microservice and external consumers. This project must be published to a NuGet server, alongside the Client since the models contained here are used in the Client as well.

- **Example.UserManagementService.Internal**

This project contains functionality that should not be directly exposed on the outside. For example, our database communication is performed in the Internal project. Mappers and model mappings are also contained in here. We use a separate project for internal functionality to ensure that responsibility is split across our solution properly and to prevent outside sources from tampering with internal functionality. Additionally, the main microservice component can act as middleware and filter requests going through our Internal project.

- **Example.UserManagementService.Web**

This is an empty ASP .NET project which starts our microservice up and exposes it to the internet. Configuration files are loaded through this project and passed to the main microservice project through the `AppHost` class. Any webserver related configuration should be performed in this project. Other than bootstrapping our service and passing configuration, this project does nothing more.

## Extending the microservice

Let's assume that a programmer wants to implement new functionality on the microservice. For example, a programmer wants to allow users to change their username. Their first step on the approach would be to create the request model.

In the `Example.UserManagementService.Common` project, under the folder `Requests`, we create a new class called `UpdateUserUsernameRequest` that looks like this:

```csharp
using Codelux.Common.Requests;

namespace Example.UserManagementService.Common.Requests
{
    public class UpdateUserUsernameRequest : AuthenticatedRequest
    {
        public string NewUsername { get; set; }
        public string Password { get; set; }
    }
}
```

Then we create a validator for this request in the `Example.UserManagementService` project, under the folder `Validators`, which we call `UpdateUserUsernameRequestValidator` and looks like this:

```csharp
public class UpdateUserUsernameRequestValidator : AbstractValidator<UpdateUserUsernameRequest>
{
    public UpdateUserUsernameRequestValidator(IUsernameUniqueValidationRule usernameUniqueValidationRule)
    {
        usernameUniqueValidationRule.Guard(nameof(usernameUniqueValidationRule));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Please provide a valid user ID.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Please enter your password.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Please enter your desired username")
            .Length(4, 150).WithMessage("Username must be 4 to 150 characters long.")
            .Must(usernameUniqueValidationRule.Matches).WithMessage("Username is in use. Please choose a different username.");
    }
}
```

**Note:** By validating that the request contains a user ID, we know that this request comes from a user who has authenticated in our service. If you look closely above, our request inherits `AuthenticatedRequest` which is basically a request that contains a `UserId` and a `Username` property.

`IUsernameUniqueValidationRule` is injected with `UsernameUniqueValidationRule` from the IoC container. It queries the database to ensure that the selected username is not taken.

---
Now that we have our request and validator objects, we can continue to the next step and create an executor which will handle this request. On `Example.UserManagementService` , we create a new folder under the `Executors` folder which we name `UpdateUserUsernameExecutor` and we create an interface and a class implementing our functionality, as shown below:

- **IUpdateUserUsernameExecutor:**
```csharp
public interface IUpdateUserUsernameExecutor : IExecutor<UpdateUserUsernameRequest, bool> { }
```
We use an interface to adhere to the SOLID principle. Our interface here serves as a "typedef" of `IExecutor<UpdateUserUsernameRequest, bool>`. `IExecutor` is a generic interface which takes an input type and outputs another one. In this case, our executor's input is `UpdateUserUsernameRequest` and the output is `bool` which will tell us if the username has been updated successfully. More on `IExecutor` can be found [in the Codelux library](https://github.com/Codeh4ck/Codelux/tree/main/Codelux/Executors). 

- **UpdateUserUsernameExecutor:**
```csharp
public class UpdateUserUsernameExecutor 
    : ExecutorBase<UpdateUserUsernameRequest, bool>, IUpdateUserUsernameExecutor
{
    private readonly IClockService _clockService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncryptor _passwordEncryptor;

    public UpdateUserUsernameExecutor(IClockService clockService, IUserRepository userRepository, IPasswordEncryptor passwordEncryptor)
    {
        clockService.Guard(nameof(clockService));
        userRepository.Guard(nameof(userRepository));
        passwordEncryptor.Guard(nameof(passwordEncryptor));

        _clockService = clockService;
        _userRepository = userRepository;
        _passwordEncryptor = passwordEncryptor;
    }

    protected override async Task<bool> OnExecuteAsync(UpdateUserUsernameRequest tin, CancellationToken token = new CancellationToken())
    {
        User user = await _userRepository.GetUserByIdAsync(tin.UserId, token).ConfigureAwait(false);
        if (user == null) throw ServiceErrors.UserNotFoundException;

        if (user.Password != _passwordEncryptor.Encrypt(tin.Password))
            return false;

        user.Username = tin.NewUsername;
        user.UpdatedAt = _clockService.Now();

        return await _userRepository.UpdateUserAsync(user, token).ConfigureAwait(false);
    }
}
```

Our executor has three possible results based on different criteria:

- User exists & password is correct: **Username changed, true returned**
- User exists & password is incorrect: **Username not changed, false returned**
- User does not exist: **Exception is thrown**

We can modify this behavior to our liking. We can throw an exception when the user has provided the wrong password or we can return a POCO containing more information, instead of a `bool`. A similar behavior can be observed in [UpdateUserEmailExecutor](https://github.com/Codeh4ck/Example.UserManagementService/blob/main/Example.UserManagementService/Executors/UpdateUserEmailExecutor/UpdateUserEmailExecutor.cs).

---
Now that we have our request, request validator and executor, it's time to enable this functionality on our microserve and expose it to the internet.

First, we register our executor to the IoC container. We will use the `ExecutorsModule` which is a class that registers executors in the IoC container. This class is located in `Example.UserManagementService`, under the `Dependencies` folder.

`ExecutorsModule` should look like the following snippet:

```csharp
public class ExecutorsModule : DependencyModuleBase
{
    public ExecutorsModule(ServiceStackHost appHost) : base(appHost) { }

    public override void RegisterDependencies()
    {
        AppHost.Container.RegisterAutoWiredAs<RegisterUserExecutor, IRegisterUserExecutor>();
        AppHost.Container.RegisterAutoWiredAs<AuthenticateUserExecutor, IAuthenticateUserExecutor>();
        AppHost.Container.RegisterAutoWiredAs<UpdateUserEmailExecutor, IUpdateUserEmailExecutor>();
        AppHost.Container.RegisterAutoWiredAs<UpdateUserPasswordExecutor, IUpdateUserPasswordExecutor>();
        AppHost.Container.RegisterAutoWiredAs<UpdateUserUsernameExecutor, IUpdateUserUsernameExecutor>();
    }
}
```

Then we add a route in the `RouteFeature` class, which is located in the root directory of `Example.UserManagementService` project, like so:

```csharp
 public class RouteFeature : IPlugin
 {
     public void Register(IAppHost appHost)
     {
         appHost.Routes.Add<RegisterUserRequest>("/api/users", ApplyTo.Post);
         appHost.Routes.Add<AuthenticateUserRequest>("/api/users", ApplyTo.Get);
         appHost.Routes.Add<UpdateUserEmailRequest>("/api/users/email", ApplyTo.Put);
         appHost.Routes.Add<UpdateUserPasswordRequest>("/api/users/password", ApplyTo.Put);
         appHost.Routes.Add<UpdateUserUsernameRequest>("/api/users/username", ApplyTo.Put);
     }
 }
```
**Note:** We should always adhere to REST principles. Since our request will update an existing user, we add a route that expects the `PUT` verb.

Finally, we add a request handler on `UserService` class, which is also located in the root directory of `Example.UserManagementService` project:

```csharp
public class UserService : Service
{
    private readonly IRegisterUserExecutor _registerUserExecutor;
    private readonly IAuthenticateUserExecutor _authenticateUserExecutor;
    private readonly IUpdateUserEmailExecutor _updateUserEmailExecutor;
    private readonly IUpdateUserPasswordExecutor _updateUserPasswordExecutor;
    private readonly IUpdateUserUsernameExecutor _updateUserUsernameExecutor;

    public UserService(
        IRegisterUserExecutor registerUserExecutor,
        IAuthenticateUserExecutor authenticateUserExecutor,
        IUpdateUserEmailExecutor updateUserEmailExecutor,
        IUpdateUserPasswordExecutor updateUserPasswordExecutor, 
        IUpdateUserUsernameExecutor updateUserUsernameExecutor)
    {
        registerUserExecutor.Guard(nameof(registerUserExecutor));
        authenticateUserExecutor.Guard(nameof(authenticateUserExecutor));
        updateUserEmailExecutor.Guard(nameof(updateUserEmailExecutor));
        updateUserPasswordExecutor.Guard(nameof(updateUserPasswordExecutor));
        updateUserUsernameExecutor.Guard(nameof(updateUserUsernameExecutor));

        _registerUserExecutor = registerUserExecutor;
        _authenticateUserExecutor = authenticateUserExecutor;
        _updateUserEmailExecutor = updateUserEmailExecutor;
        _updateUserPasswordExecutor = updateUserPasswordExecutor;
        _updateUserUsernameExecutor = updateUserUsernameExecutor;
    }

    public Task<RegisterUserResponse> Post(RegisterUserRequest request) => _registerUserExecutor.ExecuteAsync(request);
    public Task<AuthenticateUserResponse> Get(AuthenticateUserRequest request) => _authenticateUserExecutor.ExecuteAsync(request);
    public Task<UpdateUserEmailResponse> Put(UpdateUserEmailRequest request) => _updateUserEmailExecutor.ExecuteAsync(request);
    public Task<UpdateUserPasswordResponse> Put(UpdateUserPasswordRequest request) => _updateUserPasswordExecutor.ExecuteAsync(request);
    public Task<bool> Put(UpdateUserUsernameRequest request) => _updateUserUsernameExecutor.ExecuteAsync(request);
}
```

---

This is it! We have added new functionality to our service in a few simple steps. This design ensures that any developer, of any seniority, can easily pick up the project and extend it or maintain it. Our architecture has ensured that all responsibility is split and therefore, the microservice is very scallable, maintainable and testable in its entirety. We can additionally add a new function in our `UserServiceClient` class in the `Example.UserManagementService.Client` project to expose the new functionality to consumers who use our NuGet package to interact with our microservice.

## What if I want a second service that falls under the same business domain?

Let's assume a programmer wants to add support for user settings and does not want to create a separate microservice for it. Since user settings management could be a concept applied to a user management service, we can extend our microservice to support this new addition. A programmer would have to create a new model representing user settings. Then he would have to create a repository to interact with the new database table that holds user settings, after making sure that the model is properly mapped to the table. You can see how a model is mapped to a table by using the mapping of the `User` model to the `users` table as reference: [UserMapping.cs](https://github.com/Codeh4ck/Example.UserManagementService/blob/main/Example.UserManagementService.Internal/DataAccess/Mappings/UserMapping.cs)

A mapping basically tells `OrmLite` in which column of a given table a property can be found. These mappings are instantiated when our service is started and you only need to create a mapping class.

Finally, when you create your new model and repository, the rest of the process is the same as the one described in our previous step. 

## Why choose this approach over the classic ASP .NET approach?

### A few notes about ServiceStack
I personally choose to create my RESTful APIs with this approach for multiple reasons. First of all, ServiceStack is an established framework which has proven itself to be faster on multiple aspects compared to vanilla ASP .NET. 

Among other things, it requires no XML configuration, has a built-in IoC framework, it's host agnostic, highly testable, it enforces a message-based & model-driven development and it also enforces good practises in web service development. Additionally, it's highly smart and can infer a great deal of intelligence from strongly typed DTOs. Finally, it's a very mature framework, with over 10 years of development and a company backing it up. To add an interesting remark, the CTO of ServiceStack himself answers questions about ServiceStack on StackOverflow quite fastly.

### A few notes about the architecture of the microservice

The microservice is split in different components to ensure adhesion to the SOLID principle. This means that our service has highly testable components and is easily scallable. As demonstrated above, a programmer who begins working on a similar architecture will be able to pickup development very fast, with minimum explanation needed. The code is very clean and easy to follow. Furthermore, small classes that perform only one functionality are easy to test. We can test every single concrete class of our project since they are loosely coupled and therefore we can mock every dependency they have. 


## I have a question that I cannot find an answer to in this documentation!

Feel free to create a new issue with your inquiry. I will try to answer as fast as I can. Any question is welcome!

# Contributing

## Found an issue?

Please report any issues you have found by [creating a new issue](https://github.com/Codeh4ck/Example.UserManagementService/issues). We will review the case and if it is indeed a problem with the code, I will try to fix it as soon as possible. I want to maintain a healthy and bug-free standard for our code. Additionally, if you have a solution ready for the issue please submit a pull request. 

## Submitting pull requests

Before submitting a pull request to the repository please ensure the following:

* Your code follows the naming conventions [suggested by Microsoft](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
* Your code works flawlessly, is fault tolerant and it does not break the library or aspects of it
* Your code follows proper object oriented design principles. Use interfaces!

Your code will be reviewed and if it is found suitable it will be merged. Please understand that the final decision always rests with me. By submitting a pull request you automatically agree that I hold the right to accept or deny a pull request based on my own criteria.

## Contributor License Agreement

By contributing your code to **Example.UserManagementService** you grant Nikolas Andreou a non-exclusive, irrevocable, worldwide, royalty-free, sublicenseable, transferable license under all of Your relevant intellectual property rights (including copyright, patent, and any other rights), to use, copy, prepare derivative works of, distribute and publicly perform and display the Contributions on any licensing terms, including without limitation: (a) open source licenses like the MIT license; and (b) binary, proprietary, or commercial licenses. Except for the licenses granted herein, You reserve all right, title, and interest in and to the Contribution.

You confirm that you are able to grant us these rights. You represent that you are legally entitled to grant the above license. If your employer has rights to intellectual property that you create, You represent that you have received permission to make the contributions on behalf of that employer, or that your employer has waived such rights for the contributions.

You represent that the contributions are your original works of authorship and to your knowledge, no other person claims, or has the right to claim, any right in any invention or patent related to the contributions. You also represent that you are not legally obligated, whether by entering into an agreement or otherwise, in any way that conflicts with the terms of this license.

Nikolas Andreou acknowledges that, except as explicitly described in this agreement, any contribution which you provide is on an "as is" basis, without warranties or conditions of any kind, either express or implied, including, without limitation, any warranties or conditions of title, non-infringement, merchantability, or fitness for a particular purpose.
using System.Text;
using System.Web;
using Codelux.Http;
using Newtonsoft.Json;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;

namespace Example.UserManagementService.Client
{
    public class UserServiceClient : ServiceClient, IUserServiceClient
    {
        private readonly Uri _endpoint;
        private Uri _registerUserUri;
        private Uri _authenticateUserUri;
        private Uri _updateUserEmailUri;
        private Uri _updateUserPasswordUri;

        public UserServiceClient(Uri endpoint) : base(ServiceConstants.ServiceName, endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            _endpoint = endpoint;

            Setup();
        }

        public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request, CancellationToken token = default)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, _registerUserUri);

            string json = JsonConvert.SerializeObject(request);
            message.Content = new StringContent(json, Encoding.UTF8);

            return await MakeRequestAndResponseAsync<RegisterUserResponse>(request, message, false, token).ConfigureAwait(false);
        }

        public async Task<BasicUser> AuthenticateUserAsync(AuthenticateUserRequest request, CancellationToken token = default)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get,
                _authenticateUserUri + $"?Username={HttpUtility.UrlEncode(request.Username)}&Password={HttpUtility.UrlEncode(request.Password)}");

            return await MakeRequestAndResponseAsync<BasicUser>(request, message, true, token).ConfigureAwait(false);
        }

        public async Task<UpdateUserEmailResponse> UpdateUserEmailAsync(UpdateUserEmailRequest request, CancellationToken token = default)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, _updateUserEmailUri);

            string json = JsonConvert.SerializeObject(request);
            message.Content = new StringContent(json, Encoding.UTF8);

            return await MakeRequestAndResponseAsync<UpdateUserEmailResponse>(request, message, true, token).ConfigureAwait(false);
        }

        public async Task<UpdateUserPasswordResponse> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CancellationToken token = default)
        {
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, _updateUserPasswordUri);

            string json = JsonConvert.SerializeObject(request);
            message.Content = new StringContent(json, Encoding.UTF8);

            return await MakeRequestAndResponseAsync<UpdateUserPasswordResponse>(request, message, true, token).ConfigureAwait(false);
        }

        private void Setup()
        {
            _registerUserUri = new Uri(_endpoint, "/api/users");
            _authenticateUserUri = new Uri(_endpoint, "/api/users");
            _updateUserEmailUri = new Uri(_endpoint, "/api/users/email");
            _updateUserPasswordUri = new Uri(_endpoint, "/api/users/passwords");
        }
    }
}
using System.Net.Http.Headers;
using System.Text;
using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Identity.Core.Models;
using Devkit.Common.Identity.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Devkit.Common.Identity.Providers.Keycloak
{
    public class KeycloakIdentityService(HttpClient httpClient, IOptions<IdentityOptions> options)
        : IAuthenticationService, IUserService
    {
        private readonly KeycloakOptions _options = options.Value.Keycloak;

        #region IAuthenticationService (Login/Logout)

        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            try
            {
                var url = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _options.ClientId),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", request.Username),
                    new KeyValuePair<string, string>("password", request.Password)
                });

                var response = await httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new AuthResponse { IsSuccess = false, ErrorMessage = responseString };
                }

                dynamic json = JsonConvert.DeserializeObject(responseString)!;
                return new AuthResponse
                {
                    IsSuccess = true,
                    AccessToken = json.access_token,
                    RefreshToken = json.refresh_token,
                    ExpiresIn = json.expires_in
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                var url = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/logout";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _options.ClientId),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                    new KeyValuePair<string, string>("refresh_token", refreshToken)
                });

                var response = await httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region IUserService (CRUD & Management)

        public async Task<string> CreateUserAsync(CreateUserDto userDto)
        {
            var token = await GetAdminAccessTokenAsync();
            var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users";

            var newUser = new
            {
                username = userDto.Username,
                email = userDto.Email,
                firstName = userDto.FirstName,
                lastName = userDto.LastName,
                enabled = true,
                credentials = new[] { new { type = "password", value = userDto.Password, temporary = false } }
            };

            var response = await SendJsonRequestAsync(HttpMethod.Post, url, newUser, token);

            if (response.IsSuccessStatusCode)
            {
                var locationHeader = response.Headers.Location?.ToString();
                return locationHeader?.Split('/').LastOrDefault() ?? string.Empty;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"CreateUser Failed: {error}");
        }

        public async Task UpdateUserAsync(UpdateUserDto userDto)
        {
            var token = await GetAdminAccessTokenAsync();
            var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userDto.Id}";

            var updateModel = new
            {
                firstName = userDto.FirstName,
                lastName = userDto.LastName,
                email = userDto.Email
            };

            var response = await SendJsonRequestAsync(HttpMethod.Put, url, updateModel, token);
            EnsureSuccess(response, "UpdateUser");
        }

        public async Task DeleteUserAsync(string userId)
        {
            var token = await GetAdminAccessTokenAsync();
            var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}";

            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            EnsureSuccess(response, "DeleteUser");
        }

        public async Task<UserDetailDto> GetUserByIdAsync(string userId)
        {
            var token = await GetAdminAccessTokenAsync();
            var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            EnsureSuccess(response, "GetUserById");

            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic user = JsonConvert.DeserializeObject(jsonString)!;

            return new UserDetailDto
            {
                Id = user.id,
                Username = user.username,
                Email = user.email,
                FirstName = user.firstName,
                LastName = user.lastName,
                IsEnabled = user.enabled,
                CreatedTimestamp = user.createdTimestamp
            };
        }

        public async Task SetUserStatusAsync(string userId, bool isEnabled)
        {
            var token = await GetAdminAccessTokenAsync();
            var url = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}";

            var model = new { enabled = isEnabled };
            var response = await SendJsonRequestAsync(HttpMethod.Put, url, model, token);
            EnsureSuccess(response, "SetUserStatus");
        }

        public async Task SendForgotPasswordEmailAsync(string email)
        {
            var token = await GetAdminAccessTokenAsync();

            var searchUrl = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users?email={email}";
            var request = new HttpRequestMessage(HttpMethod.Get, searchUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var searchResponse = await httpClient.SendAsync(request);
            var searchJson = await searchResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<dynamic>>(searchJson);

            if (users == null || users.Count == 0)
                throw new Exception("Kullanıcı bulunamadı.");

            string userId = users[0].id;

            var actionUrl = $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}/execute-actions-email";
            var actions = new[] { "UPDATE_PASSWORD" };

            var response = await SendJsonRequestAsync(HttpMethod.Put, actionUrl, actions, token);
            EnsureSuccess(response, "SendForgotPasswordEmail");
        }

        #endregion

        #region Helpers

        private async Task<string> GetAdminAccessTokenAsync()
        {
            var url = $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token";
            var content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            ]);

            var response = await httpClient.PostAsync(url, content);
            if(!response.IsSuccessStatusCode)
                 throw new Exception("Keycloak Admin Token alınamadı. Service Account yetkilerini kontrol edin.");

            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(jsonString)!;
            return json.access_token;
        }

        private async Task<HttpResponseMessage> SendJsonRequestAsync(HttpMethod method, string url, object content, string token)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(content, settings);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await httpClient.SendAsync(request);
        }

        private void EnsureSuccess(HttpResponseMessage response, string actionName)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"{actionName} Failed. Status: {response.StatusCode}, Detail: {error}");
            }
        }

        #endregion
    }
}
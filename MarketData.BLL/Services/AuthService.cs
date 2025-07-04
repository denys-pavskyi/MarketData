using System.Net;
using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Other;
using MarketData.BLL.Models.Responses;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Net.Http;

namespace MarketData.BLL.Services;

public class AuthService: IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly FintachartsSettings _settings;

    private string _accessToken;
    private string _refreshToken;
    private DateTime _tokenExpiresAt;
    private DateTime _refreshTokenExpiresAt;

    public AuthService(IHttpClientFactory httpClientFactory, IOptions<FintachartsSettings> options)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _settings = options.Value;
    }

    public async Task<Result<string>> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiresAt)
        {
            return Result<string>.Success(_accessToken);
        }

        if (!string.IsNullOrWhiteSpace(_refreshToken) && DateTime.UtcNow < _refreshTokenExpiresAt)
        {
            var refreshResult = await RefreshAccessTokenAsync();
            if (refreshResult.IsSuccess)
            {
                return Result<string>.Success(_accessToken);
            }
        }

        var authResult = await AuthenticateAsync();
        if (authResult.IsSuccess)
        {
            return Result<string>.Success(_accessToken);
        }

        return Result<string>.Failure(authResult.Error!);
    }

    private async Task<Result> AuthenticateAsync()
    {
        var formData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", "app-cli" },
            { "username", _settings.Username },
            { "password", _settings.Password }
        };

        var content = new FormUrlEncodedContent(formData);
        var tokenUrl = $"/identity/realms/fintatech/protocol/openid-connect/token";

        var response = await _httpClient.PostAsync(tokenUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure(new ErrorResponse
            {
                HttpCode = HttpStatusCode.Forbidden,
                Message = "Failed to authenticate with Fintacharts API"
            });
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
        {
            return Result.Failure(new ErrorResponse
            {
                HttpCode = HttpStatusCode.Forbidden,
                Message = "Empty or invalid token received"
            });
        }

        _accessToken = result.AccessToken;
        _refreshToken = result.RefreshToken;


        _tokenExpiresAt = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60);
        _refreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(result.RefreshExpiresIn - 60);

        return Result.Success();
    }


    private async Task<Result> RefreshAccessTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(_refreshToken))
        {
            return Result.Failure(new ErrorResponse
            {
                HttpCode = HttpStatusCode.Unauthorized,
                Message = "Refresh token is missing"
            });
        }

        var formData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", "app-cli" },
            { "refresh_token", _refreshToken }
        };

        var content = new FormUrlEncodedContent(formData);
        var tokenUrl = $"/identity/realms/fintatech/protocol/openid-connect/token";

        try
        {
            var response = await _httpClient.PostAsync(tokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure(new ErrorResponse
                {
                    HttpCode = HttpStatusCode.Unauthorized,
                    Message = "Refresh token expired or invalid"
                });
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            {
                return Result.Failure(new ErrorResponse
                {
                    HttpCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to parse refresh token response"
                });
            }

            _accessToken = result.AccessToken;
            _refreshToken = result.RefreshToken;

            _tokenExpiresAt = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60);
            _refreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(result.RefreshExpiresIn - 60);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new ErrorResponse
            {
                HttpCode = HttpStatusCode.InternalServerError,
                Message = $"Exception during token refresh: {ex.Message}"
            });
        }
    }

}
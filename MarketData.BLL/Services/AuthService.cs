using System.Net;
using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Other;
using MarketData.BLL.Models.Responses;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace MarketData.BLL.Services;

public class AuthService: IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly FintachartsSettings _settings;

    private string _accessToken;
    private string _refreshToken;
    private DateTime _tokenExpiresAt;

    public AuthService(HttpClient httpClient, IOptions<FintachartsSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
    }

    public async Task<Result<string>> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiresAt)
        {
            return Result<string>.Success(_accessToken);
        }

        var authResult = await AuthenticateAsync();
        if (!authResult.IsSuccess)
        {
            return Result<string>.Failure(authResult.Error!);
        }

        return Result<string>.Success(_accessToken);
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
        var tokenUrl = $"{_settings.ApiUri}/identity/realms/fintatech/protocol/openid-connect/token";

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

        return Result.Success();
    }


}
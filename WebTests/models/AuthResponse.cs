using System.Text.Json.Serialization;

namespace WebTests.models;

public class AuthResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}
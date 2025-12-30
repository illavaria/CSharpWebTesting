using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebTests.models;

namespace WebTests;

public class TestBase
{
    protected HttpClient _client;
    protected string _booksEndpoint;
    private string _access_token;

    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(TestConfig.Config["ApiSettings:BaseUrl"])
        };
        
        var authUrl = TestConfig.Config["AuthSettings:AuthEndpoint"];
        var payload = new Dictionary<string, string>
        {
            ["client_id"] = TestConfig.Config["AuthSettings:ClientId"],
            ["client_secret"] = TestConfig.Config["AuthSettings:ClientSecret"],
            ["scope"] = TestConfig.Config["AuthSettings:Scope"],
            ["grant_type"] = TestConfig.Config["AuthSettings:GrantType"]
        };

        var authResponse = await new HttpClient().PostAsync(
            authUrl,
            new FormUrlEncodedContent(payload));

        authResponse.EnsureSuccessStatusCode();
        var responseContent = await authResponse.Content.ReadAsStringAsync();

        var auth = System.Text.Json.JsonSerializer.Deserialize<AuthResponse>(responseContent);
        _access_token = auth.AccessToken;

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _access_token);
        
        _booksEndpoint = TestConfig.Config["ApiSettings:BooksEndpoint"];
    }

    // [SetUp]
    // public async Task Setup()
    // {
    //     var response = await _client.PostAsJsonAsync(
    //         TestConfig.Config["ApiSettings:BooksEndpoint"],
    //         new
    //         {
    //             title = "Test Book",
    //             author = "Test Author",
    //             publishedDate = "2025-12-30T17:58:05.248Z",
    //             isbn = "1234567890",
    //         });
    //     response.EnsureSuccessStatusCode();
    //
    //     var book = await response.Content.ReadFromJsonAsync<Book>();
    //     _testBookId = book.Id;
    // }
    //
    // [TearDown]
    // public async Task TearDown()
    // {
    //     await _client.DeleteAsync(
    //         $"{TestConfig.Config["ApiSettings:BooksEndpoint"]}/{_testBookId}");
    // }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
    }
}
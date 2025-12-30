using System.Net;
using System.Net.Http.Json;
using WebTests.models;
using Allure.NUnit;
using NLog;

namespace WebTests;

[AllureNUnit]
public class TestDelete: TestBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private string _testBookId;

    [SetUp]
    public async Task Setup()
    {
        Logger.Info("Setting up TestDelete.");

        var createRequest = new BookCreate
        {
            Title = "Delete Test Book",
            Author = "Delete Author",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "DELETE-123"
        };

        var response = await _client.PostAsJsonAsync(_booksEndpoint, createRequest);
        response.EnsureSuccessStatusCode();

        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        _testBookId = createdBook.Id;
    }

    [Test]
    public async Task DeleteBook_ValidId_Returns204AndBookIsNoLongerRetrievable()
    {
        Logger.Info("Starting DeleteBook_ValidId_Returns204AndBookIsNoLongerRetrievable.");

        var deleteResponse = await _client.DeleteAsync($"{_booksEndpoint}/{_testBookId}");

        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var getResponse = await _client.GetAsync($"{_booksEndpoint}/{_testBookId}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        Logger.Info("Finished DeleteBook_ValidId_Returns204AndBookIsNoLongerRetrievable.");
    }

    [Test]
    public async Task DeleteBook_NonExistentId_Returns404()
    {
        Logger.Info("Starting DeleteBook_NonExistentId_Returns404.");

        var nonExistentId = Guid.NewGuid().ToString();

        var response = await _client.DeleteAsync($"{_booksEndpoint}/{nonExistentId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        Logger.Info("Finished DeleteBook_NonExistentId_Returns404.");
    }

    [Test]
    public async Task DeleteBook_InvalidIdFormat_Returns400()
    {
        Logger.Info("Starting DeleteBook_InvalidIdFormat_Returns400.");

        var invalidId = "invalid-id";

        var response = await _client.DeleteAsync($"{_booksEndpoint}/{invalidId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        Logger.Info("Finished DeleteBook_InvalidIdFormat_Returns400.");
    }

    [TearDown]
    public async Task TearDown()
    {
        Logger.Info("Tearing down TestDelete.");

        if (!string.IsNullOrEmpty(_testBookId))
        {
            await _client.DeleteAsync($"{_booksEndpoint}/{_testBookId}");
        }

        TestReportCollector.Record($"Delete book tests finished for: {TestContext.CurrentContext.Test.Name}");
    }
}

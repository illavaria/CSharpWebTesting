using System.Net;
using System.Net.Http.Json;
using WebTests.models;
using Allure.NUnit;
using NLog;

namespace WebTests;

[AllureNUnit]
public class TestUpdate: TestBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private string _testBookId;
    private BookCreate _originalBook;

    [SetUp]
    public async Task Setup()
    {
        Logger.Info("Setting up TestUpdate.");

        _originalBook = new BookCreate
        {
            Title = "Update Test Book",
            Author = "Update Author",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "UPDATE-123"
        };

        var response = await _client.PostAsJsonAsync(_booksEndpoint, _originalBook);
        response.EnsureSuccessStatusCode();

        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        _testBookId = createdBook.Id;
    }

    [Test]
    public async Task UpdateBook_ValidData_UpdatesBookAndReturnsUpdatedData()
    {
        Logger.Info("Starting UpdateBook_ValidData_UpdatesBookAndReturnsUpdatedData.");

        var updateRequest = new BookCreate
        {
            Title = "Updated Title",
            Author = "Updated Author",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "UPDATE-999"
        };

        var response = await _client.PutAsJsonAsync(
            $"{_booksEndpoint}/{_testBookId}",
            updateRequest
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        Logger.Info("Finished UpdateBook_ValidData_UpdatesBookAndReturnsUpdatedData.");
    }

    [Test]
    public async Task UpdateBook_NonExistentId_Returns404()
    {
        Logger.Info("Starting UpdateBook_NonExistentId_Returns404.");

        var nonExistentId = Guid.NewGuid().ToString();

        var updateRequest = new BookCreate
        {
            Title = "Does Not Matter",
            Author = "Nobody",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "NO-BOOK"
        };

        var response = await _client.PutAsJsonAsync(
            $"{_booksEndpoint}/{nonExistentId}",
            updateRequest
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        Logger.Info("Finished UpdateBook_NonExistentId_Returns404.");
    }

    [Test]
    public async Task UpdateBook_InvalidIdFormat_Returns400()
    {
        Logger.Info("Starting UpdateBook_InvalidIdFormat_Returns400.");

        var invalidId = "invalid-id";

        var updateRequest = new BookCreate
        {
            Title = "Does Not Matter",
            Author = "Nobody",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "INVALID-ID"
        };

        var response = await _client.PutAsJsonAsync(
            $"{_booksEndpoint}/{invalidId}",
            updateRequest
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        Logger.Info("Finished UpdateBook_InvalidIdFormat_Returns400.");
    }

    [TearDown]
    public async Task TearDown()
    {
        Logger.Info("Tearing down TestUpdate.");

        if (!string.IsNullOrEmpty(_testBookId))
        {
            await _client.DeleteAsync($"{_booksEndpoint}/{_testBookId}");
        }

        TestReportCollector.Record($"Update book tests finished for: {TestContext.CurrentContext.Test.Name}");
    }
}

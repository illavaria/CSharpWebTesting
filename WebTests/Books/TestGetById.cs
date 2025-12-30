using System.Net;
using System.Net.Http.Json;
using WebTests.models;
using Allure.NUnit;
using NLog;

namespace WebTests;

[AllureNUnit]
public class TestGetById: TestBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private string _testBookId;
    private BookCreate _bookRequest;

    [SetUp]
    public async Task Setup()
    {
        Logger.Info("Setting up TestGetById.");

        _bookRequest = new BookCreate
        {
            Title = "GetById Test Book",
            Author = "GetById Author",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "GETBYID-123"
        };

        var response = await _client.PostAsJsonAsync(_booksEndpoint, _bookRequest);
        response.EnsureSuccessStatusCode();

        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        _testBookId = createdBook.Id;
    }

    [Test]
    public async Task GetBookById_ValidId_ReturnsCorrectBook()
    {
        Logger.Info("Starting GetBookById_ValidId_ReturnsCorrectBook.");

        var response = await _client.GetAsync($"{_booksEndpoint}/{_testBookId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var book = await response.Content.ReadFromJsonAsync<Book>();
        Assert.That(book, Is.Not.Null);
        Assert.That(book.Id, Is.EqualTo(_testBookId));
        Assert.That(book.Title, Is.EqualTo(_bookRequest.Title));
        Assert.That(book.Author, Is.EqualTo(_bookRequest.Author));
        Assert.That(book.PublishedDate, Is.EqualTo(_bookRequest.PublishedDate));
        Assert.That(book.ISBN, Is.EqualTo(_bookRequest.ISBN));

        Logger.Info("Finished GetBookById_ValidId_ReturnsCorrectBook.");
    }

    [Test]
    public async Task GetBookById_NonExistentId_Returns404()
    {
        Logger.Info("Starting GetBookById_NonExistentId_Returns404.");

        var nonExistentId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync($"{_booksEndpoint}/{nonExistentId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        Logger.Info("Finished GetBookById_NonExistentId_Returns404.");
    }

    [Test]
    public async Task GetBookById_InvalidIdFormat_Returns400()
    {
        Logger.Info("Starting GetBookById_InvalidIdFormat_Returns400.");

        var invalidId = "invalid-id";

        var response = await _client.GetAsync($"{_booksEndpoint}/{invalidId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        Logger.Info("Finished GetBookById_InvalidIdFormat_Returns400.");
    }

    [TearDown]
    public async Task TearDown()
    {
        Logger.Info("Tearing down TestGetById.");

        if (!string.IsNullOrEmpty(_testBookId))
        {
            await _client.DeleteAsync($"{_booksEndpoint}/{_testBookId}");
        }

        TestReportCollector.Record($"Get by id tests finished for: {TestContext.CurrentContext.Test.Name}");
    }
}

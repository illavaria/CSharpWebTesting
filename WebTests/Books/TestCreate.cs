using System.Net;
using System.Net.Http.Json;
using WebTests.models;
using Allure.NUnit;
using NLog;

namespace WebTests;

[AllureNUnit]
public class TestCreate : TestBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private string _testBookId;
    private BookCreate _validBook;

    [SetUp]
    public async Task Setup()
    {
        Logger.Info("Setting up TestCreate.");

        _validBook = new BookCreate
        {
            Title = "Test Book",
            Author = "Test Author",
            PublishedDate = "2025-12-30T17:58:05.248Z",
            ISBN = "123456789"
        };
    }

    [Test]
    public async Task CreateBook_ValidRequest_Returns201AndCorrectBody()
    {
        Logger.Info("Starting CreateBook_ValidRequest_Returns201AndCorrectBody.");

        var response = await _client.PostAsJsonAsync(_booksEndpoint, _validBook);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var created_book = await response.Content.ReadFromJsonAsync<Book>();
        _testBookId = created_book.Id;
        Assert.That(created_book.Title, Is.EqualTo(_validBook.Title));
        Assert.That(created_book.Author, Is.EqualTo(_validBook.Author));
        Assert.That(created_book.PublishedDate, Is.EqualTo(_validBook.PublishedDate));
        Assert.That(created_book.Id, Is.Not.Null.Or.Empty);

        Logger.Info("Finished CreateBook_ValidRequest_Returns201AndCorrectBody.");
    }

    [Test]
    public async Task CreateBook_DuplicateBook_ReturnsConflictOrBadRequest()
    {
        Logger.Info("Starting CreateBook_DuplicateBook_ReturnsConflictOrBadRequest.");

        var first_response = await _client.PostAsJsonAsync(_booksEndpoint, _validBook);
        Assert.That(first_response.IsSuccessStatusCode, Is.True);
        var created_book = await first_response.Content.ReadFromJsonAsync<Book>();
        _testBookId = created_book.Id;

        var duplicate_response = await _client.PostAsJsonAsync(_booksEndpoint, _validBook);

        Assert.That(duplicate_response.StatusCode == HttpStatusCode.Conflict);

        Logger.Info("Finished CreateBook_DuplicateBook_ReturnsConflictOrBadRequest.");
    }

    [TearDown]
    public async Task TearDown()
    {
        Logger.Info("Tearing down TestCreate.");

        await _client.DeleteAsync(
            $"{_booksEndpoint}/{_testBookId}");

        TestReportCollector.Record($"Create book tests finished for: {TestContext.CurrentContext.Test.Name}");
    }
}

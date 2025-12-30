using System.Net;
using System.Net.Http.Json;
using WebTests.models;

namespace WebTests;

public class TestGetById: TestBase
{
    private string _testBookId;
    private BookCreate _bookRequest;

    [SetUp]
    public async Task Setup()
    {
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
        var response = await _client.GetAsync($"{_booksEndpoint}/{_testBookId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var book = await response.Content.ReadFromJsonAsync<Book>();
        Assert.That(book, Is.Not.Null);
        Assert.That(book.Id, Is.EqualTo(_testBookId));
        Assert.That(book.Title, Is.EqualTo(_bookRequest.Title));
        Assert.That(book.Author, Is.EqualTo(_bookRequest.Author));
        Assert.That(book.PublishedDate, Is.EqualTo(_bookRequest.PublishedDate));
        Assert.That(book.ISBN, Is.EqualTo(_bookRequest.ISBN));
    }

    [Test]
    public async Task GetBookById_NonExistentId_Returns404()
    {
        var nonExistentId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync($"{_booksEndpoint}/{nonExistentId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetBookById_InvalidIdFormat_Returns400()
    {
        var invalidId = "invalid-id";

        var response = await _client.GetAsync($"{_booksEndpoint}/{invalidId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TearDown]
    public async Task TearDown()
    {
        if (!string.IsNullOrEmpty(_testBookId))
        {
            await _client.DeleteAsync($"{_booksEndpoint}/{_testBookId}");
        }
    }
}
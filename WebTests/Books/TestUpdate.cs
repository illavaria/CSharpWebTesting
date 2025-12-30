using System.Net;
using System.Net.Http.Json;
using WebTests.models;

namespace WebTests;

public class TestUpdate: TestBase
{
    private string _testBookId;
    private BookCreate _originalBook;

    [SetUp]
    public async Task Setup()
    {
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

    }

    [Test]
    public async Task UpdateBook_NonExistentId_Returns404()
    {
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
    }

    [Test]
    public async Task UpdateBook_InvalidIdFormat_Returns400()
    {
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
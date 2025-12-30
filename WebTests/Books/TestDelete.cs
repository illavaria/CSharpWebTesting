using System.Net;
using System.Net.Http.Json;
using WebTests.models;

namespace WebTests;

public class TestDelete: TestBase
{
    private string _testBookId;

    [SetUp]
    public async Task Setup()
    {
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
        var deleteResponse = await _client.DeleteAsync($"{_booksEndpoint}/{_testBookId}");

        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var getResponse = await _client.GetAsync($"{_booksEndpoint}/{_testBookId}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteBook_NonExistentId_Returns404()
    {
        var nonExistentId = Guid.NewGuid().ToString();

        var response = await _client.DeleteAsync($"{_booksEndpoint}/{nonExistentId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteBook_InvalidIdFormat_Returns400()
    {
        var invalidId = "invalid-id";

        var response = await _client.DeleteAsync($"{_booksEndpoint}/{invalidId}");

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
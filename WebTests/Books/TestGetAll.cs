using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using WebTests.models;

namespace WebTests;

public class TestGetAll: TestBase
{
    private List<string> _testBookIds = [];
    private List<BookCreate> _books = [];

    [SetUp]
    public async Task Setup()
    {
        for (var i = 0; i < 3; i++)
        {
            var create_book = new BookCreate
            {
                Title = $"Test Book {i}",
                Author = "Test Author",
                PublishedDate = "2025-12-30T17:58:05.248Z",
                ISBN = $"{i}00000"
            };
            _books.Add(create_book);
            var response = await _client.PostAsJsonAsync(_booksEndpoint, create_book);
                                                
            response.EnsureSuccessStatusCode();
    
            var book = await response.Content.ReadFromJsonAsync<Book>();
            _testBookIds.Add(book.Id);
        }
    }
    
    [Test]
    public async Task GetAllBooks_ReturnsListOfValidBooks()
    {
        var uri = QueryHelpers.AddQueryString(
            _booksEndpoint,
            "Take",
            "100"
        );

        var response = await _client.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var books = await response.Content.ReadFromJsonAsync<List<Book>>();
        Assert.That(books, Is.Not.Null);
        Assert.That(books.Count, Is.GreaterThanOrEqualTo(_testBookIds.Count));

        foreach (var testBookId in _testBookIds)
        {
            var book = books.FirstOrDefault(b => b.Id == testBookId);
            Assert.That(book, Is.Not.Null);
            var originalBook = _books[_testBookIds.IndexOf(testBookId)];
            Assert.That(book.Title, Is.EqualTo(originalBook.Title));
            Assert.That(book.Author, Is.EqualTo(originalBook.Author));
            Assert.That(book.PublishedDate, Is.EqualTo(originalBook.PublishedDate));
            Assert.That(book.ISBN, Is.EqualTo(originalBook.ISBN));
        }
    }
    
    [TearDown]
    public async Task TearDown()
    {
        foreach (var testBookId in _testBookIds)
        {
            await _client.DeleteAsync(
                        $"{TestConfig.Config["ApiSettings:BooksEndpoint"]}/{testBookId}");
        }
    }
}
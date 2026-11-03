using BookNexusAPI.Controllers;
using BookNexusAPI.DTOs;
using BookNexusAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookNexusAPI.Tests;

public class BookControllerTests
{
    private readonly Mock<IBookService> service = new();

    [Fact]
    public async Task GetAll_ReturnsOkWithBooks()
    {
        var books = new List<BookDto>
        {
            new() { BookId = 1, Title = "The Hobbit" },
            new() { BookId = 2, Title = "Dune" }
        };
        service.Setup(x => x.GetAllAsync()).ReturnsAsync(books);
        var controller = new BookController(service.Object);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(books, ok.Value);
        service.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenBookExists_ReturnsOkWithBook()
    {
        var book = new BookDto { BookId = 7, Title = "The Hobbit" };
        service.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(book);
        var controller = new BookController(service.Object);

        var result = await controller.GetById(7);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(book, ok.Value);
        service.Verify(x => x.GetByIdAsync(7), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        service.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((BookDto?)null);
        var controller = new BookController(service.Object);

        var result = await controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
        service.Verify(x => x.GetByIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task Add_WhenServiceSucceeds_ReturnsCreatedAtAction()
    {
        var dto = CreateBookDto();
        var added = new BookDto { BookId = 12, Title = dto.Title };
        service.Setup(x => x.AddAsync(dto)).ReturnsAsync(added);
        var controller = new BookController(service.Object);

        var result = await controller.Add(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BookController.GetById), created.ActionName);
        Assert.Equal(12, created.RouteValues!["id"]);
        Assert.Same(added, created.Value);
        service.Verify(x => x.AddAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Add_WhenServiceRejectsDuplicate_ReturnsConflictWithMessage()
    {
        var dto = CreateBookDto();
        service
            .Setup(x => x.AddAsync(dto))
            .ThrowsAsync(new InvalidOperationException("A book with title 'The Hobbit' already exists"));
        var controller = new BookController(service.Object);

        var result = await controller.Add(dto);

        var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
        var message = conflict.Value!.GetType().GetProperty("message")!.GetValue(conflict.Value);
        Assert.Equal("A book with title 'The Hobbit' already exists", message);
        service.Verify(x => x.AddAsync(dto), Times.Once);
    }

    private static AddBookDto CreateBookDto() => new()
    {
        Title = "The Hobbit",
        Author = "J.R.R. Tolkien",
        Image = "hobbit.jpg",
        Description = "An adventure",
        Genre = "Fantasy",
        Quantity = 5,
        Language = "English",
        Price = 19.99m
    };
}

using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class CreateNoteTests
{
    private readonly INoteRepository _noteRepository;

    public CreateNoteTests()
    {
        this._noteRepository = Substitute.For<INoteRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddNote_WhenCalled()
    {
        // Arrange
        var request = new CreateNoteRequest
        {
            Title = "Test Title",
            Content = "Test Content",
        };
        var userId = Guid.NewGuid();

        // Act
        await CreateNote.HandleAsync(request, userId, this._noteRepository, CancellationToken.None);

        // Assert
        this._noteRepository.Received(1).Add(Arg.Any<NoteEntity>());
        await this._noteRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldSetCorrectProperties_WhenCalled()
    {
        // Arrange
        var request = new CreateNoteRequest
        {
            Title = "Test Title",
            Content = "Test Content",
        };
        var userId = Guid.NewGuid();

        NoteEntity? captured = null;
        this._noteRepository.When(x => x.Add(Arg.Any<NoteEntity>()))
            .Do(x => captured = x.Arg<NoteEntity>());

        // Act
        await CreateNote.HandleAsync(request, userId, this._noteRepository, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Title.Should().Be("Test Title");
        captured.Content.Should().Be("Test Content");
        captured.UserId.Should().Be(userId);
    }
}

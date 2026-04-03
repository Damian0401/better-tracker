using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class UpdateNoteTests
{
    private readonly INoteRepository _noteRepository;

    public UpdateNoteTests()
    {
        this._noteRepository = Substitute.For<INoteRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateNote_WhenNoteExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingNote = new NoteEntity { Id = id, Title = "Old Title", Content = "Old Content" };
        this._noteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<NoteEntity?>(existingNote));

        var request = new UpdateNoteRequest
        {
            Id = id,
            Title = "New Title",
            Content = "New Content",
        };

        NoteEntity? captured = null;
        this._noteRepository.When(x => x.Update(Arg.Any<NoteEntity>()))
            .Do(x => captured = x.Arg<NoteEntity>());

        // Act
        await UpdateNote.HandleAsync(request, this._noteRepository, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Title.Should().Be("New Title");
        captured.Content.Should().Be("New Content");
        await this._noteRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

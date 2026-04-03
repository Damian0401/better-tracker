using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data;
using FluentAssertions;
using NSubstitute;

namespace BetterTracker.Tests.Commands;

public class DeleteNoteTests
{
    private readonly INoteRepository _noteRepository;

    public DeleteNoteTests()
    {
        this._noteRepository = Substitute.For<INoteRepository>();
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveNote_WhenNoteExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var note = new NoteEntity { Id = id, Title = "Test Title", Content = "Test Content" };
        this._noteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<NoteEntity?>(note));

        var request = new DeleteNoteRequest { Id = id };

        // Act
        await DeleteNote.HandleAsync(request, this._noteRepository, CancellationToken.None);

        // Assert
        this._noteRepository.Received(1).Remove(Arg.Is(note));
        await this._noteRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

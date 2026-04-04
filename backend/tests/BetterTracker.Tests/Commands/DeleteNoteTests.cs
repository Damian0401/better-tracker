using BetterTracker.Contracts;
using BetterTracker.Core.Notes.Commands;
using BetterTracker.Data.Entities;
using BetterTracker.Data.Repositories;
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
        var result = await DeleteNote.HandleAsync(request, this._noteRepository, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        this._noteRepository.Received(1).Remove(Arg.Is(note));
        await this._noteRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenNoteDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        this._noteRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<NoteEntity?>(null));

        var request = new DeleteNoteRequest { Id = id };

        // Act
        var result = await DeleteNote.HandleAsync(request, this._noteRepository, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        this._noteRepository.DidNotReceive().Remove(Arg.Any<NoteEntity>());
        await this._noteRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

using Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace NxT.Tests.Application;
[TestFixture]
public class CreateClinicalTrialMetadataCommandHandlerTests
{
    private readonly Mock<IClinicalTrialMetadataRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateClinicalTrialMetadataCommandHandler _handler;

    public CreateClinicalTrialMetadataCommandHandlerTests()
    {
        _mockRepository = new Mock<IClinicalTrialMetadataRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateClinicalTrialMetadataCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_InsertsMetadataAndReturnsId()
    {
        var command = new CreateClinicalTrialMetadataCommand(
            "Test Trial",
            DateTime.Now,
            DateTime.Now.AddMonths(1),
            100,
            TrialStatus.Ongoing
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        _mockRepository.Verify(r => r.Insert(It.IsAny<ClinicalTrialMetadata>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.That(result, Is.TypeOf<Guid>());
        Assert.That(result, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task Handle_WhenEndDateIsNotProvidedAndStatusIsOngoing_ShouldSetEndDateToOneMonthFromStartDate()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var command = new CreateClinicalTrialMetadataCommand("Trial 1", startDate, null, 100, TrialStatus.Ongoing);

        var mockMetadata = new ClinicalTrialMetadata(Guid.NewGuid(), command.Title, startDate, null, command.Participants, command.Status, 0);
        
        _mockRepository
            .Setup(repo => repo.Insert(It.IsAny<ClinicalTrialMetadata>()))
            .Callback<ClinicalTrialMetadata>(metadata => mockMetadata = metadata); // Capture the object passed to Insert

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var expectedEndDate = startDate.AddMonths(1);
        Assert.That(mockMetadata.EndDate, Is.EqualTo(expectedEndDate));
    }


    [Test]
    public async Task Handle_WhenEndDateIsProvided_ShouldUseProvidedEndDate()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2025, 2, 1, 12, 0, 0, DateTimeKind.Utc);
        var expectedDurationInDays = (endDate - startDate).Days;

        var command = new CreateClinicalTrialMetadataCommand("Trial 1", startDate, endDate, 100, TrialStatus.NotStarted);

        ClinicalTrialMetadata? capturedMetadata = null;

        _mockRepository
            .Setup(repo => repo.Insert(It.IsAny<ClinicalTrialMetadata>()))
            .Callback<ClinicalTrialMetadata>(metadata => capturedMetadata = metadata);

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(capturedMetadata, Is.Not.Null, "Metadata was not captured during repository insert.");
        Assert.Multiple(() =>
        {
            Assert.That(capturedMetadata!.StartDate, Is.EqualTo(startDate), "StartDate does not match.");
            Assert.That(capturedMetadata.EndDate, Is.EqualTo(endDate), "EndDate does not match.");
            Assert.That(capturedMetadata.DurationInDays, Is.EqualTo(expectedDurationInDays), "DurationInDays is incorrect.");
            Assert.That(capturedMetadata.Id, Is.EqualTo(result), "The returned ID does not match the metadata ID.");
        });
    }


    [Test]
    public async Task Handle_WhenStatusIsOngoing_AndEndDateIsNotProvided_ShouldCalculateDuration()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var expectedEndDate = startDate.AddMonths(1);
        var expectedDurationInDays = (expectedEndDate - startDate).Days;

        var command = new CreateClinicalTrialMetadataCommand("Trial 1", startDate, null, 100, TrialStatus.Ongoing);

        ClinicalTrialMetadata? capturedMetadata = null;

        _mockRepository
            .Setup(repo => repo.Insert(It.IsAny<ClinicalTrialMetadata>()))
            .Callback<ClinicalTrialMetadata>(metadata => capturedMetadata = metadata); // Capture the object passed to Insert

        _mockUnitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(capturedMetadata, Is.Not.Null, "Metadata was not captured during repository insert.");
        Assert.Multiple(() =>
        {
            Assert.That(capturedMetadata!.StartDate, Is.EqualTo(startDate));
            Assert.That(capturedMetadata.EndDate, Is.EqualTo(expectedEndDate));
            Assert.That(capturedMetadata.DurationInDays, Is.EqualTo(expectedDurationInDays));
            Assert.That(capturedMetadata.Id, Is.EqualTo(result), "The returned ID does not match the metadata ID.");
        });
    }


    [Test]
    public async Task Handle_WhenStartDateIsUtc_ShouldNotChangeStartDateKind()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var command = new CreateClinicalTrialMetadataCommand("Trial 1", startDate, null, 100, TrialStatus.NotStarted);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(command.StartDate.Kind, Is.EqualTo(DateTimeKind.Utc));
    }

    [Test]
    public async Task Handle_WhenEndDateIsBeforeStartDate_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2024, 12, 1, 12, 0, 0, DateTimeKind.Utc); // End date is before start date

        var command = new CreateClinicalTrialMetadataCommand("Trial 1", startDate, endDate, 100, TrialStatus.NotStarted);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert the exception message
        Assert.That(exception.Message, Is.EqualTo("The end date must be after the start date."));
    }
}
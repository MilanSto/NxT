using Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;
using Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace NxT.Tests.Application
{
    [TestFixture]
    public class ClinicalTrialMetadataRepositoryTests
    {
        private Mock<IClinicalTrialMetadataRepository> _mockRepository;
        private GetClinicalTrialMetadataQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IClinicalTrialMetadataRepository>();
            _handler = new GetClinicalTrialMetadataQueryHandler(_mockRepository.Object);
        }

        [Test]
        public async Task GetClinicalTrialByIdAsync_ShouldReturnCorrectRecord()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            var metadata = new ClinicalTrialMetadata(trialId, "Trial 1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), 100, TrialStatus.Ongoing, 30);

            _mockRepository
                .Setup(repo => repo.GetClinicalTrialByIdAsync(trialId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(metadata);

            // Act
            var result = await _mockRepository.Object.GetClinicalTrialByIdAsync(trialId, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(trialId));
                Assert.That(result.Title, Is.EqualTo("Trial 1"));
            });
        }

        [Test]
        public async Task GetClinicalTrialByIdAsync_ShouldReturnNull_WhenRecordDoesNotExist()
        {
            // Arrange
            var trialId = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.GetClinicalTrialByIdAsync(trialId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClinicalTrialMetadata?)null);

            // Act
            var result = await _mockRepository.Object.GetClinicalTrialByIdAsync(trialId, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Handle_WithMatchingStatus_ShouldReturnMetadata()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            var expectedStatus = "Ongoing";
            var metadata = new ClinicalTrialMetadata(trialId, "Trial 1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), 100, TrialStatus.Ongoing, 30);

            _mockRepository
                .Setup(repo => repo.GetClinicalTrialByIdAsync(trialId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(metadata);

            var query = new GetClinicalTrialMetadataByIdQuery(trialId, expectedStatus);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Status.ToString(), Is.EqualTo(expectedStatus));
                Assert.That(result.Title, Is.EqualTo(metadata.Title));
            });
        }

        [Test]
        public async Task Handle_WithoutStatus_ShouldReturnMetadata()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            var metadata = new ClinicalTrialMetadata(trialId, "Trial 2", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), 50, TrialStatus.Completed, 30);

            _mockRepository
                .Setup(repo => repo.GetClinicalTrialByIdAsync(trialId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(metadata);

            var query = new GetClinicalTrialMetadataByIdQuery(trialId, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Status, Is.EqualTo(metadata.Status));
                Assert.That(result.Title, Is.EqualTo(metadata.Title));
            });
        }

        [Test]
        public async Task Handle_WithNonMatchingStatus_ShouldReturnNull()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            var metadata = new ClinicalTrialMetadata(trialId, "Trial 3", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), 70, TrialStatus.Ongoing, 30);

            _mockRepository
                .Setup(repo => repo.GetClinicalTrialByIdAsync(trialId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(metadata);

            var query = new GetClinicalTrialMetadataByIdQuery(trialId, "Completed");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Handle_WithNonExistingMetadata_ShouldReturnNull()
        {
            // Arrange
            var trialId = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.GetClinicalTrialByIdAsync(trialId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClinicalTrialMetadata?)null);

            var query = new GetClinicalTrialMetadataByIdQuery(trialId, "Ongoing");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}


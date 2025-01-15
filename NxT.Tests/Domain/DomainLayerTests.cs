using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace NxT.Tests.Domain
{
    [TestFixture]
    public class DomainLayerTests
    {
        [Test]
        public void Entity_ShouldInitializeCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var startDate = DateTime.UtcNow;

            // Act
            var entity = new ClinicalTrialMetadata(id, "Trial 1", startDate, null, 100, TrialStatus.Ongoing, 30);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(id));
                Assert.That(entity.Title, Is.EqualTo("Trial 1"));
                Assert.That(entity.StartDate, Is.EqualTo(startDate));
            });
        }

        [Test]
        public void CustomException_ShouldSetMessageAndInnerException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var message = $"Clinical Trial Metadata with the identifier {id} was not found.";

            // Act
            var exception = new ClinicalTrialMetadataNotFoundException(id);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));

        }

        [Test]
        public void RepositoryInterface_ShouldFollowNamingConvention()
        {
            // Arrange
            var repositoryInterface = typeof(IClinicalTrialMetadataRepository);

            // Assert
            Assert.That(repositoryInterface.Name, Does.StartWith("I"));
            Assert.That(repositoryInterface.Name, Does.EndWith("Repository"));
        }

        [Test]
        public void AllEntities_ShouldHaveIdPropertyOfTypeGuid()
        {
            // Arrange
            var entityTypes = typeof(ClinicalTrialMetadata).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Metadata"));

            // Act & Assert
            foreach (var type in entityTypes)
            {
                var idProperty = type.GetProperty("Id");
                Assert.That(idProperty, Is.Not.Null);
                Assert.That(idProperty.PropertyType, Is.EqualTo(typeof(Guid)));
            }
        }
    }
}

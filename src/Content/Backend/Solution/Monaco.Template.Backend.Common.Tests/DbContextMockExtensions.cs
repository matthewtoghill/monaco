using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Monaco.Template.Backend.Common.Domain.Model;
using Moq;

namespace Monaco.Template.Backend.Common.Tests;

public static class DbContextMockExtensions
{
	extension<TDbContext>(Mock<TDbContext> dbContextMock) where TDbContext : DbContext
	{
		public Mock<TDbContext> SetupDbSetMock<T>(T entity) where T : Entity
		{
			var entityDbSetMock = new List<T> { entity }.BuildMockDbSet();
			dbContextMock.Setup(x => x.Set<T>()).Returns(entityDbSetMock.Object);
			entityDbSetMock.Setup(x => x.FindAsync(new object[] { entity.Id }, It.IsAny<CancellationToken>()))
						   .ReturnsAsync(entity);
			return dbContextMock;
		}

		public Mock<TDbContext> CreateEntityMockAndSetupDbSetMock<T>(out Mock<T> entityMock) where T : Entity
		{
			entityMock = new Mock<T>();
			var entityDbSetMock = new List<T> { entityMock.Object }.BuildMockDbSet();
			dbContextMock.Setup(x => x.Set<T>()).Returns(entityDbSetMock.Object);
			entityDbSetMock.Setup(x => x.FindAsync(new object[] { It.IsAny<Guid>() }, It.IsAny<CancellationToken>()))
						   .ReturnsAsync(entityMock.Object);

			return dbContextMock;
		}

		public Mock<TDbContext> CreateEntityMockAndSetupDbSetMock<T>() where T : Entity
			=> dbContextMock.CreateEntityMockAndSetupDbSetMock<TDbContext, T>(out _);

		public Mock<TDbContext> CreateAndSetupDbSetMock<T>(T entity, out Mock<DbSet<T>> entityDbSetMock) where T : Entity
		{
			entityDbSetMock = new[] { entity }.BuildMockDbSet();
			dbContextMock.Setup(x => x.Set<T>()).Returns(entityDbSetMock.Object);
			entityDbSetMock.Setup(x => x.FindAsync(new object[] { It.IsAny<Guid>() }, It.IsAny<CancellationToken>()))
						   .ReturnsAsync(entity);

			return dbContextMock;
		}

		public Mock<TDbContext> CreateAndSetupDbSetMock<T>(T entity) where T : Entity
			=> dbContextMock.CreateAndSetupDbSetMock(entity, out _);

		public Mock<TDbContext> CreateAndSetupDbSetMock<T>(ICollection<T> entities, out Mock<DbSet<T>> entityDbSetMock) where T : Entity
		{
			entityDbSetMock = entities.BuildMockDbSet();
			dbContextMock.Setup(x => x.Set<T>()).Returns(entityDbSetMock.Object);

			return dbContextMock;
		}

		public Mock<TDbContext> CreateAndSetupDbSetMock<T>(ICollection<T> entities) where T : Entity
			=> dbContextMock.CreateAndSetupDbSetMock(entities, out _);
	}
}

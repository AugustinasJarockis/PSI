using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Microsoft.EntityFrameworkCore;
using Moq;
using NOTEA.Database;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;
using NOTEA.Repositories.GenericRepositories;
using NOTEA.Repositories.UserRepositories;
using NOTEA.Services.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteaTests
{
    public class RepositoryTests
    {
        [Fact]
        public void LoadConspect_ShouldCall_FindMethod()
        {
            var logsServiceMock = new Mock<ILogsService>();
            var options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("TestDatabase").Options;
            var dbContextMock = new Mock<DatabaseContext>(options);
            var conspectTypesSetMock = new Mock<DbSet<ConspectModel>>();
            dbContextMock.Setup(db => db.Set<ConspectModel>()).Returns(conspectTypesSetMock.Object);

            var repository = new GenericRepository<ConspectModel>(logsServiceMock.Object, dbContextMock.Object);

            repository.LoadConspect(1);

            conspectTypesSetMock.Verify(set => set.Find(1), Times.Once);
        }
        [Fact]
        public void SaveConspect_ShouldAddOrUpdate_ConspectModel()
        {
            var logsServiceMock = new Mock<ILogsService>();
            var options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("TestDatabase").Options;
            var dbContextMock = new Mock<DatabaseContext>(options);
            var conspectTypesSetMock = new Mock<DbSet<ConspectModel>>();
            dbContextMock.Setup(db => db.Set<ConspectModel>()).Returns(conspectTypesSetMock.Object);

            var repository = new GenericRepository<ConspectModel>(logsServiceMock.Object, dbContextMock.Object);

            var conspect = new ConspectModel { Id = 1, Name = "Example" };

            repository.SaveConspect(conspect, conspect.Id);

            conspectTypesSetMock.Verify(set => set.Find(conspect.Id), Times.Once);
            conspectTypesSetMock.Verify(set => set.Add(conspect), Times.Once);
            dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
        }
        [Fact]
        public void CheckLogIn_ShouldReturnTrue_WhenUserExists()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "CheckLogIn_ShouldReturnTrue_WhenUserExists")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                var logsServiceMock = new Mock<ILogsService>();
                var userRepository = new UserRepository<UserModel>(logsServiceMock.Object, context);

                var userModel = new UserModel { Username = "existingUser", Password = "password", Email = "test@example.com" };
                context.Users.Add(userModel);
                context.SaveChanges();

                var result = userRepository.CheckLogIn(userModel);

                Assert.True(result);
            }
        }


        [Fact]
        public void CheckLogIn_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "CheckLogIn_ShouldReturnFalse_WhenUserDoesNotExist")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                var logsServiceMock = new Mock<ILogsService>();
                var userRepository = new UserRepository<UserModel>(logsServiceMock.Object, context);

                var userModel = new UserModel { Username = "nonexistentUser", Password = "password" };

                // Act
                var result = userRepository.CheckLogIn(userModel);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task SaveUserAsync_ShouldAddUserToDatabase_WhenNoExceptions()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "SaveUserAsync_ShouldAddUserToDatabase_WhenNoExceptions")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                var logsServiceMock = new Mock<ILogsService>();
                var userRepository = new UserRepository<UserModel>(logsServiceMock.Object, context);

                await userRepository.SaveUserAsync(new UserModel { Username = "newUser", Password = "password", Email = "test@example.com" });

                Assert.Equal(1, context.Users.Count());
                Assert.Equal("newUser", context.Users.Single().Username);
                Assert.Equal("password", context.Users.Single().Password);
                Assert.Equal("test@example.com", context.Users.Single().Email);
            }
        }



    }
}

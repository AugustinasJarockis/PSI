using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NoteaAPI.Controllers;
using NoteaAPI.Database;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Services.FolderService;
using NoteaAPI.Services.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiUnitTest
{
    public class ConspectControllerTests
    {
        Mock<IGenericRepository<ConspectModel>> repositoryMock = new Mock<IGenericRepository<ConspectModel>>();
        Mock<ILogsService> logsServiceMock = new Mock<ILogsService>();
        Mock<IFolderService> folderServiceMock = new Mock<IFolderService>();
        Mock<IUserRepository<UserModel>> userRepositoryMock = new Mock<IUserRepository<UserModel>>();
        [Fact]
        public void CreateConspects_ValidName_ReturnsOkResult()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            Mock<DatabaseContext> databaseMock = new Mock<DatabaseContext>(options);

            var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, databaseMock.Object);

            var conspectModel = new ConspectModel { Id = 1, Name = "ValidName" };

            var result = controller.CreateConspects(1, 1, conspectModel);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateConspects_InvalidName_ReturnsConflictResult()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            Mock<DatabaseContext> databaseMock = new Mock<DatabaseContext>(options);

            var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, databaseMock.Object);

            var conspectModel = new ConspectModel { Id = 1, Name = "../@#/dr" };

            var result = controller.CreateConspects(1, 1, conspectModel);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateConspects_Exception_ReturnsBadRequestResult()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            Mock<DatabaseContext> databaseMock = new Mock<DatabaseContext>(options);

            repositoryMock.Setup(r => r.SaveConspect(It.IsAny<ConspectModel>(), It.IsAny<int>()))
                          .Throws(new ArgumentNullException());

            var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, databaseMock.Object);

            var conspectModel = new ConspectModel { Id = 1, Name = "ValidName" };

            var result = controller.CreateConspects(1, 1, conspectModel);

            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void UploadConspect_ValidConspect_ReturnsOkResult()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            Mock<DatabaseContext> databaseMock = new Mock<DatabaseContext>(options);

            var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, databaseMock.Object);

            var conspectModel = new ConspectModel { Id = 1, Name = "ValidName" };

            var result = controller.UploadConspect(1, 1, conspectModel);

            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public void ShareConspect_GoodName_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                context.Users.Add(new UserModel { Username = "ab", Password = "password123", Email = "test@example.com" });
                context.SaveChanges();

                var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, context);

                var conspectModel = new ConspectModel { Id = 1, Name = "ValidName" };

                var result = controller.ShareConspect("a", "ab", conspectModel);

                Assert.IsType<OkResult>(result);
            }
        }
        [Fact]
        public void ShareConspect_SameName_ReturnsBad()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                context.Users.Add(new UserModel { Username = "a", Password = "password123", Email = "test@example.com" });
                context.SaveChanges();

                var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, context);

                var conspectModel = new ConspectModel { Id = 1, Name = "ValidName" };

                var result = controller.ShareConspect("a", "a", conspectModel);

                Assert.IsType<BadRequestObjectResult>(result);
            }
        }
        [Fact]
        public void ShareConspect_DoesNotExist_ReturnsBad()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                context.Users.Add(new UserModel { Username = "a", Password = "password123", Email = "test@example.com" });
                context.SaveChanges();

                var controller = new ConspectController(repositoryMock.Object, logsServiceMock.Object, folderServiceMock.Object, userRepositoryMock.Object, context);

                var conspectModel = new ConspectModel { Id = 1, Name = "ValidName" };

                var result = controller.ShareConspect("a", "does not exist", conspectModel);

                Assert.IsType<BadRequestObjectResult>(result);
            }
        }
    }
}

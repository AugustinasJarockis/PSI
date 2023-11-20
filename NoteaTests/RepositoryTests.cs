using Microsoft.EntityFrameworkCore;
using Moq;
using NOTEA.Database;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;
using NOTEA.Repositories.GenericRepositories;
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
        public void AssignToUser_ShouldAddUserConspectsModelAndSaveChanges()
        {
            var logsServiceMock = new Mock<ILogsService>();
            var options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("TestDatabase").Options;
            var dbContextMock = new Mock<DatabaseContext>(options);
            var conspectTypesSetMock = new Mock<DbSet<ConspectModel>>();
            var userConspectsSetMock = new Mock<DbSet<UserConspectsModel>>();
            dbContextMock.Setup(db => db.Set<ConspectModel>()).Returns(conspectTypesSetMock.Object);
            dbContextMock.Setup(db => db.UserConspects).Returns(userConspectsSetMock.Object);

            var repository = new GenericRepository<ConspectModel>(logsServiceMock.Object, dbContextMock.Object);

            repository.AssignToUser(1, 2, 'a');

            userConspectsSetMock.Verify(set => set.Add(It.IsAny<UserConspectsModel>()), Times.Once);
            dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
        }
    }
}

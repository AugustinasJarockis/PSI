using Microsoft.EntityFrameworkCore;
using Moq;
using NoteaAPI.Database;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Services.LogServices;

namespace NoteaApiTest
{
    public class UnitTest1
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
    }
}
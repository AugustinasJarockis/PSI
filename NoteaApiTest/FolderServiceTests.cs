using Microsoft.EntityFrameworkCore;
using Moq;
using NoteaAPI.Database;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.FileTree;
using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Services.FolderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiUnitTest
{
    public class FolderServiceTests
    {
        [Fact]
        public void AddFolder_Should_Add_Folder_And_Return_Id()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var databaseContext = new DatabaseContext(options))
            {
                var repositoryMock = new Mock<IGenericRepository<ConspectModel>>();
                var folderService = new FolderService(databaseContext, repositoryMock.Object);

                var folderId = folderService.AddFolder("TestFolder");

                Assert.NotEqual(0, folderId);
            }
        }

        [Fact]
        public void GetFolderName_Should_Return_Folder_Name_IfExists()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var databaseContext = new DatabaseContext(options))
            {
                var repositoryMock = new Mock<IGenericRepository<ConspectModel>>();
                var folderService = new FolderService(databaseContext, repositoryMock.Object);

                var folderId = 1;
                var folderName = "TestFolder";
                databaseContext.Folders.Add(new FolderModel("TestFolder"));

                databaseContext.SaveChanges();

                var result = folderService.GetFolderName(folderId);

                Assert.Equal(folderName, result);
            }
        }

        [Fact]
        public void GetFolderList_Should_Return_Folder_List()
        { 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var databaseContext = new DatabaseContext(options))
            {
                var repositoryMock = new Mock<IGenericRepository<ConspectModel>>();
                var folderService = new FolderService(databaseContext, repositoryMock.Object);

                var userId = 1;
                var folderId = 1;
                var folderModel = new FolderModel("TestFolder");
                databaseContext.Folders.Add(folderModel);
                databaseContext.FileStructure.Add(new TreeNodeModel { NodeType = NodeType.Folder, UserID = userId, FolderID = folderId, UnderlyingId = folderId });

                databaseContext.SaveChanges();

                var result = folderService.GetFolderList(userId, folderId);

                Assert.Single(result);
                Assert.Equal(folderModel.Name, result.First().Name);
            }
        }

        [Fact]
        public void GetPreviousFolderID_Should_Return_Previous_Folder_ID_If_Folder_Exists()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var databaseContext = new DatabaseContext(options))
            {
                var repositoryMock = new Mock<IGenericRepository<ConspectModel>>();
                var folderService = new FolderService(databaseContext, repositoryMock.Object);

                var userId = 1;
                var currentFolderId = 2;
                var folderId = 1;
                databaseContext.FileStructure.Add(new TreeNodeModel { NodeType = NodeType.Folder, UserID = userId, FolderID = folderId, UnderlyingId = currentFolderId });

                databaseContext.SaveChanges();

                var result = folderService.GetPreviousFolderID(userId, currentFolderId);

                Assert.Equal(folderId, result);
            }
        }

        [Fact]
        public void GetPreviousFolderID_Should_Return_Current_Folder_ID_If_Folder_Not_Exists()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var databaseContext = new DatabaseContext(options))
            {
                var repositoryMock = new Mock<IGenericRepository<ConspectModel>>();
                var folderService = new FolderService(databaseContext, repositoryMock.Object);

                var userId = 1;
                var currentFolderId = 2;

                databaseContext.SaveChanges();

                var result = folderService.GetPreviousFolderID(userId, currentFolderId);

                Assert.Equal(currentFolderId, result);
            }
        }


    }
}

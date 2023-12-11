using Microsoft.AspNetCore.Mvc;
using Moq;
using NoteaAPI.Controllers;
using NoteaAPI.Database;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.OnlineUserListModels;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Repositories.GenericRepositories;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Services.FolderService;
using NoteaAPI.Services.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiUnitTest
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository<UserModel>> userRepositoryMock = new Mock<IUserRepository<UserModel>>();
        private readonly Mock<IOnlineUserList> onlineUserListMock = new Mock<IOnlineUserList>();

        [Fact]
        public async Task SignInAsync_ValidUser_ReturnsOk()
        {
            var controller = new UserController(userRepositoryMock.Object, onlineUserListMock.Object);
            var validUser = new SignInUserModel { Username = "ValidUser", Password = "Password", PasswordCheck = "Password", Email = "user@example.com" };

            var result = await controller.SignInAsync(validUser);

            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public void UpdateUser_InvalidEmail_ReturnsBadRequest()
        {
            var controller = new UserController(userRepositoryMock.Object, onlineUserListMock.Object);
            var userId = 1;
            var invalidEmailUser = new UserModel { Id = userId, Username = "InvalidUser", Email = "invalidemail" };

            var result = controller.UpdateUser(userId, invalidEmailUser);

            Assert.IsType<BadRequestObjectResult>(result);
        }


    }
}

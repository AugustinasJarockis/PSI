﻿using Microsoft.EntityFrameworkCore;
using Moq;
using NoteaAPI.Database;
using NoteaAPI.Models.UserModels;
using NoteaAPI.Repositories.UserRepositories;
using NoteaAPI.Services.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiTest
{
    public class UserGenericRepositoryTests
    {
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
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "CheckLogIn_ShouldReturnFalse_WhenUserDoesNotExist")
                .Options;

            using (var context = new DatabaseContext(options))
            {
                var logsServiceMock = new Mock<ILogsService>();
                var userRepository = new UserRepository<UserModel>(logsServiceMock.Object, context);

                var userModel = new UserModel { Username = "nonexistentUser", Password = "password" };

                var result = userRepository.CheckLogIn(userModel);

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

                await userRepository.SaveUserAsync(new UserModel { Username = "newUser", Password = "Abc123123", Email = "test@example.com" });

                Assert.Equal(1, context.Users.Count());
                Assert.Equal("newUser", context.Users.Single().Username);
                Assert.Equal("Abc123123", context.Users.Single().Password);
                Assert.Equal("test@example.com", context.Users.Single().Email);
            }
        }
    }
}

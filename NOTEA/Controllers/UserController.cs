﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NOTEA.Models;
using System.IO;

namespace NOTEA.Controllers
{
    public class UserController : Controller
    {
        public readonly IHttpContextAccessor _contextAccessor;
        public UserController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        UserListModel userList = new UserListModel();
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string username, string password, string passwordCheck)
        {
            bool usernameTaken = false;
            if (username.IsValidFilename() && password.IsValidFilename() && passwordCheck.IsValidFilename())
            {
                userList = LoadUsers();
                foreach (var userModel in userList.userList)
                {
                    if(username == userModel.Username)
                    {
                        usernameTaken = true;
                    }
                }
                if (!usernameTaken)
                {
                    if (password == passwordCheck )
                    {
                        UserModel user = new UserModel(username, password);
                    
                        userList.userList.Add(user);

                        using (StreamWriter writer = new StreamWriter("Users//Users.txt"))
                            try
                            {
                                string serializedJSON = JsonConvert.SerializeObject(userList);
                                writer.Write(serializedJSON);
                            }
                            catch (Exception exp)
                            {
                                Console.WriteLine("Error: could not save user ");
                            }

                        TempData["SuccessMessage"] = "Your registration has been successfull!";
                        return RedirectToAction("LogIn", "User");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The passwords you entered does not match!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "This username is already taken";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Your username or password is invalid! It can't be empty, longer than 80 symbols or contain any of the following characters:\n \\\\ / : * . ? \" < > | ";
            }
            return View();
        }

        public UserListModel LoadUsers()
        {
            string text = System.IO.File.ReadAllText("Users//Users.txt");
            UserListModel userList = JsonConvert.DeserializeObject<UserListModel>(text);
            return userList;
        }
        public IActionResult LogIn()
        {
            //ViewBag.hasLogedIn = "t";
            /* HttpContext.Session.SetString("user", "lala");   */      /*   Session["lele"] = "scf";*/
            _contextAccessor.HttpContext.Session.SetString("User", "");

            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            UserModel user = new UserModel();
            if (username.IsValidFilename() && password.IsValidFilename())
            {
                userList = LoadUsers();
                foreach (var userModel in userList.userList)
                {
                    if(userModel.Username.Equals(username) && userModel.Password.Equals(password))
                    {
                        user = new UserModel(username, password);
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Your username or password is invalid! It can't be empty, longer than 80 symbols or contain any of the following characters:\n \\\\ / : * . ? \" < > | ";
            }
            if(user.Username == "" && user.Password == "")
            {
                TempData["ErrorMessage"] = "Your username or password is wrong";
            }
            else
            {
                //Console.WriteLine(ViewBag.hasLogedIn);
                //ViewBag.hasLogedIn = true;
                //Console.WriteLine(ViewBag.hasLogedIn);
                ////ViewData["user"] = user.Username;
                _contextAccessor.HttpContext.Session.SetString("User", user.Username);
                //SessionExtensions.SetString(this Http.Sesion)
                //_contextAccessor.HttpContext.Session.SetString("user", user.Username);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}

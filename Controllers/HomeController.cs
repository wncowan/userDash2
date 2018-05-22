using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using userDash2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace userDash2.Controllers
{
    public class HomeController : Controller
    {

        private DashboardContext _context;

        private bool checkLogStatus()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return false;
            }
            else 
            {
                return true;
            }
        }
        private string hashPW(User user, string password)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, password);
            return user.Password;
        }
        // private string hashPW(User user)
        // {
        //     PasswordHasher<User> hasher = new PasswordHasher<User>();
        //     user.Password = hasher.HashPassword(user, (string)user.Password);
        //     return user.Password;
        // }
        public HomeController(DashboardContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("")]
        public IActionResult Index(RegisterViewModel model)
        {
            List<User> users = _context.Users.ToList();
            if (ModelState.IsValid)
            {
                User newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now
                };

                if(users.Count == 0)
                {
                    newUser.UserLevel = "admin";
                }
                else 
                {
                    newUser.UserLevel = "user";
                }
                newUser.Password = hashPW(newUser, model.Password);
                // newUser.Password = hashPW(newUser);

                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("currentUserId", newUser.UserId);

                if(newUser.UserLevel == "admin")
                {
                    return RedirectToAction("AdminDash");
                }
                else
                {
                    return RedirectToAction("UserDash");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string email, string password)
        {
            User logUser = _context.Users.SingleOrDefault(user => user.Email == email);
            if (logUser == null)
            {
                TempData["EmailError"] = "Invalid email!";
                return RedirectToAction("Index");
            }
            else
            {
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                if (hasher.VerifyHashedPassword(logUser, logUser.Password, password) != 0)
                {
                    HttpContext.Session.SetInt32("currentUserId", logUser.UserId);
                    if(logUser.UserLevel == "admin")
                    {
                        return RedirectToAction("AdminDash");
                    }
                    else
                    {
                        return RedirectToAction("UserDash");
                    }
                }
                else
                {
                    TempData["PasswordError"] = "Invalid password";
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet]
        [Route("/dashboard/admin")]
        public IActionResult AdminDash()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                List<User> users = _context.Users.ToList();

                ViewBag.currentUser = currentUser;
                ViewBag.AllUsers = users;
                return View();
            }
        }

        [HttpGet]
        [Route("/dashboard/user")]
        public IActionResult UserDash()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                List<User> users = _context.Users.ToList();

                ViewBag.currentUser = currentUser;
                ViewBag.AllUsers = users;
                return View();
            }
        }

        [HttpGet]
        [Route("/users/new")]
        public IActionResult AddUser()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                ViewBag.currentUser = currentUser;
                return View();
            }
        }

        [HttpGet]
        [Route("/users/{userId}/edit")]
        public IActionResult EditUser(int userId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                User editUser = _context.Users.SingleOrDefault(eU => eU.UserId == userId);

                ViewBag.currentUser = currentUser;
                ViewBag.thisUser = editUser;
                return View();
            }
        }

        [HttpGet]
        [Route("/admin/edit/{userId}")]
        public IActionResult AdminEdit(int userId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                User editUser = _context.Users.SingleOrDefault(eU => eU.UserId == userId);

                ViewBag.currentUser = currentUser;
                ViewBag.thisUser = editUser;
                return View();
            }
        }

        [HttpGet]
        [Route("/users/show/{userId}")]
        public IActionResult ShowUser(int userId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                User showUser = _context.Users.SingleOrDefault(eU => eU.UserId == userId);

                List<User> users = _context.Users.Include(u => u.Messages).Include(u => u.Comments).ToList();
                List<Message> messages = _context.Messages.Where(m => m.PostedOn == userId).Include(m => m.Comments).ThenInclude(c => c.User).ToList();
                List<Comment> comments = _context.Comments.Include(c => c.Message).Include(c => c.User).ToList();

                ViewBag.thisUser = showUser;
                ViewBag.currentUser = currentUser;

                Wrapper model = new Wrapper(users, messages, comments);
                return View(model);
            }
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/users/{userId}/delete")]
        public IActionResult DeleteUser(int userId)
        {
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User delUser = _context.Users.SingleOrDefault(u => u.UserId == userId);
                _context.Remove(delUser);
                _context.SaveChanges();
                return RedirectToAction("AdminDash");
            }
        }

        [HttpPost]
        [Route("/users/create")]
        public IActionResult CreateUser(CreateUserVM model)
        {
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    User newUser = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserLevel = model.UserLevel,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now
                    };
                    newUser.Password = hashPW(newUser, model.Password);
                    // newUser.Password = hashPW(newUser);
                    _context.Add(newUser);
                    _context.SaveChanges();
                    return RedirectToAction("AdminDash");
                }
                else
                {
                    return RedirectToAction("AddUser");
                }
            }
        }

        [HttpPost]
        [Route("/users/{userId}/update")]
        public IActionResult UpdateUser(User model, int userId)
        {
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    User updateUser = _context.Users.SingleOrDefault(u => u.UserId == userId);

                    updateUser.FirstName = model.FirstName;
                    updateUser.LastName = model.LastName;
                    updateUser.Email = model.Email;
                    updateUser.Password = hashPW(updateUser, model.Password);
                    // updateUser.Password = hashPW(updateUser);

                    //_context.Update(updateUser);
                    _context.SaveChanges();
                    return RedirectToAction("UserDash");
                }
                return RedirectToAction("EditUser");
            }
        }

        [HttpPost]
        [Route("/admin/update/{userId}")]
        public IActionResult AdminUpdate(User model, int userId)
        {
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    User updateUser = _context.Users.SingleOrDefault(u => u.UserId == userId);

                    updateUser.FirstName = model.FirstName;
                    updateUser.LastName = model.LastName;
                    updateUser.Email = model.Email;
                    updateUser.UserLevel = model.UserLevel;
                    updateUser.Password = hashPW(updateUser, model.Password);
                    // updateUser.Password = hashPW(updateUser);

                    //_context.Update(updateUser);
                    _context.SaveChanges();
                    return RedirectToAction("AdminDash");
                }
                return RedirectToAction("EditUser");
            }
        }



        [HttpPost]
        [Route("/users/{userId}/messages/new")]
        public IActionResult AddMessage(Message model, int userId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                User thisUser = _context.Users.SingleOrDefault(u => u.UserId == userId);

                if (ModelState.IsValid)
                {
                    Message newMessage = new Message
                    {
                        Content = model.Content,
                        UserId = currentUser.UserId,
                        Creator = currentUser,
                        PostedOn = thisUser.UserId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Add(newMessage);
                    _context.SaveChanges();
                    return RedirectToAction("ShowUser");
                }
            }
            return RedirectToAction("ShowUser");
        }

        [HttpPost]
        [Route("/messages/{messageId}/comments/new")]
        public IActionResult AddComment(Comment model, int messageId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (checkLogStatus() == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)currentUserId);
                Message thisMessage = _context.Messages.SingleOrDefault(m => m.MessageId == messageId);

                if (ModelState.IsValid)
                {
                    Comment newComment = new Comment
                    {
                        Content = model.Content,
                        UserId = currentUser.UserId,
                        User = currentUser,
                        MessageId = thisMessage.MessageId,
                        Message = thisMessage,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Add(newComment);
                    _context.SaveChanges();
                    return RedirectToAction("UserDash");
                }
            }
            return RedirectToAction("ShowUser");
        }
    }
}
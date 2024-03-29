﻿using EternityWebServiceApp.Interfaces;
using EternityWebServiceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EternityWebServiceApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly EternityDBContext _context;
        private readonly IUserRepository _repository;

        public UserController(EternityDBContext context, IUserRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public IActionResult Index()
        {
            ViewBag.Roles = _context.Roles;  //список ролей
            return View(_repository.Get());
        }

        public ActionResult Create()
        {
            ViewBag.Roles = _context.Roles;
            return View();
        }

        [HttpPost]
        public IActionResult Create(User newUser, IFormFile uploadedFile)
        {
            ViewBag.Roles = _context.Roles;
            if (_context.Users.FirstOrDefault(x => x.Email == newUser.Email) != null)
            {
                ModelState.AddModelError("Email", "Email уже используется");
            }

            if (_context.Users.FirstOrDefault(x => x.UserName == newUser.UserName) != null)
            {
                ModelState.AddModelError("Username", "Никнейм уже используется");
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserId = null,
                    Email = newUser.Email,
                    UserName = newUser.UserName,
                    Password = newUser.Password,
                    RoleId = newUser.RoleId,
                };

                _repository.Create(user, uploadedFile);
                return RedirectToAction("Index");
            }

            return View(newUser);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Roles = _context.Roles;
            if (id == 1)
            {
                return RedirectToAction("Index");
            }

            User user = _repository.Get(id);
            if (user != null)
            {
                ViewData["RoleId"] = (int)user.RoleId;
                return View(user);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Edit(User newUser, IFormFile uploadedFile)
        {
            ViewBag.Roles = _context.Roles;
            User user = _repository.Get((int)newUser.UserId);
            if (_context.Users.FirstOrDefault(x => x.Email == newUser.Email && newUser.Email != user.Email) != null)
            {
                ModelState.AddModelError("Email", "Email уже используется");
            }

            if (_context.Users.FirstOrDefault(x => x.UserName == newUser.UserName && newUser.UserName != user.UserName) != null)
            {
                ModelState.AddModelError("Username", "Никнейм уже используется");
            }

            if (ModelState.IsValid)
            {
                user.Email = newUser.Email;
                user.UserName = newUser.UserName;
                user.Password = newUser.Password;
                user.RoleId = newUser.RoleId;
                _repository.Update(user, uploadedFile);
                return RedirectToAction("Index");
            }

            ViewData["RoleId"] = (int)user.RoleId;
            return View(newUser);
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            if (id == 1)
            {
                return RedirectToAction("Index");
            }

            User user = _repository.Get(id);
            if (user != null)
            {
                ViewData["RoleName"] = _context.Roles.First(x => x.RoleId == user.RoleId).Name;
                return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _repository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}

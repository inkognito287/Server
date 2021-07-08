using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public string Check(User model)
        {
            foreach (var item in db.Users) {
                if (item.Name == model.Name && item.Password == model.Password)
                    return "true";
            }
        
               
            return "false";

        }
        [HttpPost]
        public async Task<string> AddAsync(User user)
        {
            db.Add(user);
            await db.SaveChangesAsync();
            return "Added";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using System.Net.Mail;
using System.Net;

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
        public async Task<bool> AddAsync(User user)
        {
            foreach (var item in db.Users)
            {
                if (item.Name==user.Name)
                    return false;
            
            }
                db.Add(user);
            await db.SaveChangesAsync();
            return true;
        }
        public void email()
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("vip.cod042@mail.ru", "Обеспечиваем безопасноть");
            // кому отправляем
            MailAddress to = new MailAddress("vip.cod05@mail.ru");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Тест";
            // текст письма
            m.Body = "<h2>Письмо-тест работы smtp-клиента</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("somemail@gmail.com", "mypassword");
            smtp.EnableSsl = true;
            smtp.Send(m);
            Console.Read();
        }
    }
}

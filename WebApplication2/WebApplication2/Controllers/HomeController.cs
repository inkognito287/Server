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
        [HttpPost]
        public async Task<bool> DeleteAsync(String name)
        {
            foreach (var item in db.Users)
            {
                if (item.Name == name)
                    db.Remove(item);
                break;
            }
            await db.SaveChangesAsync();
            return true;
        }
        [HttpPost]
        public async Task<bool> DeleteAll(String name)
        {
            foreach (var item in db.Users)
            {
                
                    db.Remove(item);
                
            }
            await db.SaveChangesAsync();
            return true;
        }
        [HttpPost]
        public String Email(String email, int number)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("smtp.mail.ru");
  
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("vip.cod05@mail.ru"); // Адрес отправителя
                mail.To.Add(new MailAddress(email)); // Адрес получателя
                mail.Subject = "Потвердите регистрацию";
                mail.Body = "Ваш код "+number;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.mail.ru";
                client.Port = 587; // Обратите внимание что порт 587
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("vip.cod05@mail.ru", "nikita"); // Ваши логин и пароль
                client.Send(mail);
                return "true";
            }
            catch(SmtpException ex) 
            {
                return ex.Message;

            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
    }
}

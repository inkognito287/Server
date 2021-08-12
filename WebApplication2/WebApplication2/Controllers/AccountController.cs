using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication2.Models;
using ZXing;

namespace WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        Token token = new Token();

        // тестовые данные вместо использования базы данных
        private List<Person> people = new List<Person>
        {
            new Person {Login="admin@gmail.com", Password="12345", Role = "admin" },
            new Person { Login="qwerty@gmail.com", Password="55555", Role = "user" },
             new Person { Login="Nikita", Password="123", Role = "user" }
        };

        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            token.tokenName = encodedJwt;
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };


            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            Person person = people.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }


        [HttpPost]
        public IActionResult test( string password)
        {

            if (password == "123") {
                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: null,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                token.tokenName = encodedJwt;
                var response = new
                {
                    access_token = encodedJwt,

                };

                return Json(response); 
                
            }
            else return BadRequest(new { errorText = "false" });
        }

        [HttpPost]
        public string testService(string name,string password)
        {
            if (password == "123" && name=="Nikita") {

                return "correct";

            }
            else  return "Неверный логин, или пароль" ;

        }

        [HttpPost]
        public bool image(byte[] image, string name, string code)
        {
            if (Directory.Exists("images")) { }
            else
                Directory.CreateDirectory("images");

            using (FileStream fileStream = new FileStream("images/" + name + ".txt", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(code);
                fileStream.Write(array);
            }
           Bitmap imgFromStream;
            using (var imageMemoryStream = new MemoryStream(image))
            {
                 imgFromStream = (Bitmap)Bitmap.FromStream(imageMemoryStream);
                imgFromStream.Save("images/" + name +DateTime.Now.Second+".bmp");

            }
            try
            {
                analyze(name, imgFromStream);
            }catch(Exception e) { 
            
            
            };
                return true;
        }

        private void analyze(string name , Bitmap bitmap)
        {
            if (Directory.Exists("serverAnalyze")) { }
            else
                Directory.CreateDirectory("serverAnalyze");


            BarcodeReader reader =  new BarcodeReader();
            var result = reader.Decode(bitmap);
            Console.WriteLine(result.Text);

            using (FileStream fileStream = new FileStream("serverAnalyze/" + name + ".txt", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(result.Text);
                fileStream.Write(array);
            }
        }
        
    }
}
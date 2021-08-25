using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
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
{   [Authorize] 
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
        [AllowAnonymous]
        [HttpPost]
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

        [AllowAnonymous]
        [HttpPost]
        public bool test( string password)
        {

            if (password == "123") {
               
                return true;
                
            }
            else return false;
        }

        [AllowAnonymous]
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
            if (User.Identity.IsAuthenticated)
            if (Directory.Exists("images")) { }
            else
                Directory.CreateDirectory("images");

            var time = DateTime.Now.Second;

            using (FileStream fileStream = new FileStream("images/" + name +time+".txt", FileMode.OpenOrCreate))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(code);
                fileStream.Write(array);
            }
           Bitmap imgFromStream;
            using (var imageMemoryStream = new MemoryStream(image))
            {
                 imgFromStream = (Bitmap)Bitmap.FromStream(imageMemoryStream);
                imgFromStream.Save("images/" + name +time+".bmp");

            }
            try
            {
                analyze(name+time, imgFromStream);
            }catch(Exception e) { 
            
            
            };
                return true;
        }

        private void analyze(string name , Bitmap bitmap)
        {
            if (Directory.Exists("serverAnalyze")) { }
            else
                Directory.CreateDirectory("serverAnalyze");

            try
            {
                BarcodeReader reader = new BarcodeReader();
                var result = reader.Decode(bitmap);
                Console.WriteLine(result.Text);
                var analyzeText = result.Text;
                using (FileStream fileStream = new FileStream("serverAnalyze/" + name + ".txt", FileMode.OpenOrCreate))
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(result.Text);
                    fileStream.Write(array);
                }

                Directory.CreateDirectory("noVerified");

                using (StreamReader fileRead = new StreamReader("images/" + name + ".txt"))
                {
                    if (analyzeText == fileRead.ReadLine()) 
                    {
                       
                    }
                    else
                    {
                        var pathOld = "images/" + name + ".bmp";
                        var pathNew = "noVerified/" + name + ".bmp";

                        FileInfo fileInf = new FileInfo(pathOld);
                        if (fileInf.Exists)
                        {
                            if (Directory.Exists(pathNew)) { }
                            else
                                Directory.CreateDirectory(pathNew);


                            fileInf.MoveTo(pathNew);

                        }
                    }
                }
            }
            catch (Exception e) {
                var pathOld = "images/" + name + ".bmp";
                var pathNew = "noVerified/" + name + ".bmp";

                FileInfo fileInf = new FileInfo(pathOld);
                if (fileInf.Exists)
                {
                    if (Directory.Exists(pathNew)) { }
                    else
                        Directory.CreateDirectory(pathNew);


                    fileInf.MoveTo(pathNew);

                }

                System.IO.File.WriteAllText("error.txt", e.ToString());
            }



        }
        
    }
}
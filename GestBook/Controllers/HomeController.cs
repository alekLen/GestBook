using GestBook.Models;
using GestBook.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using AutoMapper;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace GestBook.Controllers
{
    public class HomeController : Controller
    {

        IRepository rep;

        public HomeController(IRepository context)
        {
           rep = context;
        }

        public async Task< IActionResult> Index()
        {
           return View();
           
        }
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            IEnumerable<Message> list = await rep.GetMessage();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Message,NewMess>()
           .ForMember("user", opt => opt.MapFrom(c => c.user.Name)));
            var mapper = new Mapper(config);
            IEnumerable<NewMess> list1= mapper.Map<IEnumerable<Message>, IEnumerable<NewMess>>(await rep.GetMessage());
            string response = JsonConvert.SerializeObject(list1);
            return Json(response);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterModel user)
        {
            if (ModelState.IsValid)
            {
                User u = new();
                u.Name = user.Login;
                byte[] saltbuf = new byte[16];
                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(saltbuf);
                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();
                Salt s = new();
                s.salt = salt;
                string password = salt + user.Password;
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                u.Password = hashedPassword;
                try
                {
                    await rep.AddUser(u);
                    await rep.Save();
                    s.user = u;
                    await rep.AddSalt(s);
                    await rep.Save();
                }
                catch { }
                string response = "Вы успешно зарегестрировались!";
                return Json(response);
            }
            return Problem("Проблемы регистрации!");
        }

    }
}
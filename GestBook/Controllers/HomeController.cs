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
using Azure;

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

        public IActionResult AddMessage()
        {
            return PartialView("Message");
        }
        [HttpPost]
        public async Task<IActionResult> AddMessage([Bind("Text")] Message mes)
        {
            if(mes.Text==null)
                return Json(false);
            if (ModelState.IsValid)
            {
                var u = await rep.GetUser(HttpContext.Session.GetString("login"));
                mes.user = u;
                mes.MessageDate = DateTime.Now.ToString();
                try
                {
                    await rep.AddMessage(mes);
                    await rep.Save();
                    string response = "Ваш отзыв добавлен в Гостевую книгу!";
                    return Json(response);
                }
                catch
                {
                    string response1 = "Ошибка добавления! попробуйте позже!";
                    return Json(response1);
                }

            }
            string response2 = "Ошибка добавления! попробуйте позже!";
            return Json(response2);
        }
    }
}
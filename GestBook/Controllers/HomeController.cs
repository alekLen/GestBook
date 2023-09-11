using GestBook.Models;
using GestBook.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using AutoMapper;
using System.Numerics;

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

       

    }
}
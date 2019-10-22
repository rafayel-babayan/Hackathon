using System;
using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using reCAPTCHA.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hackathon.Controllers
{
    public class AdController : Controller
    {
        private readonly IRecaptchaService _rcService;
        private readonly IAdRepository _adRepository;
        private readonly IUserRepository _userRepository;
        public AdController(IRecaptchaService rcService, IAdRepository adRepository, IUserRepository userRepository)
        {
            _rcService = rcService;
            _adRepository = adRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index() 
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_userRepository.GetAllUsers(),"Id","Name"); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ad ad)
        { 
            var recaptcha = await _rcService.Validate(Request);

            if (recaptcha.success && ModelState.IsValid)
            {
                User user = new User { Name = "Rafo" };

                Ad ads = new Ad
                {
                    Content = "Content",
                    CreationDate = DateTime.Now,
                    Number = 1,
                    Rating = 10,
                    User = user

                };

                _adRepository.SaveAd(ads);

                
            }else
                return View();

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User usr)
        {
            var recaptcha = await _rcService.Validate(Request);

            if (recaptcha.success)
            {
                User user = new User { Name = "Rafo" };

                Ad ad = new Ad
                {
                    Content = "Content",
                    CreationDate = DateTime.Now,
                    Number = 1,
                    Rating = 10,
                    User = user

                };

                _adRepository.SaveAd(ad);
                
                return View();
            }

            return BadRequest();
        }
    }
}

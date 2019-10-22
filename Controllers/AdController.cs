using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Mvc;
using reCAPTCHA.AspNetCore;
using System;
using System.Threading.Tasks;

namespace Hackathon.Controllers
{
    public class AdController : Controller
    {
        private readonly IRecaptchaService _rcService;
        private readonly IAdRepository _adRepository;
        public AdController(IRecaptchaService rcService, IAdRepository adRepository)
        {
            _rcService = rcService;
            _adRepository = adRepository;
        }

        public async Task<IActionResult> Index() 
        {
            
            return View();
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

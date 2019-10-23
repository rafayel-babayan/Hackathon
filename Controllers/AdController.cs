using System;
using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using reCAPTCHA.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hackathon.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;

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
            ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdViewModel ad)
        {
            var recaptcha = await _rcService.Validate(Request);
            Ad newAd=null;
            using (MemoryStream stream = new MemoryStream())
            {
                await ad.Image.CopyToAsync(stream);

                if (stream.Length < 2097152)
                {
                    newAd = new Ad
                    {
                        Content = ad.Content,
                        CreationDate = ad.CreationDate,
                        Number = ad.Number,
                        Rating = ad.Rating,
                        UserId = ad.UserId,
                        Image = stream.ToArray()
                    };
                }
                else
                {
                    ModelState.AddModelError("Image", "The file is too large.");
                }
            }

            if (recaptcha.success && ModelState.IsValid)
            {
                _adRepository.SaveAd(newAd);
                RedirectToAction(nameof(Create));
            }
            else
            {
                ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");
                return View();
            }

            return BadRequest();
        }
    }
}

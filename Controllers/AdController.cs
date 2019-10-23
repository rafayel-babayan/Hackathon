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
using System.Linq;
using System.Collections;

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


        public async Task<IActionResult> Index(string orderBy, string searchStr, byte pageSize=10, int pageIndex=1)
        {
            IQueryable<Ad> source = _adRepository.GetAllAds();
            if (!string.IsNullOrEmpty(searchStr))
                source = source.Where(x => x.Content.Contains(searchStr));
            switch (orderBy)
            {
                case "number":
                    {
                        source = source.OrderBy(x => x.Number);
                        break;
                    }
            }

            return View(await PaginatedList<Ad>.CreateAsync(source, pageIndex, pageSize));
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
            Ad newAd = null;
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

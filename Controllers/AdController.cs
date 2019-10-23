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

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
           return View(_adRepository.GetAd(id));
        }

        [HttpPost]
        public IActionResult Delete(Ad ad)
        {
            _adRepository.DeleteAd(ad.Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index(string orderBy, string searchStr, byte pageSize = 1, int pageIndex = 1)
        {
            ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");
            IQueryable<Ad> source = _adRepository.GetAllAds();

            if (!string.IsNullOrEmpty(searchStr))
                source = source.Where(x => x.Content.Contains(searchStr));

            ViewBag.Num = orderBy == "numAsc" ? "numDesc" : "numAsc";
            ViewBag.Date = orderBy == "dateAsc" ? "dateDesc" : "dateAsc";
            ViewBag.Cont = orderBy == "contAsc" ? "contDesc" : "contAsc";
            ViewBag.Rate = orderBy == "rateAsc" ? "rateDesc" : "rateAsc";
            ViewBag.Usr = orderBy == "usrAsc" ? "usrDesc" : "usrAsc";
            ViewBag.OrderBy = orderBy;
            switch (orderBy)
            {
                case "numAsc":
                    source = source.OrderBy(x => x.Number);
                    break;
                case "numDesc":
                    source = source.OrderByDescending(x => x.Number);
                    break;
                case "dateAsc":
                    source = source.OrderBy(x => x.CreationDate);
                    break;
                case "dateDesc":
                    source = source.OrderByDescending(x => x.CreationDate);
                    break;
                case "contAsc":
                    source = source.OrderBy(x => x.Content);
                    break;
                case "contDesc":
                    source = source.OrderByDescending(x => x.Content);
                    break;
                case "rateAsc":
                    source = source.OrderBy(x => x.Rating);
                    break;
                case "rateDesc":
                    source = source.OrderByDescending(x => x.Rating);
                    break;
                case "usrAsc":
                    source = source.OrderBy(x => x.User.Name);
                    break;
                case "usrDesc":
                    source = source.OrderByDescending(x => x.User.Name);
                    break;
                default: break;
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
            if (!recaptcha.success)
                ModelState.AddModelError("", "Подтвердите что вы человек.");

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

            if (ModelState.IsValid)
            {
                _adRepository.SaveAd(newAd);
                RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");

            var model = _adRepository.GetAd(Id);
            var viewModel = new AdViewModel
            {
                Id = model.Id,
                Content = model.Content,
                CreationDate = model.CreationDate,
                Number = model.Number,
                Rating = model.Rating,
                UserId = model.UserId,
                OldImage = model.Image
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdViewModel ad)
        {

            var recaptcha = await _rcService.Validate(Request);
            if (!recaptcha.success)
                ModelState.AddModelError("", "Подтвердите что вы человек.");

            var model = _adRepository.GetAd(ad.Id);
            model.Content = ad.Content;
            model.CreationDate = ad.CreationDate;
            model.Number = ad.Number;
            model.Rating = ad.Rating;
            model.UserId = ad.UserId;

            if (ad.Image != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    await ad.Image.CopyToAsync(stream);

                    if (stream.Length < 2097152)
                    {
                        model.Image = stream.ToArray();
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "The file is too large.");
                    }
                }
            }

            if (ModelState.IsValid)
                _adRepository.UpdateAd(model);
            else
            {
                ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");
                return View();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

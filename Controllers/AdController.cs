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

        public async Task<IActionResult> Index(Guid user,
            DateTime from,
            DateTime to, 
            string searchStr,
            string sortStr,
            int pageSize = 3,
            int page = 1)
        {
            ViewBag.Users = new SelectList(_userRepository.GetAllUsers(), "Id", "Name");

            IQueryable<Ad> source = _adRepository.GetAllAds();

            //filtering
            if(user != Guid.Empty)
            {
                source = source.Where(x => x.UserId == user);
            }
            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                source = source.Where(x => x.CreationDate > from && x.CreationDate < to);
            }
            if (!string.IsNullOrEmpty(searchStr))
            {
                source = source.Where(x => x.Content.Contains(searchStr) 
                || x.User.Name.Contains(searchStr));
            }

            ViewBag.Num = sortStr == "numAsc" ? "numDesc" : "numAsc";
            ViewBag.Date = sortStr == "dateAsc" ? "dateDesc" : "dateAsc";
            ViewBag.Cont = sortStr == "contAsc" ? "contDesc" : "contAsc";
            ViewBag.Rate = sortStr == "rateAsc" ? "rateDesc" : "rateAsc";
            ViewBag.Usr = sortStr == "usrAsc" ? "usrDesc" : "usrAsc";
            ViewBag.OrderBy = sortStr;
            ViewBag.PageSize= pageSize;
            switch (sortStr)
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

            //model.;
            var T = new IndexViewModel
            {
                Ads = await PaginatedList<Ad>.CreateAsync(source, page, pageSize),
                FilterViewModel = new FilterViewModel(_userRepository.GetAllUsers().ToList(), user, searchStr, from, to)
            };
            return View(T);
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

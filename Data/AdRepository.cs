using Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hackathon.Data
{
    public interface IAdRepository
    {
        void SaveAd(Ad ad);
        IEnumerable<Ad> GetAllAds();
        Ad GetAd(Guid id);
        void DeleteAd(Guid id);
        void UpdateAd(Ad ad);
    }

    public class AdRepository : IAdRepository
    {
        private readonly ApplicationDbContext _context;
      
        public AdRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void DeleteAd(Guid id)
        {
            _context.Ads.Remove(GetAd(id));
        }

        public Ad GetAd(Guid id)
        {
            return _context.Ads.SingleOrDefault(ad => ad.Id.Equals(id));
        }

        public IEnumerable<Ad> GetAllAds()
        {
           return _context.Ads.AsEnumerable();
        }

        public void SaveAd(Ad ad)
        {
           _context.Ads.AddAsync(ad);
           _context.SaveChanges();
        }

        public void UpdateAd(Ad student)
        {
            _context.Ads.Update(student);
            _context.SaveChanges();
        }
    }
}

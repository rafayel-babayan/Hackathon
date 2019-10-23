using Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.ViewModels
{
    public class IndexViewModel
    {
        public  PaginatedList<Ad> Ads { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}

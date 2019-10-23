using Hackathon.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.ViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<User> users, Guid user, string searchstr, DateTime from, DateTime to)
        {
            
            users.Insert(0, new User { Name = "Все", Id = Guid.Empty });
            Users = new SelectList(users, "Id", "Name", user);
            SelectedUser = user;
            SearchString = searchstr;
            From = from;
            To = to;
        }
        public SelectList Users { get; private set; }
        public Guid SelectedUser{ get; private set; }
        public string SearchString { get; private set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}

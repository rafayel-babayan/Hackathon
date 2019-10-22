using Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hackathon.Data
{
    public interface IUserRepository
    {
        void SaveUser(User ad);
        IEnumerable<User> GetAllUsers();
        User GetUser(Guid id);
        void DeleteUser(Guid id);
        void UpdateUser(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void DeleteUser(Guid id)
        {
            _context.Users.Remove(GetUser(id));
        }

        public IEnumerable<User> GetAllUsers() => _context.Users.AsEnumerable();

        public User GetUser(Guid id) => _context.Users.SingleOrDefault(x=>x.Id.Equals(id));

        public void SaveUser(User ad)
        {
            _context.Users.Add(ad);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}

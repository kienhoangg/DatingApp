using System;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public Byte[] PasswordHash { get; set; }
        public Byte[] PasswordSalt { get; set; }

    }
}

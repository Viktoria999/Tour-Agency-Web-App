﻿namespace Domain.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string PasswordHash { get; set; }
        public bool? IsAdmin { get; set; }
    }
}

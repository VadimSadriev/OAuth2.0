﻿namespace OAuth.Server.Contracts
{
    public class RegisterAccountContract
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
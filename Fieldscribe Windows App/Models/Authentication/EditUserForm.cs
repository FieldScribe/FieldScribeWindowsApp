﻿using System;

namespace Fieldscribe_Windows_App.Models
{
    public class EditUserForm
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
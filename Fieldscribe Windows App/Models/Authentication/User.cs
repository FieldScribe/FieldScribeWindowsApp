﻿using System;
using System.Collections.Generic;

namespace Fieldscribe_Windows_App.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public IList<string> Roles { get; set; }
    }
}

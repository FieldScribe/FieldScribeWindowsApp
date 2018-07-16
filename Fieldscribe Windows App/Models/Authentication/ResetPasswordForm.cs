using System;

namespace Fieldscribe_Windows_App.Models
{
    public class ResetPasswordForm
    {
        public Guid UserId { get; set; }

        public string NewPassword { get; set; }
    }
}

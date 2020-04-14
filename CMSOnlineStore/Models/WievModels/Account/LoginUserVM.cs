﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMSOnlineStore.Models.WievModels.Account
{
    public class LoginUserVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}
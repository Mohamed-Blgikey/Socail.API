﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Helper
{
    public class AuthModel
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public bool IsAuthencated { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}

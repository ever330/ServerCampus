﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Models
{
    public class ResLoginToAPI
    {
        public ErrorCode Result { get; set; }
        public UserGameData GameData { get; set; }
    }
}

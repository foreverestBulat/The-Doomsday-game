﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Models;

[Serializable]
public class Message
{ 
    public string Name { get; set; }
    public string MessageText { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Models;

[Serializable]
public class Message
{
    public int UserID { get; set; }
    public string Name { get; set; }
    public string Sentence { get; set; }

    public override string ToString()
    {
        return $"{Name}: {Sentence}";
    }
}

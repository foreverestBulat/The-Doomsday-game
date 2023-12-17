using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Models;

[Serializable]
public class Player
{
    public int ID { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }
    public bool IsReady { get; set; }
    public Role Role { get; set; }
    public Loyalty FirstCard { get; set; }
    public Loyalty SecondCard { get; set; }
    public bool IsYourMove { get; set; }
    public Arsenal Gun { get; set; }
    public int HealthPoints { get; set; }
    public bool IsWatchCard { get; set; }


    public override string ToString()
    {
        return $"Name: {Name}\nColor: {Color}";
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Models;

[Serializable]
[KnownType(typeof(Player))]
[DataContract]
public class Player
{
    [DataMember]
    public int ID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public Color Color { get; set; }
    [DataMember]
    public bool IsReady { get; set; }

    [DataMember]
    public Role Role { get; set; }
    [DataMember]
    public Loyalty FirstCard { get; set; }
    [DataMember]
    public Loyalty SecondCard { get; set; }
    [DataMember]
    public bool IsYourMove { get; set; }
    //[DataMember]
    //public List<Arsenal> Guns { get; set; }


    public override string ToString()
    {
        return $"Name: {Name}\nColor: {Color}";
    }
}
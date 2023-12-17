using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Protocol.Models;

[Serializable]
public class Loyalty
{
    public string Name { get; set; }
    public int Score { get; set; }
    public string PathImage { get; set; }
    public bool IsAvailable = false;
    public string DefaultImage = "loyaltycard.jpg";
}

[Serializable]
public class HumanCard : Loyalty
{
    public HumanCard(int score)
    {
        Name = $"Человек X{score}";
        Score = score;
        PathImage = "humanloyalty.jpg";
    }
}

[Serializable]
public class RobotCard : Loyalty
{
    public RobotCard(int score)
    {
        Name = $"Робот X{Score}";
        Score = score;
        PathImage = "robotloyalty.png";
    }
}

[Serializable]
public class OutcastCard : Loyalty
{
    public OutcastCard(int score)
    {
        Name = $"Изгой X{score}";
        Score = score;
        PathImage = "outcastloyalty.jpg";
    }
}


//public class HumanX2Card : X2
//{
//    public HumanX2Card() { }
//}

//public class RobotX2Card : X2
//{
//    public RobotX2Card() { }
//}

//public class OutcastX2Card : X2
//{
//    public OutcastX2Card() { }
//}
//public class HumanCard : Loyalty
//{
//    public HumanCard()
//    {
//        Score = 1;
//    }
//}

//public class RobotCard : Loyalty
//{
//    public RobotCard()
//    {
//        Score = 1;
//    }
//}

//public class OutcastCard : Loyalty
//{
//    public OutcastCard()
//    {
//        Score = 1;
//    }
//}

//public class HumanX2Card : Loyalty
//{
//    public HumanX2Card()
//    {
//        Score = 2;
//    }
//}

//public class RobotX2Card : Loyalty
//{
//    public RobotX2Card()
//    {
//        Score = 2;
//    }
//}

//public class OutcastX2Card : Loyalty
//{
//    public OutcastX2Card()
//    {
//        Score = 2;
//    }
//}
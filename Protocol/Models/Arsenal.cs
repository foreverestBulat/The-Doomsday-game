using Protocol.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Models;

[Serializable]
public class Arsenal 
{
    public XPacketActions Action = XPacketActions.Shot;
    public XPacketTypes Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Shot { get; set; }
    public string NameImage { get; set; }
}

[Serializable]
public class Handgun : Arsenal
{
    public Handgun()
    {
        Action = XPacketActions.Shot;
        Type = XPacketTypes.Handgun;
        Name = "Пистолет";
        Description = "Назначьте целью любого игрока, кроме себя: направьте оружие на него.";
        Shot = "Цель открывает одну из своих карт верности, или получает одно повреждение. Бросьте оружие и возьмите из колоды программу.";
        NameImage = "handgun.jpg";
    }

    //public static string nameImage = "handgun.jpg";
    //public static XPacketActions action = XPacketActions.Shot;
    //public static XPacketTypes type = XPacketTypes.Handgun;
    //public static string name = "Пистолет";
    //public static string description = "Назначьте целью любого игрока, кроме себя: направьте оружие на него.";
    //public static string shot = "Цель открывает одну из своих карт верности, или получает одно повреждение. Бросьте оружие и возьмите из колоды программу.";
}

[Serializable]
public class Rifle : Arsenal
{
    public Rifle()
    {
        Action = XPacketActions.Shot;
        Type = XPacketTypes.Rifle;
        Name = "Винтовка";
        Description = "Назначьте целью любого игрока, кроме себя: направьте оружие на него.";
        Shot = "Цель открывает карту роли, или получает два повреждения. Бросьте это оружие.";
        NameImage = "rifle.jpg";
    }

    //public static string nameImage = "rifle.jpg";
    //public static XPacketActions action = XPacketActions.Shot;
    //public static XPacketTypes type = XPacketTypes.Rifle;
    //public static string name = "Винтовка";
    //public static string description = "Назначьте целью любого игрока, кроме себя: направьте оружие на него.";
    //public static string shot = "Цель открывает карту роли, или получает два повреждения. Бросьте это оружие.";
}

[Serializable]
public class Laser : Arsenal
{
    public Laser()
    {
        Action = XPacketActions.Shot;
        Type = XPacketTypes.Laser;
        Name = "Лазер";
        Description = "Не направляйте его ни на кого. Выберите и назовите цель в момент выстрела.";
        Shot = "Назовите цель. Этот игрок открывает карту роли, или получает два повреждения. Бросьте это оружие.";
        NameImage = "lasergun.jpg";
    }

    //public static string nameImage = "lasergun.jpg";
    //public static XPacketActions action = XPacketActions.Shot;
    //public static XPacketTypes type = XPacketTypes.Laser;
    //public static string name = "Лазер";
    //public static string description = "Не направляйте его ни на кого. Выберите и назовите цель в момент выстрела.";
    //public static string shot = "Назовите цель. Этот игрок открывает карту роли, или получает два повреждения. Бросьте это оружие.";
}

[Serializable]
public class FlareGun : Arsenal
{
    public FlareGun()
    {
        Action = XPacketActions.Shot;
        Type = XPacketTypes.FlareGun;
        Name = "Ракетница";
        Description = "Выберите двух игроков в качестве цели. Эти игроки должны сидеть рядом друг с другом.";
        Shot = "Каждая цель открывает одну карту верности или получаете одно повреждение. Бросьте это оружие и позьмите из колоды программу.";
        NameImage = "rocketgun.jpg";
    }

    //public static string nameImage = "rocketgun.jpg";
    //public static XPacketActions action = XPacketActions.Shot;
    //public static XPacketTypes type = XPacketTypes.FlareGun;
    //public static string name = "Ракетница";
    //public static string description = "Выберите двух игроков в качестве цели. Эти игроки должны сидеть рядом друг с другом.";
    //public static string shot = "Каждая цель открывает одну карту верности или получаете одно повреждение. Бросьте это оружие и позьмите из колоды программу.";
}

[Serializable]
public class Assistant : Arsenal
{
    public Assistant()
    {
        Action = XPacketActions.Shot;
        Type = XPacketTypes.Assistant;
        Name = "Напарник";
        Description = "Назначьте целью любого игрока, кроме себя: направьте оружие на него.";
        Shot = "Цель открывает одну из своих карт верности, или получает одно повреждение. Вы можете посстановить однужизнь уцели вместо стрельбы. Бросьте это оружие и возьмите из колоды программу.";
        NameImage = "assistant.jpg";
    }

    //public static string nameImage = "assistant.jpg";
    //public static XPacketActions action = XPacketActions.Shot;
    //public static XPacketTypes type = XPacketTypes.Assistant;
    //public static string name = "Напарник";
    //public static string description = "Назначьте целью любого игрока, кроме себя: направьте оружие на него.";
    //public static string shot = "Цель открывает одну из своих карт верности, или получает одно повреждение. Вы можете посстановить однужизнь уцели вместо стрельбы. Бросьте это оружие и возьмите из колоды программу.";
}
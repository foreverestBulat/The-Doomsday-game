using Newtonsoft.Json.Serialization;
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
    public bool IsAvailable { get; set; }
    public bool IsReadyShoot { get; set; }

    public static ((int, XPacketActions), (XPacketActions, XPacketActions)) ActionShot(Arsenal gun)
    {
        switch (gun)
        {
            case Handgun:
                return ((1, XPacketActions.OpenCard), 
                        (XPacketActions.TakeProgram, XPacketActions.DropGun));
                
            case Rifle:
                return ((2, XPacketActions.OpenRole),
                        (XPacketActions.Nothing, XPacketActions.DropGun));
                

            case Laser:
                return ((2, XPacketActions.OpenRole),
                        (XPacketActions.Nothing,XPacketActions.DropGun));
                

            case FlareGun:
                return ((1, XPacketActions.OpenCard),
                        (XPacketActions.TakeProgram, XPacketActions.DropGun));
                

            case Assistant:
                return ((1, XPacketActions.OpenCard),
                        (XPacketActions.TakeProgram, XPacketActions.DropGun));

            default:
                throw new NotImplementedException();
        }
    }
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
        IsAvailable = true;
        IsReadyShoot = false;
    }
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
        IsAvailable = true;
        IsReadyShoot = false;
    }
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
        IsAvailable = true;
        IsReadyShoot = false;
    }
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
        Shot = "Каждая цель открывает одну карту верности или получаете одно повреждение. Бросьте это оружие и возьмите из колоды программу.";
        NameImage = "rocketgun.jpg";
        IsAvailable = true;
        IsReadyShoot = false;
    }
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
        Shot = "Цель открывает одну из своих карт верности, или получает одно повреждение. Вы можете восстановить одну жизнь уцели вместо стрельбы. Бросьте это оружие и возьмите из колоды программу.";
        NameImage = "assistant.jpg";
        IsAvailable = true;
        IsReadyShoot = false;
    }
}
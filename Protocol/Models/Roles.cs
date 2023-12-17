using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Protocol.Models;

[Serializable]
public class Role
{
    public bool IsAlways;
    public string IsAlwaysWho;

    public Color Color;

    public string Name;
    public string Description;
    public string PermanentEffect;
    public string ResourceImage;
    public static string DefaultImage = "rolecard.jpg";
    public string Hint;

    public bool IsAvailable;

}


[Serializable]
public class Human : Role
{
    public Human()
    {
        Color = Color.Blue;
        IsAlwaysWho = "Человек";
        IsAvailable = false;
    }
}
[Serializable]
public class Robot : Role
{
    public Robot()
    {
        Color = Color.Red;
        IsAlwaysWho = "Робот";
        IsAvailable = false;
    }
}
[Serializable]
public class Outcast : Role
{
    public Outcast()
    {
        Color = Color.Gray;
        IsAlwaysWho = "Изгой";
        IsAvailable = false;
    }
}


public static class Data
{
    public static List<Role> Roles = new List<Role>()
    {
        new Human()
        {
            IsAlways = false,
            Name = "Невидимка",
            Description = null,
            PermanentEffect = "Если все ваши карты роли и верности открыты, вы можете скрыть их обратно. Можно скрыть одну, две или все.",
            Hint = null,
            ResourceImage = "invisibleman.jpg"
        },
        new Outcast()
        {
            IsAlways = true,
            IsAlwaysWho = "Павший",
            Name = "Несущий смерть",
            Description = "Когда открыта, верните всех убитых игроков в игру с одной жизнью и одной скрытой картой верности. Эти игроки становятся павшими(только если верны легиону).",
            PermanentEffect = null,
            Hint = "(Возможна смена вероятности)",
            ResourceImage = "death.jpg"
        },
        new Robot()
        {
            IsAlways = false,
            Name = "Шутник",
            Description = "Возьмите из колоды три программы и сбросьте после этого одну программу.",
            PermanentEffect = "В качестве действия вы можете вооружиться ракетницей.",
            Hint = null,
            ResourceImage = "shut.jpg"
        },
        new Outcast()
        {
            IsAlways = false,
            Name = "Крот",
            Description = null,
            PermanentEffect = "В начале хода каждого игрока вы можете подсмотреть одну его карту верности и роли.",
            Hint = null,
            ResourceImage = "krot.jpeg"
        },
        new Human()
        {
            IsAlways = false,
            Name = "Грюнвальд",
            Description = null,
            PermanentEffect = "В начале вашего хода, если у вас есть одна или больше программ, вы можете поменяться одной или двумя программами на выбор с любым другим игроком.",
            Hint = null,
            ResourceImage = "grunvald.jpg"
        },
        new Robot()
        {
            IsAlways = false,
            Name = "Исполнитель",
            Description = null,
            PermanentEffect = "Когда любой другой игрок бросает оружие, вы можете вооружиться им, если сейчас не вооружены. Когда любой другой игрок играет программу без постоянного эффекта вы можете взять себе эту программу.",
            Hint = null,
            ResourceImage = "ispolnitel.jpg"
        },
        new Human()
        {
            IsAlways = false,
            Name = "Фотограф",
            Description = null,
            PermanentEffect = "Когда открывается чья-то карта верности, вы можете вылечить одну жизнь любому игроку. Если открывается чья-то роль, скройте свою карту роли.",
            Hint = null,
            ResourceImage = "photograph.jpg"
        },
        new Outcast()
        {
            IsAlways = true,
            IsAlwaysWho = "Легион",
            Name = "Пожиратель разума",
            Description = "Когда открыта, все другие игроки закрывают глаза и кладут правый кулак на стол. Коснитесь одного кулака (или двух, если играет больше семи человек). Эти игроки отныне легион. Они теперь в вашей команде до конца игры. Все игроки убирают кулаки со стола и открывают глаза.",
            PermanentEffect = null,
            Hint = null,
            ResourceImage = "pozhiratel.jpg"
        },
        new Robot()
        {
            IsAlways = false,
            Name = "Посторонним в",
            Description = null,
            PermanentEffect = "Когда игрок подсматривает чью-то карту роли или верности, откройте или скройте любую карту роли по вашему выбору.",
            Hint = null,
            ResourceImage = "postoronimv.jpg"
        },
        new Outcast()
        {
            IsAlways = true,
            IsAlwaysWho = "Изгой",
            Name = "Бездомный",
            Description = null,
            PermanentEffect = "Все другие игроки больше не могут брать из колоды программмы.",
            Hint = "(Действует ксли вас убили)",
            ResourceImage = "homeless.jpg"
        },
        new Human()
        {
            IsAlways = false,
            Name = "Астронавт",
            Description = null,
            PermanentEffect = "В свой ход можете разыграть два разных действия вместо одного. Только если вы не вооружены.",
            Hint = null,
            ResourceImage = "astronaut.jpeg"
        },
        new Human()
        {
            IsAlways = false,
            Name = "Тайный агент",
            Description = "Когда открываете, все игроки бросают оружие. Можете вооружить другого игрока пистолетом, а себя ракетницей.",
            PermanentEffect = null,
            Hint = null,
            ResourceImage = "tainiyagent.jpg"
        },
        new Human()
        {
            IsAlways = false,
            Name = "Госпожа",
            Description = null,
            PermanentEffect = "Все игроки могут вооружиться заблокированным оружием, но не могут целиться в вас этим оружием. Ракетница и Врата ада пропускают вас.",
            Hint = null,
            ResourceImage = "gospozha.jpg"
        },
        new Human()
        {
            IsAlways = false,
            Name = "Верующий",
            Description = "Когда открываете, берите карты программ, пока не возьмете портал. Положите его рядом с колодой программ, а остальные взятые программы сбросьте.",
            PermanentEffect = "Вас не могут ранить боссы Экс-автоматом",
            Hint = null,
            ResourceImage = "veruushiy.jpg"
        },



        


        //{
        //    IsAlways = ,
        //    IsAlwaysWho = ,
        //    Name = ,
        //    Description = ,
        //    PermanentEffect = ,
        //    Hint = ,
        //}


    };

    public static List<Role> SuperRole = new List<Role>
    {
        new Robot()
        {
            IsAlways = true,
            Name = "Бог из машины",
            Description = "Когда открыта, все другие мгроки должны открыть свои карты верности.",
            PermanentEffect = "В качестве дейсвия вы можете вооружиться лазером.",
            Hint = null,
            ResourceImage = "godrobot1.jpg"
        },
        new Robot()
        {
            IsAlways = true,
            Name = "Бог из машины",
            Description = "Когда открыта, все другие мгроки должны открыть свои карты ролей.",
            PermanentEffect = "В качестве дейсвия вы можете вооружиться ракетницей.",
            Hint = null,
            ResourceImage = "godrobot2.jpg"
        },
    };

    //public bool IsAlways;
    //public string IsAlwaysWho;

    //public Color Color;

    //public string Name;
    //public string Description;
    //public string PermanentEffect;
    //public string Hint;

}


//public class InvisibleMan : Human
//{
//    public InvisibleMan()
//    {
//        Name = "Невидимка";
//        PermanentEffect = "Если все ваши карты роли и верности открыты, вы можете скрыть их обратно. Можно скрыть одну, две или все."
//    }
//}

//public class FireWall : Robot
//{
//    public FireWall()
//    {
//        Description = "Когда открываете, все игроки "
//    }
//}



//public class Role
//{
//    public class Robot
//    {
//        public static readonly Color Color = Color.Red;
//    }

//    public class Human
//    {
//        public static readonly Color Color = Color.Blue;
//    }

//    public class Outcast
//    {
//        public static readonly Color Color = Color.Gray;
//    }
//}

//public string Name = "Вирус";
//public 
//public string Desription = "Когда открываете, поменяйте две любые карты верновсти. Эти карты остаются как лежали, скрытыми или открытыми."



//public enum Roles
//{
//    Human,
//    Robot,
//    Outcast
//}

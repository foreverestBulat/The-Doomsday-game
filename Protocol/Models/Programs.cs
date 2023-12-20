using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Models;

[Serializable]
public enum Programs
{
    RoleExchange,
    ChangeDirectionEveryoneWeapons,
    RestoreHealthPoints,
    GeneralPain,
    OpenAccessNewWeapons,
    ChangeDirectionAnyWeapons,
}

[Serializable]
public class AppleyProgramSend
{
    public Program Program { get; set; }
    public object Data { get; set; }
}


[Serializable]
public class Program
{
    public string Description { get; set; }
    public string PermanentEffect { get; set; }
    public string Hint { get; set; }
    public Programs ProgramAction { get; set;}

    public override string ToString()
    {
        string decription = "\n" + Description + "\n";
        if (PermanentEffect != null)
            decription += $"Постоянный эффект:\n{PermanentEffect}\n";

        if (Hint != null)
            decription += $"Подсказка:\n{Hint}\n";

        return decription;
    }


    public static List<Program> Programs = new List<Program>()
    {
        new Program()
        {
            Description = "Обменяйте две любые карты роли. Не скрывайте и не открывайте карты при обмене - они остаются как были",
            PermanentEffect = null,
            Hint = "(Возможна смена верности)",
            ProgramAction = Models.Programs.RoleExchange,
        },
        //new Program()
        //{
        //    Description = "Поменяйте цель каждого оружия на следующего игрока по часовой стрелке",
        //    PermanentEffect = null,
        //    Hint = null,
        //    ProgramAction = Models.Programs.ChangeDirectionEveryoneWeapons
        //},
        //new Program()
        //{
        //    Description = "Вы можете восстановить одну жизнь любому живому игроку",
        //    PermanentEffect = null,
        //    Hint = null,
        //    ProgramAction = Models.Programs.RestoreHealthPoints,
        //},
        //new Program()
        //{
        //    Description = "Выберите игрока, когда вы получаете повреждение, этот игрок получает столько же повреждений",
        //    PermanentEffect = null,
        //    Hint = null,
        //    ProgramAction = Models.Programs.GeneralPain
        //},
        //new Program()
        //{
        //    Description = "Вы можете вооружится лазером или ракетницей",
        //    PermanentEffect = null,
        //    Hint = null,
        //    ProgramAction = Models.Programs.OpenAccessNewWeapons
        //},
        //new Program()
        //{
        //    Description = "Измените цель любого оружия",
        //    PermanentEffect = null,
        //    Hint = null,
        //    ProgramAction = Models.Programs.ChangeDirectionAnyWeapons
        //},
    };
}


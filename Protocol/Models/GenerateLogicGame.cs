using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Protocol.Models;

public class GenerateLogicGame
{
    private static Random Random = new Random();

    public static void SetRolePlayers(List<Player> players)
    {
        (int countHumans, int countRobots, int countOutcasts) = GetCountRoles(players.Count);

        var humans = Data.Roles.Where(role => role is Human);
        var robots = Data.Roles.Where(role => role is Robot);
        var outcasts = Data.Roles.Where(role => role is Outcast);

        humans = SelectShuffleCount(humans, countHumans);
        robots = SelectShuffleCount(robots, countRobots);
        outcasts = SelectShuffleCount(outcasts, countOutcasts);

        var roles = new List<Role>();

        foreach (var role in humans)
            roles.Add(role);
        foreach (var role in robots) 
            roles.Add(role);        
        foreach (var role in outcasts)
            roles.Add(role);

        Shuffle(roles);

        int i = 0;
        foreach (var player in players)
            player.Role = roles[i++];

        var numIsFirstMove = Random.Next(0, players.Count - 1);
        Console.WriteLine(numIsFirstMove);
        players[numIsFirstMove].IsYourMove = true;

        //foreach (var player in players)
        //{
        //    player.IsYourMove = true;//
        //}

        (int countX1Humans,
        int countX1Robots,
        int countX1Outcast,
        int countX2Humans,
        int countX2Robots,
        int countX2Outcast) = GetCountLoyaltyCard(players.Count);

        var cards = GenerateLoyaltyCard
            (countX1Humans,
            countX1Robots,
            countX1Outcast,
            countX2Humans,
            countX2Robots,
            countX2Outcast);

        Shuffle(cards);


        int j = 0;
        foreach (var player in players)
        {
            player.FirstCard = cards[j++];
            player.SecondCard = cards[j++];
        }
        //var guns = GetGuns(players.Count);
        //player.Guns = guns;
    }

    private static List<Role> SelectShuffleCount(IEnumerable<Role> roles, int count)
    {
        var oldRoles = roles.ToList();
        Shuffle(oldRoles);

        var newRoles = new List<Role>();
        for (int i = 0; i < count; i++)
            newRoles.Add(oldRoles[i]);
        
        return newRoles;
    }

    private static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private static List<Loyalty> GenerateLoyaltyCard
        (int countX1Humans,
        int countX1Robots,
        int countX1Outcast,
        int countX2Humans,
        int countX2Robots,
        int countX2Outcast)
    {
        List<Loyalty> cards = new List<Loyalty>();

        for (int i = 0; i < countX1Humans; i++)
            cards.Add(new HumanCard(1));

        for (int i = 0; i < countX1Robots; i++)
            cards.Add(new RobotCard(1));

        for (int i = 0; i < countX1Outcast; i++)
            cards.Add(new OutcastCard(1));


        for (int i = 0; i < countX2Humans; i++)
            cards.Add(new HumanCard(2));

        for (int i = 0; i < countX2Robots; i++)
            cards.Add(new RobotCard(2));

        for (int i = 0; i < countX2Outcast; i++)
            cards.Add(new OutcastCard(2));

        return cards;
    }

    private static (int, int, int, int, int, int) GetCountLoyaltyCard(int countPlayers)
    {
        switch (countPlayers)
        {
            case 4:
                return (3, 2, 0, 1, 1, 1);
                
            case 5:
                return (4, 3, 0, 1, 1, 1);
                
            case 6:
                return (3, 4, 2, 2, 1, 1);

            case 7:
                return (4, 3, 3, 2, 2, 0);

            case 8:
                return (6, 5, 0, 2, 2, 1);

            case 9:
                return (6, 5, 0, 2, 2, 1);

            case 10:
                return (6, 5, 0, 2, 2, 1);
            default:
                throw new NotImplementedException();
        }
    }

    private static (int, int, int) GetCountRoles(int countPlayers)
    {
        switch (countPlayers)
        {
            case 4:
                return (2, 2, 1);
                
            case 5:
                return (2, 2, 2); 
                
            case 6:
                return (3, 2, 2);
                
            case 7:
                return (3, 3, 2);
                
            case 8:
                return (4, 3, 2);
                
            case 9:
                return (4, 3, 3);
                
            case 10:
                return (4, 4, 3);
                
            default:
                throw new NotImplementedException();
        }
    }

    public static List<Arsenal> GetGuns(int countPlayers)
    {
        switch (countPlayers)
        {
            case 4:
                return new List<Arsenal>
                {
                    new Handgun(),
                    new Rifle(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            case 5:
                return new List<Arsenal>
                {
                    new Handgun(),
                    new Rifle(),
                    new Assistant(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            case 6:
                return new List<Arsenal> 
                {
                    new Handgun(),
                    new Rifle(),
                    new Assistant(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            case 7:
                return new List<Arsenal> 
                { 
                    new Handgun(),
                    new Rifle(),
                    new Rifle(),
                    new Assistant(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            case 8:
                return new List<Arsenal>
                {
                    new Handgun(),
                    new Rifle(),
                    new Rifle(),
                    new Assistant(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            case 9:
                return new List<Arsenal>
                {
                    new Handgun(),
                    new Rifle(),
                    new Rifle(),
                    new Assistant(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            case 10:
                return new List<Arsenal>
                {
                    new Handgun(),
                    new Rifle(),
                    new Rifle(),
                    new Assistant(),
                    new Laser()
                    {
                        IsAvailable = false,
                    },
                    new FlareGun()
                    {
                        IsAvailable = false
                    }
                };

            default:
                throw new NotImplementedException();
        }
    }
}

using Newtonsoft.Json.Serialization;
using Protocol;
using Protocol.Models;
using Protocol.Packet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerUseSocket;

public class ConnectedClient
{
    public int ID { get; set; }
    public XServer Server {  get; private set; }
    public Socket Client { get; set; }
    public Player Person { get; set; }

    public Queue<XPacket> ReceivedPackets { get; set; }

    public ConnectedClient(Socket client, XServer server)
    {
        Client = client;
        Server = server;

        ReceivedPackets = new Queue<XPacket>();

        // run receive and send packets
    }

    public void RunPackets()
    {
        Task.Run(ReceivePackets);
        Task.Run(CompleteReceivedPackets);
    }

    public async Task<XPacket> ReceivePacket()
    {
        var buffer = new byte[1024];
        int bytesRead = await Client.ReceiveAsync(buffer);
        var packetData = new List<byte>(buffer.Take(bytesRead));

        while (bytesRead == buffer.Length)
        {
            buffer = new byte[1024];
            bytesRead = await Client.ReceiveAsync(buffer);
            packetData.AddRange(buffer.Take(bytesRead));
        }

        return XPacketConverter.FromByteArray(packetData.ToArray());
    }

    public async Task SendPacket(XPacket packet)
    {
        var buffer = XPacketConverter.ToByteArray(packet);
        await Client.SendAsync(buffer);
    }

    public async Task ReceivePackets()
    {
        while (true)
        {
            try
            {
                await Task.Delay(100);
                var packet = await ReceivePacket();
                Console.WriteLine("Client прислал пакет");
                ReceivedPackets.Enqueue(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Client разорвал соединение");
                Close();
                break;
            }
        }
    }

    public async void CompleteReceivedPackets()
    {
        while (true)
        {
            if (ReceivedPackets.Count == 0)
                continue;

            await Complete(ReceivedPackets.Dequeue());
        }
    }

    public async Task Complete(XPacket packet)
    {
        switch (packet.Action)
        {
            case XPacketActions.Readiness:
                {
                    var person = (Player)packet.Content;
                    Person.IsReady = person.IsReady;
                    var isEveryOneReady = Server.IsEveryoneReady();

                    if (isEveryOneReady)
                        await Server.StartGame();
                    else
                        await Server.BroadcastUsers();
                    break;
                }

            case XPacketActions.SendMessage:
                {
                    await Server.BroadcastMessage(this, packet.Content.ToString());
                    break;
                }

            case XPacketActions.ArmYourself:
                {
                    Person.Gun = (Arsenal)packet.Content;
                    var newPacket = new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Gun,
                        Content = $"{Person.Name} вооружился оружием {Person.Gun.Name}"
                    };
                    await Server.BroadcastPacket(newPacket);
                    break;
                }

            case XPacketActions.AimWeapon:
                {
                    var id = (int)packet.Content;
                    var client = Server.Clients.Where(cl => cl.ID == id).FirstOrDefault();
                    client.Person.GunsIsPointedMe.Add(Person.Gun);
                    var newPacket = new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Gun,
                        Content = $"{Person.Name} направил {Person.Gun.Name} на игрока {client.Person.Name}"
                    };
                    Person.Gun.IsReadyShoot = true;
                    await Server.BroadcastPacket(newPacket);
                    break;
                }

            case XPacketActions.Shot:
                {
                    var id = (int)packet.Content;
                    var client = Server.Clients.Where(cl => cl.ID == id).FirstOrDefault();
                    var newPacket = new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Gun,
                        Content = $"{Person.Name} выстрелил {Person.Gun.Name} в игрока {client.Person.Name}"
                    };
                    await Server.BroadcastPacket(newPacket);

                    ((int lostHealthPoints, var actionForVictim),
                    (var actionForAggressor1, var actionForAggressor2)) = Arsenal.ActionShot(Person.Gun);

                    await client.SendPacket(new XPacket()
                    {
                        Action = XPacketActions.Shot,
                        Type = XPacketTypes.Gun,
                        Content = (lostHealthPoints, actionForVictim)
                    });

                    await SendPacket(new XPacket()
                    {
                        Action = actionForAggressor1,
                        Type = XPacketTypes.Gun,
                    });

                    await SendPacket(new XPacket()
                    {
                        Action = actionForAggressor2,
                        Type = XPacketTypes.Gun,
                    });

                    Person.Gun = null;
                    break;
                }

            case XPacketActions.TakeProgram:
                {
                    var program = Server.Programs.Dequeue();
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Program,
                        Content = $"{Person.Name} взял программу:\n{program}"
                    });
                    break;
                }

            case XPacketActions.ApplyProgram:
                {
                    var applyProgram = (AppleyProgramSend)packet.Content;
                    await Server.ApplyProgram(applyProgram);
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Program,
                        Content = $"{Person.Name} применяет программу: \n{applyProgram.Program}"
                    });
                    break;
                }

            case XPacketActions.OpenFirstCard:
                {
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.OpenFirstCard,
                        Type = XPacketTypes.Card,
                        Content = ID
                    });
                    break;
                }

            case XPacketActions.OpenSecondCard:
                {
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.OpenSecondCard,
                        Type = XPacketTypes.Card,
                        Content = ID
                    });
                    break;
                }

            case XPacketActions.OpenRole:
                {
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.OpenRole,
                        Type = XPacketTypes.Card,
                        Content = ID
                    });
                    break;
                }

            case XPacketActions.LostHealthPoints:
                {
                    Person.HealthPoints -= (int)packet.Content;

                    if (Person.HealthPoints <= 0)
                    {
                        Person.Role.IsAvailable = true;
                        Person.FirstCard.IsAvailable = true;
                        Person.SecondCard.IsAvailable = true;
                        await Server.BroadcastUsers();
                        Close();
                    }

                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Card,
                        Content = $"{Person.Name} потерял очки жизни"
                    });
                    await Server.BroadcastUsers();
                    break;
                }

            case XPacketActions.FinishStep:
                {
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Message,
                        Content = $"{Person.Name} закончил ход"
                    });
                    var client = Server.GetNextPlayer();
                    await Server.BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Message,
                        Content = $"{client.Person.Name} делает следующий ход"
                    });
                    Person.IsMyMove = false;
                    client.Person.IsMyMove = true;
                    await Server.BroadcastUsers();
                    break;
                }
        }
    }

    public void Close()
    {
        Server.Clients.Remove(this);
        Client.Close();
        Server.BroadcastUsers();
    }

    public override string ToString()
    {
        return $"Socket client: {Client}\nName: {Person.Name}\nColor: {Person.Color}";
    }
}
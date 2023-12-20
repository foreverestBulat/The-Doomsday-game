using Protocol;
using Protocol.Models;
using Protocol.Packet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ApplyPrograms = Protocol.Models.Programs;

namespace ServerUseSocket;

public class XServer
{
    public IPEndPoint IPEndPoint { get; private set; }
    public Socket Listener { get; private set; }
    public bool Listening {  get; private set; }
    public List<ConnectedClient> Clients { get; set; }
    public Queue<Protocol.Models.Program> Programs { get; set; }

    public XServer(IPEndPoint ipEndPoint)
    {
        IPEndPoint = ipEndPoint;
        Listener = new Socket(
            ipEndPoint.AddressFamily, 
            SocketType.Stream, 
            ProtocolType.Tcp
        );

        Console.WriteLine("Создание сервера");
    }

    public void Start()
    {
        Listener.Bind(IPEndPoint);
        Listener.Listen(100);
        Listening = true;
        Clients = new List<ConnectedClient>();
    }


    public async Task BroadcastPacket(XPacket packet, int? id=null)
    {
        foreach (var client in Clients)
        {
            if (id != null && client.ID == id)
                continue;

            await client.SendPacket(packet);
        }
    }

    public async Task<bool> AddUser(ConnectedClient newClient)
    {
        var players = GetPlayers();
        players.Add(newClient.Person);
        await newClient.SendPacket(new XPacket()
        {
            Action = XPacketActions.AddUser,
            Type = XPacketTypes.Persons,
            Content = players
        });

        var packet = new XPacket()
        {
            Action = XPacketActions.AddUser,
            Type = XPacketTypes.Person,
            Content = newClient.Person
        };

        foreach (var client in Clients)
            await client.SendPacket(packet);
        return true;
    }

    public async Task<Socket> AcceptClient()
    {
        Console.WriteLine("Прослушивание одного клиента");
        Socket handler = await Listener.AcceptAsync();
        return handler;
    }

    public async Task SendPacket(Socket client, XPacket packet)
    {
        var buffer = XPacketConverter.ToByteArray(packet);
        await client.SendAsync(buffer);
    }

    public async Task<XPacket> ReceivePacket(Socket client)
    {
        var buffer = new byte[8192 * 2];
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        return XPacketConverter.FromByteArray(buffer);
    }

    public async Task BroadcastUsers()
    {
        var packet = new XPacket()
        {
            Action = XPacketActions.AddUser,
            Type = XPacketTypes.Persons,
            Content = GetPlayers()
        };

        foreach (var client in Clients)
        {
            await client.SendPacket(packet);
            Console.WriteLine($"Отправил {client.Person.Name}");
            Console.WriteLine("Broadcast users");
        }
    }

    public async Task BroadcastMessage(ConnectedClient clientSend, string message)
    {
        foreach (var client in Clients)
        {
            XPacket packet;
            if (client.ID == clientSend.ID)
            {
                packet = new XPacket()
                {
                    Action = XPacketActions.SendMessage,
                    Type = XPacketTypes.Message,
                    Content = $"Я: {message}"
                };
            }
            else
            {
                packet = new XPacket()
                {
                    Action = XPacketActions.SendMessage,
                    Type = XPacketTypes.Message,
                    Content = $"{clientSend.Person.Name}: {message}"
                };
            }
            
            await client.SendPacket(packet);
        }
    }

    public ConnectedClient GetNextPlayer()
    {
        bool isNextPlayer = false;
        foreach (var client in Clients)
        {
            if (isNextPlayer)
                return client;
            if (client.Person.IsMyMove)
                if (client == Clients.Last())
                    return Clients.First();
                else
                    isNextPlayer = true;
        }
        return null;
    }

    public List<Player> GetPlayers()
    {
        var list = new List<Player>();
        foreach (var client in Clients)
            list.Add(client.Person);
        return list;
    }

    public async Task<ConnectedClient> SignIn(Socket client)
    {
        var packet = await ReceivePacket(client);

        if (packet.Type == XPacketTypes.SignIn)
        {
            var person = (Player)packet.Content;
            for (int i = 0; i < Clients.Count; i++)
            {
                if (person.Name == Clients[i].Person.Name)
                {
                    client.Close();
                    Console.WriteLine("Не удалось войти в систему");
                    return null;
                }
            }
            person.ID = Clients.Count;
            var connectedClient = new ConnectedClient(client, this)
            {
                ID = person.ID,
                Person = person,
            };

            packet = new XPacket()
            {
                Type = XPacketTypes.Handshake,
                Action = XPacketActions.SetID,
                Content = person.ID
            };
            await connectedClient.SendPacket(packet);
            Console.WriteLine($"{connectedClient}\nУспешно вошел в систему");
            Clients.Add(connectedClient);
            // receive
            await BroadcastUsers();
            Task.Run(connectedClient.RunPackets);

            return connectedClient;
        }
        return null;
    }

    public bool IsEveryoneReady()
    {
        int countReady = 0;

        foreach (var client in Clients)
        {
            if (!client.Person.IsReady)
                return false;
            countReady++;
        }

        if (countReady < 4)
            return false;
        return true;
    }

    public async Task StartGame()
    {
        Console.WriteLine("Все готовы играть");
        var players = GetPlayers();
        GenerateLogicGame.SetRolePlayers(players);

        Programs = GenerateLogicGame.GetPrograms();

        var startGamePacket = new XPacket()
        {
            Action = XPacketActions.StartGame,
            Type = XPacketTypes.Unknown,
            Content = (players, 
                       GenerateLogicGame.GetGuns(players.Count), 
                       Programs)
        };
        await BroadcastPacket(startGamePacket);
    }

    public async Task AcceptClients()
    {
        while (true)
        {
            if (!Listening)
                return;
            
            var client = await AcceptClient();
            var connectedClient = await SignIn(client);
        }
    }

    public async Task ApplyProgram(AppleyProgramSend applyProgram)
    {
        switch (applyProgram.Program.ProgramAction)
        {
            case ApplyPrograms.RoleExchange:
                {
                    List<int> ids = (List<int>)applyProgram.Data;
                    var client1 = Clients.Where(cl => cl.ID == ids[0]).FirstOrDefault();
                    var client2 = Clients.Where(cl => cl.ID == ids[1]).FirstOrDefault();

                    var role = client1.Person.Role;
                    client1.Person.Role = client2.Person.Role;
                    client2.Person.Role = role;
                    await BroadcastUsers();
                    await BroadcastPacket(new XPacket()
                    {
                        Action = XPacketActions.SendMessage,
                        Type = XPacketTypes.Program,
                        Content = $"{client1.Person.Name} и {client2.Person.Name} поменялсь ролями"   
                    });
                    break;
                }

            case ApplyPrograms.ChangeDirectionEveryoneWeapons:
                {
                    List<Arsenal> guns = null;
                    var count = Clients.Count;

                    var gunsPointed = Clients.Select(cl => cl.Person.GunsIsPointedMe).ToList();

                    for (int i = 0; i < count; i++)
                    {
                        if (i == count - 1) break;
                        if (i == 0)
                            Clients[i].Person.GunsIsPointedMe =
                                gunsPointed[i + 1];

                        Clients[i + 1].Person.GunsIsPointedMe = gunsPointed[i];
                    }

                    await BroadcastUsers();
                    break;
                }
        }
    }
}
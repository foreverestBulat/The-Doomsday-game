using Protocol;
using Protocol.Models;
using Protocol.Packet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ServerUseSocket;

public class XServer
{
    public IPEndPoint IPEndPoint { get; private set; }
    public Socket Listener { get; private set; }
    public bool Listening {  get; private set; }
    public List<ConnectedClient> Clients { get; set; }

    public Queue<(XPacket, ConnectedClient)> ReceivedPackets = new Queue<(XPacket, ConnectedClient)>();
    //public Queue<SettingsPacket> ReceivedPackets2 = new Queue<SettingsPacket>();
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

    public async Task CompleteReceivedPackets()
    {
        while (true)
        {
            await Task.Delay(10);
            if (ReceivedPackets.Count == 0)
                continue;

            (XPacket packet, ConnectedClient client) = ReceivedPackets.Dequeue();
            //await Complete(packet, client);
            Console.WriteLine("ReceivedPackets.Dequeue()");
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
        var buffer = new byte[8192];
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

            //if (await AddUser(connectedClient))
            //    Clients.Add(connectedClient);

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
        //await BroadcastUsers();

        //var gunsPacket = new XPacket()
        //{
        //    Action = XPacketActions.SetGuns,
        //    Type = XPacketTypes.Guns,
        //    Content = GenerateLogicGame.GetGuns(players.Count)
        //};
        //await BroadcastPacket(gunsPacket);

        var startGamePacket = new XPacket()
        {
            Action = XPacketActions.StartGame,
            Type = XPacketTypes.Unknown,
            Content = (players, GenerateLogicGame.GetGuns(players.Count))
        };
        await BroadcastPacket(startGamePacket);
    }

    public async Task AcceptClients()
    {
        //Task.Run(CompleteReceivedPackets);

        while (true)
        {
            if (!Listening)
                return;
            
            var client = await AcceptClient();
            var connectedClient = await SignIn(client); //
            //var connectedClient = new ConnectedClient(client, this);
        }
    }

    public enum TypeSending
    {
        SendEveryone,
        SendOne
    }

    public class SettingsPacket
    {
        public TypeSending TypeSending { get; set; }
        public ConnectedClient Sender { get; set; }
        public XPacket Packet { get; set; }
    }

}



//public async Task Complete(XPacket packet, ConnectedClient connectedClient)
//{
//    switch (packet.Action)
//    {
//        case XPacketActions.SignIn:
//            {
//                var person = (Player)packet.Content;
//                for (int i = 0; i < Clients.Count; i++)
//                {
//                    if (person.Name == Clients[i].Person.Name)
//                    {
//                        connectedClient.Close();
//                        Console.WriteLine("Не удалось войти в систему");
//                        break;
//                    }
//                }

//                person.ID = Clients.Count;

//                packet = new XPacket()
//                {
//                    Action = XPacketActions.SetID,
//                    Type = XPacketTypes.Handshake,
//                    Content = person.ID
//                };
//                await connectedClient.SendPacket(packet);

//                connectedClient.Person = person;
//                connectedClient.ID = connectedClient.Person.ID;

//                // await BroadcastUsers();

//                var newPacket = new XPacket()
//                {
//                    Action = XPacketActions.AddUser,
//                    Type = XPacketTypes.Person,
//                    Content = connectedClient.Person
//                };

//                foreach (var client in Clients)
//                    await client.SendPacket(newPacket);

//                Clients.Add(connectedClient);

//                await connectedClient.SendPacket(new XPacket()
//                {
//                    Action = XPacketActions.AddUser,
//                    Type = XPacketTypes.Persons,
//                    Content = GetPlayers()
//                });

//                // add userov for everyone(connectedClient)

//                Console.WriteLine($"Новый клиент {connectedClient} успешно вошел в систему");

//                //await BroadcastUsers(); //
//                break;
//            }

//        case XPacketActions.Readiness:
//            {
//                var person = (Player)packet.Content;
//                var senderClient = Clients.Where(client => client.ID == person.ID).FirstOrDefault();
//                if (senderClient != null)
//                    senderClient.Person.IsReady = person.IsReady;

//                if (IsEveryoneReady())
//                    await StartGame();


//                var newPacket = new XPacket()
//                {
//                    Action = XPacketActions.Readiness,
//                    Type = XPacketTypes.Unknown,
//                    Content = person.ID
//                };
//                foreach (var client in Clients)
//                {
//                    //if (client.ID != connectedClient.ID)
//                    await client.SendPacket(newPacket);
//                }

//                //BroadcastUsers();
//                break;
//            }

//        case XPacketActions.SendMessage:
//            {
//                await BroadcastMessage(connectedClient, packet.Content.ToString());
//                break;
//            }

//        case XPacketActions.RemoveUser:
//            {
//                int id = connectedClient.ID;
//                Clients.RemoveAt(id);
//                var newPacket = new XPacket()
//                {
//                    Action = XPacketActions.RemoveUser,
//                    Type = XPacketTypes.Person,
//                    Content = id
//                };
//                foreach (var client in Clients)
//                    await client.SendPacket(newPacket);

//                connectedClient.Close();
//                break;
//            }
//    }
//}
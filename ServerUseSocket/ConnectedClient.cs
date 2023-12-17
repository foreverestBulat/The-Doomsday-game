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

    public ConnectedClient(Socket client, XServer server)
    {
        Client = client;
        Server = server;

        Task.Run(ReceivePackets);

        // run receive and send packets
    }

    public async Task<XPacket> ReceivePacket()
    {
        var buffer = new byte[1024];
        await Client.ReceiveAsync(buffer);
        return XPacketConverter.FromByteArray(buffer);
    }

    public async Task SendPacket(XPacket packet)
    {
        var buffer = XPacketConverter.ToByteArray(packet);
        await Client.SendAsync(buffer);
    }

    public async void ReceivePackets()
    {
        while (true)
        {
            try
            {
                var packet = await ReceivePacket();
                Console.WriteLine("Client прислал пакет");
                //Server.ReceivedPackets.Enqueue((packet, this));
                await Complete(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Client разорвал соединение");
                //Server.ReceivedPackets.Enqueue((new XPacket()
                //{
                //    Action = XPacketActions.RemoveUser,
                //    Type = XPacketTypes.Person,
                //    Content = this
                //}, this));
                Close();
                break;
            }
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

            //case XPacketActions.SignIn:
            //    {
            //        //Server.ReceivedPackets.Enqueue((packet, this));


            //        break;
            //    }
            
        }
    }

    public async Task SendPackets()
    {
        while (true)
        {
            
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



//Server.SentNeedPackets.Enqueue(new XServer.SettingsPacket
//{
//    TypeSending = XServer.TypeSending.SendEveryone,
//    Sender = this,
//    Packet = packet
//});
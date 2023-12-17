using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Protocol.Models;
using Protocol.Packet;
using Protocol;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;

namespace GameClient;

public class XClient
{
    public IPEndPoint ServerEndPoint { get; private set; }
    public Socket Socket { get; private set; }
    public Player Person { get; set; }

    public List<Player> Players = new List<Player>();
    //public List<Arsenal> Guns { get; set; }
    public ContentPage CurrentPage { get; set; }

    private readonly Queue<XPacket> PacketSendingQueue = new Queue<XPacket>();

    public Queue<XPacket> PacketReceivedQueue = new Queue<XPacket>();


    public XClient(IPEndPoint ipEndPoint)
    {
        ServerEndPoint = ipEndPoint;
        Socket = new Socket(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
    }

    public void Connect()
    {
        Socket.Connect(ServerEndPoint);

        //Task.Run(ReceivePackets);
        //Task.Run(CompletePackets);
        // run tasks
    }

    public async Task SignIn(Player person)
    {
        var packet = new XPacket()
        {
            Action = XPacketActions.SignIn,
            Type = XPacketTypes.SignIn,
            Content = person
        };
        await SendPacket(packet);

        packet = await ReceivePacket();
        if (packet.Action == XPacketActions.SetID)
            person.ID = (int)packet.Content;
        
        Person = person;

        Task.Run(ReceivePackets);
        Task.Run(CompleteReceivingPackets);
    }


    public async Task<XPacket> ReceivePacket()
    {
        var buffer = new byte[1024];
        int bytesRead = await Socket.ReceiveAsync(buffer);
        var packetData = new List<byte>(buffer.Take(bytesRead));

        while (bytesRead == buffer.Length)
        {
            buffer = new byte[1024];
            bytesRead = await Socket.ReceiveAsync(buffer);
            packetData.AddRange(buffer.Take(bytesRead));
        }

        return XPacketConverter.FromByteArray(packetData.ToArray());
    }

    public async Task SendPacket(XPacket packet)
    {
        var buffer = XPacketConverter.ToByteArray(packet);
        await Socket.SendAsync(buffer);
    }

    public async void ReceivePackets()
    {
        while (true)
        {
            await Task.Delay(100);
            var packet = await ReceivePacket();
            PacketReceivedQueue.Enqueue(packet);
        }
    }

    public async void CompleteReceivingPackets()
    {
        while (true)
        {
            await Task.Delay(100);
            if (PacketReceivedQueue.Count == 0)
                continue;

            if (CurrentPage.GetType().GetInterfaces().Contains(typeof(PageClient)))
            {
                var methodInfo = CurrentPage.GetType().GetMethod("ChangeData");
                methodInfo.Invoke(CurrentPage, new[] { PacketReceivedQueue.Dequeue() });
            }
        }
    }

    public void Close()
    {
        Socket.Close();
    }
}


//public async Task Complete(XPacket packet)
//{
//    switch (packet.Action)
//    {
//        case XPacketActions.SetID:
//            {
//                Person.ID = (int)packet.Content;
//                break;
//            }

//        case XPacketActions.AddUser:
//            {
//                if (CurrentPage is PageClient)
//                {
//                    //var persons = (List<Player>)packet.Content;
//                    //MainThread.BeginInvokeOnMainThread(() =>
//                    //{
//                    //    ((PageClient)CurrentPage).Users.Clear();
//                    //    foreach (var player in persons)
//                    //    {
//                    //        Players.Add(player);

//                    //        ((PageClient)CurrentPage).Users.Add
//                    //            (UserInPage.ConverterUserForListView(player));
//                    //    }
//                    //});
//                    if (XPacketTypes.Person == packet.Type)
//                    {
//                        MainThread.BeginInvokeOnMainThread(() =>
//                        {
//                            Players.Add((Player)packet.Content);
//                            ((PageClient)CurrentPage).Users.Add
//                                (UserInPage.ConverterUserForListView((Player)packet.Content));
//                        });
//                    }
//                    else if (XPacketTypes.Persons == packet.Type)
//                    {
//                        //((PageClient)CurrentPage).Users = new ObservableCollection<UserInPage>();

//                        var persons = (List<Player>)packet.Content;
//                        MainThread.BeginInvokeOnMainThread(() =>
//                        {
//                            foreach (var player in persons)
//                            {
//                                Players.Add(player);
//                                ((PageClient)CurrentPage).Users.Add
//                                    (UserInPage.ConverterUserForListView(player));
//                            }
//                        });
//                    }
//                }
//                break;
//            }

//        case XPacketActions.RemoveUser:
//            {
//                if (CurrentPage is PageClient)
//                {
//                    int id = (int)packet.Content;
//                    MainThread.BeginInvokeOnMainThread(() =>
//                    {
//                        ((PageClient)CurrentPage).Users.RemoveAt(id);
//                    });
//                }
//                break;
//            }

//        case XPacketActions.SendMessage:
//            {
//                if (CurrentPage is PageClient)
//                {
//                    MainThread.BeginInvokeOnMainThread(() =>
//                    {
//                        ((PageClient)CurrentPage).Messages.Add
//                            (packet.Content.ToString());
//                    });
//                }
//                break;
//            }

//        case XPacketActions.Readiness:
//            {

//                break;
//            }

//            //case XPacketActions.StartGame:
//            //    {
//            //        if (currentPage is GamePage)
//            //        {
//            //            var page = new ProcessPage(((PageClient)currentPage).Users);
//            //            await currentPage.Navigation.PushAsync(page);
//            //        }

//            //        // MainThread ?
//            //        break;
//            //    }

//    }
//}
using ClientUseSocket;
using Protocol;
using Protocol.Models;
using Protocol.Packet;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;




//var packet = new XPacket()
//{
//    Action = XPacketActions.AddUser,
//    Type = XPacketTypes.Persons,
//    Content = new List<Player>()
//    {
//        new Player()
//        {
//            Name = "Bulat",
//        },
//        new Player()
//        {
//            Name = "Islam",
//        },
//        new Player()
//        {
//            Name = "Islam",
//        },
//        new Player()
//        {
//            Name = "Islam",
//        }
//    }
//};

//var bytes = XPacketConverter.ToByteArray(packet);
//Console.WriteLine(bytes.Length);

//Console.WriteLine(String.Join(' ', bytes));

//var newPacket = XPacketConverter.FromByteArray(bytes);

//Console.WriteLine(String.Join(' ', (List<Player>)newPacket.Content));




//var ipAddress = new IPAddress(new byte[] {127, 0, 0, 1 });
//var ipEndPoint = new IPEndPoint(ipAddress, 8888);


//var client = new XClient(ipEndPoint);
//client.Connect();

//var rand = new Random();

//var person = new Player()
//{
//    Name = Console.ReadLine(),
//    Color = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256))
//};

//await client.SignIn(person);

//await Task.Run(CompleteActions);

//async Task CompleteActions()
//{
//    while (true)
//    {
//        var packet = await client.ReceivePacket();
//        Console.WriteLine(String.Join(' ', (List<Player>)packet.Content));
//    }
//}


//
//await client.SendPacket(packet);

//await client.SendPacket();
//var packet = await client.ReceivePacket();

//foreach (var item in (Person[])packet.Content)
//{
//    Console.WriteLine(item);
//}

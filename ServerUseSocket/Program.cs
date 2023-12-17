using Protocol.Models;
using Protocol.Packet;
using ServerUseSocket;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;




var ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
var ipEndPoint = new IPEndPoint(ipAddress, 8888);

var server = new XServer(ipEndPoint);

server.Start();
await server.AcceptClients();




//byte[] bytes = null;

////var list = new List<string>() { "bulat", "islam" };
//var pers = new MyPerson()
//{
//    Name = "bulat"
//};

//var packet = new XPacket()
//{
//    Type = XPacketTypes.SignIn,
//    Content = pers
//};

//using (var ms = new MemoryStream())
//{
//    DataContractSerializer serializer = new DataContractSerializer(typeof(XPacket));
//    serializer.WriteObject(ms, packet);
//    bytes = ms.ToArray();
//}


//object packet2 = null;
//using (var ms = new MemoryStream(bytes))
//{
//    DataContractSerializer serializer = new DataContractSerializer(typeof(XPacket));
//    packet2 = (XPacket)serializer.ReadObject(ms);
//}

//Console.WriteLine(packet2 is XPacket);


//[DataContract(Name = "MyPerson")]
//public class MyPerson
//{
//    [DataMember]
//    public string Name { get; set; }
//}











//var client = await server.AcceptClient();
//await server.SignIn(client);

//var client2 = await server.AcceptClient();
//await server.SignIn(client2);

// server

//var packet = new XPacket()
//{
//    Type = XPacketTypes.Person,
//    Content = new Person[]
//    {
//        new Person()
//        {
//            Name = "bulat",
//            Color = Color.Orange
//        },
//        new Person()
//        {
//            Name = "islam",
//            Color = Color.Red
//        }
//    }
//};

//await server.SendPacket(client, packet);

//await server.ReceivePacket(client);

//var bulat = new Person("bulat", 1);

//var list = "safdgd";

////MemoryStream stream = new MemoryStream();
//var formatter = new BinaryFormatter();
//byte[] bytes = null;
//Console.WriteLine(bytes);
//using (var ms = new MemoryStream())
//{
//#pragma warning disable SYSLIB0011 // Тип или член устарел
//    formatter.Serialize(ms, list);
//#pragma warning restore SYSLIB0011 // Тип или член устарел
//    ms.Flush();
//    bytes = ms.ToArray();
//}

//Console.WriteLine(String.Join(' ', bytes));

//object pers = null;

//using (var ms = new MemoryStream(bytes))
//{
//    formatter = new BinaryFormatter();
//#pragma warning disable SYSLIB0011 // Тип или член устарел
//    pers = formatter.Deserialize(ms);
//#pragma warning restore SYSLIB0011 // Тип или член устарел
//}
//Console.WriteLine(pers);

//if (pers is List<string>)
//{
//    Console.WriteLine(String.Join(' ', (List<string>)pers));
//}

//var bytes = stream.ToArray();
//var a = formatter.Serialize(bulat)

//byte[] bytes;
//using (MemoryStream stream = new MemoryStream())
//{
//    BinaryFormatter formatter = new BinaryFormatter();
//    formatter.Serialize(stream, bulat);
//    bytes = stream.ToArray();
//}


//[Serializable]
//class Person
//{
//    public string Name { get; set; }
//    public int ID { get; set; }

//    public Person(string name, int id)
//    {
//        Name = name;
//        ID = id;
//    }

//    public override string ToString()
//    {
//        return $"ID: {ID}\nName: {Name}";
//    }
//}
using Newtonsoft.Json;
using Protocol.Models;
using Protocol.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Protocol;

public class XPacketConverter
{
    static List<Type> KnownTypes = new List<Type> { typeof(Player), typeof(List<Player>) };

    public static byte[] ToByteArray(XPacket packet)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Тип или член устарел
            formatter.Serialize(stream, packet);
#pragma warning restore SYSLIB0011 // Тип или член устарел
            return stream.ToArray();
        }
    }

    public static XPacket FromByteArray(byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Тип или член устарел
            return (XPacket)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Тип или член устарел
        }

    }
}

        //var serializer = new DataContractJsonSerializer(typeof(XPacket), KnownTypes);
        //using (MemoryStream ms = new MemoryStream(bytes))
        //{
        //    return (XPacket)serializer.ReadObject(ms);
        //}



        //    var serializer = new DataContractJsonSerializer(typeof(XPacket), KnownTypes);
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        serializer.WriteObject(stream, packet);
        //        return stream.ToArray();
        //    }
        //}

        //DataContractSerializer serializer = new DataContractSerializer(typeof(XPacket), KnownTypes);
        //using (MemoryStream stream = new MemoryStream())
        //{
        //    serializer.WriteObject(stream, packet);
        //    return stream.ToArray();
        //}

        //DataContractSerializer serializer = new DataContractSerializer(typeof(XPacket), KnownTypes);
        //using (MemoryStream stream = new MemoryStream(bytes))
        //{
        //    return (XPacket)serializer.ReadObject(stream);
        //}

//        using (MemoryStream stream = new MemoryStream(bytes))
//        {
//            BinaryFormatter formatter = new BinaryFormatter();
//#pragma warning disable SYSLIB0011 // Тип или член устарел
//            return (XPacket)formatter.Deserialize(stream);
//#pragma warning restore SYSLIB0011 // Тип или член устарел
//        }



        //using (MemoryStream stream = new MemoryStream())
        //{

        //    string json = JsonConvert.SerializeObject(packet, stream);

        //    //string json = JsonConvert.SerializeObject(yourObject);
        //    //byte[] bytes = Encoding.UTF8.GetBytes(json);

        //    //            BinaryFormatter formatter = new BinaryFormatter();
        //    //#pragma warning disable SYSLIB0011 // Тип или член устарел
        //    //            formatter.Serialize(stream, packet);
        //    //#pragma warning restore SYSLIB0011 // Тип или член устарел
        //    //            return stream.ToArray();
        //}
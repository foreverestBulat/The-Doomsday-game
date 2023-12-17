using Protocol.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packet;

[Serializable]
[KnownType(typeof(Player))]
[KnownType(typeof(List<Player>))]
[DataContract]
public class XPacket
{
    [DataMember]
    public XPacketActions Action { get; set; }
    [DataMember]
    public XPacketTypes Type { get; set; }
    [DataMember]
    public object Content { get; set; }
}
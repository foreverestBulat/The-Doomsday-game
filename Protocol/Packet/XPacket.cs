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
public class XPacket
{
    public XPacketActions Action { get; set; }
    public XPacketTypes Type { get; set; }
    public object Content { get; set; }
}
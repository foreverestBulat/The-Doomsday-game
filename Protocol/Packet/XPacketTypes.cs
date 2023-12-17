using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packet;

[Serializable]
[DataContract]
public enum XPacketTypes
{
    [EnumMember] Handshake,
    [EnumMember] SignIn,
    [EnumMember] Person,
    [EnumMember] Persons,
    [EnumMember] Message,

    [EnumMember] Guns,
    [EnumMember] Handgun,
    [EnumMember] Rifle,
    [EnumMember] Laser,
    [EnumMember] FlareGun,
    [EnumMember] Assistant,

    [EnumMember] Unknown
}
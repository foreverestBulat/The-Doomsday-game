using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packet;

[Serializable]
[DataContract]
public enum XPacketActions
{
    [EnumMember] AddUser,
    [EnumMember] SignIn,
    [EnumMember] RemoveUser,
    [EnumMember] SetID,
    [EnumMember] Readiness,
    [EnumMember] StartGame,
    [EnumMember] SendMessage,
    [EnumMember] SetGuns,
    [EnumMember] Shot,
    [EnumMember] Unknown
}
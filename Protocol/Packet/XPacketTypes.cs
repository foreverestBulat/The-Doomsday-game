using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packet;

[Serializable]
public enum XPacketTypes
{
    Handshake,
    SignIn,
    Person,
    Persons,
    Message,
    Role,
    Card,
    Guns,
    Gun,
    Handgun,
    Rifle,
    Laser,
    FlareGun,
    Assistant,
    Program,
    Nothing,
    DropGun,
    HealthPoints,
    Unknown
}
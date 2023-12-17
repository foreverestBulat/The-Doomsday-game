using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packet;

[Serializable]
public enum XPacketActions
{
    AddUser,
    SignIn,
    RemoveUser,
    SetID,
    Readiness,
    StartGame,
    SendMessage,
    SetGuns,
    ArmYourself,
    AimWeapon,
    Shot,
    FinishStep,
    OpenCard,
    OpenFirstCard,
    OpenSecondCard,
    OpenRole,
    LostHealtPoints,
    TakeProgram,
    DropGun,
    Nothing,
    Unknown
}
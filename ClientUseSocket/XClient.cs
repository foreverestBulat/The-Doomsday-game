using Protocol;
using Protocol.Models;
using Protocol.Packet;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientUseSocket;

public class XClient
{
    public IPEndPoint ServerEndPoint { get; private set; }
    public Socket Socket {  get; private set; }

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

        // run tasks
    }

    public async Task SignIn(Player person)
    {
        var packet = new XPacket()
        {
            Type = XPacketTypes.SignIn,
            Content = person
        };
        await SendPacket(packet);
        packet = await ReceivePacket();
    }


    public async Task<XPacket> ReceivePacket()
    {
        byte[] buffer = new byte[1024];
        var received = await Socket.ReceiveAsync(buffer);
        return XPacketConverter.FromByteArray(buffer);
    }

    public async Task SendPacket(XPacket packet)
    {
        var buffer = XPacketConverter.ToByteArray(packet);
        await Socket.SendAsync(buffer);
    }
}
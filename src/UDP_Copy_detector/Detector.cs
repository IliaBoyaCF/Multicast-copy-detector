using System.Net;
using System.Net.Sockets;
using static Program.Radar;

namespace Program;

public class Detector : IDetector
{
    private const int _defaultPingDelay = 1_000;

    public static readonly int s_defaultPingDelay = _defaultPingDelay;

    public event OnListUpdateHandler ListUpdatedEvent;

    private readonly IRunnable _beacon;
    private readonly IRunnable _radar;
    private readonly Socket _multicastSocket;
    private readonly Socket _sendingSocket;

    public Detector(CommandLineArguments arguments)
    {
        _multicastSocket = NewMulticastSocket(arguments.Address, arguments.Port);
        _sendingSocket = NewSendingSocket(arguments.Address.AddressFamily);
        _beacon = new Beacon(new IPEndPoint(arguments.Address, arguments.Port), _sendingSocket, s_defaultPingDelay);
        _radar = new Radar(_multicastSocket);
        ((Radar)_radar).ListUpdatedEvent += (o, a) => ListUpdatedEvent?.Invoke(this, a);
    }

    private Socket NewMulticastSocket(IPAddress address, int port)
    {
        Socket mcastSocket = new(address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        mcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        IPAddress localIP = GetLocalIP(address.AddressFamily);
        IPEndPoint endPoint = new(localIP, port);
        mcastSocket.Bind(endPoint);
        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            MulticastOption multicastOption = new(address);
            mcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
            return mcastSocket;
        }
        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            IPv6MulticastOption multicastOption = new(address);
            mcastSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, multicastOption);
            return mcastSocket;
        }
        throw new ArgumentException("Unknown address family for provided arguments.");
    }

    private Socket NewSendingSocket(AddressFamily addressFamily)
    {        
        Socket sendingSocket = new(addressFamily, SocketType.Dgram, ProtocolType.Udp);
        return sendingSocket;
    }

    private IPAddress GetLocalIP(AddressFamily addressFamily)
    {
        IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress curAdd in heserver.AddressList)
        {
            if (curAdd.AddressFamily == addressFamily)
            {
                return curAdd;
            }
        }
        throw new Exception($"No address supporting {addressFamily} was found.");
    }

    public void Run()
    {
        _radar.Run();
        _beacon.Run();
        Console.WriteLine("Detector started.");
    }

    public bool IsRunning()
    {
        return _radar.IsRunning() && _beacon.IsRunning();
    }

    public void Stop()
    {
        _radar.Stop();
        _beacon.Stop();
        _sendingSocket.Close();
        _multicastSocket.Close();
        Console.WriteLine("Detector stopped.");
    }
}
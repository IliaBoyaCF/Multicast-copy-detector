using System.Net;
using System.Net.Sockets;

namespace Program;

internal class PingListener
{

    public delegate void PingHandler(object sender, EventArgs e);
    public event PingHandler PingEvent;

    private readonly IPEndPoint _senderEndPoint;

    private Socket _socket;
    private byte[] _buffer = new byte[1024];

    public class PingArgs : EventArgs
    {
        public PingArgs(EndPoint Source) 
        { 
            this.Source = Source;
        }
        public EndPoint Source { get; } 
    }

    public PingListener(Socket socket)
    {
        _socket = socket;
        switch (_socket.AddressFamily)
        {
            case AddressFamily.InterNetwork:
                _senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                break;
            case AddressFamily.InterNetworkV6:
                _senderEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0); 
                break;
            default:
                throw new ArgumentException("Socket address family is not supported.");
        }
    }

    public void ListenFor(int timeoutMicrosec)
    {
        EndPoint senderRemote = _senderEndPoint;
        bool hasDatagramToReceive = _socket.Poll(timeoutMicrosec, SelectMode.SelectRead);
        if (!hasDatagramToReceive)
        {
            return;
        }
        _socket.ReceiveFrom(_buffer, ref senderRemote);
        PingEvent.Invoke(this, new PingArgs(senderRemote));
    }
}

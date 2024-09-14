using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Program;

internal class Beacon : IRunnable
{
    private static readonly byte[] _emptyBuffer = [];
    
    private readonly Socket _socket;
    private readonly int _delay;
    private readonly IPEndPoint _mcastGroupAddress;
    
    private System.Timers.Timer _timer;

    public Beacon(IPEndPoint McastGroupAddress, Socket Socket, int Delay)
    {
        _mcastGroupAddress = McastGroupAddress;
        _socket = Socket;
        _delay = Delay;
    }

    private void Ping(Object source, ElapsedEventArgs e)
    {
        _socket.SendTo(_emptyBuffer, _mcastGroupAddress);
    }

    public void Run()
    {
        _timer = new System.Timers.Timer(_delay);
        _timer.Elapsed += Ping;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        _timer.Start();
    }
    public bool IsRunning()
    {
        return _timer.Enabled;
    }
    public void Stop()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
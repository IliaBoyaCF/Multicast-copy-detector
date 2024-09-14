using System.Net.Sockets;

namespace Program;

public class Radar : IRunnable
{
    public static readonly int s_defaultUpdateDelay = 500; // milliseconds
    public static readonly int s_defaultExpirationDelay = 2_000; // milliseconds
    public static readonly int s_defaultListeningTimeout = 500_000; // microseconds

    public delegate void OnListUpdateHandler(object source, EventArgs eventArgs);
    public event OnListUpdateHandler ListUpdatedEvent;

    private readonly LiveCopyList _liveCopyList;

    private PingListener _pingListener;

    private Thread? _thread;
    public Radar(Socket Socket)
    {
        _pingListener = new PingListener(Socket);
        _liveCopyList = new LiveCopyList(s_defaultUpdateDelay, s_defaultExpirationDelay);
        _pingListener.PingEvent += _liveCopyList.OnPingReceived;
        _liveCopyList.ListUpdatedEvent += (o, a) => ListUpdatedEvent?.Invoke(this, a);
    }
    public void Run()
    {
        _thread = new Thread(Scan);
        _thread.Start();
    }
    public bool IsRunning()
    {
        if (_thread == null)
        {
            return false;
        }
        return _thread.IsAlive;
    }
    public void Stop()
    {
        _thread?.Interrupt();
    }

    private void Scan()
    {
        while (true)
        {
            _pingListener.ListenFor(s_defaultListeningTimeout);
            _liveCopyList.Update();
        }
    }
}

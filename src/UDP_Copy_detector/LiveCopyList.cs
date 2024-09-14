using System.Net;
using static Program.PingListener;
using static Program.Radar;


namespace Program;

public class LiveCopyList
{
    public event OnListUpdateHandler ListUpdatedEvent;

    private readonly int _updateDelay;
    private readonly Dictionary<EndPoint, long> _liveCopies = [];
    private readonly int _expirationTime;

    private long _lastTimeUpdate;

    public class LiveCopyListArgs : EventArgs
    {
        public LiveCopyListArgs(List<EndPoint> addresses)
        {
            this.Addresses = addresses;
        }
        public List<EndPoint> Addresses{ get; }
    }

    public LiveCopyList(int updateDelay, int expirationTime)
    {
        _updateDelay = updateDelay;
        _lastTimeUpdate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _expirationTime = expirationTime;
    }
    public void OnPingReceived(object sender, EventArgs e)
    {
        PingArgs pingArgs = (PingArgs)e;

        long currTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        bool alreadyInList = !_liveCopies.TryAdd(pingArgs.Source, currTime);

        if (alreadyInList)
        {
            _liveCopies.Remove(pingArgs.Source);
            _liveCopies.Add(pingArgs.Source, currTime);
            return;
        }

        ListUpdatedEvent?.Invoke(this, new LiveCopyListArgs([.. _liveCopies.Keys]));
    }
    public void Update()
    {
        long currTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currTime - _lastTimeUpdate <= _updateDelay)
        {
            return;
        }

        IList<EndPoint> keysToRemove = new List<EndPoint>();
        IEnumerator<KeyValuePair<EndPoint, long>> enumerator = _liveCopies.GetEnumerator();
        while (enumerator.MoveNext()) 
        {
            KeyValuePair<EndPoint, long> keyValuePair = enumerator.Current;
            if (_lastTimeUpdate - keyValuePair.Value > _expirationTime)
            {
                keysToRemove.Add(keyValuePair.Key);
            }
        }

        foreach (EndPoint keyToRemove in keysToRemove)
        {
            _liveCopies.Remove(keyToRemove);
        }

        _lastTimeUpdate = currTime;

        if (keysToRemove.Count == 0)
        {
            return;
        }

        ListUpdatedEvent.Invoke(this, new LiveCopyListArgs([.. _liveCopies.Keys]));
    }
}
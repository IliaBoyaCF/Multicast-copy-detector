namespace Program;

public interface IDetector : IRunnable
{
    public event Radar.OnListUpdateHandler ListUpdatedEvent;
}
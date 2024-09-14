namespace Program;

public interface IRunnable
{
    public void Run();
    public bool IsRunning();
    public void Stop();
}
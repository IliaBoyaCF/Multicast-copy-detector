namespace Program;

internal class EntryPoint
{
    public static void Main(string[] args)
    {
        Program.Start(args, () => { while (true) { } });
    }
}

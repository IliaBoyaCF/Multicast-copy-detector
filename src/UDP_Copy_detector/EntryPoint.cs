public class EntryPoint
{
    public static void Main(string[] args)
    {
        Program.Program.Start(args, () => { while (true) { } });
    }
}

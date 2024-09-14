using System.Net;
using System.Text;

namespace Program;
public class Program
{
    public static void Start(string[] args, ThreadStart payload)
    {
        IParser parser = new Parser();

        CommandLineArguments arguments;
        try
        {
            arguments = parser.ParseArguments(args);
        }
        catch (Parser.ParserException e)
        {
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }
        if (arguments.AskedForHelpInfo)
        {
            PrintHelpInfo();
            return;
        }
        IDetector detector = new Detector(arguments);
        detector.ListUpdatedEvent += OnListChangedHandler;
        detector.Run();

        payload.Invoke();

        detector.Stop();
    }

    private static void PrintHelpInfo()
    {
        const string ProgName = "copy-detector";
        Console.WriteLine("Usage:\n{0} 'Multicast-group_address' 'port'", ProgName);
    }
    private static void OnListChangedHandler(object source, EventArgs args)
    {
        Console.WriteLine("---Live copies:\n{0}", EndPointsToString(((LiveCopyList.LiveCopyListArgs)args).Addresses));
    }
    private static string EndPointsToString(List<EndPoint> endPoints)
    {
        StringBuilder sb = new StringBuilder();
        foreach (EndPoint endPoint in endPoints)
        {
            sb.Append(endPoint.ToString()).Append("\n");
        }
        return sb.ToString();
    }
}
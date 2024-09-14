using System.Net;

namespace Program;

public record CommandLineArguments(IPAddress Address, int Port, bool AskedForHelpInfo)
{
    public static readonly IPAddress DefaultIP = IPAddress.Any;
    public static readonly int DefaultPort = 2000;

    public static readonly CommandLineArguments AskedForHelp = new(DefaultIP, DefaultPort, true);
    public static readonly CommandLineArguments NoInput = AskedForHelp;
    public static CommandLineArguments NewCommandLineArguments(IPAddress Address, int Port)
    {
        return new CommandLineArguments(Address, Port, false);
    }
}
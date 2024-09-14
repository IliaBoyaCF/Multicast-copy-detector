using System.Net;

namespace Program;

public class Parser : IParser
{
    public class ParserException : Exception
    {
        public ParserException() : base() { }

        public ParserException(string message) : base(message) { }
        
        public ParserException(string message, Exception innerException) : base(message, innerException) { }
    }

    private CommandLineArguments? _arguments;
    
    public CommandLineArguments GetLastParsedArguments()
    {
        if (!HasParsedArguments())
        {
            throw new InvalidOperationException("Nothing has been parsed yet.");
        }
        return _arguments;
    }

    public bool HasParsedArguments()
    {
        return _arguments != null;
    }

    public CommandLineArguments ParseArguments(string[] args)
    {
        if (args == null)
        {
            throw new NullReferenceException("Args must be not null.");
        }
        if (args.Length == 0)
        {
            return CommandLineArguments.NoInput;
        }
        if (args.Length == 1)
        {
            if (args[0] == "--help" || args[0] == "-h")
            {
                return CommandLineArguments.AskedForHelp;
            }
        }
        if (args.Length != 2)
        {
            throw new ArgumentException("Illegal number of arguments. Try '--help'");
        }
        try 
        {
            return CommandLineArguments.NewCommandLineArguments(IPAddress.Parse(args[0]), Convert.ToInt32(args[1])); 
        }
        catch (Exception e) when (e is OverflowException || e is FormatException) 
        {
            throw new ParserException(e.Message, e);
        }
    }
}
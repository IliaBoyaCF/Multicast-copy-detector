namespace Program;

public interface IParser
{
    CommandLineArguments ParseArguments(string[] args);
    CommandLineArguments GetLastParsedArguments();
    bool HasParsedArguments();
}
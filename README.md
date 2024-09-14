# Multicast-copy-detector

General
----

Application implements detection of it's copies within local network and prints actual list of working copies into the command line. Implementation is based on IP multicast technology.

The src provides:

- Demo program that you can provide with your own payload(default payload is spin in the while loop).
- Detector class that implements copy detection mechanism.

Demo program
-----------
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            // Program.Start(string[] args, ThreadTask payload);
            Program.Program.Start(args, () => { while (true) { } });
        }
    }


Detector class
---------
We provide _IDetector_ interface and it's implementation: _Detector_. Iterface has _ListUpdatedEvent_ which raises on changing current situation with live copies(on new copy appearance or old copy death).  
Also solution provides _IParser_ with _Parser_ implementation that consumes command line arguments and parses it. The result of parsing is suitable for creating an instance of _Detector_ class.
Usage:

    // ...
    IParser parser = new Parser();
    CommandLineArguments commandLineArgs = parser.ParseArguments(args); // string[] args
    
    IDtedector detector = new Detector(commandLineArgs);
    detector.ListUpdatedEvent += OnListChangedHandler;

    detector.Run();
    // some work
    detector.Stop();
    //...


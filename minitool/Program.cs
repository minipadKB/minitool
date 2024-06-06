using CommandLine;
using CommandLine.Text;
using minitool.Commands;
using minitool.Models;
using minitool.Utils;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace minitool
{
  public class Program
  {
    /// <summary>
    /// The vendor ID for the minipad device, used to identify the devices.
    /// </summary>
    public const int MINIPAD_VID = 0x0727;

    /// <summary>
    /// The product ID for the minipad device, used to identify the devices.
    /// </summary>
    public const int MINIPAD_PID = 0x0727;

    public static void Main(string[] args)
    {
      // If the application was ran by executing the exe file from the desktop environment, rather than the commandline,
      // open a new commandline window and run the application from there. This helps users that are not that familiar with CLI.
      // This check can be done by looking whether no args are specified and the cursor is all the way at the top.
      if(args.Length == 0 && Console.CursorTop == 0)
      {
        // Run a CMD session and initially print the help text to guide the user on how to start.
        Process.Start(new ProcessStartInfo("cmd.exe", "/k minitool help")
        {
          UseShellExecute = false
        });

        return;
      }

      // Set the culture for the whole application to invariant to allow consistent parsing.
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      // Create the commandline parser and parse the input.
      new Parser(x =>
      {
        x.EnableDashDash = true;
        x.HelpWriter = Console.Out;
      }).ParseArguments<Devices.Options, Info.Options, Boot.Options, Send.Options, Calibrate.Options, Visualize.Options, Flash.Options, Console_.Options>(args).MapResult(
        (Devices.Options o) => Devices.Handle(o),
        (Info.Options o) => Info.Handle(o),
        (Boot.Options o) => Boot.Handle(o),
        (Send.Options o) => Send.Handle(o),
        (Calibrate.Options o) => Calibrate.Handle(o),
        (Visualize.Options o) => Visualize.Handle(o),
        (Flash.Options o) => Flash.Handle(o),
        (Console_.Options o) => Console_.Handle(o),
        Error);
    }

    /// <summary>
    /// Error handler. Unused for now.
    /// </summary>
    /// <returns></returns>
    private static int Error(IEnumerable<Error> options)
    {
      return 1;
    }
  }
}
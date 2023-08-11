using CommandLine;
using CommandLine.Text;
using minitool.Commands;
using minitool.Models;
using minitool.Utils;
using System;
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
      // Set the culture for the whole application to invariant to allow consistent parsing.
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      // Create the commandline parser and parse the input.
      new Parser(x =>
      {
        x.EnableDashDash = true;
        x.HelpWriter = Console.Out;
      }).ParseArguments<Devices.Options, Info.Options, Boot.Options, Send.Options, Calibrate.Options, Visualize.Options>(args).MapResult(
        (Devices.Options o) => Devices.Handle(o),
        (Info.Options o) => Info.Handle(o),
        (Boot.Options o) => Boot.Handle(o),
        (Send.Options o) => Send.Handle(o),
        (Calibrate.Options o) => Calibrate.Handle(o),
        (Visualize.Options o) => Visualize.Handle(o),
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
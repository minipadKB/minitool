using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace minitool.Commands;

public static class Boot
{
  [Verb("boot", HelpText = "Sets the specified minipad into bootloader/flashing mode.")]
  public class Options
  {
    [Value(0, Required = true, HelpText = "The port number of the minipad.")]
    public int Port { get; private set; }
  }

  public static int Handle(Options options)
  {
    // Set the device into bootloader mode by setting the serial baud rate to 1200.
    Process.Start(new ProcessStartInfo()
    {
      FileName = "mode.com",
      Arguments = $"COM{options.Port}:Baud=1200",
      CreateNoWindow = true,
      UseShellExecute = false
    })?.WaitForExit();

    return 0;
  }
}

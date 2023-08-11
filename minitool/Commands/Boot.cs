using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    // Send the boot command by simulating a send command input. ("minitool send ... boot")
    return Send.Handle(new Send.Options(options.Port, "boot"));
  }
}

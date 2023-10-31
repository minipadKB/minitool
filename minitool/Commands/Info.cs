using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minitool.Models;
using minitool.Utils;
using minitool.Enums;
using System.Diagnostics;

namespace minitool.Commands;

public static class Info
{
  [Verb("info", HelpText = "Outputs the info and settings of the specified minipad.")]
  public class Options
  {
    [Value(0, Required = true, HelpText = "The port number of the minipad.")]
    public int Port { get; private set; }
  }

  public static int Handle(Options options)
  {
    MinipadDevice device;
    try
    {
      // Get the minipad device from the port.
      device = MinipadHelper.GetDeviceAsync(options.Port).Result;
      Console.Write("State: ");
    }
    catch (Exception ex)
    {
      // If any error occured, output it to the user.
      Console.Write("State: ");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Error");
      Console.WriteLine($"{ex.InnerException!.Message}");
      Console.WriteLine(ex.InnerException!.StackTrace);
      Console.ForegroundColor = ConsoleColor.Gray;
      return 3;
    }

    // Just output the state of the device if it's not connected.
    if (device.State == DeviceState.DISCONNECTED)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Disconnected");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 2;
    }
    else if (device.State == DeviceState.BUSY)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("Busy");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 1;
    }

    // Output all general info known about the minipad.
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Connected");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write($"Name: ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(device);
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine($"Version: {device.FirmwareVersion.IfNull("unknown")}");
    Console.WriteLine();
    Console.WriteLine($"Hysteresis Tolerance: {device.Configuration.HysteresisTolerance.IfNull("unknown")}");
    Console.WriteLine($"Rapid Trigger Tolerance: {device.Configuration.RapidTriggerTolerance.IfNull("unknown")}");
    Console.WriteLine($"Analog Resolution: {device.Configuration.AnalogResolution.IfNull("unknown", "{}-bit")}");
    Console.WriteLine($"Travel Distance: {(device.Configuration.TravelDistance == null ? "unknown" : Math.Round(device.Configuration.TravelDistance.Value / 100d, 2)):F}mm");
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"Hall Effect Keys ({device.Configuration.HallEffectKeys.Length}):");
    Console.ForegroundColor = ConsoleColor.Gray;

    // If there are any hall effect keys, output their config.
    if (device.Configuration.HallEffectKeys.Length > 0)
      // Go through each key and output their config.
      foreach (HallEffectKey key in device.Configuration.HallEffectKeys)
        Console.WriteLine(key.ToString());
    else
      Console.WriteLine("This minipad has no Hall Effect keys.");

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"Digital Keys ({device.Configuration.DigitalKeys.Length}):");
    Console.ForegroundColor = ConsoleColor.Gray;

    // If there are any digital keys, output their config.
    if (device.Configuration.DigitalKeys.Length > 0)
      // Go through each key and output their config.
      foreach (DigitalKey key in device.Configuration.DigitalKeys)
        Console.WriteLine(key.ToString());
    else
      Console.WriteLine("This minipad has no digital keys.");

    return 0;
  }
}

using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minitool.Enums;
using minitool.Models;
using System.IO.Ports;

namespace minitool.Commands;

public static class Console_
{
  [Verb("console", HelpText = "Opens a raw command console for the specified minipad.")]
  public class Options
  {
    [Value(0, Required = true, HelpText = "The port number of the minipad.")]
    public int Port { get; private set; }
    
    public Options() { }
  }

  public static int Handle(Options options)
  {
    MinipadDevice device;
    try
    {
      // Get the minipad device from the port.
      device = MinipadHelper.GetDeviceAsync(options.Port).Result;
    }
    catch (Exception ex)
    {
      // If any error occured, output it to the user.
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"Error: {ex.InnerException!.Message}");
      Console.WriteLine(ex.InnerException!.StackTrace);
      Console.ForegroundColor = ConsoleColor.Gray;
      return 3;
    }

    // Just output the state of the device if it's not connected.
    if (device.State == DeviceState.DISCONNECTED)
    {
      Console.Write($"The minipad on COM{options.Port} is currently ");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("disconneted");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 2;
    }
    else if (device.State == DeviceState.BUSY)
    {
      Console.Write($"The minipad on COM{options.Port} is currently ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("busy");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 1;
    }

    // Keep a serial port opened through-out the console session to block other applications from using it.
    SerialPort port = MinipadHelper.Open(device.PortName);

    // Ask for user input in a loop and only exit if the user types an exit keyword.
    string input = "";
    while(true)
    {
      // Ask for user input.
      Console.Write($"{device.Configuration.Name} ({device.PortName})> ");
      input = Console.ReadLine()!;

      // If the user typed an exit keyword, exit the loop.
      if (input is "exit" or "q" or "quit")
        break;

      // Try to send the command to the minipad.
      try
      {
        port.WriteLine(input);
      }
      catch (Exception ex)
      {
        // If any error occured, output it to the user.
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Error: ");
        Console.WriteLine($"{ex.InnerException!.Message}");
        Console.ForegroundColor = ConsoleColor.Gray;
      }
    }

    // Close the serial port and return 0 exit code for success.
    port.Close();
    return 0;
  }
}

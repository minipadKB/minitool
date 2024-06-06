using CommandLine;
using minitool.Enums;
using minitool.Models;
using Console = System.Console;

namespace minitool.Commands;

public static class Send
{
  [Verb("send", HelpText = "Send a raw command to the specified minipad.")]
  public class Options
  {
    [Value(0, Required = true, HelpText = "The port number of the minipad.")]
    public int Port { get; private set; }

    [Value(1, Required = true, HelpText = "The raw command to be sent.")]
    public string Command { get; private set; } = "";

    public Options() { }

    public Options(int port, string command)
    {
      Port = port;
      Command = command;
    }
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

    try
    {
      // Send the specified command to the device.
      MinipadHelper.Send(device, options.Command);
      Console.WriteLine("OK");
      return 0;
    }
    catch (Exception ex)
    {
      // If an error occurs, report it to the user.
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"Failed to send the command: {ex.Message}");
      Console.WriteLine(ex.StackTrace);
      Console.ForegroundColor = ConsoleColor.Gray;
      return 4;
    }
  }
}

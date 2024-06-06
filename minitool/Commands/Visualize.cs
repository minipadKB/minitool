using CommandLine;
using minitool.Enums;
using minitool.Models;

namespace minitool.Commands;
public static class Visualize
{
  [Verb("visualize", HelpText = "Visualizes the hall effect keys of the specified minipad.")]
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
    }
    catch (Exception ex)
    {
      // If any error occured, output it to the user.
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write("Error: ");
      Console.WriteLine($"{ex.InnerException!.Message}");
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

    // Poll the key states at high speed and visualize them.
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"  Visualizer for {device}");
    while (true)
    {
      // Get the mapped sensor values.
      (int[] rawValues, int[] mappedValues) = MinipadHelper.GetSensorValues(device);

      // Output each in it's row.
      for (int i = 0; i < rawValues.Length; i++)
      {
        // Calculate some widths and other numbers for the strings.
        int mappedMaxLength = device.Configuration.TravelDistance!.Value.ToString().Length;
        int rawMaxLength = (Math.Pow(device.Configuration.AnalogResolution!.Value, 2) - 1).ToString().Length;
        double travelledDistance = (device.Configuration.TravelDistance.Value - mappedValues[i]) / 100d;

        // Generate the strings for the part below and after the bar.
        string prefix = $"hkey{(i + 1).ToString().PadRight(rawValues.Length.ToString().Length)}: [";
        string suffix = $"] {travelledDistance.ToString("0.00").PadLeft(mappedMaxLength)}mm ({rawValues[i].ToString().PadLeft(rawMaxLength)})";

        // Calculate the total width available for the visualizer bar, based on the console width, prefix and suffix.
        int totalWidth = Console.WindowWidth - prefix.Length - suffix.Length - 3;
        int width = (int)Math.Ceiling(mappedValues[i] / (device.Configuration.TravelDistance.Value * 1d / totalWidth));

        // Print the line, including prefix, visualizer bar and suffix.
        Console.Write(prefix);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("".PadLeft(width, ' ').PadLeft(totalWidth, '='));
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(suffix);
      }

      // Write some empty lines in order to remove artifacts from resizing.
      for (int i = 0; i < rawValues.Length; i++)
        Console.WriteLine("".PadLeft(Console.WindowWidth));

      // Reset the cursor position.
      Console.CursorTop -= rawValues.Length * 2;
    }
  }
}

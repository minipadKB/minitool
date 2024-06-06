using CommandLine;
using minitool.Enums;
using minitool.Models;

namespace minitool.Commands;

public static class Devices
{
  [Verb("devices", HelpText = "Outputs a list of all connected minipads.")]
  public class Options
  {
    [Option('a', "all", HelpText = "Specifies whether disconnected and error'd devices are also shown.")]
    public bool ShowAll { get; private set; } = false;
  }

  public static int Handle(Options options)
  {
    // Get all minipad ports available. This includes disconnected minipads since each device
    // gets it's own unique COM port upon first interaction with the operating system.
    IEnumerable<int> ports = RegistryHelper.GetPortsByIDs(Program.MINIPAD_VID, Program.MINIPAD_PID);

    // Try to get all minipads on the COM ports.
    Dictionary<int, MinipadDevice?> devices = new Dictionary<int, MinipadDevice?>();
    foreach (int port in ports)
    {
      try
      {
        // Try to get the minipad device and add it to the dictionary.
        devices.Add(port, MinipadHelper.GetDeviceAsync(port).Result);
      }
      catch
      {
        // If it failed, add the port with null, indicating an error occured.
        devices.Add(port, null);
      }
    }

    // Output all connected devices, including their name, COM port and state.
    Console.WriteLine($"List of connected minipads ({devices.Where(x => x.Value?.State == DeviceState.CONNECTED).Count()} connected, " +
                                                  $"{devices.Where(x => x.Value?.State == DeviceState.BUSY).Count()} busy):");
    foreach (var kvp in devices.Where(x => x.Value?.State >= DeviceState.BUSY).OrderBy(x => x.Value!.State))
    {
      if (kvp.Value!.State == DeviceState.CONNECTED)
      {
        Console.Write($"  {kvp.Value} ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("connected");
        Console.ForegroundColor = ConsoleColor.Gray;
      }
      else if (kvp.Value.State == DeviceState.BUSY)
      {
        Console.Write($"  {kvp.Value} ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("busy");
        Console.ForegroundColor = ConsoleColor.Gray;
      }
    }

    // Output all disconnected devices if specifically specified.
    if (options.ShowAll)
    {
      Console.WriteLine();
      Console.WriteLine($"List of disconnected minipads ({devices.Where(x => x.Value?.State == DeviceState.DISCONNECTED).Count()} disconnected, {devices.Where(x => x.Value == null).Count()} error):");
      foreach (var kvp in devices.Where(x => x.Value!.State == DeviceState.DISCONNECTED).Concat(devices.Where(x => x.Value == null)))
      {
        Console.Write($"  COM{kvp.Key} ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(kvp.Value == null ? "error" : "disconnected");
        Console.ForegroundColor = ConsoleColor.Gray;
      }
    }

    // Return process exit code 0.
    return 0;
  }
}

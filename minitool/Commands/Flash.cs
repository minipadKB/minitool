using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minitool.Models;
using minitool.Enums;
using System.Diagnostics;

namespace minitool.Commands;

public static class Flash
{
  [Verb("flash", HelpText = "Sets the specified minipad into bootloader mode and flashes the specified firmware file.")]
  public class Options
  {
    [Value(0, Required = true, HelpText = "The port number of the minipad.")]
    public int Port { get; private set; }

    [Value(1, Required = true, HelpText = "The path to the firmware file.")]
    public string FirmwareFile { get; private set; }

    public Options() { }

    public Options(int port, string firmwareFile)
    {
      Port = port;
      FirmwareFile = firmwareFile;
    }
  }

  public static int Handle(Options options)
  {
    // Check whether the specified firmware file exists and is a .utf 2 file.
    if (!File.Exists(options.FirmwareFile))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Error: The specified firmware file does not exist.");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 6;
    }
    else if (Path.GetExtension(options.FirmwareFile) != ".uf2")
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Error: The specified firmware file is not a .uf2 file.");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 5;
    }

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
      // Save a list of drives before setting the device into bootloader mode to track the drive latter.
      string[] drives = Environment.GetLogicalDrives();

      // Set the device into bootloader mode by setting the serial baud rate to 1200.
      Process.Start(new ProcessStartInfo()
      {
        FileName = "mode.com",
        Arguments = $"COM{options.Port}:Baud=1200",
        CreateNoWindow = true,
        UseShellExecute = false
      })?.WaitForExit();

      // Waiting for a new drive to appear.
      Console.WriteLine("Waiting for bootloader...");
      while(Environment.GetLogicalDrives().SequenceEqual(drives))
        Thread.Sleep(100);

      // Get the drive letter of the new drive and copy the firmware file on it.
      Console.WriteLine("Flashing...");
      string drive = Environment.GetLogicalDrives().Except(drives).First();
      File.Copy(options.FirmwareFile, Path.Combine(drive, "firmware.uf2"), true);

      Console.WriteLine("Done.");
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

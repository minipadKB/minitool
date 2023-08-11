using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minitool.Enums;
using minitool.Models;
using minitool.Utils;
using System.Security.Cryptography.X509Certificates;

namespace minitool.Commands;

public static class Calibrate
{
  [Verb("calibrate", HelpText = "Enters a calibration mode to calibrate the hall effect keys on the minipad.")]
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
      Console.WriteLine($"{ex.InnerException!.Message}\n{ex.InnerException!.StackTrace}");
      Console.ForegroundColor = ConsoleColor.Gray;
      return 3;
    }

    // Just output the state of the device if it's not connected.
    if (device.State == DeviceState.DISCONNECTED)
    {
      Console.Write($"The minipad on COM{options.Port} is currently ");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("disconnected");
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

    // Deactivate hid output on the hall effect keys for calibration.
    bool hidEnabled = device.Configuration.HallEffectKeys.Any(x => x.HidEnabled!.Value);
    MinipadHelper.Send(device, "hkey.hid 0");

    Console.Write("Please make sure all keys are ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("fully released");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(" and press the enter key.");

    // Block until the enter key was pressed.
    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
      ;

    // Get the current sensor values of the minipad.
    int[] restPositions = MinipadHelper.GetSensorValues(device).raw;
    
    Console.WriteLine();
    Console.Write("Please make sure all keys are ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("fully pressed");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(" and press the enter key.");

    // Block until the enter key was pressed.
    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
      ;

    // Get the current sensor values of the minipad.
    int[] downPositions = MinipadHelper.GetSensorValues(device).raw;

    // Apply a tolerance to the rest and down positions for better stability.
    restPositions = restPositions.Select(x => x - 3).ToArray();
    downPositions = downPositions.Select(x => x + 3).ToArray();

    // Output the old and new values.
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Results:");
    Console.ForegroundColor = ConsoleColor.Gray;
    for (int i = 0; i < downPositions.Length; i++)
      Console.WriteLine($"hkey{i + 1}: rest={restPositions[i]} (current: {device.Configuration.HallEffectKeys[i].RestPosition}) " +
                        $"down={downPositions[i]} (current: {device.Configuration.HallEffectKeys[i].DownPosition})");

    // Ask the user whether they'd like to apply the new values.
    Console.WriteLine();
    Console.Write("Would you like to apply the new calibration values? (y/n)");
    ConsoleKeyInfo cki;
    do { cki = Console.ReadKey(true); }
    while (!new char[] { 'y', 'n' }.Contains(cki.KeyChar));
    Console.WriteLine();

    // If the user said yes, apply the new values.
    if(cki.KeyChar == 'y')
    {
      // Generate a list of commands that are being sent.
      List<string> commands = new List<string>();
      for (int i = 0; i < restPositions.Length; i++)
        commands.AddRange(new[] { $"hkey{i + 1}.rest {restPositions[i]}", $"hkey{i + 1}.down {downPositions[i]}" });

      // Don't forget to save the config.
      commands.Add("save");

      // Send the commands.
      MinipadHelper.Send(device, commands.ToArray());
      Console.WriteLine("New calibration applied.");

      // Ask the user whether they'd like to enable HID if HID had been disabled on all keys before.
      if (!hidEnabled)
      {
        Console.WriteLine();
        Console.Write("Would you like to enable HID output? (y/n)");
        do
        { cki = Console.ReadKey(true); }
        while (!new char[] { 'y', 'n' }.Contains(cki.KeyChar));
        Console.WriteLine();

        // If the user said yes, enable HID output.
        if (cki.KeyChar == 'y')
        {
          // Send the hid enable command and save the config.
          MinipadHelper.Send(device, "hkey.hid 1", "save");
          Console.WriteLine("HID output enabled.");
        }
      }
    }

    return 0;
  }
}

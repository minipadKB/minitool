using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;
using minitool.Enums;
using minitool.Models;

namespace minitool;

/// <summary>
/// Helper class for the device and serial communication.
/// </summary>
public static class MinipadHelper
{
  public static void Send(MinipadDevice device, params string[] commands)
  {
    // Open the serial port.
    SerialPort serialPort = new SerialPort(device.PortName, 115200, Parity.Even, 8, StopBits.One);
    serialPort.RtsEnable = true;
    serialPort.DtrEnable = true;
    serialPort.Open();

    // Write all specified commands to the serial interface.
    foreach (string command in commands)
      serialPort.WriteLine(command);

    // Safely close the serial port for usage by other processes.
    serialPort.Close();
  }

  public static (int[] raw, int[] mapped) GetSensorValues(MinipadDevice device)
  {
    // Initialize the raw values and mapped values arrays.
    int[] rawValues = new int[device.Configuration.HallEffectKeys.Length];
    int[] mappedValues = new int[device.Configuration.HallEffectKeys.Length];
    for (int i = 0; i < rawValues.Length; i++)
    {
      rawValues[i] = -1;
      mappedValues[i] = -1;
    }

    SerialPort port = new SerialPort(device.PortName, 115200, Parity.None, 8, StopBits.One);
    port.RtsEnable = true;
    port.DtrEnable = true;
    port.DataReceived += (sender, e) =>
    {
      lock (port)
      {
        // Read from the serial interface while data is available.
        while (port.BytesToRead > 0)
        {
          // Read the current line and remove the \r at the end.
          string line = port.ReadLine().Replace("\r", "");

          // Check whether the line starts with "OUT" indicating the sensor output. ("OUT hkey?=rawValue mappedValue")
          if (!line.StartsWith("OUT"))
            continue;

          // Parse the key index and the  sensor value sfrom the output received and remember it.
          int keyIndex = int.Parse(line.Split('=')[0].Substring(8)) - 1;
          rawValues[keyIndex] = int.Parse(line.Split('=')[1].Split(' ')[0]);
          mappedValues[keyIndex] = int.Parse(line.Split('=')[1].Split(' ')[1]);
        }
      }
    };

    // Open the port, send the out command, wait until no value in the array is -1 anymore and safely close it.
    port.Open();
    port.WriteLine("out");
    while (rawValues.Any(x => x == -1))
      ;
    lock (port)
      port.Close();

    // Return the read values.
    return (rawValues, mappedValues);
  }

  public static async Task<MinipadDevice> GetDeviceAsync(int port)
  {
    try
    {
      // Open the serial port.
      SerialPort serialPort = new SerialPort($"COM{port}", 115200, Parity.Even, 8, StopBits.One);
      serialPort.RtsEnable = true;
      serialPort.DtrEnable = true;
      serialPort.Open();

      // Create a semaphore for timeouting the serial data reading.
      SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0);
      Dictionary<string, string> values = new Dictionary<string, string>();

      // Callback method for processing the data received from the serial monitor.
      void DataReceivedCallback(object sender, SerialDataReceivedEventArgs e)
      {
        // Lock the serial port to make sure it's not being closed when the 200ms timeout runs out.
        lock (serialPort)
        {
          // Read the data all the way to the end, line by line.
          while (serialPort.BytesToRead > 0)
          {
            // Read the next line.
            string line = serialPort.ReadLine().TrimEnd('\r');

            // If the end of the 'get' command was reached (indicated by "GET END"), release the semaphore thus returning.
            if (line == "GET END")
              semaphoreSlim.Release();
            // If the received line starts with "GET", it's a key-value pair with the keypad's specifications.
            else if (line.StartsWith($"GET "))
            {
              // Add the key and value as a string to the values dictionary.
              string key = line[4..].Split('=')[0];
              values.Add(key, line[(4 + key.Length + 1)..]);
            }
          }
        }
      }

      // Subscribe to the callback method, write the 'get' command that returns the keypad specifications.
      serialPort.DataReceived += DataReceivedCallback;
      serialPort.WriteLine("get");

      // Give the response 200ms until this reading process times out.
      await semaphoreSlim.WaitAsync(10);

      // Safely close the serial port for usage by other processes.
      lock (serialPort)
        serialPort.Close();

      // Create a Configuration object from the retrieved values, starting with the global settings.
      Configuration configuration = new Configuration()
      {
        Name = values.ContainsKey("name") ? values["name"] : null,
        HysteresisTolerance = values.ContainsKey("htol") ? int.Parse(values["htol"]) : null,
        RapidTriggerTolerance = values.ContainsKey("rtol") ? int.Parse(values["rtol"]) : null,
        TravelDistance = values.ContainsKey("trdt") ? int.Parse(values["trdt"]) : null,
        AnalogResolution = values.ContainsKey("ares") ? int.Parse(values["ares"]) : null,
        HallEffectKeys = values.ContainsKey("hkeys") ? new HallEffectKey[int.Parse(values["hkeys"])] : new HallEffectKey[0],
        DigitalKeys = values.ContainsKey("dkeys") ? new DigitalKey[uint.Parse(values["dkeys"])] : new DigitalKey[0]
      };

      // Parse the hall effect keys.
      for (int i = 0; i < (values.ParseKey<int>("hkeys") ?? 0); i++)
        configuration.HallEffectKeys[i] = new HallEffectKey()
        {
          Index = i,
          RapidTrigger = values.ParseKey<bool>($"hkey{i + 1}.rt"),
          ContinuousRapidTrigger = values.ParseKey<bool>($"hkey{i + 1}.crt"),
          RapidTriggerUpSensitivity = values.ParseKey<int>($"hkey{i + 1}.rtus"),
          RapidTriggerDownSensitivity = values.ParseKey<int>($"hkey{i + 1}.rtds"),
          LowerHysteresis = values.ParseKey<int>($"hkey{i + 1}.lh"),
          UpperHysteresis = values.ParseKey<int>($"hkey{i + 1}.uh"),
          KeyChar = values.ParseKey<char>($"hkey{i + 1}.char"),
          RestPosition = values.ParseKey<int>($"hkey{i + 1}.rest"),
          DownPosition = values.ParseKey<int>($"hkey{i + 1}.down"),
          HidEnabled = values.ParseKey<bool>($"hkey{i + 1}.hid"),
        };

      // Parse the digital keys.
      for (int i = 0; i < (values.ParseKey<int>("dkeys") ?? 0); i++)
        configuration.DigitalKeys[i] = new DigitalKey()
        {
          Index = i,
          KeyChar = values.ParseKey<char>($"dkey{i + 1}.char"),
          HidEnabled = values.ParseKey<bool>($"dkey{i + 1}.hid")
        };

      // Create a MinipadDevice object with the parsed info and return it.
      return new MinipadDevice(port, DeviceState.CONNECTED, values.ContainsKey("version") ? values["version"] : null, configuration);
    }
    catch (UnauthorizedAccessException)
    {
      // If an UnauthorizedAccessException was thrown, the device is connected but the serial interface occupied by another process.
      return new MinipadDevice(port, DeviceState.BUSY, null, new Configuration());
    }
    catch (FileNotFoundException)
    {
      // If a FileNotFoundException was thrown, the device is disconnected.
      return new MinipadDevice(port, DeviceState.DISCONNECTED, null, new Configuration());
    }
  }

  /// <summary>
  /// Tries to parse the specified key into the specified type.
  /// </summary>
  /// <typeparam name="T">The type to parse into.</typeparam>
  /// <param name="dict">The dictionary containing the key-value-pair.</param>
  /// <param name="key">The target key whichs value is being parsed.</param>
  /// <returns>The parsed object or null if the key was not found or parsing failed.</returns>
  private static T? ParseKey<T>(this Dictionary<string, string> dict, string key) where T : struct
  {
    // If the dictionary does not contain the key at all, return null.
    if (!dict.ContainsKey(key))
      return null;

    // Parse the string value properly depending on the specified generic type.
    if (typeof(T) == typeof(bool))
      return (T) (object) (dict[key] == "1");
    else if (typeof(T) == typeof(char))
    {
      bool successful = int.TryParse(dict[key], out int result) && result <= char.MaxValue;
      return successful ? (T) (object) (char) result : null;
    }
    else if (typeof(T) == typeof(int))
    {
      bool successful = int.TryParse(dict[key], out int result);
      return successful ? (T) (object) result : null;
    }

    // If no type matched, return null.
    return null;
  }
}
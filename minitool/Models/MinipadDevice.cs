using minitool.Enums;

namespace minitool.Models;

/// <summary>
/// Represents a keypad including it's configuration.
/// </summary>
#pragma warning disable CS0659
public class MinipadDevice
#pragma warning restore CS0659
{
  /// <summary>
  /// The COM port on which the device is communicating at.
  /// </summary>
  public int Port { get; private set; }

  /// <summary>
  /// The COM port name on which the device is communicating at.
  /// </summary>
  public string PortName => $"COM{Port}";

  /// <summary>
  /// The state of the device.
  /// </summary>
  public DeviceState State { get; private set; }

  /// <summary>
  /// The version of the firmware on the keypad.
  /// </summary>
  public string? FirmwareVersion { get; private set; }

  /// <summary>
  /// The configuration of the keypad.
  /// </summary>
  public Configuration Configuration { get; }
  
  /// <summary>
  /// Initialize a new MinipadDevice instance with the specified COM port, firmware version and configuration.
  /// </summary>
  /// <param name="port">The COM port on which the device is communicating at.</param>
  /// <param name="firmwareVersion">The version of the firmware on the keypad.</param>
  /// <param name="configuration">The configuration of the keypad.</param>
  public MinipadDevice(int port, DeviceState state, string? firmwareVersion, Configuration configuration)
  {
    Port = port;
    State = state;
    FirmwareVersion = firmwareVersion;
    Configuration = configuration;
  }

  /// <summary>
  /// Returns a readable string representation of the MinipadDevice object.
  /// </summary>
  public override string ToString() => Configuration.Name != null ? $"{Configuration.Name} ({PortName})" : $"{PortName}";

  /// <summary>
  /// Checks whether the MinipadDevices specified are equal by comparing their COM port.
  /// </summary>
  /// <param name="obj">The MinipadDevice object to compare to.</param>
  /// <returns>Bool whether the compared MinipadDevice objects have the same COM port.</returns>
  public override bool Equals(object? obj) => obj is MinipadDevice device && device.Port == Port;
}

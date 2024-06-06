namespace minitool.Enums;

/// <summary>
/// The state of the device.
/// </summary>
public enum DeviceState
{
  /// <summary>
  /// The device is currently not connected.
  /// </summary>
  DISCONNECTED = 0,

  /// <summary>
  /// The device is connected but the serial interface occupied.
  /// </summary>
  BUSY = 1,

  /// <summary>
  /// The device is connected and accessble via serial.
  /// </summary>
  CONNECTED = 2
}

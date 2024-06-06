using minitool.Utils;

namespace minitool.Models;

/// <summary>
/// Represents a digital key on the keypad, also acting as the base class for the hall effect keys.
/// </summary>
public class DigitalKey
{
  /// <summary>
  /// The index of the key.
  /// </summary>
  public int? Index { get; set; }

  /// <summary>
  /// The corresponding key sent via HID interface.
  /// </summary>
  public char? KeyChar { get; set; }

  /// <summary>
  /// Bool whether HID commands are sent on the key.
  /// </summary>
  public bool? HidEnabled { get; set; }

  /// <summary>
  /// Outputs a readable string representation of the key.
  /// </summary>
  public override string ToString()
  {
    // dkey1: char='a' hid=true

    return $"dkey{(Index + 1).IfNull("?")}: char={KeyChar.IfNull("?", "'{}'")} hid={HidEnabled.IfNull("?").ToLower()}";
  }
}
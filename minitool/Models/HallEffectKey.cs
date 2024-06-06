using minitool.Utils;

namespace minitool.Models;

/// <summary>
/// Represents a single hall effect key on the keypad.
/// </summary>
public class HallEffectKey : DigitalKey
{
  /// <summary>
  /// Bool whether rapid trigger is enabled or not.
  /// </summary>
  public bool? RapidTrigger { get; set; }
  /// <summary>
  /// Bool whether continuous rapid trigger is enabled or not.
  /// </summary>
  public bool? ContinuousRapidTrigger { get; set; }

  /// <summary>
  /// The sensitivity of the rapid trigger algorithm when pressing up.
  /// </summary>
  public int? RapidTriggerUpSensitivity { get; set; }

  /// <summary>
  /// The sensitivity of the rapid trigger algorithm when pressing down.
  /// </summary>
  public int? RapidTriggerDownSensitivity { get; set; }

  /// <summary>
  /// The value below which the key is pressed and rapid trigger is active in rapid trigger mode.
  /// </summary>
  public int? LowerHysteresis { get; set; }

  /// <summary>
  /// The value below which the key is no longer pressed and rapid trigger is no longer active in rapid trigger mode.
  /// </summary>
  public int? UpperHysteresis { get; set; }

  /// <summary>
  /// The value read from the sensor when the key is in rest position.
  /// </summary>
  public int? RestPosition { get; set; }

  /// <summary>
  /// The value read from the sensor when the key is in down position.
  /// </summary>
  public int? DownPosition { get; set; }

  /// <summary>
  /// Outputs a readable string representation of the key.
  /// </summary>
  public override string ToString()
  {
    // hkey1: rt=true crt=true rtus=20 rtds=60 lh=280 uh=330 rest=1800 down=1100 char='a' hid=true

    return $"hkey{(Index + 1).IfNull("?")}: rt={RapidTrigger.IfNull("?").ToLower()} crt={ContinuousRapidTrigger.IfNull("?").ToLower()} " +
           $"rtus={RapidTriggerUpSensitivity.IfNull("?")} rtds={RapidTriggerDownSensitivity.IfNull("?")} lh={LowerHysteresis.IfNull("?")} " +
           $"uh={UpperHysteresis.IfNull("?")} rest={RestPosition.IfNull("?")} down={DownPosition.IfNull("?")}{base.ToString().Split(':')[1]}";
  }
}
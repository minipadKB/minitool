namespace minitool.Models;

/// <summary>
/// Represents the configuration of a minipad device.
/// </summary>
public class Configuration
{
  /// <summary>
  /// The name of the keypad.
  /// </summary>
  public string? Name { get; set; }

  /// <summary>
  /// The tolerance of the hysteresis, meaning the minimum difference between lower
  /// and upper hysteresis, as well as the upper hysteresis and maximum travel distance.
  /// </summary>
  public int? HysteresisTolerance { get; set; }

  /// <summary>
  /// The tolerance of rapid trigger, meaning the minimum sensitivity for the up and down movements.
  /// </summary>
  public int? RapidTriggerTolerance { get; set; }

  /// <summary>
  /// The travel distance of the hall effect keys on the keypad, in steps of 0.01mm.
  /// </summary>
  public int? TravelDistance { get; set; }

  /// <summary>
  /// The analog resolution of the ADC of the keypad's micro controller.
  /// </summary>
  public int? AnalogResolution { get; set; }

  /// <summary>
  /// The hall effect keys of the keypad.
  /// </summary>
  public HallEffectKey[] HallEffectKeys { get; set; } = new HallEffectKey[0];

  /// <summary>
  /// The digital keys of the keypad.
  /// </summary>
  public DigitalKey[] DigitalKeys { get; set; } = new DigitalKey[0];
}
namespace minitool.Utils;

/// <summary>
/// Helper class containing extension methods for various types.
/// </summary>
public static class ExtensionMethods
{
  /// <summary>
  /// Returns the .ToString() result of the object or the specified string if it's null.
  /// Also supports specifiying a format with "{}" as the placeholder.
  /// </summary>
  /// <param name="obj">The object to convert to a string.</param>
  /// <param name="alternative">The alternative string if the object is null.</param>
  /// <param name="format">A format string where "{}" is being replaced with the actual object string.</param>
  /// <returns>The object as a string or the specified string if the object is null.</returns>
  public static string IfNull(this object? obj, string alternative, string format = "")
  {
    // Check whether the object is null and if so, return the alternative.
    if (obj == null)
      return alternative;

    // Return the plain .ToString() string if no format is specified.
    if (format == "")
      return obj.ToString()!;

    // If a format is specified, return the format string with "{}" being replaced with the .ToString() string.
    return format.Replace("{}", obj.ToString()!);
  }
}

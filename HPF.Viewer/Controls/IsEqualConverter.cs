using System;
using System.Globalization;

namespace HPF.Viewer.Controls
{
  public class IsEqualConverter<TEquatable> : ValueConverter<TEquatable, bool>
  {
    public readonly bool Invert;
    public readonly TEquatable TrueValue;

    public IsEqualConverter(TEquatable trueValue, bool invert = false)
    {
      Invert = invert;
      TrueValue = trueValue;
    }

    public override bool Convert(TEquatable value, object parameter, CultureInfo culture)
      => Invert
        ? !TrueValue.Equals(value)
        : TrueValue.Equals(value);

    public override TEquatable ConvertBack(bool value, object parameter, CultureInfo culture)
      => value && !Invert
        ? TrueValue
        : default(TEquatable);
  }
}
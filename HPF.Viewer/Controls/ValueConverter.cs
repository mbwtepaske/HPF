using System;
using System.Globalization;
using System.Windows.Data;

namespace HPF.Viewer.Controls
{
  public abstract class ValueConverter<TSource, TDestination> : IValueConverter
  {
    public virtual TDestination Convert(TSource source, object parameter, CultureInfo culture) => default(TDestination);
    
    public virtual TSource ConvertBack(TDestination destination, object parameter, CultureInfo culture) => default(TSource);
    
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => targetType == typeof(TDestination) && value is TSource source
        ? (object) Convert(source, parameter, culture)
        : default(TSource);

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      => targetType == typeof(TSource) && value is TDestination destination
        ? (object) ConvertBack(destination, parameter, culture)
        : default(TSource);
  }
}
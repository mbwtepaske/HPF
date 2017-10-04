using System;
using System.Windows;

using PostSharp.Patterns.Xaml;

namespace HPF.Viewer
{
  using Models;

  public class ApplicationViewModel : DependencyObject
  {
    [DependencyProperty]
    public Surface Surface
    {
      get;
      set;
    }
  }
}
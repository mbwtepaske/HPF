using System;
using System.Windows;

namespace HPF.Viewer
{
  public partial class MainWindow
  {
    public MainWindow() => InitializeComponent();

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      DataContext = new ApplicationViewModel
      {
        Surface = Program.Current.Surface
      };
    }
  }
}

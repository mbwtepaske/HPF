using System.IO;
using System.Windows;

namespace HPF.Viewer
{
  using Models;

  public partial class Program
  {
    public new static Program Current => (Program) Application.Current;

    private static readonly FileInfo SurfaceFileInfo = new FileInfo("Surface.dat");

    public Surface Surface
    {
      get;
      private set;
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
      Surface.Save(SurfaceFileInfo.FullName);
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
      SurfaceFileInfo.Refresh();

      if (SurfaceFileInfo.Exists)
      {
        Surface = Surface.Load(SurfaceFileInfo.FullName);
      }
      else
      {
        Surface = new Surface();
        Surface.GenerateChunk((0, 0));
        Surface.GenerateChunk((1, 0));
        Surface.GenerateChunk((2, 0));
        Surface.GenerateChunk((0, 1));
        Surface.GenerateChunk((2, 1));
        Surface.GenerateChunk((0, 2));
        Surface.GenerateChunk((1, 2));
        Surface.GenerateChunk((2, 2));
      }
    }
  }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using PostSharp.Patterns.Xaml;

namespace HPF.Viewer.Controls
{
  using Models;

  public sealed class TileControl : Control
  {
    static TileControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TileControl), new FrameworkPropertyMetadata(typeof(TileControl)));

    [DependencyProperty]
    public bool IsBlocked
    {
      get;
      set;
    }

    private void OnIsBlockedChanged() => UpdateVisualState(true);

    public static DependencyProperty IsBlockedProperty
    {
      get;
      private set;
    }

    [DependencyProperty]
    public Tile Tile
    {
      get;
      set;
    }

    private void OnTileChanged()
    {
      ToolTip = Tile.Position.ToString();

      SetBinding(IsBlockedProperty, new Binding(nameof(Tile.Blocked))
      {
        Mode = BindingMode.TwoWay,
        Source = Tile
      });

      UpdateVisualState(true);
    }

    private SurfaceControl _surfaceControl;

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      for (var current = (DependencyObject) this; current != null; current = VisualTreeHelper.GetParent(current))
      {
        if (current is SurfaceControl parentSurfaceControl)
        {
          _surfaceControl = parentSurfaceControl;

          break;
        }
      }

      base.OnVisualParentChanged(oldParent);
    }

    public override void OnApplyTemplate() => UpdateVisualState(false);

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (e.LeftButton == MouseButtonState.Pressed && _surfaceControl != null)
      {
        IsBlocked = SurfaceControl.BlockState[_surfaceControl];
      }
    }

    protected override void OnMouseEnter(MouseEventArgs e) => UpdateVisualState(true);

    protected override void OnMouseLeave(MouseEventArgs e) => UpdateVisualState(true);

    private void UpdateVisualState(bool useTransitions)
    {
      VisualStateManager.GoToState(this, IsBlocked ? "Blocked" : "Default", useTransitions);
      VisualStateManager.GoToState(this, IsMouseOver ? "MouseOver" : "Normal", useTransitions);
    }
  }
}
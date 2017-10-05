using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

    private SurfaceControl _surfaceControl;

    #region Is Blocked
    
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

    #endregion

    #region Is Goal Tile

    [DependencyProperty]
    public bool IsGoalTile
    {
      get;
      private set;
    }

    public static DependencyProperty IsGoalTileProperty
    {
      get;
      private set;
    }

    private void OnIsGoalTileChanged() => UpdateVisualState(true);

    #endregion

    #region Is Start Tile

    [DependencyProperty]
    public bool IsStartTile
    {
      get;
      private set;
    }

    public static DependencyProperty IsStartTileProperty
    {
      get;
      private set;
    }

    private void OnIsStartTileChanged() => UpdateVisualState(true);

    #endregion

    #region Tile

    [DependencyProperty]
    public Tile Tile
    {
      get;
      set;
    }

    private void OnTileChanged()
    {
      ToolTip = Tile.Position.ToString();

      UpdateVisualState(true);
    }

    #endregion
    
    public override void OnApplyTemplate() => UpdateVisualState(false);

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (e.LeftButton == MouseButtonState.Pressed && _surfaceControl != null)
      {
        switch (_surfaceControl.EditorMode)
        {
          case SurfaceEditMode.AssignBlocking:
            IsBlocked = true;
            break;

          case SurfaceEditMode.AssignDefault:
            IsBlocked = false;
            break;
        }
      }
    }

    protected override void OnMouseEnter(MouseEventArgs e) => UpdateVisualState(true);

    protected override void OnMouseLeave(MouseEventArgs e) => UpdateVisualState(true);

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      for (var current = (DependencyObject) this; current != null; current = VisualTreeHelper.GetParent(current))
      {
        if (current is SurfaceControl parentSurfaceControl)
        {
          _surfaceControl = parentSurfaceControl;

          UpdateBindings();
          UpdateParent();
          
          break;
        }
      }

      base.OnVisualParentChanged(oldParent);
    }

    private void UpdateVisualState(bool useTransitions)
    {
      VisualStateManager.GoToState(this, IsGoalTile ? "Goal" : "NoGoal", useTransitions);
      VisualStateManager.GoToState(this, IsStartTile ? "Start" : "NoStart", useTransitions);
      VisualStateManager.GoToState(this, IsBlocked ? "Blocked" : "Default", useTransitions);
      VisualStateManager.GoToState(this, IsMouseOver ? "MouseOver" : "Normal", useTransitions);
    }

    private void UpdateBindings()
    {
      SetBinding(IsBlockedProperty, new Binding
      {
        Converter = new IsEqualConverter<TileMode>(TileMode.Blocked),
        Mode      = BindingMode.TwoWay,
        Path      = new PropertyPath(nameof(Tile.Mode)),
        Source    = Tile
      });

      SetBinding(IsGoalTileProperty, new Binding
      {
        Converter = new IsEqualConverter<Position>(Tile.Position),
        Path      = new PropertyPath(SurfaceControl.GoalPositionProperty),
        Source    = _surfaceControl
      });

      SetBinding(IsStartTileProperty, new Binding
      {
        Converter = new IsEqualConverter<Position>(Tile.Position),
        Path      = new PropertyPath(SurfaceControl.StartPositionProperty),
        Source    = _surfaceControl
      });
    }

    private void UpdateParent()
    {
      var isGoalButton = new MenuItem
      {
        Command           = _surfaceControl.AssignGoalPosition,
        CommandParameter  = this,
        Header            = "Goal",
        IsCheckable       = true,
      };
      
      isGoalButton.SetBinding(MenuItem.IsCheckedProperty, new Binding
      {
        Mode    = BindingMode.OneWay,
        Path    = new PropertyPath(IsGoalTileProperty),
        Source  = this
      });
      
      var isStartButton = new MenuItem
      {
        Command           = _surfaceControl.AssignStartPosition,
        CommandParameter  = this,
        Header            = "Start",
        IsCheckable       = true
      };
      
      isStartButton.SetBinding(MenuItem.IsCheckedProperty, new Binding
      {
        Mode    = BindingMode.OneWay,
        Path    = new PropertyPath(IsStartTileProperty),
        Source  = this
      });
      
      ContextMenu = new ContextMenu
      {
        Items =
        {
          isGoalButton,
          isStartButton
        },
        Placement = PlacementMode.Bottom
      };
    }
  }
}
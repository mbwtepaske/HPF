using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using PostSharp.Patterns.Xaml;

namespace HPF.Viewer.Controls
{
  using Algorithms;
  using Models;

  [TemplatePart(Name = Parts.Canvas,  Type = typeof(Canvas))]
  [TemplatePart(Name = Parts.Panel,   Type = typeof(Grid))]
  [TemplatePart(Name = Parts.ToolBar, Type = typeof(ToolBar))]
  public sealed class SurfaceControl : Control
  {
    public static class Parts
    {
      public const string Canvas  = "PART_Canvas";
      public const string Panel   = "PART_Panel";
      public const string ToolBar = "PART_ToolBar";
    }

    private const double ChuckSize = 384D;

    private readonly Dictionary<Position, UniformGrid> _chunkGridMapping = new Dictionary<Position, UniformGrid>();

    static SurfaceControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SurfaceControl), new FrameworkPropertyMetadata(typeof(SurfaceControl)));

    private Canvas _canvas;
    private Grid _itemsPanel;
    private ToolBar _instructionMenu;

    #region Navigate

    [Command]
    public ICommand Navigate
    {
      get;
      set;
    }

    //private bool CanExecuteNavigate() => Surface != null && StartPosition.HasValue && GoalPosition.HasValue;
    
    public void ExecuteNavigate()
    {
      var path = TileNavigator.AStar(Surface, StartPosition.GetValueOrDefault(), GoalPosition.GetValueOrDefault()).GetPath();

      if (_canvas != null)
      {
        var tileSize = ChuckSize / Surface.ChunkSize;
        var tileCenterOffset = tileSize / 2D;
        var tilePositions = path
          .Select(node => new Point(node.Tile.Position.X * tileSize + tileCenterOffset, node.Tile.Position.Y * tileSize + tileCenterOffset))
          .ToArray();

        _canvas.Children.Clear();
        _canvas.Children.Add(new Polyline
        {
          Points = new PointCollection(tilePositions),
          Stroke = Brushes.Green,
          StrokeDashArray = { 3D, 3D },
          StrokeDashCap = PenLineCap.Round,
          StrokeThickness = 3D,
        });
      }
    }
    
    #endregion

    #region Goal Position

    public static readonly DependencyProperty GoalPositionProperty = DependencyProperty.Register(nameof(GoalPosition), typeof(Position?), typeof(SurfaceControl));

    public Position? GoalPosition
    {
      get => (Position?)GetValue(GoalPositionProperty);
      set => SetValue(GoalPositionProperty, value);
    }

    [Command]
    public ICommand AssignGoalPosition
    {
      get;
      set;
    }

    private void ExecuteAssignGoalPosition(object parameter)
    {
      if (parameter is TileControl tileControl)
      {
        GoalPosition = tileControl.Tile.Position;
      }
    }

    #endregion

    #region Start Position

    public static readonly DependencyProperty StartPositionProperty = DependencyProperty.Register(nameof(StartPosition), typeof(Position?), typeof(SurfaceControl));
    
    public Position? StartPosition
    {
      get => (Position?) GetValue(StartPositionProperty);
      set => SetValue(StartPositionProperty, value);
    }
    
    [Command]
    public ICommand AssignStartPosition
    {
      get;
      set;
    }

    private void ExecuteAssignStartPosition(object parameter)
    {
      if (parameter is TileControl tileControl)
      {
        StartPosition = tileControl.Tile.Position;
      }
    }

    #endregion

    #region Surface

    [DependencyProperty]
    public Surface Surface
    {
      get;
      set;
    }

    private void OnSurfaceChanged()
    {
      SetBinding(GoalPositionProperty, new Binding
      {
        Path = new PropertyPath(nameof(Surface.GoalPosition)),
        Mode = BindingMode.TwoWay,
        Source = Surface,
      });
      
      SetBinding(StartPositionProperty, new Binding
      {
        Path    = new PropertyPath(nameof(Surface.StartPosition)),
        Mode    = BindingMode.TwoWay,
        Source  = Surface,
      });
      
      RegenerateTiles();
    }

    #endregion
    
    #region Surface Edit Mode

    [DependencyProperty]
    public SurfaceEditMode EditorMode
    {
      get;
      set;
    }

    public static DependencyProperty EditorModeProperty
    {
      get;
      private set;
    }

    #endregion
    
    public override void OnApplyTemplate()
    {
      _canvas = (Canvas) GetTemplateChild(Parts.Canvas);
      _itemsPanel = (Grid) GetTemplateChild(Parts.Panel);
      _instructionMenu = (ToolBar) GetTemplateChild(Parts.ToolBar);

      RegenerateMenu();
      //RegenerateTiles();
    }

    private void RegenerateMenu()
    {
      if (_instructionMenu != null)
      {
        var blockButton = new ToggleButton
        {
          Content     = "\uF0C8",
          FontFamily  = (FontFamily) FindResource("FontAwesome"),
          ToolTip     = TileMode.Blocked
        };
        
        blockButton.SetBinding(ToggleButton.IsCheckedProperty, new Binding
        {
          Converter = new IsEqualConverter<SurfaceEditMode>(SurfaceEditMode.AssignBlocking),
          Mode      = BindingMode.TwoWay,
          Path      = new PropertyPath(EditorModeProperty),
          Source    = this
        });
        
        var defaultButton = new ToggleButton
        {
          Content     = "\uF047",
          FontFamily  = (FontFamily) FindResource("FontAwesome"),
          ToolTip     = TileMode.Default
        };

        defaultButton.SetBinding(ToggleButton.IsCheckedProperty, new Binding
        {
          Converter = new IsEqualConverter<SurfaceEditMode>(SurfaceEditMode.AssignDefault),
          Mode      = BindingMode.TwoWay,
          Path      = new PropertyPath(EditorModeProperty),
          Source    = this
        });

        _instructionMenu.Items.Add(blockButton);
        _instructionMenu.Items.Add(defaultButton);
        _instructionMenu.Items.Add(new Button
        {
          Command     = Navigate,
          Content     = "A*",
          //FontFamily  = (FontFamily) FindResource("FontAwesome"),
          ToolTip     = "Navigate"
        });
      }
    }

    private void RegenerateTiles()
    {
      if (_itemsPanel != null)
      {
        _itemsPanel.Children.Clear();
        _chunkGridMapping.Clear();

        foreach (var tile in Surface)
        {
          var tileControl = new TileControl
          {
            Tile = tile
          };

          var chunkPosition = tile.Position / Surface.ChunkSize;

          if (!_chunkGridMapping.TryGetValue(chunkPosition, out var chunkGrid))
          {
            while (_itemsPanel.ColumnDefinitions.Count < chunkPosition.X + 1)
            {
              _itemsPanel.ColumnDefinitions.Add(new ColumnDefinition
              {
                Width = new GridLength(ChuckSize)
              });
            }

            while (_itemsPanel.RowDefinitions.Count < chunkPosition.Y + 1)
            {
              _itemsPanel.RowDefinitions.Add(new RowDefinition
              {
                Height = new GridLength(ChuckSize)
              });
            }

            _chunkGridMapping[chunkPosition] = chunkGrid = new UniformGrid
            {
              Columns = (int) Surface.ChunkSize,
              Rows    = (int) Surface.ChunkSize
            };

            Grid.SetColumn(chunkGrid, (int) chunkPosition.X);
            Grid.SetRow(chunkGrid, (int) chunkPosition.Y);

            _itemsPanel.Children.Add(chunkGrid);
          }

          chunkGrid.Children.Add(tileControl);
        }
      }
    }
  }
}
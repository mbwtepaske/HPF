using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

  [TemplatePart(Name = Parts.Panel, Type = typeof(Grid))]
  public sealed class SurfaceControl : Control
  {
    public static class Parts
    {
      public const string Panel = "PART_Panel";
    }

    private const double ChuckSize = 384D;

    private readonly Dictionary<Position, UniformGrid> _chunkGridMapping = new Dictionary<Position, UniformGrid>();

    static SurfaceControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SurfaceControl), new FrameworkPropertyMetadata(typeof(SurfaceControl)));

    private Grid ItemsPanel
    {
      get;
      set;
    }

    public static DependencyProperty SurfaceProperty
    {
      get;
      // ReSharper disable once UnusedAutoPropertyAccessor.Local
      private set;
    }

    [DependencyProperty]
    public Surface Surface
    {
      get;
      set;
    }

    private void OnSurfaceChanged() => RegenerateTiles();

    public override void OnApplyTemplate() => ItemsPanel = (Grid) GetTemplateChild(Parts.Panel);

    [AttachedProperty]
    public static Attached<bool> BlockState
    {
      get;
      set;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      if (e.LeftButton == MouseButtonState.Pressed)
      {
        for (var current = e.MouseDevice.Target as DependencyObject; current != null; current = VisualTreeHelper.GetParent(current))
        {
          if (current is TileControl tileControl)
          {
            BlockState[this] = !tileControl.IsBlocked;

            break;
          }
        }
      }
    }

    private void RegenerateTiles()
    {
      if (ItemsPanel != null)
      {
        ItemsPanel.Children.Clear();
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
            while (ItemsPanel.ColumnDefinitions.Count < chunkPosition.X + 1)
            {
              ItemsPanel.ColumnDefinitions.Add(new ColumnDefinition
              {
                Width = new GridLength(ChuckSize)
              });
            }

            while (ItemsPanel.RowDefinitions.Count < chunkPosition.Y + 1)
            {
              ItemsPanel.RowDefinitions.Add(new RowDefinition
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

            ItemsPanel.Children.Add(chunkGrid);
          }

          chunkGrid.Children.Add(tileControl);
        }
      }
    }
  }
}
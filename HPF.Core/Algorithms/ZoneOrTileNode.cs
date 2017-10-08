using System.Collections.Generic;
using System.Linq;

using HPF.Models;

namespace HPF.Algorithms
{
  public class ZoneOrTileNode //: IHierarchicNode<ZoneOrTileNode, TileNode>
  {
    private const uint AreaSize = Surface.ChunkSize / 2U;
    
    private struct Cardinal<T>
    {
      public readonly T North;
      public readonly T East;
      public readonly T South;
      public readonly T West;

      public Cardinal(T north, T east, T south, T west)
      {
        North = north;
        East = east;
        South = south;
        West = west;
      }
    }
    
    public static ZoneOrTileNode Construct(Surface surface)
    {
      var clusters = new Dictionary<Position, Area>();
      var borders = new List<Area>();
      
      foreach (var chunk in surface.GetChunks())
      {
        var position = chunk * Surface.ChunkSize;
        
        for (var offsetY = 0U; offsetY < Surface.ChunkSize; offsetY += AreaSize)
        {
          for (var offsetX = 0U; offsetX < Surface.ChunkSize; offsetX += AreaSize)
          {
            var areaPosition = position + (offsetX, offsetY);
            
            clusters.Add(areaPosition, new Area(areaPosition, new Size(AreaSize, AreaSize)));
          }
        }
      }

      var navigator = new ZoneOrTileNode(null)
      {
        ZoneList = clusters.Values.ToArray()
      };

      return navigator;
    }

    public IReadOnlyList<Area> ZoneList
    {
      get;
      private set;
    }

    //IReadOnlyList<ZoneOrTileNode> IHierarchicNode<ZoneOrTileNode, TileNode>.Children => Children;

    public List<ZoneOrTileNode> Children
    {
      get;
    } = new List<ZoneOrTileNode>();

    //IHierarchicNode<ZoneOrTileNode> IHierarchicNode<ZoneOrTileNode>.Parent => Parent;

    public ZoneOrTileNode Parent
    {
      get;
    }

    public List<ZoneOrTileNode> Siblings
    {
      get;
    } = new List<ZoneOrTileNode>();

    public ZoneOrTileNode(ZoneOrTileNode parent)
    {
      Parent = parent;
    }

    public bool Equals(ZoneOrTileNode other) => ReferenceEquals(this, other);

    public IEnumerable<ZoneOrTileNode> Expand()
    {
      yield break;
    }
  }
}
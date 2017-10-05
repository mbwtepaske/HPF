using System;
using System.Collections.Generic;
using System.Diagnostics;

using PostSharp.Patterns.Model;

namespace HPF.Models
{
  [NotifyPropertyChanged]
  [Serializable]
  public class Tile : IEquatable<Tile>
  {
    public readonly Position Position;

    public TileMode Mode
    {
      get;
      set;
    }

    public Tile(Position position) => Position = position;

    public bool Equals(Tile other) => ReferenceEquals(this, other) || other != null && Position.Equals(other.Position) && Mode == other.Mode;

    public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Tile other && Equals(other);

    public override int GetHashCode() => Position.GetHashCode();

    public IEnumerable<Tile> GetNeighbours(Surface surface, bool allowDiagonal = true)
    {
      for (int index = 0, count = allowDiagonal ? 8 : 4; index < count; index++)
      {
        var nextTile = default(Tile);
        
        switch (index)
        {
          case 0: nextTile = surface[Position.X + 1, Position.Y + 0]; break;
          case 1: nextTile = surface[Position.X + 0, Position.Y + 1]; break;
          case 2: nextTile = surface[Position.X - 1, Position.Y + 0]; break;
          case 3: nextTile = surface[Position.X + 0, Position.Y - 1]; break;
          case 4: nextTile = surface[Position.X + 1, Position.Y - 1]; break;
          case 5: nextTile = surface[Position.X + 1, Position.Y + 1]; break;
          case 6: nextTile = surface[Position.X - 1, Position.Y + 1]; break;
          case 7: nextTile = surface[Position.X - 0, Position.Y - 1]; break;
        }

        if (nextTile != null)
        {
          yield return nextTile;
        }
      }
    }

    public override string ToString() => $"[{Position}]: Mode = {Mode}";
  }
}
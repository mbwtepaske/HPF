using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HPF.Algorithms
{
  using Models;
  
  [DebuggerDisplay("{" + nameof(Tile) + ",nq}")]
  public sealed class TileNavigator : INode<TileNavigator>, IEquatable<TileNavigator>
  {
    public static AStar<TileNavigator> AStar(Surface surface, Position start, Position finish)
      => (AStar<TileNavigator>) new AStar<TileNavigator>((source, target) => Position.Distance(source.Tile.Position, target.Tile.Position))
      .Setup(new TileNavigator(surface, surface[start]), new TileNavigator(surface, surface[finish]));

    public bool AllowDiagonals
    {
      get;
    }

    public Surface Surface
    {
      get;
    }

    public Tile Tile
    {
      get;
    }

    public TileNavigator(Surface surface, Tile tile, bool allowDiagonals = true)
    {
      AllowDiagonals = allowDiagonals;
      Surface = surface;
      Tile = tile;
    }
    
    public IEnumerable<TileNavigator> Expand() => Tile
      .GetNeighbours(Surface, AllowDiagonals)
      .Where(tile => tile.Mode != TileMode.Blocked)
      .Select(tile => new TileNavigator(Surface, tile, AllowDiagonals));

    public bool Equals(TileNavigator other) => other != null && Tile.Equals(other.Tile);
    
    public bool Equals(INode<TileNavigator> other) => other is TileNavigator otherTileNavigator && Tile.Equals(otherTileNavigator.Tile);

    public override bool Equals(object obj) => obj is TileNavigator other && Tile.Equals(other.Tile);

    public override int GetHashCode() => Tile.GetHashCode();

    public override string ToString() => $"Tile: {Tile}, Surface: {Surface}, Allow Diagonals: {AllowDiagonals}";
  }
}
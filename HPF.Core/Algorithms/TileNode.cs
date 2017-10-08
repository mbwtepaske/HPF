using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace HPF.Algorithms
{
  using Models;
  
  [DebuggerDisplay("{" + nameof(Tile) + ",nq}")]
  public sealed class TileNode : INode<TileNode>
  {
    public static AStar<TileNode> AStar(Surface surface, Position start, Position finish)
      => (AStar<TileNode>) new AStar<TileNode>((source, target) => Position.Distance(source.Tile.Position, target.Tile.Position))
      .Setup(new TileNode(surface, surface[start]), new TileNode(surface, surface[finish]));

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

    public TileNode(Surface surface, Tile tile, bool allowDiagonals = true)
    {
      AllowDiagonals = allowDiagonals;
      Surface = surface;
      Tile = tile;
    }
    
    public IEnumerable<TileNode> Expand() => Tile
      .GetNeighbours(Surface, AllowDiagonals)
      .Where(tile => tile.Mode != TileMode.Blocked)
      .Select(tile => new TileNode(Surface, tile, AllowDiagonals));

    public bool Equals(TileNode other) => other != null && Tile.Equals(other.Tile);
    
    public override bool Equals(object obj) => obj is TileNode other && Tile.Equals(other.Tile);

    public override int GetHashCode() => Tile.GetHashCode();

    public override string ToString() => $"Tile: {Tile}, Surface: {Surface}, Allow Diagonals: {AllowDiagonals}";
  }
}
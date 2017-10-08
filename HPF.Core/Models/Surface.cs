using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Model;

namespace HPF.Models
{
  [NotifyPropertyChanged]
  [Serializable]
  public class Surface : IEnumerable<Tile>
  {
    [Serializable]
    private class ChunkPositionComparer : IComparer<Position>
    {
      public int Compare(Position x, Position y) => (((ulong) x.X << 32) + (ulong) x.Y).CompareTo(((ulong) y.X << 32) + (ulong) y.Y);
    }

    public const uint ChunkSize = 16;

    private readonly IDictionary<Position, Tile[]> _tileChunkMapping = new SortedDictionary<Position, Tile[]>(new ChunkPositionComparer());

    public Position? GoalPosition
    {
      get;
      set;
    }

    public Position? StartPosition
    {
      get;
      set;
    }

    public Tile this[uint x, uint y] => _tileChunkMapping.TryGetValue((x / ChunkSize, y / ChunkSize), out var tiles)
      ? tiles[x % ChunkSize + y % ChunkSize * ChunkSize]
      : null;

    public Tile this[Position position] => this[position.X, position.Y];
    
    public void GenerateChunk(Position position, Action<Tile> visitor = null)
    {
      var offset = (X: position.X * ChunkSize, Y: position.Y * ChunkSize);
      var tiles = new Tile[ChunkSize * ChunkSize];

      for (var y = 0U; y < ChunkSize; y++)
      {
        for (var x = 0U; x < ChunkSize; x++)
        {
          var tile = new Tile((x + offset.X, y + offset.Y));

          visitor?.Invoke(tile);

          tiles[x + y * ChunkSize] = tile;
        }
      }

      _tileChunkMapping.Add(position, tiles);

      NotifyPropertyChangedServices.SignalPropertyChanged(this, "Item");
    }

    public IEnumerable<Position> GetChunks() => _tileChunkMapping.Keys;

    public IEnumerator<Tile> GetEnumerator()
    {
      foreach (var tiles in _tileChunkMapping.Values)
      {
        for (var index = 0; index < tiles.Length; index++)
        {
          yield return tiles[index];
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
      var maxX = 0U;
      var maxY = 0U;
      var minX = 0U;
      var minY = 0U;

      foreach (var position in _tileChunkMapping.Keys)
      {
        maxX = Math.Max(maxX, position.X);
        maxY = Math.Max(maxY, position.Y);
        minX = Math.Max(minX, position.X);
        minY = Math.Max(minY, position.Y);
      }
      
      return $"Chunks = {_tileChunkMapping.Count}, Size: [{(maxX - minX) * ChunkSize},{(maxY - minY) * ChunkSize}]";
    }

    #region Serialization

    private static readonly IFormatter Formatter = new BinaryFormatter();

    public static Surface Load(string filePath)
    {
      using (var fileStream = File.OpenRead(filePath))
      {
        return Load(fileStream);
      }
    }

    public static Surface Load(Stream stream) => (Surface) Formatter.Deserialize(stream);

    public void Save(string filePath)
    {
      using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
      {
        Save(fileStream);
      }
    }

    public void Save(Stream stream) => Formatter.Serialize(stream, this);

    #endregion
  }
}
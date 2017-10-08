using System;

namespace HPF.Models
{
  public struct Area : IEquatable<Area>
  {
    public readonly Position Position;
    public readonly Size Size;

    public (double X, double Y) Center => (Position.X + Size.SizeX * 0.5D, Position.Y + Size.SizeY * 0.5D);

    public Area(Position position, Size size)
    {
      Position = position;
      Size = size;
    }

    public bool Equals(Area other) => Position.Equals(other.Position) && Size.Equals(other.Size);

    public override bool Equals(object obj) => obj is Area other && Equals(other);

    public override int GetHashCode() => Position.GetHashCode() ^ Size.GetHashCode();

    public override string ToString() => $"Position: {Position}, Size: {Size}";
  }
}
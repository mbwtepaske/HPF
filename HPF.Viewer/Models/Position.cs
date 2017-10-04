using System;
using System.Diagnostics;

namespace HPF.Viewer.Models
{
  [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
  [Serializable]
  public struct Position : IEquatable<Position>, IComparable<Position>
  {
    public readonly uint X;
    public readonly uint Y;

    public Position(uint x, uint y)
    {
      X = x;
      Y = y;
    }

    public void Deconstruct(out uint x, out uint y)
    {
      x = X;
      y = Y;
    }

    public bool Equals(Position other)
      => X == other.X && Y == other.Y;

    public override bool Equals(object obj)
      => !ReferenceEquals(null, obj) && obj is Position other && Equals(other);

    public override int GetHashCode() => unchecked((int) (X ^ Y));

    public static implicit operator Position((uint x, uint y) tuple) => new Position(tuple.x, tuple.y);

    public static Position operator +(Position lhs, Position rhs) => new Position(lhs.X + rhs.X, lhs.Y + rhs.Y);

    public static Position operator -(Position lhs, Position rhs) => new Position(lhs.X - rhs.X, lhs.Y - rhs.Y);

    public static Position operator *(Position lhs, uint rhs) => new Position(lhs.X * rhs, lhs.Y * rhs);

    public static Position operator /(Position lhs, uint rhs) => new Position(lhs.X / rhs, lhs.Y / rhs);

    public int CompareTo(Position other)
    {
      var comparison = X.CompareTo(other.X);

      return comparison == 0 ? Y.CompareTo(other.Y) : comparison;
    }

    public override string ToString() => $"[{X},{Y}]";
  }
}
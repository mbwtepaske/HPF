using System;
using System.Diagnostics;

namespace HPF.Models
{
  [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
  [Serializable]
  public struct Size : IEquatable<Size>
  {
    public readonly uint SizeX;
    public readonly uint SizeY;

    public Size(uint sizeX, uint sizeY)
    {
      SizeX = sizeX;
      SizeY = sizeY;
    }

    public bool Equals(Size other) => SizeX == other.SizeX && SizeY == other.SizeY;

    public override bool Equals(object obj) => obj is Size other && Equals(other);

    public override int GetHashCode() => unchecked ((int) ( SizeX ^ SizeY ));

    public override string ToString() => $"[{SizeX},{SizeX}]";
  }
}
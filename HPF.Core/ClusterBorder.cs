using System;

namespace HPF
{
  public class ClusterBorder : IEquatable<ClusterBorder>
  {
    public readonly int Offset;
    public readonly int Length;
    public readonly Orientation Orientation;
    public readonly (Cluster, Cluster) Pair;

    public ClusterBorder(Orientation orientation, (Cluster, Cluster) pair, int offset, int length)
    {
      Orientation = orientation;
      Pair = pair;
      Offset = offset;
      Length = length;
    }

    public bool Equals(ClusterBorder other)
      => ReferenceEquals(this, other)
      || other != null
      && Orientation == other.Orientation
      && Offset == other.Offset
      && Length == other.Length
      && Pair.Item1.Equals(other.Pair.Item1)
      && Pair.Item2.Equals(other.Pair.Item2);
  }
}
using System;
using System.Diagnostics;

using PostSharp.Patterns.Model;

namespace HPF.Viewer.Models
{
  [DebuggerDisplay("{" + nameof(Position) + "}: Blocked = {" + nameof(Blocked) + "}")]
  [NotifyPropertyChanged]
  [Serializable]
  public class Tile
  {
    public readonly Position Position;

    public bool Blocked
    {
      get;
      set;
    }

    public Tile(Position position) => Position = position;
  }
}
using System;
using System.Collections.Generic;

namespace HPF.Algorithms
{
  public interface INode<TNode> : IEquatable<INode<TNode>>
    where TNode : INode<TNode>
  {
    IEnumerable<TNode> Expand();
  }
}
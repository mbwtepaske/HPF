using System.Collections.Generic;

namespace HPF.Algorithms
{
  public interface IPathFinder<TNode> where TNode : INode<TNode>
  {
    TNode Destination
    {
      get;
    }

    TNode Source
    {
      get;
    }

    PathFinderState State
    {
      get;
    }

    IReadOnlyList<TNode> Result
    {
      get;
    }

    IPathFinder<TNode> Setup(TNode source, TNode destination);
    
    void Step();
  }
}
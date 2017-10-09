using System;
using System.Collections.Generic;
using System.Linq;

namespace HPF
{
  public abstract class Graph<TNode, TEdge>
    where TNode : GraphNode
    where TEdge : GraphEdge<TNode>
  {
    protected Graph()
    {
    }
    
    public IEnumerable<TNode> GetNodes() => null;
  }
}
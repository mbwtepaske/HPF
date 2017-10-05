using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HPF.Algorithms
{
  public static class IPathFinderExtensions
  {
    public static IEnumerable<TNode> GetPath<TNode>(this IPathFinder<TNode> pathFinder) where TNode : INode<TNode>
    {
      var stopwatch = Stopwatch.StartNew();
      
      while (pathFinder.State == PathFinderState.Running)
      {
        pathFinder.Step();
      }

      stopwatch.Stop();
      
      Debug.WriteLine($"Time: {stopwatch.Elapsed.TotalMilliseconds:F5} ms");
      
      return pathFinder.State == PathFinderState.Finished
        ? pathFinder.Result.Reverse()
        : Array.Empty<TNode>();
    }
  }
}

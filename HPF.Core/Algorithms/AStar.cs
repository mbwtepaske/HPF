using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using PostSharp.Patterns.Contracts;

namespace HPF.Algorithms
{
  public class AStar<TNode> : BasePathFinder<TNode> where TNode : INode<TNode>
  {
    public delegate double DistanceFunction(TNode source, TNode target);
    
    [DebuggerDisplay("{" + nameof(ToDebuggerString) + "(),nq}")]
    protected class PriorityQueue<TValue>
    {
      private readonly SortedDictionary<double, Queue<TValue>> _dictionary = new SortedDictionary<double, Queue<TValue>>();

      public bool IsEmpty => _dictionary.Count == 0;
      
      public void Enqueue(double priority, TValue value)
      {
        if (!_dictionary.TryGetValue(priority, out var queue))
        {
          _dictionary.Add(priority, queue = new Queue<TValue>());
        }
        
        queue.Enqueue(value);
      }

      public void Clear() => _dictionary.Clear();

      public TValue Dequeue()
      {
        var pair = _dictionary.First();
        var priority = pair.Key;
        var queue = pair.Value;
        var value = queue.Dequeue();

        if (queue.Count == 0)
        {
          _dictionary.Remove(priority);
        }

        return value;
      }

      public string ToDebuggerString() => new StringBuilder()
        .Append($"Count = {_dictionary.Count} ")
        .Append($"Priority Range: [{_dictionary.Keys.First():G3} .. {_dictionary.Keys.Last():G3}] ")
        .Append($"T: {_dictionary.First().Value} ")
        .Append($"B: {_dictionary.Last().Value} ")
        .ToString();
    }

    [DebuggerDisplay("{" + nameof(ToDebuggerString) + "(),nq}")]
    protected class Path : IEnumerable<TNode>
    {
      public Path History
      {
        get;
      }

      public TNode Last
      {
        get;
      }

      public double TotalCost
      {
        get;
      }

      private Path(TNode last, Path history, double totalCost)
      {
        History = history;
        Last = last;
        TotalCost = totalCost;
      }

      public Path(TNode start) : this(start, null, 0)
      {
      }

      public Path Add(TNode tile, double cost) => new Path(tile, this, TotalCost + cost);

      public IEnumerator<TNode> GetEnumerator()
      {
        for (var current = this; current != null; current = current.History)
        {
          yield return current.Last;
        }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public string ToDebuggerString()
      {
        var historyLength = 0;

        for (var current = this; current != null; current = current.History)
        {
          historyLength++;
        }

        return $"Last = {Last}, History = {historyLength}";
      }
    }

    private readonly DistanceFunction _distanceFunction;
    private readonly HashSet<TNode> _closeList = new HashSet<TNode>();
    private readonly PriorityQueue<Path> _openList = new PriorityQueue<Path>();
    
    public AStar([NotNull] DistanceFunction distanceFunction) => _distanceFunction = distanceFunction;
    
    public override IPathFinder<TNode> Setup(TNode source, TNode destination)
    {
      _closeList.Clear();
      _openList.Clear();
      _openList.Enqueue(0D, new Path(source));

      return base.Setup(source, destination);
    }

    protected override PathFinderState StepCore()
    {
      if (_openList.IsEmpty)
      {
        return Abort();
      }

      var path = _openList.Dequeue();
      var current = path.Last;

      if (_closeList.Contains(current))
      {
        return Continue();
      }

      if (current.Equals(Destination))
      {
        return Finish(path);
      }

      _closeList.Add(current);

      foreach (var neighbour in current.Expand())
      {
        var newPath = path.Add(neighbour, _distanceFunction(current, neighbour));

        _openList.Enqueue(newPath.TotalCost + _distanceFunction(neighbour, Destination) * 0.70710678118655, newPath);
      }

      return Continue();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using HPF.Models;

using PostSharp.Patterns.Contracts;

namespace HPF.Algorithms
{
  public abstract class BasePathFinder<TNode> : IPathFinder<TNode> where TNode : INode<TNode>
  {
    public TNode Destination
    {
      get;
      protected set;
    }
    
    public TNode Source
    {
      get;
      protected set;
    }

    public PathFinderState State
    {
      get;
      private set;
    }
    
    public IReadOnlyList<TNode> Result
    {
      get;
      private set;
    }

    protected PathFinderState Abort() => PathFinderState.Aborted;
    
    protected PathFinderState Continue() => PathFinderState.Running;
    
    protected PathFinderState Finish([NotNull] IEnumerable<TNode> tiles)
    {
      Result = tiles.ToImmutableArray();

      return PathFinderState.Finished;
    }

    public virtual void Reset() => Result = Array.Empty<TNode>();

    public virtual IPathFinder<TNode> Setup([NotNull] TNode source, [NotNull] TNode destination)
    {
      Destination = destination;
      Source = source;
      State = PathFinderState.Running;

      return this;
    }

    [DebuggerStepThrough]
    public void Step()
    {
      var oldState = State;

      State = oldState == PathFinderState.Running
        ? StepCore()
        : oldState;
    }

    protected abstract PathFinderState StepCore();
  }
}
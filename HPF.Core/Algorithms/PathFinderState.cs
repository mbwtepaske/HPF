namespace HPF.Algorithms
{
  public enum PathFinderState
  {
    /// <summary>
    /// None of the other states.
    /// </summary>
    None,

    /// <summary>
    /// The <see cref="IPathFinder{TNode}"/> has stopped.
    /// </summary>
    Aborted,

    /// <summary>
    /// The <see cref="IPathFinder{TNode}"/> has ran to completion. The <see cref="IPathFinder{TNode}.Result"/> now has a valid value.
    /// </summary>
    Finished,

    /// <summary>
    /// The <see cref="IPathFinder{TNode}"/> is currently running.
    /// Continue calling the <see cref="IPathFinder{TNode}.Step"/>-method until <see cref="IPathFinder{TNode}.State"/> returns a different value.
    /// </summary>
    Running,
  }
}
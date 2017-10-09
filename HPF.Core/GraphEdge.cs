namespace HPF
{
  public class GraphEdge<TNode> where TNode : GraphNode
  {
    public readonly TNode Source;
    public readonly TNode Target;

    public GraphEdge(TNode source, TNode target)
    {
      Source = source;
      Target = target;
    }
  }
}
using System.Collections.Generic;

namespace HPF.Algorithms
{
  public interface IHierarchicNode<out TAbstractNode, TConcreteNode> : INode<TConcreteNode>
    where TAbstractNode : IHierarchicNode<TAbstractNode, TConcreteNode>
    where TConcreteNode : INode<TConcreteNode>
  {
    IReadOnlyList<TAbstractNode> Children
    {
      get;
    }

    IHierarchicNode<TAbstractNode, TConcreteNode> Parent
    {
      get;
    }
  }
}
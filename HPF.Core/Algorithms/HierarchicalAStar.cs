using System;

namespace HPF
{
  using Algorithms;
  
  public class HierarchicalAStar<TAbstractNode, TConcreteNode>
    where TAbstractNode : IHierarchicNode<TAbstractNode, TConcreteNode>
    where TConcreteNode : INode<TConcreteNode>
  {
    private readonly AStar<TConcreteNode> _pathFinder;
    
    public HierarchicalAStar(AStar<TConcreteNode>.DistanceFunction distanceFunction) => _pathFinder = new AStar<TConcreteNode>(distanceFunction);
    
  }
}
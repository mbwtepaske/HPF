using System;

namespace HPF
{
  public class Cluster : IEquatable<Cluster>
  {
    public readonly ConcreteMap ConcreteMap;

    public Cluster(ConcreteMap concreteMap) => ConcreteMap = concreteMap;

    public bool Equals(Cluster other)
    {
      throw new NotImplementedException();
    }
  }
}
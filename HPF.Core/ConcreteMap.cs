using System;
using System.Collections.Generic;
using System.Linq;

namespace HPF
{
  using Models;

  public class ConcreteMap
  {
    public Surface Surface
    {
      get;
    }

    public ConcreteMap(Surface surface) => Surface = surface;
  }
}
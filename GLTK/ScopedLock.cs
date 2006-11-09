using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GLTK
{
  public class ScopedLock : IDisposable
  {
    private ScopedLock(object xiObject)
    {
      mObject = xiObject;
    }

    public static ScopedLock Lock(Object xiObject)
    {
      Monitor.Enter(xiObject);
      return new ScopedLock(xiObject);
    }

    public void Dispose()
    {
      Monitor.Exit(mObject);
      mObject = null;
    }

    private object mObject;
  }
}

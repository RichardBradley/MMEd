using System;

namespace GLTK
{
  public class DeviceContextEventArgs : EventArgs
  {
    public DeviceContextEventArgs(IntPtr xiContext)
    {
      mDeviceContext = xiContext;
    }

    public IntPtr DeviceContext
    {
      get { return mDeviceContext; }
    }

    private IntPtr mDeviceContext = IntPtr.Zero;
  }
}
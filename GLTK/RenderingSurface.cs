using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace GLTK
{
  public class RenderingSurface : Control
  {
    public RenderingSurface()
    {
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.Opaque, true);
      this.SetStyle(ControlStyles.UserPaint, true);
      this.SetStyle(ControlStyles.ResizeRedraw, true);

      try
      {
        Gdi.PIXELFORMATDESCRIPTOR lPixelFormatDescription = GetPixelFormat();

        mDeviceContext = User.GetDC(Handle);
        if (mDeviceContext == IntPtr.Zero)
        {
          throw new Exception("Could not get device context");
        }

        int lPixelFormat = Gdi.ChoosePixelFormat(mDeviceContext, ref lPixelFormatDescription);
        if (Gdi.ChoosePixelFormat(mDeviceContext, ref lPixelFormatDescription) == 0)
        {
          throw new Exception("Could not find pixel format");
        }

        if (!Gdi.SetPixelFormat(mDeviceContext, lPixelFormat, ref lPixelFormatDescription))
        {
          throw new Exception("Could not set pixel format");
        }
      }
      catch
      {
        Release();
      }
    }

    protected override void OnParentChanged(EventArgs e)
    {
      base.OnParentChanged(e);
      Form lForm = FindForm();
      if (lForm != null)
      {
        lForm.FormClosing += new FormClosingEventHandler(RenderingSurface_FormClosing);
      }
    }

     protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Gdi.SwapBuffers(mDeviceContext);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
    }

    void RenderingSurface_FormClosing(object sender, FormClosingEventArgs e)
    {
      Release();
    }

    public IntPtr DeviceContext
    {
      get
      {
        if (mDeviceContext == IntPtr.Zero)
        {
          throw new InvalidOperationException("Device context not ready");
        }
        return mDeviceContext;
      }
    }

    public void Release()
    {
      if (mDeviceContext != IntPtr.Zero)
      {
        try
        {
          if (ReleaseDeviceContext != null)
          {
            ReleaseDeviceContext(this, new DeviceContextEventArgs(mDeviceContext));
          }

          if (!IsDisposed && Handle != IntPtr.Zero)
          {
            if (!User.ReleaseDC(Handle, mDeviceContext))
            {
              throw new Exception("Could not release device context");
            }
          }
        }
        catch
        {
        }
        finally
        {
          mDeviceContext = IntPtr.Zero;
        }
      }
    }

    protected Gdi.PIXELFORMATDESCRIPTOR GetPixelFormat()
    {
      Gdi.PIXELFORMATDESCRIPTOR lRet = new Gdi.PIXELFORMATDESCRIPTOR();
      lRet.nSize = (short)Marshal.SizeOf(lRet);
      lRet.nVersion = 1;                       
      lRet.dwFlags = Gdi.PFD_DRAW_TO_WINDOW |  
          Gdi.PFD_SUPPORT_OPENGL |             
          Gdi.PFD_DOUBLEBUFFER;                
      lRet.iPixelType = (byte)Gdi.PFD_TYPE_RGBA;
      lRet.cColorBits = 16;            
      lRet.cRedBits = 0;                        
      lRet.cRedShift = 0;
      lRet.cGreenBits = 0;
      lRet.cGreenShift = 0;
      lRet.cBlueBits = 0;
      lRet.cBlueShift = 0;
      lRet.cAlphaBits = 0;                      
      lRet.cAlphaShift = 0;                     
      lRet.cAccumBits = 0;                      
      lRet.cAccumRedBits = 0;                   
      lRet.cAccumGreenBits = 0;
      lRet.cAccumBlueBits = 0;
      lRet.cAccumAlphaBits = 0;
      lRet.cDepthBits = 16;                     
      lRet.cStencilBits = 0;                    
      lRet.cAuxBuffers = 0;                      
      lRet.iLayerType = (byte)Gdi.PFD_MAIN_PLANE;
      lRet.bReserved = 0;                        
      lRet.dwLayerMask = 0;                      
      lRet.dwVisibleMask = 0;
      lRet.dwDamageMask = 0;

      return lRet;
    }

    public delegate void DeviceContextEventHandler(object xiSender, DeviceContextEventArgs xiArgs);
    public event DeviceContextEventHandler ReleaseDeviceContext;

    private IntPtr mDeviceContext;
  }
}

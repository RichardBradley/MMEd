using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;

namespace GLTK
{
  public class ImmediateModeRenderer : AbstractRenderer
  {
    protected override void RenderMeshInternal(Mesh xiMesh, RenderOptions xiOptions)
    {
      using (ScopedLock lLock = LockContext())
      {
        RenderMode lMode = DefaultRenderMode;
        if (FixedRenderMode != RenderMode.Undefined)
        {
          lMode = FixedRenderMode;
        }
        else if (xiMesh.RenderMode != RenderMode.Undefined)
        {
          lMode = xiMesh.RenderMode;
        }

        switch (lMode)
        {
          case RenderMode.Points:
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_POINT);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            break;

          case RenderMode.Wireframe:
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            break;

          case RenderMode.Filled:
          case RenderMode.TranslucentFilled:
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            break;

          default:
          case RenderMode.Textured:
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, xiMesh.Texture);
            break;
        }

        switch (xiMesh.PolygonMode)
        {
          case PolygonMode.Lines:
            Gl.glBegin(Gl.GL_LINES);
            break;
          case PolygonMode.Triangles:
            Gl.glBegin(Gl.GL_TRIANGLES);
            break;
          case PolygonMode.Quads:
            Gl.glBegin(Gl.GL_QUADS);
            break;
          default:
            throw new Exception("Unrecognised polygon type");
        }

        foreach (Vertex lVertex in xiMesh.Vertices)
        {
          if (lMode == RenderMode.TranslucentFilled)
          {
            Gl.glColor4d(
              (double)lVertex.Color.R / 255,
              (double)lVertex.Color.G / 255,
              (double)lVertex.Color.B / 255,
              (double)lVertex.Color.A / 255);
          }
          else
          {
            Gl.glColor3d(
              (double)lVertex.Color.R / 255,
              (double)lVertex.Color.G / 255,
              (double)lVertex.Color.B / 255);
          }

          Gl.glNormal3d(lVertex.Normal.x, lVertex.Normal.y, lVertex.Normal.z);
          Gl.glTexCoord2d(lVertex.TexCoordX, lVertex.TexCoordY);
          Gl.glVertex3d(lVertex.Position.x, lVertex.Position.y, lVertex.Position.z);
        }

        Gl.glEnd();

        if ((xiOptions & RenderOptions.ShowNormals) == RenderOptions.ShowNormals)
        {
          Gl.glBegin(Gl.GL_LINES);

          Gl.glColor3d(0.0, 0.0, 1.0);

          foreach (Vertex lVertex in xiMesh.Vertices)
          {
            Gl.glVertex3d(lVertex.Position.x, lVertex.Position.y, lVertex.Position.z);
            Point lNormalEnd = lVertex.Position + 25 * lVertex.Normal;
            Gl.glVertex3d(lNormalEnd.x, lNormalEnd.y, lNormalEnd.z);
          }

          Gl.glEnd();
        }
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;

namespace GLTK
{
  public class ImmediateModeRenderer : AbstractRenderer
  {
    public override void RenderMesh(Mesh xiMesh)
    {
      switch (xiMesh.RenderMode)
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
          Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
          Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
          break;

        default:
        case RenderMode.Textured:
          Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
          Gl.glBindTexture(Gl.GL_TEXTURE_2D, xiMesh.Texture);
          break;
      }

      Gl.glBegin(Gl.GL_TRIANGLES);
      
      foreach (Vertex lVertex in xiMesh.Vertices)
      {
        Gl.glColor3d(
          (double)lVertex.Color.R / 255, 
          (double)lVertex.Color.G / 255, 
          (double)lVertex.Color.B / 255);
        Gl.glNormal3d(lVertex.Normal.x, lVertex.Normal.y, lVertex.Normal.z);
        Gl.glTexCoord2d(lVertex.TexCoordX, lVertex.TexCoordY);
        Gl.glVertex3d(lVertex.Position.x, lVertex.Position.y, lVertex.Position.z);
      }

      Gl.glEnd();
    }
  }
}

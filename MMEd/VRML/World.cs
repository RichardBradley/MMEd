using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using GLTK;
using MMEd.Chunks;
using MMEd.Viewers;
using MMEd.Viewers.ThreeDee;

namespace MMEd.VRML
{
  class World
  {
    private Scene mScene;

    public World(Scene scene)
    {
      mScene = scene;
    }

    public void Serialize(string filePath, ThreeDeeViewer threeDeeViewer, MainForm mMainForm)
    {
      using (TextWriter stream = new StreamWriter(filePath))
      {
        stream.WriteLine("#VRML V2.0 utf8");
        foreach (Entity entity in mScene.Entities)
        {
          Matrix localTransf = entity.Transform;

          Vector translation;
          Vector rotationAxis;
          double rotationAmount;
          Vector scale;
          localTransf.Decompose(out translation, out rotationAxis, out rotationAmount, out scale);

          stream.WriteLine("Transform {");
          stream.WriteLine("  rotation {0} {1} {2} {3}", rotationAxis[0], rotationAxis[1], rotationAxis[2], rotationAmount);
          stream.WriteLine("  scale {0} {1} {2}", scale[0], scale[1], scale[2]);
          stream.WriteLine("  translation {0} {1} {2}", translation[0], translation[1], translation[2]);

          stream.WriteLine("  children [");

          foreach (Mesh mesh in entity.Meshes)
          {
            stream.WriteLine("    Shape {");
            stream.WriteLine("      geometry IndexedFaceSet {");
            stream.WriteLine("        coord Coordinate {");
            stream.WriteLine("          point [");
            foreach (Vertex v in mesh.Vertices)
            {
              stream.WriteLine("            {0} {1} {2},", v.Position.x, v.Position.y, v.Position.z);
            }
            stream.WriteLine("          ]");
            stream.WriteLine("        }");
            stream.WriteLine("        coordIndex [");
            writeCoordIndexAscending(stream, mesh);
            stream.WriteLine("        ]");

            if (mesh.RenderMode == RenderMode.Textured)
            {
              stream.WriteLine("        texCoord TextureCoordinate {");
              stream.WriteLine("          point [");
              foreach (Vertex v in mesh.Vertices)
              {
                stream.WriteLine("            {0} {1},", v.TexCoordX, 1 - v.TexCoordY);
              }
              stream.WriteLine("          ]");
              stream.WriteLine("        }");
              stream.WriteLine("        texCoordIndex [");
              writeCoordIndexAscending(stream, mesh);
              stream.WriteLine("        ]");
            }

            stream.WriteLine("        solid TRUE");
            stream.WriteLine("      }");
            stream.WriteLine("      appearance Appearance {");
            stream.WriteLine("        material Material {");
            stream.WriteLine("           diffuseColor 0 1 1");
            stream.WriteLine("        }");
            if (mesh.RenderMode == RenderMode.Textured)
            {
              stream.WriteLine("        texture ImageTexture {");
              stream.WriteLine("           url \"{0}\"", ImageUrl(mesh, filePath).Replace('\\', '/'));
              stream.WriteLine("        }");
            }
            stream.WriteLine("      }");
            stream.WriteLine("    }");
          }
          stream.WriteLine("  ]");
          stream.WriteLine("}");
        }
      }
    }

    private string ImageUrl(Mesh mesh, String filePath)
    {
      string fileDir = Path.GetDirectoryName(filePath);
      string imageDirName = Path.GetFileName(filePath) + "_images";
      string imageDirPath = Path.Combine(fileDir, imageDirName);
      Directory.CreateDirectory(imageDirPath);
      string relUrl = Path.Combine(imageDirName, mesh.Texture + ".png");
      string absUrl = Path.Combine(fileDir, relUrl);
      if (File.Exists(absUrl))
      {
        return relUrl;
      }
      Bitmap bitmap = AbstractRenderer.TextureIdToImage(mesh.Texture);
      bitmap.Save(absUrl);
      return relUrl;
    }

    private void writeCoordIndexAscending(TextWriter stream, Mesh mesh)
    {
      int ii = 0;
      while (ii < mesh.Vertices.Count)
      {
        stream.Write("          ");
        for (int jj = 0; jj < (int)mesh.PolygonMode; jj++)
        {
          stream.Write(ii++);
          stream.Write(" ");
        }
        stream.WriteLine("-1");
      }
    }
  }
}

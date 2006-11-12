using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using MMEd.Util;
using MMEd.Viewers;
using MMEd.Viewers.ThreeDee;
using GLTK;

// Represents a PS TMD object

namespace MMEd.Chunks
{
  public class TMDChunk : Chunk, IEntityProvider
  {
    public enum TMDFlags
    {
      None = 0 //erm, that's all I know :-)  Descibed as "rel. offsets" mode in some places
    }

    public const int OBJT_ID_FOR_MALLET = 0;
    public const int OBJT_ID_FOR_WEAPONS_BOX = 1;

    public const int TMD_MAGIC_NUMBER = 0x41;

    #region public fields

    [Description("Only \"rel offsets\" mode (i.e. no flags) is supported")]
    public TMDFlags Flags;

    [Description("The number of objects in this TMD. Only 1 can be supported atm.")]
    public int ObjectCount;

    [Description(@"The polys which make up the object")]
    public Face[] Faces;

    [Description("The vertices which are used in the object")]
    public Short3Coord[] Vertices;

    [Description("Normal vectors which are used in the object. The magnitude should be 4096")]
    public Short3Coord[] Normals;

    [Description("Not used")]
    public int ObjScale;

    #endregion

    private int mDataLength;

    public override void Deserialise(System.IO.Stream inStr)
    {
      long lStartingOffset = inStr.Position;

      // magic number
      BinaryReader bin = new BinaryReader(inStr);
      int lTMDMagicNumber = bin.ReadInt32();
      if (TMD_MAGIC_NUMBER != lTMDMagicNumber)
      {
        throw new DeserialisationException(string.Format("TMD should start with 4 byte magic number 0x41, found {0:x8}", lTMDMagicNumber), inStr.Position - 4);
      }

      //mode
      int lTMDFlags = bin.ReadInt32();
      if (lTMDFlags != (int)TMDFlags.None)
      {
        throw new DeserialisationException(string.Format("Only TMDs with no flags are supported. Found {0:x8}", lTMDFlags), inStr.Position - 4);
      }

      //obj count
      int lObjectCount = bin.ReadInt32();
      if (lObjectCount != 1)
      {
        throw new DeserialisationException(string.Format("Only TMDs with one object are supported. Found {0}", lObjectCount), inStr.Position - 4);
      }
      ObjectCount = lObjectCount;

      // array pointers
      // (not stored, as they're derivable from the lengths of the arrays)
      int lVertPtr = bin.ReadInt32();
      int lVertCount = bin.ReadInt32();
      int lNormPtr = bin.ReadInt32();
      int lNormCount = bin.ReadInt32();
      int lPrimPtr = bin.ReadInt32();
      int lPrimCount = bin.ReadInt32();

      //obj scale
      ObjScale = bin.ReadInt32();
      if (ObjScale != 0)
      {
        throw new DeserialisationException(string.Format("Only TMDs with ObjScale == 0 are supported. Found {0}", ObjScale), inStr.Position - 4);
      }

      // Faces
      if (lPrimPtr + 12 != inStr.Position - lStartingOffset)
      {
        throw new DeserialisationException(string.Format("Expecting Prim array to begin immediately after TMD header. Found PrimPtr={0}", lPrimPtr), inStr.Position);
      }
      Faces = new Face[lPrimCount];
      for (int i = 0; i < lPrimCount; i++)
      {
        Faces[i] = new Face();
        Faces[i].Deserialise(bin);
      }

      // Verts
      if (lVertPtr + 12 != inStr.Position - lStartingOffset)
      {
        throw new DeserialisationException(string.Format("Expecting Vert array to begin immediately after Prim array. Found VertPtr={0} but expecting", lVertPtr, 12 + inStr.Position - lStartingOffset), inStr.Position);
      }
      Vertices = new Short3Coord[lVertCount];
      for (int i = 0; i < lVertCount; i++)
      {
        Vertices[i] = Short3Coord.ReadShort3Coord64(bin);
      }

      // Norms
      if (lNormPtr + 12 != inStr.Position - lStartingOffset)
      {
        throw new DeserialisationException(string.Format("Expecting Norm array to begin immediately after Vert array. Found NormPtr={0} but expecting", lNormPtr, 12 + inStr.Position - lStartingOffset), inStr.Position);
      }
      Normals = new Short3Coord[lNormCount];
      for (int i = 0; i < lNormCount; i++)
      {
        Normals[i] = Short3Coord.ReadShort3Coord64(bin);
      }

      //record data length
      //qq this restricts editing somewhat
      mDataLength = (int)(inStr.Position - lStartingOffset);
    }

    public override void Serialise(System.IO.Stream outStr)
    {
      Serialise(outStr, new BinaryWriter(outStr));
    }

    public void Serialise(System.IO.Stream outStr, BinaryWriter bout)
    {
      bout.Write((int)TMD_MAGIC_NUMBER);

      bout.Write((int)TMDFlags.None);

      bout.Write((int)1); //obj count

      //write the arrays to a buffer first, so that we
      //can calculate the offsets
      MemoryStream lBuff = new MemoryStream();
      BinaryWriter lBuffBout = new BinaryWriter(lBuff);

      //Faces
      int lPrimPtr = 28 + (int)lBuff.Position; //(always 40)
      int lPrimCount = Faces.Length;
      foreach (Face f in Faces)
      {
        f.Serialise(lBuff);
      }

      //Verts
      int lVertPtr = 28 + (int)lBuff.Position;
      int lVertCount = Vertices.Length;
      foreach (Short3Coord v in Vertices)
      {
        v.WriteShort3Coord64(lBuffBout);
      }

      // Norms
      int lNormPtr = 28 + (int)lBuff.Position;
      int lNormCount = Normals.Length;
      foreach (Short3Coord n in Normals)
      {
        n.WriteShort3Coord64(lBuffBout);
      }

      //now write header, then buffered body
      bout.Write(lVertPtr);
      bout.Write(lVertCount);
      bout.Write(lNormPtr);
      bout.Write(lNormCount);
      bout.Write(lPrimPtr);
      bout.Write(lPrimCount);
      bout.Write(0); //obj scale
      bout.Write(lBuff.ToArray());
    }

    /// <summary>
    ///   The length, in bytes, of this TIM when serialised
    /// </summary>
    public int DataLength
    {
      get
      {
        return mDataLength;
      }
      set
      {
        mDataLength = value;
      }
    }

    public IEnumerable<GLTK.Entity> GetEntities(Level xiLevel, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata)
    {
      return new Entity[] { GetEntity(xiLevel, xiTextureMode, xiSelectedMetadata, this) };
    }

    public Entity GetEntity(Level xiLevel, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata, object xiMeshOwner)
    {
      MMEdEntity lAcc = new MMEdEntity(xiMeshOwner);
      Mesh lColouredMesh = null;
      Mesh lTexturedMesh = null;
      int lTexPageId = int.MinValue;

      //qq move all this into Face class?
      Vertex[] lVBuff = new Vertex[4];
      foreach (Face f in Faces)
      {
        if (!f.IsUnknownType())
        {
          for (int v = 0; v < f.mVertexIds.Length; v++)
          {
            lVBuff[v] = new Vertex(ThreeDeeViewer.Short3CoordToPoint(Vertices[f.mVertexIds[v]]));
            
            //normal
            if (v < f.mNormalIds.Length)
              lVBuff[v].Normal = ThreeDeeViewer.Short3CoordToPoint(Normals[f.mNormalIds[v]]).GetPositionVector().Normalise();
            else lVBuff[v].Normal = ThreeDeeViewer.Short3CoordToPoint(Normals[f.mNormalIds[0]]).GetPositionVector().Normalise();

            //color:
            if (f.mColors != null && v < f.mColors.Length)
              lVBuff[v].Color = Utils.PSRGBColorToColor(f.mColors[v]);
            else if (f.mColors != null)
              lVBuff[v].Color = Utils.PSRGBColorToColor(f.mColors[0]);
            
            //tex coords
            if (f.mTexCoords != null && v < f.mTexCoords.Length)
            {
              lVBuff[v].TexCoordX = f.mTexCoords[v].X / (double)VRAMViewer.TEX_PAGE_WIDTH;
              lVBuff[v].TexCoordY = f.mTexCoords[v].Y / (double)VRAMViewer.TEX_PAGE_HEIGHT;
            }
          }

          //tex or solid?
          Mesh lMesh;
          if (f.mTexCoords != null)
          {
            if (lTexturedMesh == null)
            {
              lTexturedMesh = new OwnedMesh(xiMeshOwner);
              lTexturedMesh.RenderMode = RenderMode.Textured;
              int lThisFacePageId = f.mTSB & 0xff; //qq TODO ren TSB
              if (lTexPageId != int.MinValue && lThisFacePageId != lTexPageId)
              {
                throw new Exception("Object refers to multiple texture pages!");
              }
              lTexPageId = lThisFacePageId;
              lTexturedMesh.Texture = AbstractRenderer.ImageToTextureId(xiLevel.GetTexturePageById(lTexPageId)); 
            }
            lMesh = lTexturedMesh;
          }
          else
          {
            if (lColouredMesh == null)
            {
              lColouredMesh = new OwnedMesh(xiMeshOwner);
              lColouredMesh.RenderMode = RenderMode.Filled;
            }
            lMesh = lColouredMesh;
          }

          if (f.mVertexIds.Length == 3)
          {
            lMesh.AddFace(lVBuff[0], lVBuff[1], lVBuff[2]);
          }
          else //4
          {
            lMesh.AddFace(lVBuff[0], lVBuff[1], lVBuff[2]);
            lMesh.AddFace(lVBuff[0], lVBuff[2], lVBuff[3]);
          }
        }
      }

      if (lTexturedMesh != null) lAcc.Meshes.Add(lTexturedMesh);
      if (lColouredMesh != null) lAcc.Meshes.Add(lColouredMesh);

      lAcc.Scale(1, 1, -1);

      return lAcc;
    }

    public override string Name
    {
      get
      {
        return mIdx >= 0 
          ? string.Format("[{0}] TMD", mIdx) 
          : "TMD";
      }
    }

    int mIdx = -1;
    public TMDChunk() { }
    public TMDChunk(int xiIdx, Stream xiInStr)
    {
      mIdx = xiIdx;
      Deserialise(xiInStr);

      //double-check the assumption that object 1 is a weapons box:
      //this is slightly hacky, but might be justifiable because the PS has the 
      //same assumption
      // TODO: maybe later do a similar check for MALLET
      if (xiIdx == OBJT_ID_FOR_WEAPONS_BOX)
      {
        string lNotWeaponsBoxBecause = null;
        if (Faces.Length != 12) lNotWeaponsBoxBecause = string.Format("it has {0} faces, but should have 12", Faces.Length);
        for (int i=0; i<Faces.Length && lNotWeaponsBoxBecause == null; i++)
        {
          if (Faces[i].mMode != Face.Mode.Tri_Flat_Textured)
          {
            lNotWeaponsBoxBecause = string.Format("face {0} is mode {1} but expected mode {2}",
              i, Faces[i].mMode.ToString(), Face.Mode.Tri_Flat_Textured.ToString());
          }
        }
        if (lNotWeaponsBoxBecause != null)
        {
          System.Windows.Forms.MessageBox.Show(string.Format("The object with index {0} doesn't look like a weapons box because {1}\n"+
            "Please tell rtb about this, because he currently thinks that object 1 is always a weapons box\n"+
            "The 3D view may look a little odd because of this problem",
            OBJT_ID_FOR_WEAPONS_BOX,
            lNotWeaponsBoxBecause));
        }
      }
    }

    public class Face : Chunk
    {
      public enum Flags
      {
        None = 0x0,
        Colour_Per_Vertex = 0x4,
        Unknown2 = 0x2
      }

      // This could be factorised, but I'm wasting ages thinking about how to
      // do it.
      // Let's just brute force it for now.
      // Code duplication helps sometimes...
      public enum Mode
      {
        Tri_Flat_Colored = 0x20,
        Tri_Flat_Textured = 0x24,
        Tri_Gouraud_Colored = 0x30,
        Tri_Gouraud_Textured = 0x34,
        Quad_Flat_Colored = 0x28,
        Quad_Flat_Textured = 0x2c,
        Quad_Gouraud_Colored = 0x38,
        Quad_Gouraud_Textured = 0x3c,
        Unknown21 = 0x21, //?
        Unknown22 = 0x22, //probably tri flat semi-transparent
        Unknown32 = 0x32, //probably tri gouraud semi-transparent
        Unknown36 = 0x36 //probably quad gouraud semi-transparent
      }

      public byte mOLen;
      public byte mILen;
      public Flags mFlag;
      public Mode mMode;
      public int[] mColors; //stored as PS 32 bit XBGR, since the Color struct can't be XML serialised!
      public short[] mNormalIds;
      public short[] mVertexIds;
      public short mCBA; //dunno
      public short mTSB; //dunno
      public System.Drawing.Point[] mTexCoords;

      //for unknown type
      public byte[] mData;

      public bool IsUnknownType()
      {
        return mData != null;
      }

      public void Deserialise(BinaryReader bin)
      {
        mOLen = bin.ReadByte();
        mILen = bin.ReadByte();
        mFlag = (Flags)bin.ReadByte();
        if (!Enum.IsDefined(typeof(Flags), mFlag))
        {
          throw new DeserialisationException(string.Format("Unrecognised flags: {0:x}", mFlag), bin.BaseStream.Position - 1);
        }
        mMode = (Mode)bin.ReadByte();
        if (!Enum.IsDefined(typeof(Mode), mMode))
        {
          throw new DeserialisationException(string.Format("Unrecognised mode: {0:x} at {1}", mMode, bin.BaseStream.Position - 1));
        }

        // don't attempt to parse unknown face types
        if (mMode == Mode.Unknown21
            || mMode == Mode.Unknown22
            || mMode == Mode.Unknown32
            || mMode == Mode.Unknown36)
        {
          mData = bin.ReadBytes(4 * mILen);
          return;
        }

        //data is length mILen * 4

        int lVertexCount;
        switch (mMode)
        {
          case Mode.Tri_Flat_Colored:
          case Mode.Tri_Flat_Textured:
          case Mode.Tri_Gouraud_Colored:
          case Mode.Tri_Gouraud_Textured:
            lVertexCount = 3;
            break;
          case Mode.Quad_Flat_Colored:
          case Mode.Quad_Flat_Textured:
          case Mode.Quad_Gouraud_Colored:
          case Mode.Quad_Gouraud_Textured:
            lVertexCount = 4;
            break;
          default:
            throw new Exception("unreachable case!");
        }

        //load colors or tex coords
        switch (mMode)
        {
          case Mode.Tri_Flat_Colored:
          case Mode.Tri_Gouraud_Colored:
          case Mode.Quad_Flat_Colored:
          case Mode.Quad_Gouraud_Colored:
            int lColCount =
              (mFlag == Flags.Colour_Per_Vertex ? lVertexCount : 1);
            mColors = new int[lColCount];
            for (int i = 0; i < lColCount; i++)
            {
              mColors[i] = bin.ReadInt32();
            }
            break;
          case Mode.Tri_Flat_Textured:
          case Mode.Tri_Gouraud_Textured:
          case Mode.Quad_Flat_Textured:
          case Mode.Quad_Gouraud_Textured:
            mTexCoords = new System.Drawing.Point[lVertexCount];
            for (int i = 0; i < lVertexCount; i++)
            {
              mTexCoords[i] = new System.Drawing.Point(bin.ReadByte(), bin.ReadByte());
              short lTop = bin.ReadInt16();
              if (i == 0)
                mCBA = lTop;
              else if (i == 1)
                mTSB = lTop;
              else if (lTop != 0) throw new DeserialisationException(string.Format("Expecting unused bytes to be zero, found {0:x}", lTop), bin.BaseStream.Position);
            }
            break;
          default:
            throw new Exception("unreachable case!");
        }

        //load vertices and normals
        switch (mMode)
        {
          case Mode.Tri_Flat_Colored:
          case Mode.Tri_Flat_Textured:
          case Mode.Quad_Flat_Colored:
          case Mode.Quad_Flat_Textured:
            mNormalIds = new short[] { bin.ReadInt16() };
            mVertexIds = new short[lVertexCount];
            for (int i = 0; i < lVertexCount; i++)
            {
              mVertexIds[i] = bin.ReadInt16();
            }
            if (lVertexCount % 2 == 0)
            {
              short lSpare = bin.ReadInt16();
              Console.Error.WriteLine("Spare={0:x}", lSpare);
            }
            break;
          case Mode.Tri_Gouraud_Colored:
          case Mode.Tri_Gouraud_Textured:
          case Mode.Quad_Gouraud_Colored:
          case Mode.Quad_Gouraud_Textured:
            mNormalIds = new short[lVertexCount];
            mVertexIds = new short[lVertexCount];
            for (int i = 0; i < lVertexCount; i++)
            {
              mNormalIds[i] = bin.ReadInt16();
              mVertexIds[i] = bin.ReadInt16();
            }
            break;
          default:
            throw new Exception("unreachable case!");
        }
      }

      public override void Serialise(Stream outStr)
      {
        Serialise(outStr, new BinaryWriter(outStr));
      }

      public void Serialise(Stream outStr, BinaryWriter bout)
      {
        bout.Write(mOLen);
        bout.Write(mILen);
        bout.Write((byte)mFlag);
        bout.Write((byte)mMode);
        if (mData != null)
        {
          bout.Write(mData);
          return;
        }

        //data is length mILen * 4
        if (mColors != null)
        {
          foreach (int c in mColors)
          {
            bout.Write(c);
          }
        }

        if (mTexCoords != null)
        {
          for (int i = 0; i < mTexCoords.Length; i++)
          {
            bout.Write((byte)mTexCoords[i].X);
            bout.Write((byte)mTexCoords[i].Y);
            if (i == 0)
              bout.Write(mCBA);
            else if (i == 1)
              bout.Write(mTSB);
            else
              bout.Write((short)0);
          }
        }

        //save vertices and normals
        if (mNormalIds.Length == 1) //(flat)
        {
          bout.Write(mNormalIds[0]);
          for (int i = 0; i < mVertexIds.Length; i++)
          {
            bout.Write(mVertexIds[i]);
          }
          if (mVertexIds.Length % 2 == 0)
          {
            bout.Write((short)0);
          }
        }
        else //(gouraud)
        {
          for (int i = 0; i < mNormalIds.Length; i++)
          {
            bout.Write(mNormalIds[i]);
            bout.Write(mVertexIds[i]);
          }
        }
      }

      public override void Deserialise(Stream inStr)
      {
        Deserialise(new BinaryReader(inStr));
      }

      public override string Name
      {
        get { return "Face"; }
      }
    }
  }
}

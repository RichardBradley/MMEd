using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using MMEd.Util;
using MMEd.Viewers;
using GLTK;
using MMEd.Viewers.ThreeDee;
using Point = GLTK.Point;
using System.Xml.Serialization;

// A Flat from within the SHET
// Represents a drivable surface.

namespace MMEd.Chunks
{
  // The values in this enum are valid Idxs for the third 
  // dimension of the TexMetaData array
  public enum eTexMetaDataEntries
  {
    Steering = 0, // controls AI steering and respawn direction. Indexes entries in SHET.SteeringImages.
    Unknown = 1, // something which seems to always be zero (except on School2-5, which have a handful of non-zero values)
    Behaviour = 2, // controls behaviour of cars, mostly the AI ones but occasionally multiplayer (e.g. "no tumble")
    CameraPos = 3, // the camera position idx. Indexes entries in SHET.CameraPositions.
    Orientation = 4, // orientation of the square in the range (normally) 0-4. Rotates Steering.
    Waypoint = 5, // an ascending counter for the position
    Bumpmap = 6, // the bump map. Indexes entries in SHET.BumpImages.
    RespawnPos = 7 // restart location within the square where you respawn (interpreted as two 4-bit values x,y)
  }

  // The weapons available in the game
  public enum eWeaponType
  {
    Grabber = 1,
    Ommer = 2,
    Molotovs = 3,
    TurboBall = 4, //the power to turn into a white ball
    FlameBall = 5,
    Invisibility = 6,
    Mallet = 7,
    Molotovs2 = 8, //don't know what the difference is here
    FireTrail = 9,
    Missiles = 10,
    Mines = 11,
    GroupGrabber = 12,
    OmniOmmer = 13,
    MultiMallet = 14,
    GroupSpeedUp = 15
  }

  public class FlatChunk : Chunk, IEntityProvider, IPositionable, IRotatable
  {
    [Description("The number at the start of the Flat. I haven't seen it refrenced anywhere yet")]
    public short DeclaredIdx;

    [Description("The name at the start of the Flat. I haven't seen it refrenced anywhere yet")]
    public string DeclaredName;

    [Description("The position of the top left corner of this sheet, in world co-ordinates")]
    public Short3Coord OriginPosition
    {
      get { return mOriginPosition; }
      set { mOriginPosition = value; }
    }
    private Short3Coord mOriginPosition;

    [Description(@"A 3-rotation vector for this Flat. X measures the positive rotation about
            the X-axis, with 1024 being 90 degrees, and so on for Y and Z. The order of rotation
            is cruicial. qq: write it in here, then!")]
    public Short3Coord RotationVector
    {
      get { return mRotationVector; }
      set { mRotationVector = value; }
    }
    private Short3Coord mRotationVector;

    [Description("The width, in texture squares of this Flat")]
    public short Width;

    [Description("The height, in texture squares of this Flat")]
    public short Height;

    [Description("The width of each texture square, in world co-ordinates")]
    public short ScaleX;

    [Description("The height of each texture square, in world co-ordinates")]
    public short ScaleY;

    [Description(@"An array of the texture associated with each square in the tex
 array. This indexes the ""tiles"" NamedImageGroup.")]
    public short[][] TextureIds;

    [Description(@"An array of the height of the top left corner of each square in 
the Flat. Negative is ""up""")]
    public short[][] TerrainHeight;

    [Description("Short Flag: determines whether the Flat is solid and has a metadata array")]
    public bool HasMetaData;

    [Description("Short Flag: unk")]
    public bool FlgB;

    [Description("Short Flag: unk")]
    public bool FlgC;

    [Description(@"Short Flag: seems to control whether the Flat is visible. 
        May be dependent on A. Sometimes ignored")]
    public bool Visible;

    [Description("Short Flag: many ramps have this set to true")]
    public bool FlgE;

    [Description("No idea. Length is determined by HasMetaData")]
    public byte[] NextN;

    [Description(@"One metadata entry for each tex square.
See enum TexMetaDataEntries. Arry dimensions are Width*Height*8. Only Flats with HasMetaData have this")]
    public byte[][][] TexMetaData;

    [Description("The objects on this Flat")]
    public ObjectEntry[] Objects;

    [Description("The weapons on this Flat")]
    public WeaponEntry[] Weapons;

    [Description("no idea")]
    public byte[] TrailingData;

    ///========================================================================
    /// Property : eResizeOptions
    /// 
    /// <summary>
    /// 	Options for the Resize operation:
    /// 
    ///     KeepRight: Keep the right-hand edge fixed in position. Alternative 
    ///               is to keep the left-hand edge fixed.
    ///     KeepBottom: Keep the bottom edge fixed in position. Alternative is 
    ///               to keep the top edge fixed.
    /// </summary>
    ///========================================================================
    public enum eResizeOptions
    {
      Default = 0x0000,
      KeepRight = 0x0001,
      KeepBottom = 0x0002
    }

    ///========================================================================
    ///  Method : Resize
    /// 
    /// <summary>
    /// 	Resize the FlatChunk
    /// </summary>
    /// <param name="xiNewHasMetaData"></param>
    /// <param name="xiNewWidth"></param>
    /// <param name="xiNewHeight"></param>
    /// <param name="xiOptions"></param>
    /// <returns>
    ///   The increase in file size, in bytes
    /// </returns>
    ///========================================================================
    public int Resize(bool xiNewHasMetaData, short xiNewWidth, short xiNewHeight, eResizeOptions xiOptions)
    {
      int lExtraSquares = (xiNewWidth * xiNewHeight) - (Width * Height);
      int lExtraMeta = (xiNewHasMetaData ? (xiNewWidth * xiNewHeight) : 0) - (HasMetaData ? (Width * Height) : 0);
      int lExtraWidth = xiNewWidth - Width;
      int lExtraHeight = xiNewHeight - Height;

      bool lKeepRight = (xiOptions & eResizeOptions.KeepRight) == eResizeOptions.KeepRight;
      bool lKeepBottom = (xiOptions & eResizeOptions.KeepBottom) == eResizeOptions.KeepBottom;

      short[][] lOldTextureIds = TextureIds;
      short[][] lOldTerrainHeight = TerrainHeight;
      Byte[][][] lOldTexMetaData = TexMetaData;

      if (lOldTerrainHeight.Length != lOldTextureIds.Length)
      {
        throw new Exception("Error: TerrainHeight and TextureIds arrays were of different length!");
      }
      if (HasMetaData && lOldTexMetaData.Length != lOldTextureIds.Length)
      {
        throw new Exception("Error: TexMetaData and TextureIds arrays were of different length!");
      }

      Width = xiNewWidth;
      Height = xiNewHeight;

      TextureIds = new short[Width][];
      TerrainHeight = new short[Width][];
      TexMetaData = xiNewHasMetaData ? new Byte[Width][][] : null;

      for (int x = 0; x < Width; x++)
      {
        TextureIds[x] = new short[Height];
        TerrainHeight[x] = new short[Height];
        if (xiNewHasMetaData) TexMetaData[x] = new Byte[Height][];

        int lCopyX = lKeepRight ? x - lExtraWidth : x;
        if (lCopyX >= lOldTextureIds.Length || lCopyX < 0)
        {
          lCopyX = lKeepRight ? lOldTextureIds.Length - 1 : 0;
        }

        if (lOldTerrainHeight[lCopyX].Length != lOldTextureIds[lCopyX].Length)
        {
          throw new Exception("Error: TerrainHeight and TextureIds arrays were of different length!");
        }
        if (HasMetaData && lOldTexMetaData[lCopyX].Length != lOldTextureIds[lCopyX].Length)
        {
          throw new Exception("Error: TexMetaData and TextureIds arrays were of different length!");
        }

        for (int y = 0; y < Height; y++)
        {
          int lCopyY = lKeepBottom ? y - lExtraHeight : y;
          if (lCopyY >= lOldTextureIds[lCopyX].Length || lCopyY < 0)
          {
            lCopyY = lKeepBottom ? lOldTextureIds[lCopyX].Length - 1 : 0;
          }
          
          TextureIds[x][y] = lOldTextureIds[lCopyX][lCopyY];
          TerrainHeight[x][y] = lOldTerrainHeight[lCopyX][lCopyY];
          if (xiNewHasMetaData && HasMetaData)
          {
            TexMetaData[x][y] = lOldTexMetaData[lCopyX][lCopyY];
          }
          else if (xiNewHasMetaData)
          {
            TexMetaData[x][y] = new Byte[8]; // Initialise to all zero since we've got nothing better to do
          }
        }
      }

      HasMetaData = xiNewHasMetaData;
      return lExtraSquares * 4 + lExtraMeta * 8;
    }

    ///========================================================================
    ///  Method : ReplaceWeapons
    /// 
    /// <summary>
    /// 	Replace the current list of weapons in this Flat with a new one.
    /// </summary>
    /// <param name="xiNewWeapons"></param>
    /// <returns></returns>
    ///========================================================================
    public int ReplaceWeapons(IList<WeaponEntry> xiNewWeapons)
    {
      int lSizeIncrease = (xiNewWeapons.Count - (Weapons == null ? 0 : Weapons.Length)) * 12;

      if (xiNewWeapons.Count > 0 || HasMetaData)
      {
        Weapons = new WeaponEntry[xiNewWeapons.Count];
        xiNewWeapons.CopyTo(Weapons, 0);
      }
      else
      {
        Weapons = null;
      }

      return lSizeIncrease;
    }

    ///========================================================================
    ///  Method : ReplaceObjects
    /// 
    /// <summary>
    /// 	Replace the current list of objects in this Flat with a new one.
    /// </summary>
    /// <param name="xiNewWeapons"></param>
    /// <returns></returns>
    ///========================================================================
    public int ReplaceObjects(IList<ObjectEntry> xiNewObjects)
    {
      int lSizeIncrease = (xiNewObjects.Count - (Objects == null ? 0 : Objects.Length)) * 22;

      if (xiNewObjects.Count > 0 || HasMetaData)
      {
        Objects = new ObjectEntry[xiNewObjects.Count];
        xiNewObjects.CopyTo(Objects, 0);
      }
      else
      {
        Objects = null;
      }

      return lSizeIncrease;
    }

    public void Deserialise(BinaryReader bin)
    {
      //header
      DeclaredIdx = bin.ReadInt16();
      DeclaredName = StreamUtils.ReadASCIINullTermString(bin.BaseStream);
      if (DeclaredName.Length != 8)
        throw new DeserialisationException("Expecting name to be length 8 & null-terminated", bin.BaseStream.Position);

      //more header:
      OriginPosition = Short3Coord.ReadShort3Coord64(bin);
      RotationVector = Short3Coord.ReadShort3Coord64(bin);
      Width = bin.ReadInt16();
      Height = bin.ReadInt16();
      ScaleX = bin.ReadInt16();
      ScaleY = bin.ReadInt16();

      //every flat has a tex array:
      TextureIds = new short[Width][]; //not allowed 2d initalisers!!!
      TerrainHeight = new short[Width][];
      for (int x = 0; x < Width; x++)
      {
        TextureIds[x] = new short[Height];
        TerrainHeight[x] = new short[Height];
        for (int y = 0; y < Height; y++)
        {
          TextureIds[x][y] = bin.ReadInt16();
          TerrainHeight[x][y] = bin.ReadInt16();
        }
      }

      //General flags
      HasMetaData = StreamUtils.ReadShortFlag(bin);
      FlgB = StreamUtils.ReadShortFlag(bin);
      FlgC = StreamUtils.ReadShortFlag(bin);
      Visible = StreamUtils.ReadShortFlag(bin);
      FlgE = StreamUtils.ReadByteFlag(bin);  //qq that looks wrong to me, but it works!

      //what do these mean?
      NextN = bin.ReadBytes(HasMetaData ? 2 : 6);

      //unless (HasMetaData), we're done...
      if (HasMetaData)
      {
        //load the tex metadata
        TexMetaData = new Byte[Width][][];
        for (int x = 0; x < Width; x++)
        {
          TexMetaData[x] = new Byte[Height][];
          for (int y = 0; y < Height; y++)
          {
            TexMetaData[x][y] = bin.ReadBytes(8);
          }
        }

        //objects:
        short objectCount = bin.ReadInt16();
        Objects = new ObjectEntry[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
          Objects[i] = new ObjectEntry(bin);
        }

        //weapons:
        short weapCount = bin.ReadInt16();
        Weapons = new WeaponEntry[weapCount];
        for (int i = 0; i < weapCount; i++)
        {
          Weapons[i] = new WeaponEntry(bin);
        }
      }
      else if (Utils.ArrayCompare(NextN, new byte[] { 156, 255, 1, 0, 4, 0 }))
      {
        //applying POOL5 hack!! qqq
        Console.Error.WriteLine("Warning: applying POOL5 specific hack to trailing bytes of jumprmp3 Flat");
        TrailingData = bin.ReadBytes(22);
      }
      else if (Utils.ArrayCompare(NextN, new byte[] { 106, 255, 1, 0, 243, 255 }))
      {
        //applying GARDEN3 hack!! qqq
        Console.Error.WriteLine("Warning: applying GARDEN3 specific hack to trailing bytes of grasssht Flat");
        TrailingData = bin.ReadBytes(22);
      }
    }

    public override void Deserialise(System.IO.Stream inStr)
    {
      Deserialise(new BinaryReader(inStr));
    }

    public override void Serialise(Stream outStr)
    {
      BinaryWriter bout = new BinaryWriter(outStr);

      //header
      bout.Write(DeclaredIdx);
      StreamUtils.WriteASCIINullTermString(outStr, DeclaredName);
      OriginPosition.WriteShort3Coord64(bout);
      RotationVector.WriteShort3Coord64(bout);
      bout.Write(Width);
      bout.Write(Height);
      bout.Write(ScaleX);
      bout.Write(ScaleY);

      //every flat has a tex array:
      for (int x = 0; x < Width; x++)
      {
        for (int y = 0; y < Height; y++)
        {
          bout.Write(TextureIds[x][y]);
          bout.Write(TerrainHeight[x][y]);
        }
      }

      //General flags
      StreamUtils.WriteShortFlag(bout, HasMetaData);
      StreamUtils.WriteShortFlag(bout, FlgB);
      StreamUtils.WriteShortFlag(bout, FlgC);
      StreamUtils.WriteShortFlag(bout, Visible);
      StreamUtils.WriteByteFlag(bout, FlgE); //N.B. byte here. See Deserialise.

      //what do these mean?
      bout.Write(NextN);

      //unless (HasMetaData), we're done...
      if (HasMetaData)
      {
        //load the tex metadata
        for (int x = 0; x < Width; x++)
        {
          for (int y = 0; y < Height; y++)
          {
            bout.Write(TexMetaData[x][y]);
          }
        }

        //objects:
        bout.Write((short)Objects.Length);
        foreach (ObjectEntry o in Objects)
        {
          o.WriteToStream(bout);
        }

        //weapons:
        bout.Write((short)Weapons.Length);
        foreach (WeaponEntry w in Weapons)
        {
          w.WriteToStream(bout);
        }
      }

      //pool5 hack:
      if (TrailingData != null)
      {
        bout.Write(TrailingData);
      }
    }

    public override string Name
    {
      get
      {
        return string.Format("[{0}] {1}", DeclaredIdx, DeclaredName);
      }
    }

    public override List<string> GetDifferences(Chunk xiChunk)
    {
      FlatChunk lOther = xiChunk as FlatChunk;
      if (DeclaredIdx != lOther.DeclaredIdx ||
        DeclaredName != lOther.DeclaredName ||
        !OriginPosition.Equals(lOther.OriginPosition) ||
        !RotationVector.Equals(lOther.RotationVector) ||
        Width != lOther.Width ||
        Height != lOther.Height ||
        ScaleX != lOther.ScaleX ||
        ScaleY != lOther.ScaleY ||
        HasMetaData != lOther.HasMetaData ||
        FlgB != lOther.FlgB ||
        FlgC != lOther.FlgC ||
        Visible != lOther.Visible ||
        FlgE != lOther.FlgE ||
        !Utils.ArrayCompare(TerrainHeight, lOther.TerrainHeight) ||
        !Utils.ArrayCompare(TextureIds, lOther.TextureIds) ||
        !Utils.ArrayCompare(TexMetaData, lOther.TexMetaData))
      {
        List<string> lRet = base.GetDifferences(xiChunk);
        lRet.Add("Changed flat #" + DeclaredIdx.ToString() + " (" + DeclaredName + ")");
        return lRet;
      }

      return base.GetDifferences(xiChunk);
    }

    ///========================================================================
    /// Property : ByteSize
    /// 
    /// <summary>
    /// 	The size of this Flat in bytes
    /// </summary>
    ///========================================================================
    [XmlIgnore]
    public int ByteSize
    {
      get
      {
        int lSize =
          2 + // DeclaredIdx
          9 + // DeclaredName
          8 + // OriginPosition
          8 + // RotationVector
          8 + // Width, Height, ScaleX, ScaleY
          4 * Width * Height + // Texture IDs, Terrain Height
          9 + // HasMetaData, ..., FlgE
          NextN.Length;
        
        if (HasMetaData)
        {
          lSize += 
            8 * Width * Height + // TexMetaData
            2 + // Object count
            Objects.Length * 22 +
            2 + // Weapon count
            Weapons.Length * 12;
        }

        return lSize + (TrailingData != null ? TrailingData.Length : 0);
      }
    }

    public class ObjectEntry : Chunk, IPositionable, IRotatable, IEntityProvider
    {
      [Description("The position of the origin of the object, in the co-ordinate space of the parent Flat")]
      public Short3Coord OriginPosition
      {
        get { return mOriginPosition; }
        set { mOriginPosition = value; }
      }
      private Short3Coord mOriginPosition;

      [Description("A 3-rotation vector for this object. See Flat.RotationVector")]
      public Short3Coord RotationVector
      {
        get { return mRotationVector; }
        set { mRotationVector = value; }
      }
      private Short3Coord mRotationVector;


      [Description("The type of the object. Indexes the OBJT array (TODO)")]
      public short ObjtType;

      [Description("A byte flag of unknown meaning")]
      public bool FlagUnknown;

      [Description("Whether this object is solid. May not always be honoured.")]
      public bool IsSolid;

      [Description("dunno")]
      public short ShortUnknown;

      public ObjectEntry() { }
      public ObjectEntry(BinaryReader bin)
      {
        RotationVector = Short3Coord.ReadShort3Coord64(bin);
        OriginPosition = Short3Coord.ReadShort3Coord64(bin);
        ObjtType = bin.ReadInt16();
        FlagUnknown = StreamUtils.ReadByteFlag(bin);
        IsSolid = StreamUtils.ReadByteFlag(bin);
        ShortUnknown = bin.ReadInt16();
      }

      public void WriteToStream(BinaryWriter bout)
      {
        RotationVector.WriteShort3Coord64(bout);
        OriginPosition.WriteShort3Coord64(bout);
        bout.Write(ObjtType);
        StreamUtils.WriteByteFlag(bout, FlagUnknown);
        StreamUtils.WriteByteFlag(bout, IsSolid);
        bout.Write(ShortUnknown);
      }

      public IEnumerable<GLTK.Entity> GetEntities(Chunk xiRootChunk, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata)
      {
        List<GLTK.Entity> lRet = new List<Entity>();
        TMDChunk lObjt = ((Level)xiRootChunk).GetObjtById(this.ObjtType);
        if (lObjt != null)
        {
          Entity lE = lObjt.GetEntity(xiRootChunk, xiTextureMode, xiSelectedMetadata, this);

          //nasty, but required because the PS 3D world coords are left-handed,
          //and the OpenGl 3D coords are right handed
          lE.Scale(1, 1, -1);

          if (this.RotationVector.Norm() != 0)
          {
            //the rotation is z-y-x
            lE.RotateAboutWorldOrigin(-this.RotationVector.Z / 1024.0 * Math.PI / 2.0, Vector.ZAxis);
            lE.RotateAboutWorldOrigin(-this.RotationVector.Y / 1024.0 * Math.PI / 2.0, Vector.YAxis);
            lE.RotateAboutWorldOrigin(-this.RotationVector.X / 1024.0 * Math.PI / 2.0, Vector.XAxis);
          }

          lE.Position = ThreeDeeViewer.Short3CoordToPoint(this.OriginPosition);

          //return to right handed coords
          lE.Scale(1, 1, -1);
          
          lRet.Add(lE);
        }

        return lRet;
      }

      // Note: serialisation and deserialisation are handled by FlatChunk
      public override void Serialise(Stream outStr)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public override void Deserialise(Stream inStr)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      // qq could perhaps do better than this
      public override string Name
      {
        get { return string.Format("Object - Type:{0}", this.ObjtType); }
      }

      public override List<string> GetDifferences(Chunk xiChunk)
      {
        ObjectEntry lOther = xiChunk as ObjectEntry;

        if (!mOriginPosition.Equals(lOther.mOriginPosition) ||
          !mRotationVector.Equals(lOther.mRotationVector) ||
          ObjtType != lOther.ObjtType ||
          FlagUnknown != lOther.FlagUnknown ||
          IsSolid != lOther.IsSolid ||
          ShortUnknown != lOther.ShortUnknown)
        {
          List<string> lRet = base.GetDifferences(xiChunk);
          lRet.Add(string.Format("Changed object of type {0}", ObjtType));
          return lRet;
        }

        return base.GetDifferences(xiChunk);
      }
    }

    public class WeaponEntry : Chunk, IPositionable, IEntityProvider
    {
      [Description("The type of the weapon. See enum eWeaponType")]
      public eWeaponType WeaponType;

      [Description("dunno")]
      public short ShortUnknown;

      [Description("The position of the weapon, in the co-ordinate space of the parent Flat")]
      public Short3Coord OriginPosition
      {
        get { return mPosition; }
        set { mPosition = value; }
      }
      public Short3Coord mPosition;

      public WeaponEntry() { }
      public WeaponEntry(BinaryReader bin)
      {
        try
        {
          WeaponType = (eWeaponType)bin.ReadInt16();
        }
        catch (InvalidCastException e)
        {
          throw new DeserialisationException("Unrecognised weapon type: " + e, bin.BaseStream.Position);
        }
        ShortUnknown = bin.ReadInt16();
        OriginPosition = Short3Coord.ReadShort3Coord64(bin);
      }

      public void WriteToStream(BinaryWriter bout)
      {
        bout.Write((short)WeaponType);
        bout.Write(ShortUnknown);
        OriginPosition.WriteShort3Coord64(bout);
      }

      public IEnumerable<GLTK.Entity> GetEntities(Chunk xiRootChunk, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata)
      {
        Entity lWeaponEntity = ((Level)xiRootChunk).GetObjtById(TMDChunk.OBJT_ID_FOR_WEAPONS_BOX)
          .GetEntity(xiRootChunk, xiTextureMode, xiSelectedMetadata, this);

        // Set the position.
        Point lWeaponPosition = ThreeDeeViewer.Short3CoordToPoint(this.OriginPosition);
        lWeaponEntity.Position = new Point(lWeaponPosition.x, lWeaponPosition.y, -lWeaponPosition.z);

        List<GLTK.Entity> lRet = new List<Entity>();
        lRet.Add(lWeaponEntity);
        return lRet;
      }

      // Note: serialisation and deserialisation are handled by FlatChunk
      public override void Serialise(Stream outStr)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public override void Deserialise(Stream inStr)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public override string Name
      {
        get { return string.Format("Weapon - {0}", this.WeaponType); }
      }

      public override List<string> GetDifferences(Chunk xiChunk)
      {
        WeaponEntry lOther = xiChunk as WeaponEntry;

        if (!mPosition.Equals(lOther.mPosition) ||
          WeaponType != lOther.WeaponType ||
          ShortUnknown != lOther.ShortUnknown)
        {
          List<string> lRet = base.GetDifferences(xiChunk);
          lRet.Add(string.Format("Changed weapon of type {0}", WeaponType));
          return lRet;
        }

        return base.GetDifferences(xiChunk);
      }
    }

    public FlatChunk() { }
    public FlatChunk(BinaryReader bin) { Deserialise(bin); }

    public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      if (xiFrom is ObjectEntry)
      {
        for (int i = 0; i < Objects.Length; i++)
          if (xiFrom == Objects[i])
          {
            Objects[i] = (ObjectEntry)xiTo;
            return;
          }
        throw new ArgumentException("xifrom not found!");
      }
      else if (xiFrom is WeaponEntry)
      {
        for (int i = 0; i < Weapons.Length; i++)
          if (xiFrom == Weapons[i])
          {
            Weapons[i] = (WeaponEntry)xiTo;
            return;
          }
        throw new ArgumentException("xifrom not found!");
      }
      else
      {
        throw new ArgumentException("xifrom not found!");
      }
    }

    public override Chunk[] GetChildren()
    {
      ArrayList acc = new ArrayList();
      if (Weapons != null) acc.AddRange(Weapons);
      if (Objects != null) acc.AddRange(Objects);
      return (Chunk[])acc.ToArray(typeof(Chunk));
    }

    private short GetTerrainHeightSafe(int x, int y)
    {
      if (x < 0 || x >= Width || y < 0 || y >= Height)
      {
        return 0;
      }
      else
      {
        return TerrainHeight[x][y];
      }
    }

    public IEnumerable<GLTK.Entity> GetEntities(Chunk xiRootChunk, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata)
    {
      if (!(xiRootChunk is Level))
      {
        throw new Exception("xiRootChunk must be Level for Level.GetEntities");
      }

      return GetEntities((Level)xiRootChunk, xiTextureMode, xiSelectedMetadata);
    }

    public IEnumerable<GLTK.Entity> GetEntities(Level xiLevel, eTextureMode xiTextureMode, eTexMetaDataEntries xiSelectedMetadata)
    {
      //a bit hacky:
      if (this.TreeNode.Checked)
      {
        return new Entity[0];
      }

      // notes: 
      // invert the textures along the y-axis
      // use level co-ords, so z is down

      /////////////////////////////////////////////////////
      // The surface
      Entity lSurface = new MMEdEntity(this);

      Font lNumberFont = null;
      Brush lNumberFGBrush = null, lNumberBGBrush = null;
      Pen lWaypointPen = null, lKeyWaypointPen = null;
      if (xiTextureMode == eTextureMode.NormalTexturesWithMetadata)
      {
        lNumberFont = new Font(FontFamily.GenericMonospace, 10);
        lNumberFGBrush = new SolidBrush(Color.Black);
        lNumberBGBrush = new SolidBrush(Color.White);
        lWaypointPen = new Pen(Color.Black, 1f);
        lKeyWaypointPen = new Pen(Color.Red, 2f);
      }

      for (int x = 0; x < Width; x++)
      {
        for (int y = 0; y < Height; y++)
        {
          Mesh lSquare = new OwnedMesh(this, PolygonMode.Quads);
          lSquare.AddFace(
            new Vertex(new Point(x, y, -GetTerrainHeightSafe(x, y)), 0, 0),
            new Vertex(new Point(x + 1, y, -GetTerrainHeightSafe(x + 1, y)), 1, 0),
            new Vertex(new Point(x + 1, y + 1, -GetTerrainHeightSafe(x + 1, y + 1)), 1, 1),
            new Vertex(new Point(x, y + 1, -GetTerrainHeightSafe(x, y + 1)), 0, 1));

          switch (xiTextureMode)
          {
            case eTextureMode.WireFrame:
              lSquare.RenderMode = RenderMode.Wireframe;
              break;

            // normal textures, optionally with metadata drawn on
            case eTextureMode.NormalTextures:
            case eTextureMode.NormalTexturesWithMetadata:
              TIMChunk lTIM = xiLevel.GetTileById(TextureIds[x][y]);

              if (lTIM != null)
              {
                //some TIMs can't be loaded yet: they're null
                Bitmap lTexture = lTIM.ToBitmap();
                if (xiTextureMode == eTextureMode.NormalTexturesWithMetadata
                    && TexMetaData != null)
                {
                  byte lVal = TexMetaData[x][y][(int)xiSelectedMetadata];

                  if (lVal != 0)
                  {
                    // we create a new bitmap based on the given texture
                    // a) so that we can modify it freely
                    // and b) to change it from indexed to full colour mode, to allow us 
                    // to draw on it (otherwise we'll get an exception)
                    lTexture = new Bitmap(lTexture);

                    Graphics g = Graphics.FromImage(lTexture);

                    string lText = lVal.ToString();

                    SizeF size = g.MeasureString(lText, lNumberFont);

                    float xf = lTexture.Width / 2.0f - size.Width / 2.0f;
                    float yf = lTexture.Height / 2.0f - size.Height / 2.0f;

                    g.FillRectangle(lNumberBGBrush, xf, yf, size.Width, size.Height);

                    g.DrawString(
                        lText,
                        lNumberFont,
                        lNumberFGBrush,
                        xf,
                        yf);

                    if (xiSelectedMetadata == eTexMetaDataEntries.Waypoint)
                    {
                      Pen lPen = xiLevel.WaypointIsKeyWaypoint(lVal)
                          ? lKeyWaypointPen
                          : lWaypointPen;

                      g.DrawRectangle(
                          lPen,
                          0, 0, lTexture.Width - 1, lTexture.Height - 1);

                    }
                  }
                }

                lSquare.Texture = AbstractRenderer.ImageToTextureId(lTexture);
              }
              break;

            //draw the bumpmap textures on:
            case eTextureMode.BumpmapTextures:
              if (TexMetaData != null)
              {
                BumpImageChunk lBIC = xiLevel.GetBumpById(TexMetaData[x][y][(int)eTexMetaDataEntries.Bumpmap]);

                if (lBIC != null)
                {
                  Bitmap lTexture = lBIC.ToBitmap();
                  lSquare.Texture = AbstractRenderer.ImageToTextureId(lTexture);
                }
              }
              break;

            default: throw new Exception("Unexpected case");

          } //end switch

          lSurface.Meshes.Add(lSquare);
        }
      }

      lSurface.Scale(ScaleX, ScaleY, 1.0);
      if (RotationVector.Norm() != 0)
      {
        //the rotation is z-y-x
        lSurface.RotateAboutWorldOrigin(RotationVector.Z / 1024.0 * Math.PI / 2.0, Vector.ZAxis);
        lSurface.RotateAboutWorldOrigin(-RotationVector.Y / 1024.0 * Math.PI / 2.0, Vector.YAxis);
        lSurface.RotateAboutWorldOrigin(-RotationVector.X / 1024.0 * Math.PI / 2.0, Vector.XAxis);
      }

      Point lNewPos = ThreeDeeViewer.Short3CoordToPoint(OriginPosition);
      lSurface.Position = new Point(lNewPos.x, lNewPos.y, -lNewPos.z);

      List<Entity> lAcc = new List<Entity>();
      lAcc.Add(lSurface);

      /////////////////////////////////////////////////////
      // The child weapons
      if (Weapons != null)
      {
        foreach (WeaponEntry lWeapon in Weapons)
        {
          lAcc.AddRange(lWeapon.GetEntities(xiLevel, xiTextureMode, xiSelectedMetadata));
        }
      }

      /////////////////////////////////////////////////////
      // The child objects
      if (Objects != null)
      {
        foreach (ObjectEntry oe in Objects)
        {
          lAcc.AddRange(oe.GetEntities(xiLevel, xiTextureMode, xiSelectedMetadata));
        }
      }

      return lAcc;
    }

    // Constants relating to the meta data
    public const int STEERINGDIMENSION = 8;
    public const int BEHAVIOUR_DEFAULT = 0;
    public const int BEHAVIOUR_DEFAULTNORESPAWN = 8;
    public const int STEERING_HIGHESTDIRECTION = 15;

    ///========================================================================
    /// Enum : eBehaviourTypes
    /// 
    /// <summary>
    /// 	Known types of behaviour.
    /// 
    ///   Note that Unknown is a special case - in fact all behaviour types
    ///   might have some unknown behaviour, but Unknown is only used where
    ///   we /know/ that we don't know something.
    /// </summary>
    ///========================================================================
    [Flags]
    public enum eBehaviourTypes
    {
      None              = 0x00,
      Unknown           = 0x01,
      BlockRespawn      = 0x02,
      NoTumble          = 0x04,
      SlowCars          = 0x08,
      SlightlySlowCars  = 0x10,
      DeathTrap         = 0x20,
      Fly               = 0x40 // Common under jumps - maybe controls how AI cars fly through the air?
    }

    ///========================================================================
    /// Static Array : Behaviours
    /// 
    /// <summary>
    /// 	All known behaviours
    /// </summary>
    ///========================================================================
    public static Behaviour[] Behaviours = new Behaviour[] {
      new Behaviour(0, eBehaviourTypes.None),
      new Behaviour(1, eBehaviourTypes.Unknown),
      new Behaviour(2, eBehaviourTypes.BlockRespawn),
      new Behaviour(3, eBehaviourTypes.NoTumble),
      new Behaviour(4, eBehaviourTypes.SlowCars),
      new Behaviour(5, eBehaviourTypes.BlockRespawn),
      new Behaviour(6, eBehaviourTypes.Unknown),
      new Behaviour(7, eBehaviourTypes.DeathTrap),
      new Behaviour(8, eBehaviourTypes.BlockRespawn),
      new Behaviour(9, eBehaviourTypes.Unknown),
      new Behaviour(10, eBehaviourTypes.SlowCars),
      new Behaviour(11, eBehaviourTypes.SlightlySlowCars),
      new Behaviour(12, eBehaviourTypes.BlockRespawn),
      new Behaviour(13, eBehaviourTypes.BlockRespawn | eBehaviourTypes.NoTumble),
      new Behaviour(14, eBehaviourTypes.BlockRespawn),
      new Behaviour(15, eBehaviourTypes.Fly),
      new Behaviour(16, eBehaviourTypes.Fly | eBehaviourTypes.BlockRespawn)
    };

    ///========================================================================
    /// Class : Behaviours
    /// 
    /// <summary>
    /// 	A particular metadata layer two (behaviour) value
    /// </summary>
    ///========================================================================
    public class Behaviour
    {
      public Behaviour(int xiValue, eBehaviourTypes xiBehaviourTypes)
      {
        Value = xiValue;
        BehaviourTypes = xiBehaviourTypes;
      }

      public Bitmap ToBitmap()
      {
        return ToBitmap(Pens.Black);
      }

      public Bitmap ToBitmap(Pen xiPen)
      {
        if (mBitmap == null || mActivePen == null || xiPen.Color != mActivePen.Color)
        {
          mBitmap = ToBitmapUncached(xiPen);
          mActivePen = xiPen;
        }
        return mBitmap;
      }

      private Bitmap ToBitmapUncached(Pen xiPen)
      {
        Bitmap lBitmap = new Bitmap(8 * SCALE, 8 * SCALE);
        Graphics lGraphics = Graphics.FromImage(lBitmap);

        if ((BehaviourTypes & eBehaviourTypes.Unknown) != 0)
        {
          lGraphics.DrawString("Unknown", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, 0));
        }

        if ((BehaviourTypes & eBehaviourTypes.BlockRespawn) != 0)
        {
          lGraphics.DrawString("Block Respawn", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, SCALE));
        }

        if ((BehaviourTypes & eBehaviourTypes.NoTumble) != 0)
        {
          lGraphics.DrawString("No Tumble", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, 2 * SCALE));
        }

        if ((BehaviourTypes & eBehaviourTypes.DeathTrap) != 0)
        {
          lGraphics.DrawString("Death Trap", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, 3 * SCALE));
        }

        if ((BehaviourTypes & eBehaviourTypes.Fly) != 0)
        {
          lGraphics.DrawString("Fly Over", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, 4 * SCALE));
        }

        if ((BehaviourTypes & eBehaviourTypes.SlowCars) != 0)
        {
          lGraphics.DrawString("Slow Cars", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, 5 * SCALE));
        }

        if ((BehaviourTypes & eBehaviourTypes.SlightlySlowCars) != 0)
        {
          lGraphics.DrawString("Slightly Slow Cars", new Font(FontFamily.GenericMonospace, 10), xiPen.Brush, new PointF(0, 6 * SCALE));
        }

        return lBitmap;
      }

      public int Value;
      public eBehaviourTypes BehaviourTypes;

      private const int SCALE = 16;
      private Bitmap mBitmap = null;
      private Pen mActivePen = null;
    }

  }
}

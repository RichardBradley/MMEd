using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using TreeNode = System.Windows.Forms.TreeNode;
using XmlIgnoreAttribute = System.Xml.Serialization.XmlIgnoreAttribute;
using MMEd.Chunks;

// Abstract base class for a "Chunk", which is a section of the level
// file.
//
// Chunks may be just about anything, from images to 3D objects to
// flats in the level.
//
// A Chunk may have children
//
// Chunks are able to serialise and deserialise themselves to and from the
// PS MMv3 file format. The serialisation must be completely reversible.

namespace MMEd
{
  [XmlInclude(typeof(Level))]
  public abstract class Chunk
  {
    // Writes this Chunk and all its children to the given stream
    public abstract void Serialise(Stream outStr);

    // Initialises this chunk and all its children from the given
    // stream.
    // Leaves the stream at the start of the next Chunk
    public abstract void Deserialise(Stream inStr);

    // returns a list of children to display in the tree view
    public virtual Chunk[] GetChildren()
    {
      return new Chunk[0];
    }

    [XmlIgnore]
    public abstract string Name
    {
      get;
    }

    /// <summary>
    ///   Modifies this Chunk, replacing the given child with
    ///   the given replacement.
    /// </summary>
    /// <param name="xiFrom">
    ///   Must be a child Chunk of this one
    /// </param>
    public virtual void ReplaceChild(Chunk xiFrom, Chunk xiTo)
    {
      throw new Exception("This operation has not been implemented or is not permitted for this chunk type.");
    }

    private TreeNode mTreeNode;

    [XmlIgnore]
    public TreeNode TreeNode
    {
      get { return mTreeNode; }
      set { mTreeNode = value; }
    }

  }
}

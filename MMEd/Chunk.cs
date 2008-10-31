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

    ///========================================================================
    /// Method : GetDifferences
    /// 
    /// <summary>
    /// 	Produce a list of differences between this chunk and another.
    /// 
    ///   Subclasses should override this method to produce a customised list
    ///   of differences for themselves, but call the base implementation
    ///   which will handle collecting data from their children.
    /// </summary>
    /// <param name="xiChunk"></param>
    /// <returns></returns>
    /// <remarks>
    ///   This is a brute force method of comparing differences. An alternative
    ///   would be to record the actual actions taken in the UI - this would
    ///   produce cleaner results, although it would be more work to implement.
    /// </remarks>
    ///========================================================================
    public virtual List<string> GetDifferences(Chunk xiChunk)
    {
      //=======================================================================
      // Get a list of all the children of this chunk, by type
      //=======================================================================
      Dictionary<Type, List<Chunk>> lMyChildren = new Dictionary<Type, List<Chunk>>();
      foreach (Chunk lChunk in GetChildren())
      {
        List<Chunk> lChunksOfType;
        if (!lMyChildren.TryGetValue(lChunk.GetType(), out lChunksOfType))
        {
          lChunksOfType = new List<Chunk>();
          lMyChildren[lChunk.GetType()] = lChunksOfType;
        }
        lChunksOfType.Add(lChunk);
      }

      //=======================================================================
      // Get a list of all the children of the old chunk, by type
      //=======================================================================
      Dictionary<Type, List<Chunk>> lOldChildren = new Dictionary<Type, List<Chunk>>();
      foreach (Chunk lChunk in xiChunk.GetChildren())
      {
        List<Chunk> lChunksOfType;
        if (!lOldChildren.TryGetValue(lChunk.GetType(), out lChunksOfType))
        {
          lChunksOfType = new List<Chunk>();
          lOldChildren[lChunk.GetType()] = lChunksOfType;
        }
        lChunksOfType.Add(lChunk);
      }

      //=======================================================================
      // Generate the list of differences
      //=======================================================================
      List<string> lDifferences = new List<string>();
      foreach (Type lChunkType in lMyChildren.Keys)
      {
        List<Chunk> lMine = lMyChildren[lChunkType];
        List<Chunk> lOld = lOldChildren[lChunkType];

        if (lOld == null)
        {
          lDifferences.Add(string.Format("  Added {0} {1}(s)", lMine.Count, lChunkType));
          continue;
        }

        for (int ii = 0; ii < Math.Min(lMine.Count, lOld.Count); ii++)
        {
          List<string> lChildDifferences = lMine[ii].GetDifferences(lOld[ii]);

          if (lChildDifferences.Count > 0)
          {
            lDifferences.AddRange(
              lChildDifferences.ConvertAll<string>(
              new Converter<string, string>(delegate(string xiDifference) 
              { return "  " + xiDifference; })));
          }
        }

        if (lMine.Count > lOld.Count)
        {
          lDifferences.Add(string.Format("  Added {0} {1}(s)", lMine.Count - lOld.Count, lChunkType));
        }
        else if (lOld.Count > lMine.Count)
        {
          lDifferences.Add(string.Format("  Removed {0} {1}(s)", lOld.Count - lMine.Count, lChunkType));
        }

        lOldChildren.Remove(lChunkType);
      }

      foreach (Type lChunkType in lOldChildren.Keys)
      {
        List<Chunk> lOld = lOldChildren[lChunkType];
        lDifferences.Add(string.Format("  Removed {0} {1}(s)", lOld.Count, lChunkType));
      }

      if (lDifferences.Count > 0)
      {
        lDifferences.Insert(0, string.Format("On {0}:", Name));
      }

      return lDifferences;
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

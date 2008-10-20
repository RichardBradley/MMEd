using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Chunks
{
  ///==========================================================================
  /// Property : IReindexableChunk
  /// 
  /// <summary>
  /// 	A chunk that can be reindexed - it needs to have a byte[] representation
  ///   that can be taken as its canonical form, and a method to clear all
  ///   its data.
  /// </summary>
  ///==========================================================================
  interface IReindexableChunk
  {
    byte[] Data
    {
      get;
      set;
    }

    void Clear();
  }
}

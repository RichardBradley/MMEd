using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
using MMEd.Util;

namespace MMEd.Chunks
{
    public class NamedImageGroup : Chunk
    {
        // use an 8-bit char set (not ASCII, which is (correctly, but unusually) 
        // 7 bit in C#)
        public static Encoding LabelEncoding = Encoding.GetEncoding("windows-1252");

        [Description("The name of this group of images. Note that the name usually contains NUL chars")]
        public byte[] Label;

        public Chunk[] mChildren;

        public override void Deserialise(System.IO.Stream inStr)
        {
            BinaryReader bin = new BinaryReader(inStr);
            int length = bin.ReadInt32();
            int type = bin.ReadInt32();
            if (type != 1)
            {
                throw new DeserialisationException(string.Format("Expecting type 1, found type {0}", type), inStr.Position - 8);
            }
            Label = bin.ReadBytes(length);

            // are there any children?
            ArrayList children = new ArrayList();
            while (true)
            {
                int nextLen = bin.ReadInt32();
                int nextType = bin.ReadInt32();
                if (nextLen != 0 && nextType == 0)
                {
                    try
                    {
                        TIMChunk tim = new TIMChunk(children.Count, inStr, nextLen);
                        children.Add(tim);
                    }
                    catch (TIMChunk.TIMTypeNotImplementedException e)
                    {
                        if (e.Offset < 0) throw;
                        //store it as a BLOB instead
                        inStr.Seek(e.Offset, SeekOrigin.Begin);
                        byte[] data = new byte[nextLen];
                        inStr.Read(data, 0, data.Length);
                        children.Add(new RawDataChunk(string.Format("[{0}] TIM of unsupported type ({1})", children.Count, e.Message), data));
                    }
                }
                else
                {
                    inStr.Seek(-8, SeekOrigin.Current);
                    break;
                }
            }
            mChildren = (Chunk[])children.ToArray(typeof(Chunk));
        }

        public override void Serialise(Stream outStr)
        {
            BinaryWriter bout = new BinaryWriter(outStr);
            bout.Write((int)Label.Length);
            bout.Write((int)1);
            bout.Write(Label);

            foreach (Chunk child in mChildren)
            {
                if (child is TIMChunk)
                {
                    TIMChunk childTim = (TIMChunk)child;
                    bout.Write((int)childTim.DataLength);
                    bout.Write((int)0);
                    childTim.Serialise(outStr);
                }
                else if (child is RawDataChunk)
                {
                    RawDataChunk childRaw = (RawDataChunk)child;
                    bout.Write((int)childRaw.mData.Length);
                    bout.Write((int)0);
                    childRaw.Serialise(outStr);
                }
                else
                {
                    throw new Exception("Expecting TIMChunk or RawDataChunk");
                }
            }
        }

        public override Chunk[] GetChildren()
        {
            return mChildren;
        }

        public override void ReplaceChild(Chunk xiFrom, Chunk xiTo)
        {
            for (int i = 0; i < mChildren.Length; i++)
                if (xiFrom == mChildren[i])
                {
                    mChildren[i] = xiTo;
                    return;
                }
            throw new ArgumentException("xifrom not found!");
        }

        public override string Name
        {
            get
            {
                string labStr = LabelEncoding.GetString(Label);
                return Utils.EscapeString(labStr.Substring(0, labStr.IndexOf('\0')));
            }
        }
    }
}

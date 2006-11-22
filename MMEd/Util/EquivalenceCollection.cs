using System;
using System.Collections.Generic;
using System.Text;

// groups IComparable objects into equivalence classes
// allows handles on the objects, in case the Comparison is expensive

namespace MMEd.Util
{
  public class EquivalenceCollection<KeyType, ValueType> : IEnumerable<List<ValueType>>
  {
    private IComparer<ValueType> mComparer;
    private Node mRootNode;
    private Dictionary<KeyType, List<ValueType>> mKeyLookup = new Dictionary<KeyType,List<ValueType>>();

    public EquivalenceCollection() {}
    public EquivalenceCollection(IComparer<ValueType> xiComparer)
    {
      mComparer = xiComparer;
    }

    public List<ValueType> GetClassByKey(KeyType xiKey)
    {
      if (mKeyLookup.ContainsKey(xiKey))
      {
        return mKeyLookup[xiKey];
      }
      else
      {
        return new List<ValueType>();
      }
    }

    public List<ValueType> this[KeyType xiKey]
    {
      get
      {
        return GetClassByKey(xiKey);
      }
    }

    public List<ValueType> GetClassByValue(ValueType xiValue)
    {
      if (mRootNode == null) return new List<ValueType>();
      return GetClassByValueRecursive(mRootNode, xiValue);
    }

    private List<ValueType> GetClassByValueRecursive(Node xiNode, ValueType xiValue)
    {
      // because it's an equivalence relation, we only need to compare to the
      // first:
      ValueType lCompareTo = xiNode.ValueList[0];
      int lCompareResult;

      if (mComparer != null)
      {
        lCompareResult = mComparer.Compare(xiValue, lCompareTo);
      }
      else
      {
        lCompareResult = ((IComparable)xiValue).CompareTo(lCompareTo);
      }

      if (lCompareResult == 0)
      {
        return xiNode.ValueList;
      }
      else if (lCompareResult < 0)
      {
        if (xiNode.Left == null)
        {
          return new List<ValueType>();
        }
        else
        {
          return GetClassByValueRecursive(xiNode.Left, xiValue);
        }
      }
      else
      {
        if (xiNode.Right == null)
        {
          return new List<ValueType>();
        }
        else
        {
          return GetClassByValueRecursive(xiNode.Right, xiValue);
        }
      }
    }

    public void Add(KeyType xiKey, ValueType xiValue)
    {
      if (mKeyLookup.ContainsKey(xiKey))
      {
        throw new ArgumentException(string.Format("The key {0} already exists", xiKey));
      }

      if (mRootNode == null)
      {
        mRootNode = new Node(xiValue);
        mKeyLookup.Add(xiKey, mRootNode.ValueList);
      }
      else
      {
        AddRecursive(mRootNode, xiKey, xiValue);
      }
    }

    private void AddRecursive(
      Node xiNode,
      KeyType xiKey,
      ValueType xiValue)
    {
      // because it's an equivalence relation, we only need to compare to the
      // first:
      ValueType lCompareTo = xiNode.ValueList[0];
      int lCompareResult;

      if (mComparer != null)
      {
        lCompareResult = mComparer.Compare(xiValue, lCompareTo);
      }
      else
      {
        lCompareResult = ((IComparable)xiValue).CompareTo(lCompareTo);
      }

      if (lCompareResult == 0)
      {
        xiNode.ValueList.Add(xiValue);
        mKeyLookup.Add(xiKey, xiNode.ValueList);
      }
      else if (lCompareResult < 0)
      {
        if (xiNode.Left == null)
        {
          xiNode.Left = new Node(xiValue);
          mKeyLookup.Add(xiKey, xiNode.Left.ValueList);
        }
        else
        {
          AddRecursive(xiNode.Left, xiKey, xiValue);
        }
      }
      else
      {
        if (xiNode.Right == null)
        {
          xiNode.Right = new Node(xiValue);
          mKeyLookup.Add(xiKey, xiNode.Right.ValueList);
        }
        else
        {
          AddRecursive(xiNode.Right, xiKey, xiValue);
        }
      }
    }

    private class Node
    {
      public Node Left, Right;
      public List<ValueType> ValueList;

      public Node(ValueType xiFirstValue)
      {
        ValueList = new List<ValueType>();
        ValueList.Add(xiFirstValue);
      }
    }

    IEnumerator<List<ValueType>> IEnumerable<List<ValueType>>.GetEnumerator()
    {
      return new NodeEnumerator(mRootNode);
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
      return ((IEnumerable<List<ValueType>>)this).GetEnumerator();
    }

    private class NodeEnumerator : IEnumerator<List<ValueType>>
    {
      private enum eState
      {
        BeforeLeft,
        AtLeft,
        AtMiddle,
        AtRight,
        AfterRight
      }

      private Node mNode;
      private eState mState;
      private IEnumerator<List<ValueType>> mChildEnumerator;

      public NodeEnumerator(Node xiRootNode)
      {
        mNode = xiRootNode;
        mState = eState.BeforeLeft;
        mChildEnumerator = null;
      }

      List<ValueType> IEnumerator<List<ValueType>>.Current
      {
        get
        {
          switch (mState)
          {
            case eState.BeforeLeft:
              throw new Exception("Current() called before move next");
            case eState.AtLeft:
              return mChildEnumerator.Current;
            case eState.AtMiddle:
              return mNode.ValueList;
            case eState.AtRight:
              return mChildEnumerator.Current;
            case eState.AfterRight:
              throw new Exception("Current() called after end of enumeration");
            default:
              throw new Exception("unreachable case");
          }
        }
      }

      public object Current
      {
        get
        {
          return ((IEnumerator<List<ValueType>>)this).Current;
        }
      }

      public bool MoveNext()
      {
        if (mNode == null)
        {
          mState = eState.AfterRight;
          return false;
        }

        if (mState == eState.BeforeLeft)
        {
          mChildEnumerator = new NodeEnumerator(mNode.Left);
          mState = eState.AtLeft;
        }

        if (mState == eState.AtLeft)
        {
          if (mChildEnumerator.MoveNext())
          {
            return true;
          }
          else
          {
            mState = eState.AtMiddle;
            return true;
          }
        }

        if (mState == eState.AtMiddle)
        {
          mChildEnumerator = new NodeEnumerator(mNode.Right);
          mState = eState.AtRight;
        }

        if (mState == eState.AtRight)
        {
          if (mChildEnumerator.MoveNext())
          {
            return true;
          }
          else
          {
            mState = eState.AfterRight;
            return false;
          }
        }

        if (mState == eState.AfterRight)
        {
          return false;
        }

        throw new Exception("unreachable case");
      }

      public void Reset()
      {
        throw new NotSupportedException();
      }

      public void Dispose()
      {
      }
    }
  }
}

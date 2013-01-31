using System;
using System.Collections.Generic;
using System.Text;
using GLTK;
using NUnit.Framework;

namespace MMEd.Tests
{
  class FloatHelperTest
  {
    [Test]
    public void TestAlmostEqual()
    {
      Assert.IsTrue(FloatHelper.AlmostEqual(2.0, 2.00000047));
      Assert.IsFalse(FloatHelper.AlmostEqual(2.0, 2.00001));
    }
  }
}

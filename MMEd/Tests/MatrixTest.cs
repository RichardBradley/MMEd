using System;
using GLTK;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace MMEd.Tests
{
  [TestFixture]
  public class MatrixTest
  {
    [Test]
    public void TestFloatHelper()
    {
      Assert.IsTrue(FloatHelper.AlmostEqual(-1.224606e-16, 0));
      Assert.IsTrue(FloatHelper.AlmostEqual(
        33.760013846626308,
        33.76002));
    }

    [Test]
    public void testDecomposition()
    {
      Matrix t = Matrix.Translation(9, 10, 11);
      Vector rotAxis = new Vector(1, 2, 3).Normalise();
      // == [0.26726, 0.5345, 0.801]
      Matrix r = Matrix.Rotation(1.5, rotAxis);
      Matrix s = Matrix.ScalingMatrix(3, 3, 3);

      Matrix m = s*r*t;

      Vector scale;
      Vector rotationAxis;
      double rotationAngle;
      Vector translation;
      m.Decompose(out translation, out rotationAxis, out rotationAngle, out scale);

      Assert.AreEqual(new Vector(9, 10, 11), translation);
      Assert.AreEqual(rotAxis, rotationAxis * -1); // TODO need to fix rotation inverse
      Assert.That(1.5, AlmostEqualTo(rotationAngle));
      Assert.AreEqual(new Vector(3,3,3), scale);

      testDecompositionRoundTrip(m);
    }

    private void testDecompositionRoundTrip(Matrix m)
    {
      Vector scale;
      Vector rotationAxis;
      double rotationAngle;
      Vector translation;
      m.Decompose(out translation, out rotationAxis, out rotationAngle, out scale);

      rotationAxis = rotationAxis*-1;

      var t = Matrix.Translation(translation.x, translation.y, translation.z);
      Matrix r = Matrix.Rotation(rotationAngle, rotationAxis);
      Matrix s = Matrix.ScalingMatrix(scale.x, scale.y, scale.z);
      Matrix m2 = s * r * t;

      Assert.That(m, Is.EqualTo(m2));
    }

    [Test]
    public void testDecompositionIdentity()
    {
      Matrix m = Matrix.Identity;

      Vector scale;
      Vector rotationAxis;
      double rotationAngle;
      Vector translation;
      m.Decompose(out translation, out rotationAxis, out rotationAngle, out scale);

      Assert.AreEqual(new Vector(0, 0, 0), translation);
      Assert.That(0.0, AlmostEqualTo(rotationAngle));
      Assert.AreEqual(new Vector(1, 1, 1), scale);

      testDecompositionRoundTrip(m);
    }

    [Test]
    public void testDecomposition2()
    {
      // observed in a MMs level:
      Matrix m = new Matrix(-1.000000e+000, 0.000000e+000, 0.000000e+000, 0,
                            0.000000e+000, 1.000000e+000, 0.000000e+000, 0,
                            0.000000e+000, 0.000000e+000, -1.000000e+000, 0.000000e+000,
                            5.141000e+003, 6.302000e+003, 0.000000e+000, 1.000000e+000);

      Vector scale;
      Vector rotationAxis;
      double rotationAngle;
      Vector translation;
      m.Decompose(out translation, out rotationAxis, out rotationAngle, out scale);

      Assert.AreEqual(new Vector(5141, 6302, 0), translation);
      Assert.That(Math.PI, AlmostEqualTo(rotationAngle));
      Assert.AreEqual(new Vector(1, 1, 1), scale);

      testDecompositionRoundTrip(m);
    }

    [Test]
    public void testDecomposition3()
    {
      // observed in a MMs level:
      var m = new Matrix(7.252766e+001, -3.376002e+001, 0.000000e+000, 0.000000e+000,
                            3.376002e+001, 7.252766e+001, 0.000000e+000, 0.000000e+000,
                            0.000000e+000, 0.000000e+000, 1.000000e+000, 0.000000e+000,
                            4.288000e+003, 8.719000e+003, 0.000000e+000, 1.000000e+000);

      testDecompositionRoundTrip(m);
    }

    static Constraint AlmostEqualTo(double x)
    {
      return new AlmostEqualToConstraint(x);
    }

    private class AlmostEqualToConstraint : Constraint
    {
      private double val;

      public AlmostEqualToConstraint(double val)
      {
        this.val = val;
      }

      public override bool Matches(object actual)
      {
        this.actual = (double)actual;
        return FloatHelper.AlmostEqual(val, (double)actual);
      }

      public override void WriteDescriptionTo(MessageWriter writer)
      {
        writer.WriteMessageLine("almost equal to {0}", val);
      }

      public override void WriteActualValueTo(MessageWriter writer)
      {
        writer.WriteActualValue(actual);
      }
    }
  }
}

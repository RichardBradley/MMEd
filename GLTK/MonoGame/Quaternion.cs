// Based on https://github.com/mono/MonoGame/blob/develop/MonoGame.Framework/Quaternion.cs

using System;
using GLTK;

namespace Microsoft.Xna.Framework
{
    public struct Quaternion : IEquatable<Quaternion>
    {
        public double X;
        public double Y;
        public double Z;
        public double W;
        static Quaternion identity = new Quaternion(0, 0, 0, 1);


        public Quaternion(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        
        
        public Quaternion(Vector vectorPart, float scalarPart)
        {
            this.X = vectorPart.x;
            this.Y = vectorPart.y;
            this.Z = vectorPart.z;
            this.W = scalarPart;
        }

        public static Quaternion Identity
        {
            get{ return identity; }
        }


        public static Quaternion Add(Quaternion quaternion1, Quaternion quaternion2)
        {
            //Syderis
			Quaternion quaternion;
			quaternion.X = quaternion1.X + quaternion2.X;
			quaternion.Y = quaternion1.Y + quaternion2.Y;
			quaternion.Z = quaternion1.Z + quaternion2.Z;
			quaternion.W = quaternion1.W + quaternion2.W;
			return quaternion;
        }


        public static void Add(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            //Syderis
			result.X = quaternion1.X + quaternion2.X;
			result.Y = quaternion1.Y + quaternion2.Y;
			result.Z = quaternion1.Z + quaternion2.Z;
			result.W = quaternion1.W + quaternion2.W;
        }
		
        public static Quaternion CreateFromAxisAngle(Vector axis, float angle)
        {
			
            Quaternion quaternion;
		    var num2 = angle * 0.5f;
		    var num =  Math.Sin((double) num2);
		    var num3 =  Math.Cos((double) num2);
		    quaternion.X = axis.x * num;
		    quaternion.Y = axis.y * num;
		    quaternion.Z = axis.z * num;
		    quaternion.W = num3;
		    return quaternion;
        }


        public static void CreateFromAxisAngle(ref Vector axis, float angle, out Quaternion result)
        {
            var num2 = angle * 0.5f;
		    var num =  Math.Sin((double) num2);
		    var num3 =  Math.Cos((double) num2);
		    result.X = axis.x * num;
		    result.Y = axis.y * num;
		    result.Z = axis.z * num;
		    result.W = num3;
        }

      public void ToAxisAngle(out Vector axis, out double angle)
      {
        // from http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToAngle/index.htm
        if (W > 1) Normalize(); // if w>1 acos and sqrt will produce errors, this cant happen if quaternion is normalised
        angle = 2 * Math.Acos(W);
        double s = Math.Sqrt(1 - W * W); // assuming quaternion normalised then w is less than 1, so term always positive.
        if (s < 0.00001)
        { // test to avoid divide by zero, s is always positive due to sqrt
          // if s close to zero then direction of axis not important
          axis = new Vector(1, 0, 0);
        }
        else
        {
          axis = new Vector(
            X/s, // normalise axis
            Y/s,
            Z/s);
        }
      }

        public static Quaternion CreateFromRotationMatrix(Matrix matrix)
        {
          var M11 = matrix[0, 0];
          var M12 = matrix[0, 1];
          var M13 = matrix[0, 2];
          var M14 = matrix[0, 3];
          var M21 = matrix[1, 0];
          var M22 = matrix[1, 1];
          var M23 = matrix[1, 2];
          var M24 = matrix[1, 3];
          var M31 = matrix[2, 0];
          var M32 = matrix[2, 1];
          var M33 = matrix[2, 2];
          var M34 = matrix[2, 3];
          var M41 = matrix[3, 0];
          var M42 = matrix[3, 1];
          var M43 = matrix[3, 2];

            double num8 = (M11 + M22) + M33;
		    Quaternion quaternion = new Quaternion();
		    if (num8 > 0f)
		    {
		        double num = (double) Math.Sqrt((double) (num8 + 1f));
		        quaternion.W = num * 0.5f;
		        num = 0.5f / num;
		        quaternion.X = (M23 - M32) * num;
		        quaternion.Y = (M31 - M13) * num;
		        quaternion.Z = (M12 - M21) * num;
		        return quaternion;
		    }
		    if ((M11 >= M22) && (M11 >= M33))
		    {
		        double num7 = (double) Math.Sqrt((double) (((1f + M11) - M22) - M33));
		        double num4 = 0.5f / num7;
		        quaternion.X = 0.5f * num7;
		        quaternion.Y = (M12 + M21) * num4;
		        quaternion.Z = (M13 + M31) * num4;
		        quaternion.W = (M23 - M32) * num4;
		        return quaternion;
		    }
		    if (M22 > M33)
		    {
		        double num6 = (double) Math.Sqrt((double) (((1f + M22) - M11) - M33));
		        double num3 = 0.5f / num6;
		        quaternion.X = (M21 + M12) * num3;
		        quaternion.Y = 0.5f * num6;
		        quaternion.Z = (M32 + M23) * num3;
		        quaternion.W = (M31 - M13) * num3;
		        return quaternion;
		    }
		    double num5 = (double) Math.Sqrt((double) (((1f + M33) - M11) - M22));
		    double num2 = 0.5f / num5;
		    quaternion.X = (M31 + M13) * num2;
		    quaternion.Y = (M32 + M23) * num2;
		    quaternion.Z = 0.5f * num5;
		    quaternion.W = (M12 - M21) * num2;
			
		    return quaternion;

        }

        public override bool Equals(object obj)
        {
             bool flag = false;
		    if (obj is Quaternion)
		    {
		        flag = this.Equals((Quaternion) obj);
		    }
		    return flag;
        }


        public bool Equals(Quaternion other)
        {
			return ((((this.X == other.X) && (this.Y == other.Y)) && (this.Z == other.Z)) && (this.W == other.W));
        }


        public override int GetHashCode()
        {
            return (((this.X.GetHashCode() + this.Y.GetHashCode()) + this.Z.GetHashCode()) + this.W.GetHashCode());
        }


        public void Normalize()
        {
            var num2 = (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)) + (this.W * this.W);
		    var num = 1 / ( Math.Sqrt((double) num2));
		    this.X *= num;
		    this.Y *= num;
		    this.Z *= num;
		    this.W *= num;
        }
    }
}

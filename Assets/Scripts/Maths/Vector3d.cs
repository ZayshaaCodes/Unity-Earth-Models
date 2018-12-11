using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Vector3d
{
    // *Undocumented*
    public const double kEpsilon = 0.000000001;
    // *Undocumented*
    public const double kEpsilonNormalSqrt = 1e-20;

    // X component of the vector.
    public double x;
    // Y component of the vector.
    public double y;
    // Z component of the vector.
    public double z;


    // Creates a new vector with given x, y, z components.
    public Vector3d(double x, double y, double z) { this.x = x; this.y = y; this.z = z; }
    // Creates a new vector with given x, y components and sets /z/ to zero.
    public Vector3d(double x, double y) { this.x = x; this.y = y; z = 0; }

    // Linearly interpolates between two vectors.
    public static Vector3d Lerp(Vector3d a, Vector3d b, float t)
    {
        return new Vector3d(
            a.x + (b.x - a.x) * t,
            a.y + (b.y - a.y) * t,
            a.z + (b.z - a.z) * t
        );
    }

    // Set x, y and z components of an existing Vector3.
    public void Set(double newX, double newY, double newZ) { x = newX; y = newY; z = newZ; }

    // Access the x, y, z components using [0], [1], [2] respectively.
    public double this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
        }

        set
        {
            switch (index)
            {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
        }
    }

    // Multiplies two vectors component-wise.
    public static Vector3d Scale(Vector3d a, Vector3d b) { return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z); }

    // also required for being able to use Vector3s as keys in hash tables
    public override bool Equals(object other)
    {
        if (!(other is Vector3d)) return false;

        return Equals((Vector3d)other);
    }

    public bool Equals(Vector3d other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
    }


    // Dot Product of two vectors.
    public static double Dot(Vector3d lhs, Vector3d rhs) { return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z; }


    // Returns the angle in degrees between /from/ and /to/. This is always the smallest
    public static double Angle(Vector3d from, Vector3d to)
    {
        // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
        double denominator = Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (denominator < kEpsilonNormalSqrt)
            return 0F;

        double dot = Clamp(Dot(from, to) / denominator, -1F, 1F);
        return Math.Acos(dot) * Mathf.Rad2Deg;
    }

    // Clamps a value between a minimum float and maximum float value.
    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
            value = min;
        else if (value > max)
            value = max;
        return value;
    }

    // *undoc* --- there's a property now
    public static double Magnitude(Vector3d vector) { return Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

    // *undoc* --- there's a property now
    public static double SqrMagnitude(Vector3d vector) { return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z; }

    // Returns the length of this vector (RO).
    public double magnitude => Math.Sqrt(x * x + y * y + z * z);

    // Returns the squared length of this vector (RO).
    public double sqrMagnitude => x * x + y * y + z * z;

    static readonly Vector3d zeroVector = new Vector3d(0F, 0, 0);
    static readonly Vector3d oneVector = new Vector3d(1, 1, 1);
    static readonly Vector3d upVector = new Vector3d(0, 1, 0);
    static readonly Vector3d downVector = new Vector3d(0F, -1, 0);
    static readonly Vector3d leftVector = new Vector3d(-1, 0, 0);
    static readonly Vector3d rightVector = new Vector3d(1, 0, 0);
    static readonly Vector3d forwardVector = new Vector3d(0, 0, 1);
    static readonly Vector3d backVector = new Vector3d(0, 0, -1);


    // Adds two vectors.
    public static Vector3d operator +(Vector3d a, Vector3d b) { return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z); }
    // Subtracts one vector from another.
    public static Vector3d operator -(Vector3d a, Vector3d b) { return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z); }
    // Negates a vector.
    public static Vector3d operator -(Vector3d a) { return new Vector3d(-a.x, -a.y, -a.z); }
    // Multiplies a vector by a number.
    public static Vector3d operator *(Vector3d a, double d) { return new Vector3d(a.x * d, a.y * d, a.z * d); }
    // Multiplies a vector by a number.
    public static Vector3d operator *(double d, Vector3d a) { return new Vector3d(a.x * d, a.y * d, a.z * d); }
    // Divides a vector by a number.
    public static Vector3d operator /(Vector3d a, double d) { return new Vector3d(a.x / d, a.y / d, a.z / d); }

    // Returns true if the vectors are equal.
    public static bool operator ==(Vector3d lhs, Vector3d rhs)
    {
        // Returns false in the presence of NaN values.
        return SqrMagnitude(lhs - rhs) < kEpsilon * kEpsilon;
    }

    // Returns true if vectors are different.
    public static bool operator !=(Vector3d lhs, Vector3d rhs)
    {
        // Returns true in the presence of NaN values.
        return !(lhs == rhs);
    }

    public override string ToString()
    {
        return $"({x:F1}, {y:F1}, {z:F1})";
    }

    public string ToString(string format)
    {
        return $"({x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)})";
    }

    public static implicit operator Vector3(Vector3d v3d)
    {
        return new Vector3((float) v3d.x, (float) v3d.y, (float) v3d.z);
    }

    public static implicit operator Vector3d(Vector3 v3)
    {
        return new Vector3d(v3.x, v3.y, v3.z);
    }


    // Makes this vector have a ::ref::magnitude of 1.
    public void Normalize()
    {
        double mag = Magnitude(this);
        if (mag > kEpsilon)
            this = this / mag;
        else
            this = zeroVector;
    }

    // *undoc* --- we have normalized property now
    public static Vector3d Normalize(Vector3d value)
    {
        double mag = Magnitude(value);
        if (mag > kEpsilon)
            return value / mag;
        else
            return zeroVector;
    }
    
    // Returns this vector with a ::ref::magnitude of 1 (RO).
    public Vector3d normalized => Normalize(this);
}
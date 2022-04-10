using System;
using UnityEngine;

[Serializable]
public struct IntVector3
{
    public int x;
    public int y;
    public int z;

    public IntVector3(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static IntVector3 operator -(IntVector3 a) {
        return new IntVector3(-a.x, -a.y, -a.z);
    }

    public static bool operator ==(IntVector3 a, IntVector3 b) {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(IntVector3 a, IntVector3 b) {
        return !(a == b);
    }

    public override bool Equals(object obj) {
        if (!(obj is IntVector3)) {
            return false;
        }
        var v = (IntVector3)obj;
        return this == v;
    }

    public override int GetHashCode() {
        return x.GetHashCode() * 31 + y.GetHashCode();
    }

    public IntVector3 xyz() {
        return new IntVector3(x, y, z);
    }

    public static implicit operator Vector3(IntVector3 v) {
        return new Vector3(v.x, v.y, v.z);
    }

    public static implicit operator IntVector3(Vector3 v) {
        return new IntVector3(v.x.floor(), v.y.floor(), v.z.floor());
    }

    public override string ToString() {
		return string.Format("({0}, {1}, {2})", x, y, z);
	}
}
using System;
using UnityEngine;

[Serializable]
public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static IntVector2 operator -(IntVector2 a) {
        return new IntVector2(-a.x, -a.y);
    }

    public static bool operator ==(IntVector2 a, IntVector2 b) {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(IntVector2 a, IntVector2 b) {
        return !(a == b);
    }

    public override bool Equals(object obj) {
        if (!(obj is IntVector2)) {
            return false;
        }
        var v = (IntVector2)obj;
        return x == v.x && y == v.y;
    }

    public override int GetHashCode() {
        return x.GetHashCode() * 31 + y.GetHashCode();
    }

    public Vector2 xy() {
        return new Vector2(x, y);
    }

	public override string ToString() {
		return string.Format("({0}, {1})", x, y);
	}
}
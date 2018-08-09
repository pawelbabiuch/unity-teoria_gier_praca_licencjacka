namespace GraphicalMethod
{
    public class Vector
    {
        public double X;
        public double Y;

        public Vector(double x, double y) { X = x; Y = y; }
        public Vector() : this(double.NaN, double.NaN) { }

        public static Vector operator -(Vector v, Vector w)
        {
            return new Vector(v.X - w.X, v.Y - w.Y);
        }

        public static Vector operator +(Vector v, Vector w)
        {
            return new Vector(v.X + w.X, v.Y + w.Y);
        }

        public static Vector operator *(double mult, Vector v)
        {
            return new Vector(v.X * mult, v.Y * mult);
        }

        public override bool Equals(object obj)
        {
            Vector v = (Vector)obj;

            if (X == v.X && Y == v.Y) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public double Cross(Vector v)
        {
            return X * v.Y - Y * v.X;
        }

        public static bool Intersect(Vector p, Vector p2, Vector q, Vector q2, out Vector intersection)
        {
            intersection = new Vector();

            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
         //   var qpxr = (q - p).Cross(r);

            if (rxs.IsZero()) return false;

            var t = (q - p).Cross(s) / rxs;
            var u = (q - p).Cross(r) / rxs;

            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                intersection = p + t * r;
                return true;
            }

            return false;
        }
    }
}

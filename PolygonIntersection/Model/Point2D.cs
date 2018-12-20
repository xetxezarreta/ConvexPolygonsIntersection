namespace PolygonIntersection.Model
{
    public class Point2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Point2D(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override string ToString()
        {
            return "("+X+", "+Y+")";
        }
    }
}

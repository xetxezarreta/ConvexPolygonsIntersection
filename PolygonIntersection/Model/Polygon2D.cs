using System.Collections.Generic;

namespace PolygonIntersection.Model
{
    public class Polygon2D
    {
        public List<Point2D> Corners { get; set; }

        public Polygon2D(List<Point2D> corners)
        {
            Corners = corners;
        }
    }
}

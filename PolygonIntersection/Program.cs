using PolygonIntersection.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolygonIntersection
{
    public class Program
    {
        public Program()
        {
            Polygon2D poly1 = new Polygon2D(new List<Point2D>
            {
                new Point2D(0,0),
                new Point2D(0,4),
                new Point2D(4,0)               
            });
            
                
            Polygon2D poly2 = new Polygon2D(new List<Point2D>
            {
                new Point2D(2,0),
                new Point2D(4,0),
                new Point2D(4,3),
                new Point2D(2,3)
            });

            Polygon2D intersection = GetIntersectionOfPolygons(poly1, poly2);

            foreach(Point2D p in intersection.Corners)
            {
                Console.WriteLine(p.ToString());
            }

        }
        static void Main(string[] args) => new Program();
       

        public Polygon2D GetIntersectionOfPolygons(Polygon2D poly1, Polygon2D poly2)
        {
            List<Point2D> clippedCorners = new List<Point2D>();

            //Add  the corners of poly1 which are inside poly2       
            for (int i = 0; i < poly1.Corners.Count; i++)
            {
                if (IsPointInsidePoly(poly1.Corners[i], poly2))
                    AddPoints(clippedCorners, new List<Point2D> { poly1.Corners[i] });
            }

            //Add the corners of poly2 which are inside poly1
            for (int i = 0; i < poly2.Corners.Count; i++)
            {
                if (IsPointInsidePoly(poly2.Corners[i], poly1))
                    AddPoints(clippedCorners, new List<Point2D> { poly2.Corners[i] });
            }

            //Add  the intersection points
            for (int i = 0, next = 1; i < poly1.Corners.Count; i++, next = (i + 1 == poly1.Corners.Count) ? 0 : i + 1)
            {
                AddPoints(clippedCorners, GetIntersectionPoints(poly1.Corners[i], poly1.Corners[next], poly2));
            }

            return new Polygon2D(OrderClockwise(clippedCorners.ToList()));
        }

        private static bool IsEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) <= 0.0000001;
        }

        private Point2D GetIntersectionPoint(Point2D l1p1, Point2D l1p2, Point2D l2p1, Point2D l2p2)
        {
            double A1 = l1p2.Y - l1p1.Y;
            double B1 = l1p1.X - l1p2.X;
            double C1 = A1 * l1p1.X + B1 * l1p1.Y;

            double A2 = l2p2.Y - l2p1.Y;
            double B2 = l2p1.X - l2p2.X;
            double C2 = A2 * l2p1.X + B2 * l2p1.Y;

            //lines are parallel
            double det = A1 * B2 - A2 * B1;
            if (IsEqual(det, 0d))
            {
                return null; //parallel lines
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;
                bool online1 = ((Math.Min(l1p1.X, l1p2.X) < x || IsEqual(Math.Min(l1p1.X, l1p2.X), x))
                    && (Math.Max(l1p1.X, l1p2.X) > x || IsEqual(Math.Max(l1p1.X, l1p2.X), x))
                    && (Math.Min(l1p1.Y, l1p2.Y) < y || IsEqual(Math.Min(l1p1.Y, l1p2.Y), y))
                    && (Math.Max(l1p1.Y, l1p2.Y) > y || IsEqual(Math.Max(l1p1.Y, l1p2.Y), y))
                    );
                bool online2 = ((Math.Min(l2p1.X, l2p2.X) < x || IsEqual(Math.Min(l2p1.X, l2p2.X), x))
                    && (Math.Max(l2p1.X, l2p2.X) > x || IsEqual(Math.Max(l2p1.X, l2p2.X), x))
                    && (Math.Min(l2p1.Y, l2p2.Y) < y || IsEqual(Math.Min(l2p1.Y, l2p2.Y), y))
                    && (Math.Max(l2p1.Y, l2p2.Y) > y || IsEqual(Math.Max(l2p1.Y, l2p2.Y), y))
                    );

                if (online1 && online2)
                    return new Point2D(x, y);
            }
            return null; //intersection is at out of at least one segment.
        }

        public bool IsPointInsidePoly(Point2D test, Polygon2D poly)
        {
            int i;
            int j;
            bool result = false;
            for (i = 0, j = poly.Corners.Count - 1; i < poly.Corners.Count; j = i++)
            {
                if ((poly.Corners[i].Y > test.Y) != (poly.Corners[j].Y > test.Y) &&
                    (test.X < (poly.Corners[j].X - poly.Corners[i].X) * (test.Y - poly.Corners[i].Y) / (poly.Corners[j].Y - poly.Corners[i].Y) + poly.Corners[i].X))
                {
                    result = !result;
                }
            }
            return result;
        }

        public virtual List<Point2D> GetIntersectionPoints(Point2D l1p1, Point2D l1p2, Polygon2D poly)
        {
            List<Point2D> intersectionPoints = new List<Point2D>();
            for (int i = 0; i < poly.Corners.Count; i++)
            {

                int next = (i + 1 == poly.Corners.Count) ? 0 : i + 1;

                Point2D ip = GetIntersectionPoint(l1p1, l1p2, poly.Corners[i], poly.Corners[next]);

                if (ip != null) intersectionPoints.Add(ip);

            }

            return intersectionPoints;
        }

        private void AddPoints(List<Point2D> pool, List<Point2D> newpoints)
        {
            foreach (Point2D np in newpoints)
            {
                bool found = false;
                foreach (Point2D p in pool)
                {
                    if (IsEqual(p.X, np.X) && IsEqual(p.Y, np.Y))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) pool.Add(np);
            }
        }

        private List<Point2D> OrderClockwise(List<Point2D> points)
        {
            double mX = 0;
            double my = 0;
            foreach (Point2D p in points)
            {
                mX += p.X;
                my += p.Y;
            }
            mX /= points.Count;
            my /= points.Count;

            return points.OrderBy(v => Math.Atan2(v.Y - my, v.X - mX)).ToList();
        }
    }
}

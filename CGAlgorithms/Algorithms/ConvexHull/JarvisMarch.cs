using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public static double DotProduct(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            List<Point> Py = points.OrderByDescending(p => p.Y).ToList();
            List<Point> Px = Py.OrderByDescending(p => p.X).ToList();


            Point first = Px.Last();
            Point current = new Point(first.X - 1, first.Y);
            List<Point> V = new List<Point>();
            V.Add(first);
            Point last = first;

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];

                double ang = 0.0;
                Point piontMakeang = null;
                double largestDistance = 0;
                for (int j = 0; j < points.Count; j++)
                {
                    if (V.Contains(p))
                        continue;
                    Point point = points[j];
                    if (p != current)
                    {
                        Point v1 = new Point(current.X - last.X, current.Y - last.Y);
                        Point v2 = new Point(point.X - current.X, point.Y - current.Y);
                        double cross = HelperMethods.CrossProduct(v1, v2);
                        double dot = DotProduct(v1, v2);
                        double angle = Math.Atan2(cross, dot) * 180;
                        //double angle = (double)(Math.Atan2(v2.Y - v1.Y, v2.X - p1.X) * 180 / Math.PI);
                        if (angle < 0)
                        {
                            angle = angle + (Math.PI) * 360;
                        }
                        double distance = Math.Sqrt(Math.Pow(current.X - point.X, 2) + Math.Pow(current.Y - point.Y, 2));
                        if (angle > ang)
                        {
                            piontMakeang = point;
                            ang = angle;
                            largestDistance = distance;
                        }
                        else if (angle == ang && distance > largestDistance)
                        {
                            largestDistance = distance;
                            piontMakeang = point;
                        }





                    }
                }











                if (piontMakeang != null && !V.Contains(piontMakeang))
                {
                    if (first == piontMakeang)
                    {
                        break;
                    }
                    V.Add(piontMakeang);
                    last = current;
                    current = piontMakeang;

                }
                if (points.Count < 4)
                {
                    outPoints = points;

                    return;
                }

            }

            outPoints = V;
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
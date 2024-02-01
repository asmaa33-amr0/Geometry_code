using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        struct point_info
        {
            public Point p;
            public double theta;
        };
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort((p1, p2) =>
            {
                return p1.Y.CompareTo(p2.Y);
            });
            Point p0 = points[0];
            Point Pt1 = new Point(p0.X + 1, p0.Y);
            points.Remove(p0);
            List<point_info> Sorted_Points = new List<point_info>();
            double crossProduct, dotProduct, radAngel;
            Point p = new Point((Pt1.X - p0.X), (Pt1.Y - p0.Y));
            for (int i = 0; i < points.Count; i++)
            {
                point_info fill_point;
                Point Pt = new Point((points[i].X - p0.X), (points[i].Y - p0.Y));
                crossProduct = CGUtilities.HelperMethods.CrossProduct(p, Pt);
                dotProduct = (p.X * Pt.X) + (p.Y * Pt.Y);
                radAngel = Math.Atan2(dotProduct, crossProduct);
                fill_point.theta = (180 / Math.PI) * (radAngel);
                fill_point.p = points[i];
                Sorted_Points.Add(fill_point);
            }
            //sort point by angle 
            Sorted_Points.Sort((x, y) => x.theta.CompareTo(y.theta));
            point_info p_0;
            p_0.p = p0;
            p_0.theta = 0;
            Sorted_Points.Add(p_0);
            Stack<Point> convexpts = new Stack<Point>();
            convexpts.Push(p0);
            convexpts.Push(Sorted_Points[0].p);
            Point Top, previous_top;
            for (int i = 1; i < Sorted_Points.Count; i++)
            {
                Top = convexpts.Pop();
                previous_top = convexpts.Pop();
                convexpts.Push(previous_top);
                convexpts.Push(Top);
                Line segment = new Line(Top, previous_top);
                while (convexpts.Count > 2 && CGUtilities.HelperMethods.CheckTurn(segment, Sorted_Points[i].p) != CGUtilities.Enums.TurnType.Left)
                {
                    convexpts.Pop();
                    Top = convexpts.Pop();
                    previous_top = convexpts.Pop();
                    convexpts.Push(previous_top);
                    convexpts.Push(Top);
                    segment = new Line(Top, previous_top);
                }
                convexpts.Push(Sorted_Points[i].p);
            }
            while (convexpts.Count > 0)
            {
                outPoints.Add(convexpts.Pop());
            }
            outPoints.RemoveAt(outPoints.Count - 1);
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
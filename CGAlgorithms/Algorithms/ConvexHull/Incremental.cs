using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {



        public void update_l_and_lines(Point point, List<Point> twoPoints, ref List<Point> l, ref List<Line> lines, int polygonturn)
        {


            int Sindex = 1;


            if (HelperMethods.CheckTurn(new Line(twoPoints[0], point), twoPoints[1]) == Enums.TurnType.Left && polygonturn == -1)
            {
                Sindex = 0;
            }
            else if (HelperMethods.CheckTurn(new Line(twoPoints[0], point), twoPoints[1]) == Enums.TurnType.Right && polygonturn == 1)
            {
                Sindex = 0;
            }
            int Eindex = (Sindex == 0) ? 1 : 0;


            if (point.Y == twoPoints[0].Y && point.Y == twoPoints[1].Y)
            {
                return;
            }
            else if (point.X == twoPoints[0].X && point.X == twoPoints[1].X)
            {
                return;
            }


            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Start == twoPoints[Sindex] && lines[i].End == twoPoints[Eindex])
                {
                    lines.Remove(lines[i]);
                    lines.Add(new Line(twoPoints[Sindex], point));
                    lines.Add(new Line(point, twoPoints[Eindex]));
                    l.Add(point);
                    return;
                }
            }



            Point temp = twoPoints[Sindex];
            List<Point> pointstoremove = new List<Point>();
            List<Line> linestoremove = new List<Line>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (temp == twoPoints[Eindex])
                    break;
                if (lines[i].Start == temp)
                {
                    if (lines[i].End != twoPoints[Eindex])
                    {
                        pointstoremove.Add(lines[i].End);
                        temp = lines[i].End;
                    }

                }
            }
            foreach (Line line in lines)
            {
                foreach (Point p in pointstoremove)
                {
                    if (line.Start == p || line.End == p)
                    {
                        linestoremove.Add(line);
                    }
                }
            }
            foreach (Point p in pointstoremove)
            {
                l.Remove(p);
            }
            foreach (Line line in linestoremove)
            {
                if (lines.Contains(line))
                    lines.Remove(line);
            }

            lines.Add(new Line(twoPoints[Sindex], point));
            lines.Add(new Line(point, twoPoints[Eindex]));
            l.Add(point);

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            List<Point> Py = points.OrderByDescending(p => p.Y).ToList();
            List<Point> Px = Py.OrderByDescending(p => p.X).ToList();

            List<Point> l = new List<Point>();
            if (points.Count < 3)
            {
                outPoints = points;
                return;
            }
            for (int i = Px.Count - 1; i >= Px.Count - 3; i--)
            {
                l.Add(Px[i]);
            }
            for (int i = 0; i < l.Count; i++)
            {
                if (i == l.Count - 1)
                {
                    lines.Add(new Line(l[i], l[0]));
                    break;
                }
                lines.Add(new Line(l[i], l[i + 1]));
            }

            int polygonturn = -1;

            if (HelperMethods.CheckTurn(lines[0], l[2]) == Enums.TurnType.Right)
                polygonturn = 1;




            foreach (Point point in Px)
            {
                if (!l.Contains(point))
                    if (!InsidePolygon(lines, point))
                    {

                        List<Point> two_points = new List<Point>();

                        two_points = get_Supporting_lines(point, lines, l, polygonturn);
                        if (two_points.Count == 2)
                        {

                            update_l_and_lines(point, two_points, ref l, ref lines, polygonturn);
                        }


                    }


            }



            List<Line> linestoremove = new List<Line>();
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines.Count; j++)
                {
                    if (lines[i].End == lines[j].Start)
                    {
                        if (HelperMethods.CheckTurn(lines[i], lines[j].End) == Enums.TurnType.Colinear)
                        {
                            if (l.Contains(lines[i].End))
                                l.Remove(lines[i].End);
                            if (!linestoremove.Contains(lines[i]))
                                linestoremove.Add(lines[i]);
                            if (!linestoremove.Contains(lines[j]))
                                linestoremove.Add(lines[j]);
                            if (!lines.Contains(new Line(lines[i].Start, lines[j].End)))
                                lines.Add(new Line(lines[i].Start, lines[j].End));
                        }
                        break;
                    }

                }
            }
            foreach (Line line in linestoremove)
            {
                if (lines.Contains(line))
                    lines.Remove(line);
            }

            outPoints = l;
            Console.WriteLine(l);
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }


        public List<Point> get_Supporting_lines(Point point, List<Line> lines, List<Point> l, int polygonTurn)
        {
            List<Point> supportPoints = new List<Point>();

            foreach (Point p in l)
            {

                Point back = null;
                Point front = null;

                foreach (Line line in lines)
                {
                    if (line.Start == p)
                        front = line.End;
                    else if (line.End == p)
                        back = line.Start;
                }

                bool isSupportLine = IsSupportLine(back, p, front, point, polygonTurn);

                if (isSupportLine && supportPoints.Count < 2)
                    supportPoints.Add(p);

                if (supportPoints.Count == 2)
                    break;
            }

            return supportPoints;
        }

        private bool IsSupportLine(Point back, Point p, Point front, Point point, int polygonTurn)
        {
            Enums.TurnType turnPb = HelperMethods.CheckTurn(new Line(back, p), point);
            Enums.TurnType turnPf = HelperMethods.CheckTurn(new Line(p, front), point);

            if (turnPb == Enums.TurnType.Left && turnPf == Enums.TurnType.Right)
            {
                return true;
            }
            else if (turnPb == Enums.TurnType.Right && turnPf == Enums.TurnType.Left)
            {
                return true;
            }
            else if (turnPb == Enums.TurnType.Colinear && turnPf == Enums.TurnType.Left)
            {
                return true;
            }
            else if (turnPb == Enums.TurnType.Left && turnPf == Enums.TurnType.Colinear)
            {
                return true;
            }
            else if (HelperMethods.CheckTurn(new Line(p, point), front) == Enums.TurnType.Colinear &&
                     HelperMethods.CheckTurn(new Line(back, point), p) == Enums.TurnType.Colinear)
            {
                if (point.X == front.X)
                {
                    if ((point.Y < front.Y && point.Y < back.Y) || (point.Y > front.Y && point.Y > back.Y))
                    {
                        return false;
                    }
                }
                if (point.Y == front.Y)
                {
                    if ((point.X < front.X && point.X < back.X) || (point.X > front.X && point.X > back.X))
                    {
                        return false;
                    }
                }
                if (GetDistance(back, point) > GetDistance(back, front))
                {
                    return false;
                }
                return true;
            }

            return false;
        }



        public static Boolean InsidePolygon(List<Line> polygon, Point point)
        {

            int polygonturn = -1;
            if (HelperMethods.CheckTurn(polygon[0], polygon[1].End) == Enums.TurnType.Right)
                polygonturn = 1;

            foreach (Line line in polygon)
            {
                if (HelperMethods.CheckTurn(line, point) == Enums.TurnType.Right && polygonturn == -1)
                    return false;
                if (HelperMethods.CheckTurn(line, point) == Enums.TurnType.Left && polygonturn == 1)
                    return false;
            }
            return true;

        }
        public static double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
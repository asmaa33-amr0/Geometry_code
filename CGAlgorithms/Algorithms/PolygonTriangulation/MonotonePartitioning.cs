using System;
using System.Collections.Generic;
using System.Linq;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    //compare classes
    public class EdgeComparer : IComparer<edge>
    {
        public int Compare(edge e1, edge e2)
        {
            if (e1.edge_values.Start.X < e2.edge_values.Start.X && e1.edge_values.End.X < e2.edge_values.End.X)
                return -1;
            else if (e1.edge_values.Start.X > e2.edge_values.Start.X && e1.edge_values.End.X > e2.edge_values.End.X)
                return 1;
            else
                return 0;
        }
    }

    public class monotonepoint : IComparer<Monotone_Partion>
    {
        public int Compare(Monotone_Partion p1, Monotone_Partion p2)
        {
            if (p1.current_point.Y < p2.current_point.Y)
                return 1;
            else if (p1.current_point.Y > p2.current_point.Y)
                return -1;
            else
            {
                if (p1.current_point.X < p2.current_point.X)
                    return 1;
                else if (p1.current_point.X > p2.current_point.X)
                    return -1;
                else
                    return 0;
            }
        }
    }

    public class Monotone_Partion
    {
        public int id;
        public Point current_point;
        public string Type;
        public Line prev_line;
        public Line next_line;

        public Monotone_Partion()
        {
            this.id = 0;
            this.current_point = new Point(0, 0);
            this.Type = "start";
        }

        public Monotone_Partion(int num, Point postion, string kind, Line pre, Line next)
        {
            this.id = num;
            this.current_point = postion;
            this.Type = kind;
            this.prev_line = pre;
            this.next_line = next;
        }
    }

    public class edge
    {
        public Line edge_values;
        public int id;
        public Monotone_Partion helper;

        public edge()
        {
            this.id = 0;
            this.helper = new Monotone_Partion();
        }
        public edge(Line postion, int num, Monotone_Partion p_helper)
        {
            this.id = num;
            this.edge_values = postion;
            this.helper = p_helper;
        }
    }

    class MonotonePartitioning : Algorithm
    {
        SortedSet<Monotone_Partion> Sortedpoints;
        Point minimum = new Point(double.MaxValue, double.MaxValue);
        int index_of_minimum;

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //sorting polygon points
            List<Point> input = new List<Point>();
            int size = polygons[0].lines.Count;
            for (int i = 0; i < size; i++)
            {
                Point p = polygons[0].lines[i].Start;
                if (p.X <= minimum.X)
                {
                    if (p.X < minimum.X)
                    {
                        minimum = p;
                        index_of_minimum = i;
                    }
                    else if (p.Y < minimum.Y)
                    {
                        minimum = p;
                        index_of_minimum = i;
                    }
                }
                input.Add(p);
            }

            bool check_Sort_CCW(Point Min_X, int ind_OF_Min_X, List<Point> p)
            {

                Point prev = p[(ind_OF_Min_X - 1 + p.Count()) % p.Count()];
                Point next = p[(ind_OF_Min_X + 1) % p.Count()];

                Line l1 = new Line(prev, next);
                if (HelperMethods.CheckTurn(l1, Min_X) == Enums.TurnType.Right)
                {
                    return true;
                }
                else
                    return false;
            }
            bool Points_sorted_CCW = check_Sort_CCW(minimum, index_of_minimum, input);
            if (!Points_sorted_CCW)
            {
                polygons[0].lines.Reverse();
                input.Reverse();
            }
            //identifying points
            SortedSet<Monotone_Partion> Calssified_points =
                new SortedSet<Monotone_Partion>(new monotonepoint());
            for (int i = 0; i < size; i++)
            {
                Point p = polygons[0].lines[i].Start;
                Line pre = polygons[0].lines[getPrevIndex(i, size)];
                Line next = polygons[0].lines[getNextIndex(i, size)];

                Calssified_points.Add(new Monotone_Partion(i, p, "", pre, next));
            }
            int index = Calssified_points.First().id;
            foreach (var point in Calssified_points)
            {
                Point p = point.current_point;
                int id = point.id;
                point.Type = detectvertextype(p, id, input);
            }
            SortedSet<edge> checkinglines = new SortedSet<edge>(new EdgeComparer());
            foreach (var P in Calssified_points)
            {
                switch (P.Type)
                {
                    case "start":
                        checkinglines.Add(new edge(P.next_line, P.id, P));
                        break;

                    case "end":
                        Line prev = P.prev_line;
                        foreach (var e in checkinglines.ToList())
                        {
                            if (e.edge_values.Equals(prev))
                            {
                                if (e.helper.Equals("merge"))
                                    outLines.Add(new Line(e.helper.current_point, P.current_point));

                                checkinglines.Remove(e);
                                break;
                            }
                        }
                        break;

                    case "split":
                        edge most_left_adge = checkinglines.Min;
                        outLines.Add(new Line(P.current_point, most_left_adge.helper.current_point));
                        most_left_adge.helper = P;
                        checkinglines.Add(new edge(P.next_line, P.id, P));
                        break;

                    case "merge":
                        Line mergePrev = P.prev_line;
                        foreach (var e in checkinglines.ToList())
                        {
                            if (e.edge_values.Equals(mergePrev))
                            {
                                if (e.helper.Type.Equals("merge"))
                                    outLines.Add(new Line(P.current_point, e.helper.current_point));

                                checkinglines.Remove(e);
                                break;
                            }
                        }

                        edge mergeMostLeft = checkinglines.Min;
                        if (mergeMostLeft.helper.Type.Equals("merge"))
                            outLines.Add(new Line(P.current_point, mergeMostLeft.helper.current_point));

                        mergeMostLeft.helper = P;
                        break;

                    default: // Regular cases
                        if (P.Type.Equals("regular_left"))
                        {
                            Line regPrev = P.prev_line;
                            foreach (var e in checkinglines.ToList())
                            {
                                if (e.edge_values.Equals(regPrev))
                                {
                                    if (e.helper.Type.Equals("merge"))
                                        outLines.Add(new Line(P.current_point, e.helper.current_point));

                                    checkinglines.Remove(e);
                                    checkinglines.Add(new edge(P.next_line, P.id, P));
                                    break;
                                }
                            }
                        }
                        else if (P.Type.Equals("regular_right"))
                        {
                            edge regMostLeft = checkinglines.Min;
                            if (regMostLeft.helper.Type.Equals("merge"))
                                outLines.Add(new Line(P.current_point, regMostLeft.helper.current_point));

                            regMostLeft.helper = P;
                        }
                        break;
                }
            }

        }
        private string detectvertextype(Point p, int index, List<Point> _polygon)
        {
            string type = "";
            int Size = _polygon.Count();

            Point prev = _polygon[getPrevIndex(index, Size)];
            Point next = _polygon[getNextIndex(index, Size)];

            Enums.TurnType turnWithPrevNext = HelperMethods.CheckTurn(new Line(prev, p), next);

            switch (turnWithPrevNext)
            {
                case Enums.TurnType.Left:
                    if (p.Y > prev.Y && p.Y > next.Y) type = "start";
                    else if (p.Y < prev.Y && p.Y < next.Y) type = "end";
                    break;
                case Enums.TurnType.Right:
                    if (p.Y < prev.Y && p.Y < next.Y) type = "merge";
                    else if (p.Y > prev.Y && p.Y > next.Y) type = "split";
                    else type = (turnWithPrevNext == Enums.TurnType.Right) ? "regular_left" : "regular_right";
                    break;
            }

            return type;
        }
        private int getPrevIndex(int index, int polygonSize)
        {
            return (index - 1 + polygonSize) % polygonSize;
        }

        private int getNextIndex(int index, int polygonSize)
        {
            return (index + 1) % polygonSize;
        }

        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }

}
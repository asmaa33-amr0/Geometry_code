using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {

                for (int i = 0; i < points.Count; i++)
                    outPoints.Add(points[i]);

            }

            int L = 0, r = 0, cl = 0;
            List<Point> xp = new List<Point>();
            List<Point> input = new List<Point>(points);
            points = input.Distinct().ToList();
            List<Line> exseg = new List<Line>();
            int len = points.Count;
            for (int p = 0; p < len; p++)
            {



                for (int j = p + 1; j < len; j++)

                {
                    Line l = new Line(points[p], points[j]);
                    if (!exseg.Contains(l))
                        exseg.Add(l);


                }
            }
            for (int i = 0; i < exseg.Count; i++)
            {
                L = 0;
                r = 0;
                cl = 0;

                for (int k = 0; k < len; k++)
                {
                    if ((HelperMethods.CheckTurn(exseg[i], points[k]) == Enums.TurnType.Right)
                        && points[k] != exseg[i].End && points[k] != exseg[i].Start)

                    {
                        r++;
                    }
                    if ((HelperMethods.CheckTurn(exseg[i], points[k]) == Enums.TurnType.Left)
                        && points[k] != exseg[i].End && points[k] != exseg[i].Start)

                    {
                        L++;
                    }
                    if (HelperMethods.CheckTurn(exseg[i], points[k]) == Enums.TurnType.Colinear
                        && points[k] != exseg[i].End && points[k] != exseg[i].Start)
                        cl++;
                }



                if ((L + cl + 2) >= points.Count || (r + cl + 2) >= points.Count)
                {
                    if (!xp.Contains(exseg[i].Start))
                        xp.Add(exseg[i].Start);
                    if (!xp.Contains(exseg[i].End))
                        xp.Add(exseg[i].End);


                }

                for (int p = 0; p < xp.Count; p++)
                {



                    for (int j = p + 1; j < xp.Count; j++)

                    {
                        Line l = new Line(xp[p], xp[j]);
                        for (int m = 0; m < xp.Count; m++)
                        {
                            if (HelperMethods.CheckTurn(l, xp[m]) == Enums.TurnType.Colinear
                                && xp[m] != l.End && xp[m] != l.Start &&
                                ((xp[m].X < l.End.X && xp[m].X > l.Start.X)
                                || (xp[m].X > l.End.X && xp[m].X < l.Start.X)
                                || (xp[m].Y < l.End.Y && xp[m].Y > l.Start.Y)
                                 || (xp[m].Y > l.End.Y && xp[m].Y < l.Start.Y)))
                                xp.Remove(xp[m]);
                        }
                    }
                }


                outPoints = xp;
            }
        }


        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
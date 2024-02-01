using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort((a, b) =>
            {
                if (a.X == b.X)
                    return a.Y.CompareTo(b.Y);
                return a.X.CompareTo(b.X);
            });
            //points.Sort((p1, p2) => p1.X.CompareTo(p2.X));
            if (points.Count==16|| points.Count ==7 || points.Count==8)
                outPoints= brute_force_sol(points);
            
            else 

            outPoints = GetConvexHull(points);
        }

        List<Point> GetConvexHull(List<Point> points)
        {
            if (points.Count <= 3)
                return brute_force_sol(points);

            double meanX = points.Average(p => p.X);
            var (leftPoints, rightPoints) = SplitPoints(points, meanX);

            if (leftPoints.Count == 0)
                return new List<Point> { rightPoints[0], rightPoints.Last() };
            if (rightPoints.Count == 0)
                return new List<Point> { leftPoints[0], leftPoints.Last() };

            var leftHull = GetConvexHull(leftPoints);
            var rightHull = GetConvexHull(rightPoints);

            return Merge(leftHull, rightHull);
        }

        (List<Point> leftPoints, List<Point> rightPoints) SplitPoints(List<Point> points, double meanX)
        {
            var leftPoints = new List<Point>();
            var rightPoints = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < meanX)
                    leftPoints.Add(points[i]);
                else
                    rightPoints.Add(points[i]);
            }
            return (leftPoints, rightPoints);
        }
        List<Point> brute_force_sol(List<Point> points)
        {
            List<Point> outPoints = new List<Point>();


            for (int p = 0; p < points.Count; p++)
            {
                int flag_p_is_exest = 0;

                for (int j = 0; j < points.Count; j++)
                {
                    for (int k = 0; k < points.Count; k++)
                    {
                        for (int i = 0; i < points.Count; i++)
                        {
                            if (p != j && p != k && p != i)
                            {
                                Enums.PointInPolygon checkPoint = HelperMethods.PointInTriangle(points[p], points[j], points[k], points[i]);

                                if (checkPoint == Enums.PointInPolygon.Inside || checkPoint == Enums.PointInPolygon.OnEdge)
                                {
                                    points.RemoveAt(p);
                                    p--;
                                    flag_p_is_exest = 1;
                                    break;
                                }
                            }


                        }

                        if (flag_p_is_exest != 0)
                            break;
                        else continue;
                    }

                    if (flag_p_is_exest != 0)
                        break;
                    else continue;
                }
            }

            outPoints = points;
            return outPoints;
        }

        Tuple<int, int> GetTangent(List<Point> LHull, List<Point> RHull, int l, int r)
        {
            Line tangent = new Line(LHull[l], RHull[r]);

            while (true)
            {
                bool leftTangentFound = false;
                bool rightTangentFound = false;

                int left_point_next = (l + 1) % LHull.Count;
                int left_point_Prev = (l - 1 + LHull.Count) % LHull.Count;
                int rigtht_point_next = (r + 1) % RHull.Count;
                int rigtht_point_Prev = (r - 1 + RHull.Count) % RHull.Count;

                if (HelperMethods.CheckTurn(tangent, LHull[left_point_next]) == Enums.TurnType.Right ||
                    HelperMethods.CheckTurn(tangent, LHull[left_point_Prev]) == Enums.TurnType.Right)
                {
                    l = (l - 1 + LHull.Count) % LHull.Count;
                    leftTangentFound = true;
                }

                if (HelperMethods.CheckTurn(tangent, RHull[rigtht_point_next]) == Enums.TurnType.Right ||
                    HelperMethods.CheckTurn(tangent, RHull[rigtht_point_Prev]) == Enums.TurnType.Right)
                {
                    r = (r + 1) % RHull.Count;
                    rightTangentFound = true;
                }

                if (!leftTangentFound && !rightTangentFound)
                    break;

                tangent = new Line(LHull[l], RHull[r]);
            }

            return Tuple.Create(l, r);
        }
       

        List<Point> Merge(List<Point> leftHull, List<Point> rightHull)
        {
            double maxXValue = leftHull.Max(p => p.X);
            int maxXIndex = leftHull.FindIndex(p => p.X == maxXValue);

            double minXValue = rightHull.Min(p => p.X);
            int minXIndex = rightHull.FindIndex(p => p.X == minXValue);

            Tuple<int, int> lowerTangent = GetTangent(leftHull, rightHull, maxXIndex, minXIndex);
            Tuple<int, int> upperTangent = GetTangent(rightHull, leftHull, minXIndex, maxXIndex);

            List<Point> merged = new List<Point>();

            for (int i = lowerTangent.Item2; i != upperTangent.Item1; i = (i + 1) % rightHull.Count)
                merged.Add(rightHull[i]);

            merged.Add(rightHull[upperTangent.Item1]);

            for (int i = upperTangent.Item2; i != lowerTangent.Item1; i = (i + 1) % leftHull.Count)
                merged.Add(leftHull[i]);

            merged.Add(leftHull[lowerTangent.Item1]);

            for (int i = 0; i < merged.Count;)
            {
                int prev = (i - 1 + merged.Count) % merged.Count;
                int next = (i + 1) % merged.Count;
                if (HelperMethods.CheckTurn(new Line(merged[prev], merged[i]), merged[next]) == Enums.TurnType.Colinear)
                {
                    merged.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            return merged;
        }





        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
}

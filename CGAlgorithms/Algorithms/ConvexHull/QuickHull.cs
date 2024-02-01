using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort((Pt1, Pt2) => {
                if (Pt1.X == Pt2.X)
                {
                    return Pt1.Y.CompareTo(Pt2.Y);
                }
                return Pt1.X.CompareTo(Pt2.X);
            });
            Point Xmin = points[0];
            Point Xmax = points[points.Count - 1];
            List<Point> right = DividePoints(points, new Line(Xmin, Xmax), Enums.TurnType.Right);
            List<Point> left = DividePoints(points, new Line(Xmin, Xmax), Enums.TurnType.Left);
            for (int i = 0; i < left.Count; ++i)
            {
                right.Add(left[i]);
            }
            for (int i = 0; i < right.Count; ++i)
            {
                if (!outPoints.Contains(right[i]))
                {
                    outPoints.Add(right[i]);
                }
            }


        }
        public List<Point> DividePoints(List<Point> inputPoints, Line dividingLine, Enums.TurnType direction)
        {
            List<Point> result = new List<Point>();

            if (inputPoints.Count == 0)
            {
                return result;
            }

            int maxIndex = FindMaxDistanceIndex(dividingLine, inputPoints, direction);

            if (maxIndex == -1)
            {
                result.Add(dividingLine.Start);
                result.Add(dividingLine.End);
                return result;
            }

            List<Point> leftSublist, rightSublist;

            if (HelperMethods.CheckTurn(new Line(inputPoints[maxIndex], dividingLine.Start), dividingLine.End) == Enums.TurnType.Right)
            {
                leftSublist = DividePoints(inputPoints, new Line(inputPoints[maxIndex], dividingLine.Start), Enums.TurnType.Left);
            }
            else
            {
                leftSublist = DividePoints(inputPoints, new Line(inputPoints[maxIndex], dividingLine.Start), Enums.TurnType.Right);
            }

            if (HelperMethods.CheckTurn(new Line(inputPoints[maxIndex], dividingLine.End), dividingLine.Start) == Enums.TurnType.Right)
            {
                rightSublist = DividePoints(inputPoints, new Line(inputPoints[maxIndex], dividingLine.End), Enums.TurnType.Left);
            }
            else
            {
                rightSublist = DividePoints(inputPoints, new Line(inputPoints[maxIndex], dividingLine.End), Enums.TurnType.Right);
            }

            result.AddRange(leftSublist);
            result.AddRange(rightSublist);

            return result;
        }

        public static int FindMaxDistanceIndex(Line line, List<Point> points, Enums.TurnType direction)
        {
            int Max_idx = -1;
            double MaxDistance = -1;
            for (int i = 0; i < points.Count; i++)
            {
                double Distance = Math.Abs((points[i].Y - line.Start.Y) * (line.End.X - line.Start.X) - (line.End.Y - line.Start.Y) * (points[i].X - line.Start.X));
                if (Distance > MaxDistance && HelperMethods.CheckTurn(new Line(line.Start, line.End), points[i]) == direction)
                {
                    Max_idx = i;
                    MaxDistance = Distance;
                }
            }
            return Max_idx;
        }



        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
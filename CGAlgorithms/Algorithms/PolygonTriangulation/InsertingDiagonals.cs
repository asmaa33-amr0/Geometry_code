using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            foreach (var polygon in polygons)
            {
                List<Point> polygonPoints = polygon.lines.Select(line => line.Start).ToList();
                Sorting(ref polygonPoints);
                Inserting_Diagonal_algorithm(polygonPoints, ref outLines);
            }

        }
        private void Inserting_Diagonal_algorithm(List<Point> vertices, ref List<Line> outLines)
        {
            if (vertices.Count <= 3) return;
            int j = 0, ID_DiagonalStart = 0;
            int count = vertices.Count;
            List<Point> polygon1 = new List<Point>(), polygon2 = new List<Point>();
            Point FarawayPoint = null;
            double FarDistance = 0;

            for (j = 0; j < count; j++)
            {
                if (CGUtilities.HelperMethods.CheckTurn(new Line(vertices[(j - 1 + count) % count], vertices[j]), vertices[(j + 1) % count]) == Enums.TurnType.Left)
                {
                    for (int k = 0; k < count; k++)
                    {
                        if (k == j || k == (j + 1) % count || k == (j - 1 + count) % count) continue;
                        if (HelperMethods.PointInTriangle(vertices[k], vertices[(j - 1 + count) % count], vertices[j], vertices[(j + 1) % count]) == Enums.PointInPolygon.Inside)
                        {
                            double Distance = Max_Distance_line_point(new Line(vertices[(j - 1 + count) % count], vertices[j]), vertices[k]);

                            if (Distance > FarDistance)
                            {
                                FarDistance = Distance;
                                FarawayPoint = vertices[k];
                            }
                        }
                    }
                    break;
                }
            }

            Line diagonal = null;
            if (FarawayPoint == null)
            {
                diagonal = new Line(vertices[(j - 1 + count) % count], vertices[(j + 1) % count]);
                ID_DiagonalStart = (j - 1 + count) % count;
            }
            else
            {
                diagonal = new Line(vertices[j], FarawayPoint);
                //The idx_Diagonal_Start is updated accordingly to store the index of the starting vertex for the diagonal.
                ID_DiagonalStart = j;
            }

            outLines.Add(diagonal);
            //The variable start is initialized with the starting vertex of the diagonal.
            Point start = vertices[ID_DiagonalStart];
            while (start != diagonal.End)
            {
                polygon1.Add(start);
                //idx_Diagonal_Start is incremented to move to the next vertex along the polygon's boundary,
                ID_DiagonalStart = (ID_DiagonalStart + 1) % count;//%N 3shan tfdal ma7sora ben range el vertices
                //the current vertex start becomes equal to the end vertex of the diagonal (diagonal.End).
                start = vertices[ID_DiagonalStart];
            }
            polygon1.Add(diagonal.End);
            while (start != diagonal.Start)
            {
                polygon2.Add(start);
                ID_DiagonalStart = (ID_DiagonalStart + 1 + count) % count;
                // vertex start becomes equal to the start vertex of the diagonal (diagonal.Start).
                start = vertices[ID_DiagonalStart];
            }
            polygon2.Add(diagonal.Start);
            Sorting(ref polygon1);
            Sorting(ref polygon2);
            Inserting_Diagonal_algorithm(polygon1, ref outLines);
            Inserting_Diagonal_algorithm(polygon2, ref outLines);
        }
        public static void Sorting(ref List<Point> points)
        {
            int ID = 0, count = points.Count;
            for (int i = 1; i < count; i++)
            {
                if (points[i].Y < points[ID].Y || (points[i].Y == points[ID].Y && points[i].X < points[ID].X))
                {
                    ID = i;
                }
            }

            if (HelperMethods.CheckTurn(new Line(points[(ID - 1 + count) % count], points[(ID + 1) % count]), points[ID]) == Enums.TurnType.Right)
            {
                return;
            }

            points.Reverse();
        }


        public double Max_Distance_line_point(CGUtilities.Line line, CGUtilities.Point point)
        {
            double vector1 = (line.End.X - line.Start.X) * (point.Y - line.Start.Y);
            double vector2 = (line.End.Y - line.Start.Y) * (point.X - line.Start.X);
            double distance = Math.Abs((vector1 - vector2) / Math.Sqrt(Math.Pow((line.End.X - line.Start.X), 2) + Math.Pow((line.End.Y - line.Start.Y), 2)));
            return distance;

        }
        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
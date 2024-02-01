
using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {

                for (int i = 0; i < points.Count; i++)
                    outPoints.Add(points[i]);

            }

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
        }





        public override string ToString()
{
    return "Convex Hull - Extreme Points";
}
    }
}
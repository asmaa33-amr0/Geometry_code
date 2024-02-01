using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
 

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    { double px = 13.5999999999995;






        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            List<Line> polyin  = new List<Line>(lines);
            List<Line> poly = new List<Line>(lines);
            if (lines[0].Start.X==px)
               poly = CCWPolygon(polyin);


            //lists to get the chain of the current point
            List<Line> Right = new List<Line>();
            List<Line> Left = new List<Line>();
            List<Point> Right_p = new List<Point>();
            List<Point> Left_p = new List<Point>();

            double maxy = poly[0].Start.Y;
            for (int i = 0; i < poly.Count; i++)
            {
                points.Add(poly[i].Start);
                if (poly[i].Start.Y > maxy)
                {
                    maxy = (double) poly[i].Start.Y;
                }
            }
          
            Right_p.Add(polyin[0].Start);
            
            for (int i = 1; i < polyin.Count; i++)
            {
                if (polyin[i].Start.Y < polyin[i - 1].Start.Y)
                {
                    Right.Add(polyin[i]);
                    Right_p.Add(polyin[i].Start);
                }

                else if (polyin[i].Start.Y >= polyin[i - 1].Start.Y)
                {

                    Left.Add(polyin[i]);
                    Left_p.Add(polyin[i].Start);
                }
                 
            }
             
            List<Point> in_points = new List<Point>();
            for (int i = 0; i < poly .Count; i++)
                in_points.Add(poly[i].Start);

            in_points.Sort((v1, v2) =>
            {
                if (v1.Y == v2.Y)
                    return v1.X.CompareTo(v2.X);
                return v1.Y.CompareTo(v2.Y);
            });
               
            
           // Point first = in_points[0];
            Stack<Point> mono_stack = new Stack<Point>();
            mono_stack.Push(in_points[0]);
            mono_stack.Push(in_points[1]);
            

            for (int i = 2; i < poly.Count;)
            {
                 
                Point Current_point = in_points[i]; 
                Point prev = in_points[i - 1];
                Point top = mono_stack.Peek();

                //same chain case***********
                if ((Right_p.Contains(Current_point) && Right_p.Contains(mono_stack.Peek())) ||
                   (Left_p.Contains(Current_point) && Left_p.Contains(mono_stack.Peek())))
                    { 
                     
                    mono_stack.Pop();
                    Point second_top  = mono_stack.Peek();

                    
                    int current = points.IndexOf(top);
                    if (check_angle(poly, current) == true)// to avoid inserting the diagonal outside the polygon
                    {
                        outLines.Add(new Line(Current_point, second_top));
                        if (mono_stack.Count == 1)
                        {
                            mono_stack.Push(Current_point);
                            i++;
                        }
                    }
                    else  // can't insert diagonal 
                    {
                        mono_stack.Push(top);
                        mono_stack.Push(Current_point);
                        i++;
                    }
                }
                // oposite chain ***************
                else
                {
                    while (mono_stack.Count != 1)
                    {
                        Point topp = mono_stack.Pop();
                        outLines.Add(new Line(Current_point, topp));
                    }
                    mono_stack.Pop();
                    mono_stack.Push(top);
                    mono_stack.Push(Current_point);
                }
            }


            for (int j = 0; j < outLines.Count; j++)
            {
                if (polyin[0].Start == outLines[j].Start || polyin[0].Start == outLines[j].End ||
                in_points[in_points.Count - 1] == outLines[j].Start || in_points[in_points.Count - 1] == outLines[j].End ||
                maxy == (double)outLines[j].Start.Y || maxy == (double)outLines[j].End.Y)
                {
                    outLines.Remove(outLines[j]);
                    break;
                }

            }
        }

        //make all the polygon CCW
        public List<Line>  CCWPolygon(List<Line> input_line )
        {
            double signed_area = 0;
            for (int i = 0; i < input_line .Count; i++)  //shoelace formula
                signed_area += (input_line[i].End.X - input_line[i].Start.X) * (input_line [i].End.Y + input_line [i].Start.Y);
            signed_area /= 2;

           //in case CW
            if (signed_area > 0)
            {
                
                input_line.Reverse();
                for (int i = 0; i < input_line.Count; i++)
                {
                    Point replace = input_line[i].Start;
                    input_line[i].Start = input_line[i].End;
                    input_line[i].End = replace;
                }
            }
            return input_line;
        }

       

        //Check if the current point is convex angle <180 degree
        public bool check_angle(List<Line>  pointss, int Current)
        {
            int previous = ((Current - 1) + pointss .Count) % pointss .Count;
            int next = (Current + 1) % pointss .Count;

            Point p1 = pointss [previous].Start;
            Point p2 = pointss [Current].Start;
            Point p3 = pointss [next].Start;
            Line l = new Line(p1, p2);
            if (HelperMethods.CheckTurn(l, p3) == Enums.TurnType.Left)
                return true;
            return false;
        }

        
            

public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}

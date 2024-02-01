using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;


namespace CGAlgorithms.Algorithms.SegmentIntersection
{



    class SweepLine : Algorithm
    {
        private class Event
        {
            public enum Type { Start, End, Intersection }

            public Type EventType { get; set; }
            public Point Point { get; set; }
            public Line Line1 { get; set; }
            public Line Line2 { get; set; }
            public Event(Point point_p, Type type_t, Line segment_s)
            {
                Point = point_p;
                EventType = type_t;
                Line1 = segment_s;
            }
            public Event(Point point_p, Type type_t, Line segment_s, Line segment_s2)
            {
                Point = point_p;
                EventType = type_t;
                Line1 = segment_s;
                Line2 = segment_s2;
            }
        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {

            List<Event> QRedl = new List<Event>();


            foreach (Line line in lines)
            {
                Event startEvent = new Event(line.Start, Event.Type.Start, line);
                Event endEvent = new Event(line.End, Event.Type.End, line); ;

                QRedl.Add(startEvent);
                QRedl.Add(endEvent);
            }
            //sorting event Queue using X axis
            QRedl.Sort((a, b) => a.Point.X.CompareTo(b.Point.X));

             
            List<Point> out_point = new List<Point>();
           
            SortedSet<Line> Qyellowl = new SortedSet<Line>(Comparer<Line>.Create((a, b) =>
            {
                int compareY = a.Start.Y.CompareTo(b.Start.Y);
                return compareY != 0 ? compareY : a.End.Y.CompareTo(b.End.Y);
            }));

            {
                int iterat = 0;
                while (QRedl.Count - 1 != iterat)
                {

                    Event p = QRedl[iterat];
                    if (p.EventType == Event.Type.Start)
                    {
                        Line seg1 = p.Line1;
                        Qyellowl.Add(seg1);

                        List<Line> Qyellowlest = Qyellowl.ToList();
                        int index = Qyellowlest.IndexOf(seg1);
                        if (index < Qyellowl.Count - 1)
                        {
                            bool intersect_flag_after = intersect(Qyellowl.ElementAt(index + 1), Qyellowl.ElementAt(index));
                            if (intersect_flag_after)
                            {


                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index + 1), Qyellowl.ElementAt(index))),
                                    Event.Type.Intersection, Qyellowl.ElementAt(index + 1), Qyellowl.ElementAt(index));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                                 

                            }
                        }
                        if (index > 0)
                        {
                            bool intersect_flag_before = intersect(Qyellowl.ElementAt(index), Qyellowl.ElementAt(index - 1));
                            if (intersect_flag_before)
                            {

                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index), Qyellowl.ElementAt(index - 1))),
                                    Event.Type.Intersection, Qyellowl.ElementAt(index), Qyellowl.ElementAt(index - 1));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                                
                            }

                        }

                    }




                    else if (p.EventType == Event.Type.End)
                    {
                        Line seg = p.Line1;

                        Qyellowl.Remove(seg);

                        List<Line> Qyellowlest = Qyellowl.ToList();
                        int index = Qyellowlest.IndexOf(p.Line1);
                        if (index > 0 && index < Qyellowl.Count - 1)
                        {
                            bool intersect_after_pop = intersect(Qyellowl.ElementAt(index + 1), Qyellowl.ElementAt(index - 1));
                            if (intersect_after_pop)
                            {

                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index + 1), Qyellowl.ElementAt(index + 1))),
                                    Event.Type.Intersection, Qyellowl.ElementAt(index + 1), Qyellowl.ElementAt(index - 1));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                                
                            }
                        }


                    }

                    else if (p.EventType == Event.Type.Intersection)

                    {
                        if (!out_point.Contains(p.Point))
                            out_point.Add(p.Point);





                        List<Line> Qyellowlest = Qyellowl.ToList();
                        int index_of_l1 = Qyellowlest.IndexOf(p.Line1);
                        int index_of_l2 = Qyellowlest.IndexOf(p.Line2);
                        for (int i = 0; i < Qyellowl.Count; i++)
                        {
                            if (Qyellowl.ElementAt(i) == p.Line1)
                            {
                                index_of_l1 = i;
                            }
                            else index_of_l1 = Qyellowl.Count - 1;
                            if (Qyellowl.ElementAt(i) == p.Line2)
                            {
                                index_of_l2 = i;
                            }
                            else index_of_l2 = Qyellowl.Count - 1;
                        }

                        if (index_of_l1 > 0)
                        {
                            bool intersect_after_sweep_prev = intersect(Qyellowl.ElementAt(index_of_l1), Qyellowl.ElementAt(index_of_l1 - 1));
                            if (intersect_after_sweep_prev)
                            {


                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index_of_l1), Qyellowl.ElementAt(index_of_l1 - 1))),
                           Event.Type.Intersection, Qyellowl.ElementAt(index_of_l1), Qyellowl.ElementAt(index_of_l1 - 1));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                               

                            }
                        }
                        if (index_of_l1 < Qyellowl.Count - 1)
                        {
                            bool intersect_after_sweep_next = intersect(Qyellowl.ElementAt(index_of_l1), Qyellowl.ElementAt(index_of_l1 + 1));
                            if (intersect_after_sweep_next)
                            {

                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index_of_l1), Qyellowl.ElementAt(index_of_l1 + 1))),
                                Event.Type.Intersection, Qyellowl.ElementAt(index_of_l1), Qyellowl.ElementAt(index_of_l1 + 1));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                                

                            }
                        }
                        if (index_of_l2 > 0)
                        {
                            bool intersect_after_sweep_prev2 = intersect(Qyellowl.ElementAt(index_of_l2), Qyellowl.ElementAt(index_of_l2 - 1));
                            if (intersect_after_sweep_prev2)
                            {
                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index_of_l2), Qyellowl.ElementAt(index_of_l2 - 1))),
                           Event.Type.Intersection, Qyellowl.ElementAt(index_of_l2), Qyellowl.ElementAt(index_of_l2 - 1));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                                

                            }

                        }
                        if (index_of_l2 < Qyellowl.Count - 1)
                        {
                            bool intersect_after_sweep_next2 = intersect(Qyellowl.ElementAt(index_of_l2), Qyellowl.ElementAt(index_of_l2 + 1));
                            if (intersect_after_sweep_next2)
                            {
                                Event intersectEvent = new Event((intersect_point(Qyellowl.ElementAt(index_of_l2), Qyellowl.ElementAt(index_of_l2 + 1))),
                          Event.Type.Intersection, Qyellowl.ElementAt(index_of_l2), Qyellowl.ElementAt(index_of_l2 + 1));
                                QRedl.Add(intersectEvent);
                                out_point.Add(intersectEvent.Point);
                                

                            }

                        }
                        Qyellowl.Remove(p.Line1);
                        Qyellowl.Remove(p.Line2);
                        Line Swap = p.Line1;
                        p.Line1 = p.Line2;
                        p.Line2 = Swap;
                        Qyellowl.Add(p.Line1);
                        Qyellowl.Add(p.Line2);

                    }
                    iterat++;
                }

                outPoints = out_point.Distinct().ToList();

            }

        }



        public void Sweep(Point intersectPoint, ref SortedSet<Line> lineSet, int lineIndex1, int lineIndex2)
        {
            // Retrieve the lines from the SortedSet based on their indices
            List<Line> temp = new List<Line>(lineSet);
            Line tmp = temp[lineIndex1];
            temp[lineIndex1] = temp[lineIndex2];
            temp[lineIndex2] = tmp;
            lineSet.Clear();
            foreach (var seg in temp)
            {
                lineSet.Add(seg);
            }

            lineSet = new SortedSet<Line>(Comparer<Line>.Create((a, b) =>
            {
                int compareY = a.Start.Y.CompareTo(b.Start.Y);
                return compareY != 0 ? compareY : a.End.Y.CompareTo(b.End.Y);
            }));

            




        }


        public bool intersect(Line l1, Line l2)
        {
            if (HelperMethods.CheckTurn(l1, l2.Start) != HelperMethods.CheckTurn(l1, l2.End) &&
                 HelperMethods.CheckTurn(l2, l1.Start) != HelperMethods.CheckTurn(l2, l1.End))
            {
                return true;
            }
            return false;
        }
        public Point intersect_point(Line l1, Line l2)
        {
            double slope1 = (l1.End.Y - l1.Start.Y) / (l1.End.X - l1.Start.X);
            double slope2 = (l2.End.Y - l2.Start.Y) / (l2.End.X - l2.Start.X);
            double px = (slope1 * l1.Start.X - slope2 * l2.Start.X + l2.Start.Y - l1.Start.Y) / (slope1 - slope2);
            double py = slope1 * (px - l1.Start.X) + l1.Start.Y;
            Point p = new Point(px, py);

            return p;
        }
        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}

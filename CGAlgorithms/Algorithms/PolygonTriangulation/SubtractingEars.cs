using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        LinkedList<Point> point;

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (polygons.Count == 0)
                return;
            point = new LinkedList<Point>();
            for (int i = 0; i < polygons[0].lines.Count; ++i)
                point.AddLast(polygons[0].lines[i].Start);

            checkPolygon();
            Queue<LinkedListNode<Point>> Ear = new Queue<LinkedListNode<Point>>();
            Ear = getAllEars();
            while (Ear.Count > 0)
                subears(Ear, outLines);
        }

        private void AddAdjacentNodesToQueue(Queue<LinkedListNode<Point>> queue, LinkedListNode<Point> prev, LinkedListNode<Point> next)
        {
            if (prev != null) queue.Enqueue(prev);
            if (next != null) queue.Enqueue(next);
        }
        private void subears(Queue<LinkedListNode<Point>> E, List<Line> outLines)
        {
            if (E.Count == 0) return;

            LinkedListNode<Point> cur = E.Dequeue();
            if (cur == null || !isEar(cur) || (cur.Next == null && cur.Previous == null))
                return;

            if (point.Count == 3)
            {
                E.Clear();
                return;
            }

            LinkedListNode<Point> next, previous;
            if (cur.Next == null)
                next = point.First;
            else
                next = cur.Next;

            if (cur.Previous == null)
                previous = point.Last;
            else
                previous = cur.Previous;


            point.Remove(cur);
            outLines.Add(new Line(previous.Value, next.Value));


            AddAdjacentNodesToQueue(E, previous, next);
        }



        private Queue<LinkedListNode<Point>> getAllEars()
        {
            Queue<LinkedListNode<Point>> res = new Queue<LinkedListNode<Point>>();
            for (LinkedListNode<Point> cur = point.First; cur != point.Last.Next; cur = cur.Next)
                if (isEar(cur))
                    res.Enqueue(cur);
            return res;
        }



        private bool isConvex(LinkedListNode<Point> p)
        {
            LinkedListNode<Point> next = p.Next == null ? point.First : p.Next;
            LinkedListNode<Point> prev = p.Previous == null ? point.Last : p.Previous;
            return HelperMethods.CheckTurn(new Line(prev.Value, next.Value), p.Value) == Enums.TurnType.Right;
        }





        public override string ToString()
        {
            return "Subtracting Ears";
        }

        private bool isEar(LinkedListNode<Point> p)
        {
            if (!isConvex(p))
                return false;
            LinkedListNode<Point> next = p.Next == null ? point.First : p.Next;
            LinkedListNode<Point> prev = p.Previous == null ? point.Last : p.Previous;

            for (LinkedListNode<Point> cur = point.First; cur != point.Last.Next; cur = cur.Next)
            {

                if (HelperMethods.PointInTriangle(cur.Value, prev.Value, next.Value, p.Value) == Enums.PointInPolygon.Inside)
                    return false;
            }
            return true;
        }
        public void checkPolygon()
        {
            LinkedListNode<Point> minP = point.First;
            LinkedListNode<Point> cur = minP.Next;

            while (cur != null)
            {
                if (cur.Value.X < minP.Value.X)
                    minP = cur;
                cur = cur.Next;
            }

            LinkedListNode<Point> next = minP.Next ?? point.First;
            LinkedListNode<Point> prev = minP.Previous ?? point.Last;

            if (HelperMethods.CheckTurn(new Line(prev.Value, next.Value), minP.Value) == Enums.TurnType.Left)
                Reverse();
        }

        private void Reverse()
        {
            LinkedList<Point> newPoint = new LinkedList<Point>(point.Reverse());
            point = newPoint;
        }
    }
}
using System.Collections;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utilities
{
    // NOTE: need to re-check, because they're generated from BingAI (daivq)
    public static class GeometryUtilities
    {
        public static Vector2[] CalculateIntersections(Vector2 size, Vector2 lineStart, Vector2 lineEnd)
        {
            float halfWidth = size.x / 2;
            float halfHeight = size.y / 2;

            Vector2[] rectangleCorners = new Vector2[4]
            {
                new Vector2(-halfWidth, -halfHeight),
                new Vector2(halfWidth, -halfHeight),
                new Vector2(halfWidth, halfHeight),
                new Vector2(-halfWidth, halfHeight)
            };

            Vector2[] intersections = new Vector2[2];
            int count = 0;

            for (int i = 0; i < rectangleCorners.Length; i++)
            {
                Vector2 start = rectangleCorners[i];
                Vector2 end = rectangleCorners[(i + 1) % rectangleCorners.Length];

                Vector2 intersection;
                if (IntersectLineSegments2D(lineStart, lineEnd, start, end, out intersection))
                {
                    intersections[count] = intersection;
                    count++;
                    if (count > 1)
                        return intersections;
                }
            }

            return null;
        }

        public static bool IntersectLineSegments2D(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end, out Vector2 intersection)
        {
            // Consider:
            //   p1start = p
            //   p1end = p + r
            //   p2start = q
            //   p2end = q + s
            // We want to find the intersection point where :
            //  p + t*r == q + u*s
            // So we need to solve for t and u
            var p = p1start;
            var r = p1end - p1start;
            var q = p2start;
            var s = p2end - p2start;
            var qminusp = q - p;

            float cross_rs = Cross(r, s);

            if (ApproximateZero(cross_rs))
            {
                // Parallel lines
                if (ApproximateZero(Cross(qminusp, r)))
                {
                    // Co-linear lines, could overlap
                    float rdotr = Vector2.Dot(r, r);
                    float sdotr = Vector2.Dot(s, r);
                    // this means lines are co-linear
                    // they may or may not be overlapping
                    float t0 = Vector2.Dot(qminusp, r / rdotr);
                    float t1 = t0 + sdotr / rdotr;
                    if (sdotr < 0)
                    {
                        // lines were facing in different directions so t1 > t0, swap to simplify check
                        float temp = t0;
                        t0 = t1;
                        t1 = temp;
                    }

                    if (t0 <= 1 && t1 >= 0)
                    {
                        // Nice half-way point intersection
                        float t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                        intersection = p + t * r;
                        return true;
                    }
                    else
                    {
                        // Co-linear but disjoint
                        intersection = Vector2.zero;
                        return false;
                    }
                }
                else
                {
                    // Just parallel in different places, cannot intersect
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // Not parallel, calculate t and u
                float t = Cross(qminusp, s) / cross_rs;
                float u = Cross(qminusp, r) / cross_rs;
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    intersection = p + t * r;
                    return true;
                }
                else
                {
                    // Lines only cross outside segment range
                    intersection = Vector2.zero;
                    return false;
                }
            }
        }

        private static bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < Mathf.Epsilon;
        }
        private static bool ApproximateZero(float value)
        {
            return Mathf.Abs(value) < Mathf.Epsilon;
        }

        private static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static Vector2 CalculateMirroredPoint(Vector2 pointToMirror, Vector2 lineAnchor, Vector2 lineDirectionNorm)
        {
            Vector2 lineToPoint = pointToMirror - lineAnchor;
            Vector2 projectedPoint = Vector2.Dot(lineToPoint, lineDirectionNorm) * lineDirectionNorm + lineAnchor;
            Vector2 mirroredPoint = 2 * projectedPoint - pointToMirror;

            return mirroredPoint;
        }

        public static float CalculateDistanceFromPointToLine(Vector2 point, Vector2 linePivot, Vector2 directionNorm)
        {
            Vector2 projectedPoint = Vector2.Dot(point - linePivot, directionNorm) * directionNorm + linePivot;
            return Vector2.Distance(point, projectedPoint);
        }
        public static Vector2 CalculateProjectFromPointToLine(Vector2 point, Vector2 linePivot, Vector2 directionNorm)
        {
            return Vector2.Dot(point - linePivot, directionNorm) * directionNorm + linePivot;
        }
        public static void ProjectFromPointToLine(Vector2 point, Vector2 linePivot, Vector2 directionNorm, out float distanceToLine, out float distanceProjected, out Vector2 projectedPoint)
        {
            distanceProjected = Vector2.Dot(point - linePivot, directionNorm);
            projectedPoint = distanceProjected * directionNorm + linePivot;
            distanceToLine = Vector2.Distance(point, projectedPoint);
        }
        public static void ProjectFromPointToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out float distanceToLine, out float distanceProjected, out Vector2 projectedPoint)
           => ProjectFromPointToLine(point, lineStart, (lineEnd - lineStart).normalized, out distanceToLine, out distanceProjected, out projectedPoint);

        public static float CalculateSignedAreaPolygon(Vector2[] points)
        {
            float area = 0;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % points.Length];
                area += p1.x * p2.y - p2.x * p1.y;
            }
            return area / 2;
        }
        public static float CalculateAreaPolygon(Vector2[] points) => Mathf.Abs(CalculateSignedAreaPolygon(points));
    }
}
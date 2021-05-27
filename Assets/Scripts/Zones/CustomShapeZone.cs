using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SebastiansShapeCreator))]
public class CustomShapeZone : Zone
{

    private SebastiansShapeCreator _sebastiansShapeCreator;

    private void Start()
    {
        _sebastiansShapeCreator = GetComponent<SebastiansShapeCreator>();
    }


    public override bool CheckPointIsInsideShape(Vector3 pointPos)
    {
        return PointInsideShape(_sebastiansShapeCreator.GetPointsWorldPosList(), pointPos);
    }

    public static bool PointInsideShape(List<Vector3> shapePoints, Vector3 targetPoint)
    {
        if (shapePoints.Count < 3) return false;

        //If point is not close to any point - return false
        Vector3 minPos = shapePoints[0];
        Vector3 maxPos = shapePoints[0];
        
        foreach (var p in shapePoints)
        {
            if (p.x < minPos.x) minPos.x = p.x;
            if (p.z < minPos.z) minPos.z = p.z;
            
            if (p.x > maxPos.x) maxPos.x = p.x;
            if (p.z > maxPos.z) maxPos.z = p.z;
        }

        if (targetPoint.x > maxPos.x ||
            targetPoint.z > maxPos.z ||
            targetPoint.x < minPos.x ||
            targetPoint.z < minPos.z)
            return false;
        
        //переводим в ось XY
        List<Vector2> points = new List<Vector2>(shapePoints.Count);
        for (int i = 0; i < shapePoints.Count; i++)
        {
            points.Add(shapePoints[i].ToXZ());
        }
        
        int pointInsideChecks = 0;
        int pointOutsideChecks = 0;
        
        //делаем 5 рандомных прямых через точку (на случай ошибок)
        for (int j = 0; j < 5; j++)
        {
            Vector2 randomPoint = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));

            Vector2 p1 = randomPoint;
            Vector2 p2 = targetPoint.ToXZ();

            int collisionCount = 0;

            //считаем количество пересечений прямой с отрезками
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p3 = points[i];
                Vector2 p4 = points[(i + 1) % points.Count];

                //точка пересечения рандомной прямой с прямой лежащей на i-той грани
                Vector2 intersectionPoint = new Vector2
                {
                    x = (
                        (p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.y)
                    ) / (
                        (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x)
                    ),
                    y = (
                        (p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.y)
                    ) / (
                        (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x)
                    )
                };
                
                print(intersectionPoint);

                //если точка лежит на грани - рандомная прямая пересекает грань
                if (PointLieOnSegment(intersectionPoint, p3, p4))
                {
                    collisionCount++;
                }
            }
            
            if (collisionCount % 2 == 0)//точка внутри фигуры
            {
                pointInsideChecks++;
            }
            else
            {
                pointOutsideChecks++;
            }

        }

        return pointInsideChecks > pointOutsideChecks;
    }

    private static bool PointLieOnSegment(Vector2 point, Vector2 A, Vector2 B)
    {
        return Math.Abs(Pseudo(B - A, point - A)) <0.001f && Vector2.Dot(A - point, B - point) <= 0.001f;
    }

    private static float Pseudo(Vector2 a, Vector2 b)
    {
        return (a.x * b.y) - (b.x * a.y);
    }

}
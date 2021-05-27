using System.Collections.Generic;
using UnityEngine;


public class SebastiansShapeCreator : MonoBehaviour
{
    //[HideInInspector]
    public List<Vector3> points = new List<Vector3>();

    public float handleRadius = 0.1f;
    

    public List<Vector3> GetPointsWorldPosList()
    {
        List<Vector3> newList = new List<Vector3>(points.Count);

        for (int i = 0; i < points.Count; i++)
        {
            newList.Add(points[i] + transform.position);
        }
        
        return newList;
    }
}




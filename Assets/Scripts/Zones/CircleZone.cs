using System;
using UnityEngine;


public class CircleZone : Zone
{
    [SerializeField] private float radius = 1f;
    
    
    public override bool CheckPointIsInsideShape(Vector3 pointPos)
    {
        return (transform.position - pointPos).magnitude < radius*radius;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

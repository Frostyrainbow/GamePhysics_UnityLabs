using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{ 
    X = 0,
    Y,
    Z
}


//We can define a plane with a normal vector and a point on the plane
public class PhysiczPlane : PhysiczColliderBase
{
    public Axis alignment = Axis.Y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override CollisionShape GetCollisionShape()
    {
        return CollisionShape.Plane;
    }

    public Vector3 GetNormal()
    {
        switch (alignment)
        {
            case (Axis.X):
                {
                    return transform.right;
                }
            case (Axis.Y):
                {
                    return transform.up;
                }
            case (Axis.Z):
                {
                    return transform.forward;
                }
            default:
                {
                    throw new Exception("Invalid plane alignment");
                }
        }
    }
}

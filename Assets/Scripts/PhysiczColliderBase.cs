using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CollisionShape
{
    Sphere = 0,
    Plane,
    AABB
}


public abstract class PhysiczColliderBase : MonoBehaviour
{


    //Abstract means that this must be overidden in a child class
    //Like a pure function in C++
    public abstract CollisionShape GetCollisionShape();
}

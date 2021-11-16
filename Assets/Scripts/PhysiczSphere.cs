using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysiczSphere : PhysiczColliderBase
{
    public float radius = 1.0f;

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
        return CollisionShape.Sphere;
    }
}

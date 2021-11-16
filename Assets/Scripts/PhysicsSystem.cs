using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{

    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<physiczobject> physiczobjects = new List<physiczobject>();

    // Start is called before the first frame update
    void Start()
    {
        physiczobject[] objects = FindObjectsOfType<physiczobject>();
        physiczobjects.AddRange(objects);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < physiczobjects.Count; i++)
        {
            physiczobjects[i].velocity += gravity * Time.fixedDeltaTime;
        }

        CollisionUpdate();

        
       
        //foreach loop works for many types of containers in C#
        //

    }

    void CollisionUpdate()
    {
        for (int i = 0; i < physiczobjects.Count; i++)
        {
            for (int j = i + 1; j < physiczobjects.Count; j++)
            {
                physiczobject objectA = physiczobjects[i];
                physiczobject objectB = physiczobjects[j];

                //if one does not have collision
                if(objectA.shape == null || objectB.shape == null)
                {
                    continue;
                }

                //if both are spheres do a sphere sphere collision
                if (objectA.shape.GetCollisionShape() == CollisionShape.Sphere && objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    //Do the collision
                    //PhysiczObject.shape is a base class refference to physiczcollisderbase
                    //to do specific things with it we need to do a cast to our derived class PhysiczSphere
                    SphereSphereCollision((PhysiczSphere)objectA.shape, (PhysiczSphere)objectB.shape);
                }
            }
        }
    }

    static void SphereSphereCollision(PhysiczSphere a, PhysiczSphere b)
    {
        Vector3 displacement = a.transform.position - b.transform.position;
        float distance = displacement.magnitude;
        float sumRadii = a.radius + b.radius;
        bool isOverlapping = distance < sumRadii;
        float penitrationDepth = sumRadii - distance;
        

        if (isOverlapping)
        {
            Debug.Log(a.name + " collided with: " + b.name);
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = b.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            b.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
        }
        
    }

    static void SpherePlaneCollision(PhysiczSphere a, PhysiczPlane b)
    {
        //use dot product to find the length of the projection of the sphere onto the plane
        //This gives the shortest distance from the plane to the cente rog the sphere
        //If the distance is less then the radius of the sphere they are overlapping

        Vector3 somePointOnThePlane = b.transform.position;
        Vector3 centerOfSphere = a.transform.position;
        Vector3 fromPlaneToSphere = centerOfSphere - somePointOnThePlane;

        //The sign of this dot product indicates which side of the normal this fromPlaneToSphere vector is on
        //If the sign is negative they point in the opisite direction
        //If the sign is positive they are at least somewhat in the same direction
        float dot = Vector3.Dot(fromPlaneToSphere, b.GetNormal());
        float distance = Mathf.Abs(dot);

    }
}

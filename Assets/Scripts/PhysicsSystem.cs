using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{

    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<physiczobject> physiczobjects = new List<physiczobject>();
    //If true this object will not be moved by our physics system
    

    // Start is called before the first frame update
    //void Start()
    //{
    //    physiczobject[] objects = FindObjectsOfType<physiczobject>();
    //    physiczobjects.AddRange(objects);
    //}

    // Update is called once per frame
    void FixedUpdate()
    {

        for (int i = 0; i < physiczobjects.Count; i++)
        {
            physiczobject obj = physiczobjects[i];
            if (!obj.lockPosition)
            {
                obj.velocity += gravity * obj.gravityScale * Time.fixedDeltaTime;
            }

            
        }

        for (int i = 0; i < physiczobjects.Count; i++)
        {
            physiczobject obj = physiczobjects[i];

            if (!obj.lockPosition)
            {
                obj.transform.position += obj.velocity * Time.fixedDeltaTime;
            }
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

                if (objectA.shape.GetCollisionShape() == CollisionShape.Sphere && objectB.shape.GetCollisionShape() == CollisionShape.Plane)
                {
                    SpherePlaneCollision((PhysiczSphere)objectA.shape, (PhysiczPlane)objectB.shape);
                }

                if (objectA.shape.GetCollisionShape() == CollisionShape.Plane && objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    SpherePlaneCollision((PhysiczSphere)objectB.shape, (PhysiczPlane)objectA.shape);
                }
            }
        }
    }

    void getLockMovementScalars(physiczobject a, physiczobject b, out float movementScalarA, out float movementScalarB)
    {
        if (a.lockPosition && !b.lockPosition)
        {
            movementScalarA = 0.0f;
            movementScalarB = 1.0f;
            return;
        }
        if (!a.lockPosition && b.lockPosition)
        {
            movementScalarA = 1.0f;
            movementScalarB = 0.0f;
            return;
        }
        if (a.lockPosition && b.lockPosition)
        {
            movementScalarA = 0.0f;
            movementScalarB = 0.0f;
            return;
        }
        movementScalarA = 0.0f;
        movementScalarB = 0.0f;
    }

    void SphereSphereCollision(PhysiczSphere a, PhysiczSphere b)
    {
        Vector3 displacement = a.transform.position - b.transform.position;
        
        float distance = displacement.magnitude;
        float sumRadii = a.radius + b.radius;
        bool isOverlapping = distance < sumRadii;
        float penitrationDepth = sumRadii - distance;
        Vector3 collisionNormalFromAToB;

        if (isOverlapping)
        {
            Debug.Log(a.name + " collided with: " + b.name);
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = b.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            b.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
        }
        else
        {
            return;
        }

        const float minimumDistance = 0.001f;
        if (distance < minimumDistance)
        {
            distance = minimumDistance;
            collisionNormalFromAToB = new Vector3(0, penitrationDepth, 0);
        }
        else
        {
            collisionNormalFromAToB = displacement / distance;
        }


        //getLockMovementScalars(a.kinematicsObject, b.kinematicsObject, out float movementScalarA, out float movementScalarB);

        Vector3 RelativeVelocity = a.kinematicsObject.velocity - b.kinematicsObject.velocity;
        Vector3 VelocityNormal = Vector3.Dot(RelativeVelocity, collisionNormalFromAToB) * collisionNormalFromAToB;
        a.kinematicsObject.velocity = a.kinematicsObject.velocity - VelocityNormal;
        b.kinematicsObject.velocity = b.kinematicsObject.velocity + VelocityNormal;


         float moveScalarA = 0.5f;
        float moveScalarB = 0.5f;

        if (a.kinematicsObject.lockPosition && !b.kinematicsObject.lockPosition)
        {
            moveScalarA = 0.0f;
            moveScalarB = 1.0f;
        }
        if (!a.kinematicsObject.lockPosition && b.kinematicsObject.lockPosition)
        {
            moveScalarA = 1.0f;
            moveScalarB = 0.0f;
        }
        if (!a.kinematicsObject.lockPosition && !b.kinematicsObject.lockPosition)
        {
            moveScalarA = 0.5f;
            moveScalarB = 0.5f;
        }

        //Collision response
        Vector3 minimumTranslationVectorAtoB = penitrationDepth * -collisionNormalFromAToB;
        Vector3 translationVectorA = -minimumTranslationVectorAtoB * moveScalarA;
        Vector3 translationVectorB = minimumTranslationVectorAtoB * moveScalarB;

        a.transform.position += translationVectorA;
        b.transform.position += translationVectorB;

        ApplyKinematicsCollisionResponse(a.kinematicsObject, b.kinematicsObject, collisionNormalFromAToB);

    }

    void SpherePlaneCollision(PhysiczSphere a, PhysiczPlane b)
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
        Vector3 penetrationDepth = new Vector3(0.0f, (distance - a.radius), 0.0f);
        bool isOverlapping = distance < a.radius;

        if (isOverlapping)
        {
            Debug.Log(a.name + " collided with: " + b.name);
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = b.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            b.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            a.kinematicsObject.velocity *= -0.8f;       // Energy Loss on bounce
            a.transform.Translate(-penetrationDepth);  // Reset position if embedded
        }

    }

    void ApplyKinematicsCollisionResponse(physiczobject objectA, physiczobject objectB, Vector3 collisionNormal)
    {
        physiczobject objA = objectA.shape.kinematicsObject;
        physiczobject objB = objectB.shape.kinematicsObject;

        //Find relative velocity between the objects along the normal
        Vector3 relatveVelocity = objB.velocity - objA.velocity;
        float relativeNormalVelocity = Vector3.Dot(relatveVelocity, collisionNormal);

        float restitution = 0.5f * (objA.bounciness + objB.bounciness);
        float changeInVelocity = -relativeNormalVelocity * (1.0f + restitution);

        //A   B     relativeNormalVelocity = 0   no bounce
        //A   B-->  relativeNormalVelocity = 2    no bounce
        //<--A  B   relativeNormalVelocity = 2
        //<--A B--> relativeNormalVelocity = 4    no bounce
        //A->  B---> relativeNormalVelocity = 2
        //A--><--B  relativeNormalVelocity = -4    bounce
        //A--> B    relativeNormalVelocity = -2
        //A---> B-> relativeNormalVelocity = -2
        //<-A  <--B relativeNormalVelocity = -1

        if (relativeNormalVelocity >= 0.0f)
        {
            return; //No bounce
        }




        //Handle different events based on whihc objects are locked
        if (objB.lockPosition && !objA.lockPosition)
        {
            //only change A
            objA.velocity -= changeInVelocity * collisionNormal;
        }
        else if (!objB.lockPosition && objA.lockPosition)
        {
            //only change B
            objB.velocity += changeInVelocity * collisionNormal;
        }
        else if (!objB.lockPosition && !objA.lockPosition)
        {
            //Both change

        }
    }
}

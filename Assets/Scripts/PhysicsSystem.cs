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

                //if Sphere and Box collided
                if (objectA.shape.GetCollisionShape() == CollisionShape.Sphere && objectB.shape.GetCollisionShape() == CollisionShape.AABB)
                {
                    //Do the collision
                    //PhysiczObject.shape is a base class refference to physiczcollisderbase
                    //to do specific things with it we need to do a cast to our derived class PhysiczSphere
                    SphereAABBCollision((PhysiczSphere)objectA.shape, (PhysiczAABB)objectB.shape);
                }
                //if Sphere and Box collided
                if (objectB.shape.GetCollisionShape() == CollisionShape.Sphere && objectA.shape.GetCollisionShape() == CollisionShape.AABB)
                {
                    //Do the collision
                    //PhysiczObject.shape is a base class refference to physiczcollisderbase
                    //to do specific things with it we need to do a cast to our derived class PhysiczSphere
                    SphereAABBCollision((PhysiczSphere)objectB.shape, (PhysiczAABB)objectA.shape);
                }

                if (objectA.shape.GetCollisionShape() == CollisionShape.Sphere && objectB.shape.GetCollisionShape() == CollisionShape.Plane)
                {
                    SpherePlaneCollision((PhysiczSphere)objectA.shape, (PhysiczPlane)objectB.shape);
                }

                if (objectA.shape.GetCollisionShape() == CollisionShape.Plane && objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    SpherePlaneCollision((PhysiczSphere)objectB.shape, (PhysiczPlane)objectA.shape);
                }

                //if Both object are AABB and collided
                if (objectA.shape.GetCollisionShape() == CollisionShape.AABB && objectB.shape.GetCollisionShape() == CollisionShape.AABB)
                {
                    // do the collision
                    // AABB to AABB
                    AABBAABBCollision((PhysiczColliderBase)objectA.shape, (PhysiczColliderBase)objectB.shape);
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

    static void AABBAABBCollision(PhysiczColliderBase objectA, PhysiczColliderBase objectB)
    {
        // GetHalf sizes along each axis (x, y, and z)
        // Get distance detween the boxes on each axis (x, y, and z)

        Vector3 halfSizeA = ((PhysiczAABB)objectA).GetHalfSize();
        Vector3 halfSizeB = ((PhysiczAABB)objectB).GetHalfSize();

        Vector3 displacementAtoB = objectB.transform.position - objectA.transform.position;

        float distX = Mathf.Abs(displacementAtoB.x);
        float distY = Mathf.Abs(displacementAtoB.y);
        float distZ = Mathf.Abs(displacementAtoB.z);

        // For each axis:
        // If the distance between the boxes (along the axis) is less than the sum of the half sizes
        // then they are overlapping

        float penetrationX = halfSizeA.x + halfSizeB.x - distX;
        float penetrationY = halfSizeA.y + halfSizeB.y - distY;
        float penetrationZ = halfSizeA.z + halfSizeB.z - distZ;

        // If there is an overlap along ALL axis then they are colliding, else they are not

        if (penetrationX < 0 || penetrationY < 0 || penetrationZ < 0)
        {
            return;
        }

        // Find minimumTraslationVector (i.e. what is the shortest path we can take)
        // Along which axis are they closest to being seperate
        // Move along that axis according to how much overlap there is

        Vector3 minimumTranslationVector;
        Vector3 collisionNormalAtoB;
        Vector3 contact;

        if (penetrationX < penetrationY && penetrationX < penetrationZ) // is penX the shortest?
        {
            collisionNormalAtoB = new Vector3(Mathf.Sign(displacementAtoB.x), 0, 0);    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationX;
        }
        else if (penetrationY < penetrationX && penetrationY < penetrationZ) // is penY the shortest?
        {
            collisionNormalAtoB = new Vector3(0, Mathf.Sign(displacementAtoB.y), 0);    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationY;
        }
        else //if (penetrationZ < penetrationY && penetrationZ < penetrationX) // is penZ the shortest?   // could just be else
        {
            collisionNormalAtoB = new Vector3(0, 0, Mathf.Sign(displacementAtoB.z));    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationZ;
        }

        contact = objectA.transform.position + minimumTranslationVector;

        ApplyMinimumTraslationVector(objectA.kinematicsObject, objectB.kinematicsObject, minimumTranslationVector, collisionNormalAtoB, contact);

    }

    static void SphereAABBCollision(PhysiczColliderBase sphere, PhysiczColliderBase box)
    {
        // GetHalf sizes along each axis (x, y, and z)
        // Get distance between the sphere and box on each axis (x, y, and z)

        float Raduis = (((PhysiczSphere)sphere).radius);
        Vector3 halfSizeB = ((PhysiczAABB)box).GetHalfSize();

        Vector3 displacementAtoB = box.transform.position - sphere.transform.position;

        float distX = Mathf.Abs(displacementAtoB.x);
        float distY = Mathf.Abs(displacementAtoB.y);
        float distZ = Mathf.Abs(displacementAtoB.z);

        // For each axis:
        // If the distance between the boxes (along the axis) is less than the sum of the half sizes
        // then they are overlapping

        float penetrationX = Raduis + halfSizeB.x - distX;
        float penetrationY = Raduis + halfSizeB.y - distY;
        float penetrationZ = Raduis + halfSizeB.z - distZ;

        // If there is an overlap along ALL axis then they are colliding, else they are not

        if (penetrationX < 0 || penetrationY < 0 || penetrationZ < 0)
        {
            return;
        }

        // Find minimumTraslationVector (i.e. what is the shortest path we can take)
        // Along which axis are they closest to being seperate
        // Move along that axis according to how much overlap there is

        Vector3 minimumTranslationVector;
        Vector3 collisionNormalAtoB;
        Vector3 contact;

        if (penetrationX < penetrationY && penetrationX < penetrationZ) // is penX the shortest?
        {
            collisionNormalAtoB = new Vector3(Mathf.Sign(displacementAtoB.x), 0, 0);    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationX;
        }
        else if (penetrationY < penetrationX && penetrationY < penetrationZ) // is penY the shortest?
        {
            collisionNormalAtoB = new Vector3(0, Mathf.Sign(displacementAtoB.y), 0);    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationY;
        }
        else //if (penetrationZ < penetrationY && penetrationZ < penetrationX) // is penZ the shortest?   // could just be else
        {
            collisionNormalAtoB = new Vector3(0, 0, Mathf.Sign(displacementAtoB.z));    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationZ;
        }

        contact = sphere.transform.position + minimumTranslationVector;

        ApplyMinimumTraslationVector(sphere.kinematicsObject, box.kinematicsObject, minimumTranslationVector, collisionNormalAtoB, contact);
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

    static void ApplyMinimumTraslationVector(physiczobject a, physiczobject b, Vector3 minimumTranslationVectorAtoB, Vector3 collisionNormalAtoB, Vector3 contact)
    {
        ComputeMovementScalars(a, b, out float moveScalarA, out float moveScalarB);

        // calculate Translations
        Vector3 TranslationVectorA = -minimumTranslationVectorAtoB * moveScalarA;
        Vector3 TranslationVectorB = minimumTranslationVectorAtoB * moveScalarB;

        //// Update Positions based on Translations
        a.transform.position += TranslationVectorA;
        b.transform.position += TranslationVectorB;

        //b.transform.Translate(TranslationVectorB);
        //a.transform.Translate(TranslationVectorA);

        Vector3 contactPoint = contact;

        //ApplyMinimumTraslationVector(a, b, minimumTranslationVectorAtoB, collisionNormalAtoB, contact);

        ApplyVelocityResponse(a, b, collisionNormalAtoB);

    }

    static void ComputeMovementScalars(physiczobject a, physiczobject b, out float mtvScalarA, out float mtvScalarB)
    {
        // Check to see if either object is Locked
        if (a.lockPosition && !b.lockPosition)
        {
            mtvScalarA = 0.0f;
            mtvScalarB = 1.0f;
            return;
        }
        if (!a.lockPosition && b.lockPosition)
        {
            mtvScalarA = 1.0f;
            mtvScalarB = 0.0f;
            return;
        }
        if (!a.lockPosition && !b.lockPosition)
        {
            mtvScalarA = 0.5f;
            mtvScalarB = 0.5f;
            return;
        }
        mtvScalarA = 0.0f;
        mtvScalarB = 0.0f;
    }

    static void ApplyVelocityResponse(physiczobject objA, physiczobject objB, Vector3 collisionNormal)
    {
        Vector3 normal = collisionNormal;

        // Velocity of B relative to A
        Vector3 relativeVelocityAB = objB.velocity - objA.velocity;
        // Find relative velocity
        float relativeNormalVelocityAB = Vector3.Dot(relativeVelocityAB, normal);

        // Early exit if they are not going towards each other (no bounce)
        if (relativeNormalVelocityAB >= 0.0f)
        {
            return;
        }

        // Choose a coefficient of restitution
        float restitution = (objA.bounciness + objB.bounciness) * 0.5f;

        float deltaV;

        float minimumRelativeVelocityForBounce = 3.0f;

        // If we only need the objects to slide and not bounce, then...
        if (relativeNormalVelocityAB < -minimumRelativeVelocityForBounce)
        {
            // Determine change in velocity 
            deltaV = (relativeNormalVelocityAB * (1.0f + restitution));
        }
        else
        {
            // no bounce
            deltaV = (relativeNormalVelocityAB);
        }

        float impulse;

        // respond differently based on locked states
        if (objA.lockPosition && !objB.lockPosition)
        {
            // Only B
            impulse = -deltaV * objB.mass;
            objB.velocity += normal * (impulse / (objB.mass));
        }
        else if (!objA.lockPosition && objB.lockPosition)
        {
            // impulse required to creat our desired change in velocity
            // impulse = Force * time = kg * m/s^2 * s = kg m/s
            // impulse / objA.mass == deltaV
            // Only A change velocity
            impulse = -deltaV * objA.mass;
            objA.velocity -= normal * (impulse / (objA.mass));
        }
        else if (!objA.lockPosition && !objB.lockPosition)
        {
            // Both
            impulse = deltaV / ((1.0f / objA.mass) + (1.0f / objB.mass));
            objA.velocity += normal * (impulse / objA.mass);
            objB.velocity -= normal * (impulse / objB.mass);
        }
        else if (!objA.lockPosition && !objB.lockPosition)
        {
            // Nadda
        }
        else
        {
            return;
        }

        // subtract the component of relative velocity that is along the normal of the collision to receive the tangential velocity
        Vector3 relativeSurfaceVelocity = relativeVelocityAB - (relativeNormalVelocityAB * normal);

        ApplyFriction(objA, objB, relativeSurfaceVelocity, normal);
    }

    static void ApplyFriction(physiczobject a, physiczobject b, Vector3 relativeSurfaceVelocityAtoB, Vector3 normalAtoB)
    {
        // Need both objects
        // Need relative surface velocity between objects to know which direction to apply the friction force


        float minFrictionSpeed = 0.0001f;
        float relativeSpeed = relativeSurfaceVelocityAtoB.magnitude;

        // Only apply friction if the relative velocity is significant
        if (relativeSurfaceVelocityAtoB.sqrMagnitude < minFrictionSpeed)
        {
            return;
        }

        float kFrictionCoefficient = (a.frictioniness + b.frictioniness) / -2.0f;

        Vector3 directionToApplyFriction = relativeSurfaceVelocityAtoB / relativeSpeed; // normalizing

        Vector3 gravity1 = new Vector3(0.0f, -9.81f, 0.0f); // Not Sure Why I can't Access Gravity??________________________________???

        float gravityAccelerationAlongNormal = Vector3.Dot(gravity1, normalAtoB);    // * by mass to find force

        Vector3 frictionAcceleration = directionToApplyFriction * gravityAccelerationAlongNormal * kFrictionCoefficient;

        if (!a.lockPosition)
        {
            a.velocity -= frictionAcceleration * Time.fixedDeltaTime;   // didn't divide by mass, but could have if we multiplied by mas earlier
        }
        if (!b.lockPosition)
        {
            b.velocity += frictionAcceleration * Time.fixedDeltaTime;
        }


        // Find Normal
        // Find force of gravity
        // Calculate Normal Force from gravity
        // Choose a coefficient of friction



        // Calculate force of friction (Fnormal * coefficientOfFriction)
        // Apply the force of friction opposite to the relative velocity to create an acceleration



        // Keep in mind we have a feature for "Locked" objects



    }
}

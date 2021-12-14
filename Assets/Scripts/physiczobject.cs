using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physiczobject : MonoBehaviour
{
    //variables
    public float gravityScale = 1.0f;
    public Vector3 velocity = Vector3.zero;
    public PhysiczColliderBase shape = null;
    public bool lockPosition = false;
    public float bounciness = 0.5f;
    public float frictioniness = 0.5f;
    public float mass = 1.0f;

    public typeOfMateral currentMateral;
    //Types of materals for friction
    public enum typeOfMateral
    { 
        Plastic,
        Steel,
        Rubber,
        Ice,
        Wood
    }


    private float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PhysicsSystem>().physiczobjects.Add(this);
        shape = GetComponent<PhysiczColliderBase>();

        if(currentMateral == typeOfMateral.Plastic)
        {
            frictioniness = 0.4f;
        }
        else if (currentMateral == typeOfMateral.Steel)
        {
            frictioniness = 0.8f;
        }
        else if (currentMateral == typeOfMateral.Rubber)
        {
            frictioniness = 1.16f;
        }
        else if (currentMateral == typeOfMateral.Ice)
        {
            frictioniness = 0.1f;
        }
        else if (currentMateral == typeOfMateral.Wood)
        {
            frictioniness = 0.5f;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position = new Vector3(0, Mathf.Sin(Time.time), 0);
        time += Time.fixedDeltaTime;
        float small = 1.0f;
        //check every 3 seconds if the objects velocity is smaller than 0.1f
        //to stop the object from infinitely moving in small amounts
        if (time >= 3.0f && (velocity.x <= small || velocity.x >= -small))
        {
            velocity.x = 0.0f;
            time = 0.0f;
        }
        if (time >= 3.0f && (velocity.y <= small || velocity.y >= -small))
        {
            velocity.y = 0.0f;
            time = 0.0f;
        }
        if (time >= 3.0f && (velocity.z <= small || velocity.z >= -small))
        {
            velocity.z = 0.0f;
            time = 0.0f;
        }

    }
}

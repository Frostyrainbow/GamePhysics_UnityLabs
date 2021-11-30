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


    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PhysicsSystem>().physiczobjects.Add(this);
        shape = GetComponent<PhysiczColliderBase>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position = new Vector3(0, Mathf.Sin(Time.time), 0);
        
        
    }
}

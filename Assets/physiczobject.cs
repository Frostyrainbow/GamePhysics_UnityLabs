using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physiczobject : MonoBehaviour
{
    //variables
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(0, Mathf.Sin(Time.time), 0);
        transform.position = transform.position + velocity * Time.deltaTime;
    }
}

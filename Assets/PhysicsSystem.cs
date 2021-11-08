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
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < physiczobjects.Count; i++)
        {
            physiczobjects[i].velocity += gravity * Time.deltaTime;
        }
       
        //foreach loop works for many types of containers in C#
        //

    }
}

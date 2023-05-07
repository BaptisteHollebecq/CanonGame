using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Vector3 billboard;

    // Update is called once per frame
    void Update()
    {      
        billboard = -(transform.position - Camera.main.transform.position);
        billboard.y = 0;     
    }

    private void LateUpdate()
    {
        transform.forward = billboard;
    }
}

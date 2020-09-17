using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePatch : MonoBehaviour
{
    private bool rotationActive = false;
    public float rotationSpeed = 2.0f;

    [HideInInspector]
    public int targetRotation = 0;
    // Update is called once per frame
    void Update()
    {
        if(!rotationActive)
            return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetRotation), rotationSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.eulerAngles, new Vector3(0, 0, targetRotation==360? 0:targetRotation)) < 0.7f)
        {
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
            rotationActive = false;
        }
    }
    
    public void BeginRotation()
    {
        if(rotationActive)
            return;
        targetRotation = targetRotation != 180 ? 180 : 360;
        rotationActive = true;
    }
}

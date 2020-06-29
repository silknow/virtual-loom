using UnityEngine;
using UnityEngine.EventSystems;

public class TrackballCamera : MonoBehaviour
{
    public float minDistance, maxDistance;
    public float distance = 15f;
    public Vector3 center;
    public Camera camera;

    public Vector3 HPR;
    public float HPRSpeed;

    public float Speed;

    public BoxCollider volume;
 
 
    private void Start()
    {
    }


    public void Center()
    {
        
    }
    private Vector3 lastMousePos;
    private void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            lastMousePos = Input.mousePosition;
        }

        // Update distance
        distance *= 1 - (Input.mouseScrollDelta.y*Speed);
    
         if (distance > -minDistance)
             distance = -minDistance;
         if (distance < -maxDistance)
             distance = -maxDistance;
        
        // Update angles
        if (Input.GetMouseButton(0))
        {
            Vector3 angles = Input.mousePosition - lastMousePos;
//            angles[2] = angles[0];
//            angles[0] = 0;
            HPR += (angles) * HPRSpeed;
        }

        if (Input.GetMouseButton(1)) {
            float meterPixel = Mathf.Tan(camera.fieldOfView*0.5f*Mathf.Deg2Rad) * distance * 2 / Screen.height;
            // center += camera.transform.rotation * (Input.mousePosition - lastMousePos) * meterPixel ;

            Vector3 offset = (Input.mousePosition - lastMousePos) * meterPixel;
            
            offset = camera.transform.localRotation * offset;
            
            // Change translation from Z to Y axis
//            offset[2] = offset[1];
//            offset[1] = 0;

            // float invertX = Vector3.Dot(Vector3.right, camera.transform.right);
            // float invertY = Vector3.Dot(Vector3.up, camera.transform.up);
            // if (invertX < 0)
            //     offset.x = -offset.x;
            // if (invertY < 0)
            //     offset.y = -offset.y;

            center += offset;
            
        }

        // Check center in volume
        if (volume != null) {
            Vector3 p = volume.ClosestPoint(center);
            center = p;
        }
        
        Quaternion cameraRot = Quaternion.Euler(-HPR.y, +HPR.x, HPR.z);
        Vector3 targetPos = center + cameraRot * Vector3.forward * distance;

        camera.transform.localPosition = targetPos;
        camera.transform.localRotation = Quaternion.LookRotation(center - targetPos, cameraRot * Vector3.up);

        lastMousePos = Input.mousePosition;
    }
 
}
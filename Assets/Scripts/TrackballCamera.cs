using UnityEngine;
using UnityEngine.EventSystems;

public class TrackballCamera : MonoBehaviour
{
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
    
        if (distance > -0.3)
            distance = -0.3f;
        if (distance < -50)
            distance = -50;
        
        // Update angles
        if (Input.GetMouseButton(0)) {
            
            HPR += (Input.mousePosition - lastMousePos) * HPRSpeed;
        }

        if (Input.GetMouseButton(1)) {
            float meterPixel = Mathf.Tan(camera.fieldOfView*0.5f*Mathf.Deg2Rad) * distance * 2 / Screen.height;
            // center += camera.transform.rotation * (Input.mousePosition - lastMousePos) * meterPixel ;

            Vector3 offset = (Input.mousePosition - lastMousePos) * meterPixel;
            
            offset = camera.transform.rotation * offset;
            offset[2] = 0;

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

        camera.transform.position = targetPos;
        camera.transform.rotation = Quaternion.LookRotation(center - targetPos, cameraRot * Vector3.up);

        lastMousePos = Input.mousePosition;
    }
 
}
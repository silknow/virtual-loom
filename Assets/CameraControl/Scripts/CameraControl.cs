using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraControl : MonoBehaviour
{
    public static CameraControl instance;
    public CinemachineTargetGroup _cinemachineTargetGroup;
    public Bounds bounds;
    public GameObject target;
    public Transform _rotationAnchor;
    public Transform _translationAnchor;
    public Transform []corners ;
    public CinemachineVirtualCamera _virtualCamera;
    public miniMapButton mmb;
    public RawImage mmrImage;
    public Button flipButton;
    public float followSpeed=1.0f;
    public float offsetSpeed=1.0f;
    public float zoomSpeed=1.0f;
    public float translationSpeed=1.0f;
    public float rotationSpeed=1.0f;
    private Vector3 _lastMousePos;
    private CinemachineTransposer _cameraBody;
    public Vector3 cameraFollowOffsetTarget;
    public float cameraFollowOffsetTargetAngle = 0.0f;
    public float cameraFollowOffsetTargetMinAngle = 5.0f;
    public float cameraFollowOffsetTargetMaxAngle = 85.0f;
    public Vector2 normalizedPosition;
    public Vector2 normalizedSize;
    public float rotation;
    public float boundScale = 3.0f;
    public float aspect;
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private bool AnchorOutOfBounds()
    {
        foreach (Transform corner in corners)
        {
            if (!bounds.Contains(corner.position)) 
                return true;
        }
        return false;
    }
    private void PutAnchorInsideOfBounds()
    {
        foreach (Transform corner in corners)
        {
            if (bounds.Contains(corner.position)) continue;
            var position = corner.position;
            Vector3 desp = bounds.ClosestPoint((position)) - position;
            _translationAnchor.position += desp;
        }
    }

    public void Start()
    {
        if (flipButton!=null)
            flipButton.onClick.AddListener(FlipCameraSide);
        _cameraBody = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        cameraFollowOffsetTarget = _cameraBody.m_FollowOffset;
        mmb.cc = this;
    }
    private void Update()
    {
        PutAnchorInsideOfBounds();
        _cameraBody.m_FollowOffset = Vector3.Lerp(_cameraBody.m_FollowOffset, cameraFollowOffsetTarget, followSpeed*Time.deltaTime);
    }
    public void FlipCameraSide()
    {
        cameraFollowOffsetTargetAngle *= -1.0f;
        float aux = cameraFollowOffsetTargetMaxAngle;
        cameraFollowOffsetTargetMaxAngle = -cameraFollowOffsetTargetMinAngle;
        cameraFollowOffsetTargetMinAngle = -aux;
    }
    public void setNormalizedPosition(Vector2 p)
    {
        _translationAnchor.localPosition=new Vector3(p.x*bounds.extents.x+bounds.center.x,0,p.y*bounds.extents.z+bounds.center.z)/boundScale*aspect;
    }
 
    public void resetRotation()
    {
        _cinemachineTargetGroup.transform.localRotation=_rotationAnchor.localRotation=Quaternion.identity;
        cameraFollowOffsetTargetAngle = cameraFollowOffsetTargetMaxAngle;
        _translationAnchor.localPosition=new Vector3(0,0,1);
        _translationAnchor.localScale = Vector3.one * 0.1f;
        while (!AnchorOutOfBounds())
            _translationAnchor.localScale *=  1.01f;
        _translationAnchor.localPosition=new Vector3(0,0,1);
    }

    public void setAspect(float aspect)
    {
        this.aspect = aspect;
        mmrImage.GetComponent<setAspect>().set(aspect);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (mmb.dragging) return;
        bounds = target.GetComponent<Patch>().bounds;
        bounds.extents *= boundScale;
        //float aspect = bounds.extents.z / bounds.extents.x;
        if (!AnchorOutOfBounds() || Input.mouseScrollDelta.y>0.0f)
            _translationAnchor.localScale *=  Mathf.Max(0.1f,1.0f-Input.mouseScrollDelta.y*Time.deltaTime*zoomSpeed);
        //Ajusta aspect ventana
        _rotationAnchor.localScale = new Vector3(1.0f*Screen.width / Screen.height, 1, 1);
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) 
            _lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 p = _translationAnchor.localPosition;
            _translationAnchor.localPosition += Time.deltaTime * translationSpeed *
                                                (_rotationAnchor.right * (Input.mousePosition.x - _lastMousePos.x) +
                                                 _rotationAnchor.forward * (Input.mousePosition.y - _lastMousePos.y));
        }

        if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
                _cinemachineTargetGroup.transform.localRotation=_rotationAnchor.localRotation *=
                    Quaternion.AngleAxis(Time.deltaTime*rotationSpeed * (Input.mousePosition.x - _lastMousePos.x), Vector3.up); 
        
            cameraFollowOffsetTargetAngle = Mathf.Clamp(cameraFollowOffsetTargetAngle + rotationSpeed * Time.deltaTime *
                                             (Input.mousePosition.y - _lastMousePos.y), cameraFollowOffsetTargetMinAngle, cameraFollowOffsetTargetMaxAngle);
            
        }

        float rotY = 0;
        if (cameraFollowOffsetTargetAngle < 0)
            rotY = 180;
        cameraFollowOffsetTarget =  Quaternion.Euler(cameraFollowOffsetTargetAngle, rotY, 0)*Vector3.back*14.14f;
        if (Input.GetKeyDown(KeyCode.Space))
            resetRotation();
        

                PutAnchorInsideOfBounds();

        rotation = _cinemachineTargetGroup.transform.localRotation.eulerAngles.y;
        if (bounds.extents.sqrMagnitude > 0.0f)
        {
            normalizedPosition = new Vector2((_translationAnchor.localPosition.x - bounds.center.x) / bounds.extents.x,
                (_translationAnchor.localPosition.z - bounds.center.z) / bounds.extents.z/aspect)*boundScale;
            normalizedSize =
                new Vector2(_translationAnchor.localScale.x * _rotationAnchor.localScale.x / bounds.extents.x*boundScale,
                    _translationAnchor.localScale.z * _rotationAnchor.localScale.z / bounds.extents.z*boundScale);
        }
        else
        {
            normalizedPosition=Vector2.zero;
            normalizedSize=Vector2.one;
        }
    }
}

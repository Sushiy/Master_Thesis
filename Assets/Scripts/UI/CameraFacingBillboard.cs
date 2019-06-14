using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    public new Camera camera;

    private void Awake()
    {
        if(camera == null)
        {
            camera = Camera.main;
        }
    }
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
            camera.transform.rotation * Vector3.up);
    }
}
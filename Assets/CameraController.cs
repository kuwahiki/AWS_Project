using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public float MaxX = 60.0f,MinX = 0.0f;
    private CinemachineVirtualCamera camera;
    public float MinY = 1.0f, MaxY = 4.0f;
    public float Scense = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        float MouseY = Input.GetAxis("Mouse Y");
        MouseY *= Scense;

        float y = camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        y += MouseY;
        y =  Mathf.Max(MinY,y);
        y = Mathf.Min(MaxY,y);

        Vector3 rotato = new Vector3(0,y,-3);

        camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = rotato;


        //float MouseX = Input.GetAxis("Mouse X");
        //float MouseY = Input.GetAxis("Mouse Y");

        //MouseY = Mathf.Max(MouseY,MinYRotation);
        //MouseY = Mathf.Min(MouseY,MaxYRotation);

        //MouseX *= Scense;
        //MouseY *= Scense;

        //Vector3 rotato = new Vector3(0, MouseX,0);
        //this.transform.rotation = Quaternion.Euler(rotato);

    }
}

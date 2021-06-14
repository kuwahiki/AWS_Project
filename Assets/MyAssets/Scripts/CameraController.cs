using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera camera;
    public float MinY = 1.0f, MaxY = 3.0f;
    public float Scense = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float MouseY = Input.GetAxis("Mouse Y");
            MouseY *= Scense;

            float y = camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
            y += MouseY;
            y = Mathf.Max(MinY, y);
            y = Mathf.Min(MaxY, y);

            Vector3 rotato = new Vector3(0, y, -3);

            camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = rotato;
        }

    }
}

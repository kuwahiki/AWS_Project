using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetController : MonoBehaviour
{
    private GameObject PlayerModel;
    private CameraController CameraController;
    // Start is called before the first frame update
    void Start()
    {
        string root = gameObject.transform.root.name;
        PlayerModel = GameObject.Find(root + "/Model");
        CameraController = GameObject.Find("CMvcam1").GetComponent<CameraController>();

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = PlayerModel.transform.position;

        if (Input.GetMouseButton(0))
        {
            float MouseX = Input.GetAxis("Mouse X");
            Vector3 rotate = new Vector3(0, MouseX * 1.5f, 0);
            this.transform.Rotate(rotate);
        }
    }
}

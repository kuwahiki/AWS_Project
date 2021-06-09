using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{
    [SerializeField] private Transform MainCamera;
    private const int foword = 1, back = 2,right = 3,left = 4;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 foword = this.transform.position - MainCamera.position;
        foword.y = 0;
        foword = foword.normalized * 10;
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(foword);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 left = new Vector3(-foword.z, 0, foword.x);
            rigidbody.AddForce(left);          
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 right = new Vector3(foword.z, 0, -foword.x);
            rigidbody.AddForce(right);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 back = -foword;
            rigidbody.AddForce(back);
        }

        Vector3 force = rigidbody.velocity;
        force += this.transform.position;
        force.y = 0;

        this.transform.rotation = Quaternion.LookRotation(force);
    }

    private void LookDirection(int direction)
    {
        switch (direction)
        {
            case foword:
                break;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{
    [SerializeField] private Transform MainCamera;
    [SerializeField] private float MaxSpeed = 3.0f;
    private const int foword = 1, back = 2,right = 3,left = 4;
    private int idel, walk;
    private Animator animator;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();

        idel = animator.GetInteger("idel");
        walk = animator.GetInteger("walk");
    }

    // Update is called once per frame
    void Update()
    {
        int animationInt = idel;
        Vector3 foword = this.transform.position - MainCamera.position;
        foword.y = 0;
        foword = foword.normalized * 10;
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(foword);
            animationInt = walk;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 left = new Vector3(-foword.z, 0, foword.x);
            rigidbody.AddForce(left);
            animationInt = walk;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 right = new Vector3(foword.z, 0, -foword.x);
            rigidbody.AddForce(right);
            animationInt = walk;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 back = -foword;
            rigidbody.AddForce(back);
            animationInt = walk;
        }
        animator.SetInteger("animation",animationInt);

        if (animationInt == 1)
        {
            Vector3 force = rigidbody.velocity * 10.0f;
            force += this.transform.position;
            force.y = 0;

            this.transform.rotation = Quaternion.LookRotation(force);
        }
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

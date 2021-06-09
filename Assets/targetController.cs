using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetController : MonoBehaviour
{
    private GameObject PlayerModel;
    // Start is called before the first frame update
    void Start()
    {
        string root = gameObject.transform.root.name;
        PlayerModel = GameObject.Find(root + "/Model");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = PlayerModel.transform.position;
    }
}

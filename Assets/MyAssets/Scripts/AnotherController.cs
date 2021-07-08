using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherController : MonoBehaviour
{
    private bool sex; //男ならtrue,女ならfalse
    [HideInInspector] 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeSex(bool mes)
    {
        if(this.sex != mes)
        {
            GameObject obj;
            Vector3 nowpos = Vector3.zero;
            Quaternion quaternion = Quaternion.identity;
            int elem = this.transform.GetChildCount();
            for (int i = 0; i < elem; i++)
            {
                nowpos = this.transform.GetChild(i).gameObject.transform.position;
                quaternion = this.transform.GetChild(i).gameObject.transform.rotation;
                Destroy(this.transform.GetChild(i).gameObject);
            }
            if (mes)
            {
                obj = (GameObject)Resources.Load("man");
            }
            else
            {
                obj = (GameObject)Resources.Load("woman");
            }
            Instantiate(obj, nowpos, quaternion, this.transform);
            sex = mes;
        }
    }
}

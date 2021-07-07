using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Aws.GameLift.Realtime.Types;

public class Configobjcontroller : MonoBehaviour
{
    public RealTimeClient realTimeClient;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void destory()
    {
        Destroy(this.gameObject);
    }


}

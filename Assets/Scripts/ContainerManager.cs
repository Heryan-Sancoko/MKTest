using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    //45.692

    public List<GameObject> containerList;
    public Transform mPlayer;
    public float offset = 45.692f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (containerList[0].transform.position.x < (mPlayer.position.x - offset))
        {
            containerList[0].SetActive(false);
            containerList[0].SetActive(true);
            containerList[0].transform.position = (containerList[containerList.Count - 1].transform.position) + (Vector3.right * offset);
            containerList.Add(containerList[0]);
            containerList.Remove(containerList[0]);
        }
    }
}

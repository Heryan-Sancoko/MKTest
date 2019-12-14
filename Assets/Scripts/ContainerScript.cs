using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerScript : MonoBehaviour
{

    public List<GameObject> platformList;


    private void OnEnable()
    {
        int selectedPlatform = Random.Range(0, platformList.Count - 1);
        foreach (GameObject platform in platformList)
        {
            platform.SetActive(false);
        }
        platformList[selectedPlatform].SetActive(true);
    }
}

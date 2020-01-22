using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerPool : MonoBehaviour
{
    //Container pool script
    /*
     * This script loads in containers to fill the screen. On my mobile it loads in about 4 during runtime.
     * To avoid instantiating objects during runtime, I could have added more in manually in the editor, however
     * I opted not to so that I could show the pool's ability to automatically fill the screen to the
     * edges with containers.
     */

    [SerializeField]
    private GameObject poolPrefab = null;
    [SerializeField]
    private List<GameObject> poolList = null;
    [SerializeField]
    private float offset = 0;


    void Update()
    {
        // Puts the first container at the end
        CycleContainers();

        // Create more containers if there won't be enough to fill the screen
        if (Camera.main.WorldToScreenPoint(poolList[poolList.Count-1].transform.position).x < Screen.width * 2)
        {
            FillScreenCheck();
        }
    }

    private void CycleContainers()
    {
        if (Camera.main.WorldToScreenPoint(poolList[0].transform.position).x < 0 - Screen.width)
        {
            poolList[0].transform.position = poolList[poolList.Count - 1].transform.position;
            poolList[0].transform.position += Vector3.right * offset;
            poolList.Add(poolList[0]);

            poolList[0].SetActive(false); // A container will randomly pick a stage layout
            poolList[0].SetActive(true);  // upon deactivation

            poolList.Remove(poolList[0]);

        }
    }

    private void FillScreenCheck()
    {
        GameObject newPool = Instantiate(poolPrefab);
        newPool.transform.position = poolList[poolList.Count - 1].transform.position + (Vector3.right * offset);
        poolList.Add(newPool);
    }

    // DeathManager deactivates this upon player's death, deactivating all containers.
    private void OnDisable()
    {
        foreach (GameObject obj in poolList)
        {
            if (obj != null)
            obj.SetActive(false);
        }
    }
}

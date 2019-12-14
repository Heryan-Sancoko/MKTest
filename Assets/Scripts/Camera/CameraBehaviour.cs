using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform mPlayer;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - mPlayer.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(mPlayer.position.x + offset.x, transform.position.y, mPlayer.position.z + offset.z);
        transform.position = newPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{

    public PlayerBehaviour pBehave;
    public float mSpeed;
    private Rigidbody rbody;
    public float speedModifier;
    private float oldPos;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        pBehave = GetComponent<ContainerManager>().mPlayer.GetComponent<PlayerBehaviour>();
        oldPos = pBehave.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        offset = pBehave.transform.position.x - oldPos;
        offset *= speedModifier;
        transform.position = transform.position + (Vector3.right * offset);
        oldPos = pBehave.transform.position.x;
    }
}

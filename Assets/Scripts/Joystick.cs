using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    public enum swipeDirection
    {
        None,  //0
        Up,    //1
        Down,  //2
        Left,  //3
        Right, //4
    }

    public Vector3 swipeOrigin;
    public Vector3 swipePosition;
    public swipeDirection mySwipeDirection;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTouch()
    {
        if (swipeOrigin == Vector3.zero)
        {
            swipeOrigin = Input.touches[0].position;
        }
        transform.position = Input.touches[0].position;
    }

    public void OnRelease()
    {

    }

}

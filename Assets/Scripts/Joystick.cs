using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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

    public Vector2 swipeOrigin, swipePosition;
    public float swipeAngle;
    public Image joystickImage, originImage;
    public swipeDirection mySwipeDirection;

    //Added the following because the swipeDirection would remain until an action is performed.
    //This led to the player being able to input a move while in the air and perform it much later
    //once they hit the ground.
    private float bufferTimer;
    [SerializeField]
    private float bufferLength = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeOutInputBuffer();
    }

    // Removes buffered input after the timer runs out.
    private void TimeOutInputBuffer()
    {
        if (bufferTimer > 0)
        {
            bufferTimer -= Time.deltaTime;
        }
        else
        {
            mySwipeDirection = swipeDirection.None;
        }
    }

    public void OnTouch()
    {
        if (Input.touches.Length != 0)
        {
            if (swipeOrigin == Vector2.zero)
            {
                swipeOrigin = Input.touches[0].position;
                originImage.enabled = true;
                originImage.transform.position = swipeOrigin;
                joystickImage.enabled = true;
            }
            transform.position = swipePosition = Input.touches[0].position;

            Vector2 dir = swipeOrigin - swipePosition;
            swipeAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }

    public void OnRelease()
    {
        joystickImage.enabled = false;
        originImage.enabled = false;

        if (swipeAngle < -45 && swipeAngle > -135)
        {
            //up
            mySwipeDirection = swipeDirection.Up;
        }
        else if (swipeAngle < -135 || swipeAngle > 135)
        {
            //right
            mySwipeDirection = swipeDirection.Right;
        }
        else if (swipeAngle < 135 && swipeAngle > 45)
        {
            //down
            mySwipeDirection = swipeDirection.Down;
        }
        else if (swipeAngle < 45 || swipeAngle > -45)
        {
            //left
            mySwipeDirection = swipeDirection.Left;
        }

        bufferTimer = bufferLength;
        swipeOrigin = Vector2.zero;
    }

}

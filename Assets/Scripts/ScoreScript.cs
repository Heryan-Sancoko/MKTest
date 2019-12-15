using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public float distance;
    private float startingPosition;
    public Transform mPlayer;
    public float watermelonBonus;
    private Text mText;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = mPlayer.transform.position.x;
        mText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = mPlayer.transform.position.x - startingPosition;
        mText.text = "DISTANCE TRAVELLED: " + distance.ToString("F2") + "km" + "\n" + "\n" + "WATERMELON BONUS:" + " " + (watermelonBonus).ToString("F1");
    }


}

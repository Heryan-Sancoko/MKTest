using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public List<GameObject> deactivationList;
    public List<GameObject> activationList;

    public ScoreScript mScore;

    public Text distanceTravelled;
    public Text watermelonMultiplier;
    public Text totalScore;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        foreach (GameObject obj in deactivationList)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in activationList)
        {
            obj.SetActive(true);
        }


        distanceTravelled.text = mScore.distance.ToString("F2");
        watermelonMultiplier.text = "+ Distance x " + mScore.watermelonBonus.ToString("F2");
        float scoreSum = mScore.distance + (mScore.distance * mScore.watermelonBonus);
        totalScore.text = scoreSum.ToString("F2");
        mScore.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Debug.Log("Scene restarted");
    }
}

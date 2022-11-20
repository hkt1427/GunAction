using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public GameObject[] stageButtons;

    public GameObject area1;
    public GameObject area2;
    public GameObject area3;
    
    public GameObject backButton;

    public AudioSource audioSourceButton;

    public AudioClip buttonAudio;

    void Start()
    {
        audioSourceButton = GetComponent<AudioSource>();
        area1.SetActive(true);
        area2.SetActive(false);
        area3.SetActive(false);

        int clearStageNo = PlayerPrefs.GetInt("CLEAR", 0);

        for (int i = 0; i <= stageButtons.GetUpperBound(0); i ++)
        {
            bool b;

            if (clearStageNo < i)
            {
                b = false;
            }
            else
            {
                b = true;
            }

            stageButtons [i].GetComponent<Button> ().interactable = b;
        }
    }

    public void PushStageselectNormal(int stageNo)
    {
        SceneManager.LoadScene("NormalStage" + stageNo);
    }

    public void PushStageselectSecond(int stageNo)
    {
        SceneManager.LoadScene("SecondStage" + stageNo);
    }

    public void PushStageselectThird(int stageNo)
    {
        SceneManager.LoadScene("ThirdStage" + stageNo);
    }

    public void Area1Button()
    {
        area1.SetActive(false);
        area2.SetActive(true);
        area3.SetActive(false);
        audioSourceButton.PlayOneShot (buttonAudio);
    }
    public void Area2LeftButton()
    {
        area1.SetActive(true);
        area2.SetActive(false);
        area3.SetActive(false);
        audioSourceButton.PlayOneShot (buttonAudio);
    }

    public void Area2LightButton()
    {
        area1.SetActive(false);
        area2.SetActive(false);
        area3.SetActive(true);
        audioSourceButton.PlayOneShot (buttonAudio);
    }

    public void Area3Button()
    {
        area1.SetActive(false);
        area2.SetActive(true);
        area3.SetActive(false);
        audioSourceButton.PlayOneShot (buttonAudio);
    }


    public void BackButton()
    {
        SceneManager.LoadScene("Title");
    }
}

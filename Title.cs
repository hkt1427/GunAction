using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public GameObject stageButton;
    public GameObject ruleButton;
    public GameObject titleBackButton;
    public GameObject rule;
    public GameObject title;
    public GameObject buttonBacks;

    void Start()
    {
        rule.SetActive(false);
        stageButton.SetActive(true);
        ruleButton.SetActive(true);
        titleBackButton.SetActive(false);
        title.SetActive(true);
        buttonBacks.SetActive(true);
    }

    public void StageButton()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void RuleButton()
    {
        rule.SetActive(true);
        stageButton.SetActive(false);
        ruleButton.SetActive(false);
        titleBackButton.SetActive(true);
        title.SetActive(false);
        buttonBacks.SetActive(false);

    }

    public void TitleBackButton()
    {
        rule.SetActive(false);
        stageButton.SetActive(true);
        ruleButton.SetActive(true);
        titleBackButton.SetActive(false);
        title.SetActive(true);
        buttonBacks.SetActive(true);
    }
}

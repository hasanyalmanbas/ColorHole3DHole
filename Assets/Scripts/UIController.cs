using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject finishPanel;
    public GameObject gameOverPanel;

    void Start()
    {
        finishPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        GameController.Instance.finishGameEvent.AddListener(FinishGame);
        GameController.Instance.gameOverEvent.AddListener(GameOver);
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    void FinishGame()
    {
        finishPanel.SetActive(true);
    }
}
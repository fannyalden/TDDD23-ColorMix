using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    [SerializeField]
    private GameObject highScorePanel;

    [SerializeField]
    private GameObject pausePanel;
    
    void Awake() {
        pausePanel.SetActive(false);
        highScorePanel.SetActive(false);
    }

    public void PausGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void NewHighScore()
    {
        highScorePanel.SetActive(true);
    }

    public void GoToHighScoreScene()
    {
        highScorePanel.SetActive(false);
        SceneManager.LoadScene("HighScore");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

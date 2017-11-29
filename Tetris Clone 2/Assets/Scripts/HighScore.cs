using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour {

	public Text highScoreText1;
    public Text highScoreText2;
    public Text highScoreText3;

    // Use this for initialization
    void Start () {
        //PlayerPrefs.SetInt("highscore", 0);
        highScoreText1.text = PlayerPrefs.GetInt("highscore1").ToString();
        highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();
        highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();
    }

    public void GoBack(){
        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using System.Text;

public sealed class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;
    public TextMesh scoreBoard;

    void Awake () {
        if (instance == null) 
            instance = this;
        else 
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore (string name, int score) {
        
        for (int i = 0; i < 5; i++) {
            if (PlayerPrefs.HasKey(i+"Score")) {
                if (PlayerPrefs.GetInt(i+"Score")  < score) {
                    PlayerPrefs.SetInt(i+"Score", score);
                    PlayerPrefs.SetString(i+"ScoreName", name);
                    break;
                }
            } else {
                PlayerPrefs.SetInt(i+"Score", score);
                PlayerPrefs.SetString(i+"ScoreName", name);
                break;
            }
        }
    }

    public void ShowScores () {
        int score;
        string name;
        StringBuilder scores = new StringBuilder();

        for (int i = 0; i < 5; i++) {
            if (PlayerPrefs.HasKey(i+"Score")) {
                score = PlayerPrefs.GetInt(i+"Score");
                name = PlayerPrefs.GetString(i+"ScoreName");
                scores.Append(name + " - " + score + "\n");
            } else {
                scores.Append("AAA - 000\n");
            }
        }

        if (scoreBoard == null) 
            scoreBoard = GameObject.FindWithTag("Ranking").GetComponent<TextMesh>();

        if (scoreBoard != null)
            scoreBoard.text = scores.ToString();

    }


}

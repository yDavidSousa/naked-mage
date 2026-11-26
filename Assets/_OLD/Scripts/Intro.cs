using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Intro : MonoBehaviour {

    public void Start () {
        Invoke("nextScene", 2f);
    }

    public void nextScene () {
		SceneManager.LoadScene("MainMenu");
    }
}

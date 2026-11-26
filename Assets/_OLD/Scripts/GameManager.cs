using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public AudioClip countDownClip;
	public float playerHp, playerMana;
	public bool playerDead, inPause;
	public int score;
	
	public Image HUD_Life, HUD_Mana;
	public TextMesh scoreCounter, hordCounter, battleStart;
	public GameObject mask, gigantPauseInMountains, bt_Menu, gameOver, inputPlayerName;

	void Awake () {
		if (instance == null) 
			instance = this;
		else Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		score = 0;
		playerHp = 100f;
		playerMana = 100f;
		playerDead = false;
		inPause = false;

	}
	
	void Update () {
		UpdateHUD();	
		//scoreCounter.text = "" + score;

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (!inPause) Pause();
			else unPause();
		}

		if (playerDead) 
			GameOver();

	}

	void UpdateHUD () {
		//HUD_Life.fillAmount = playerHp / 100;
		//HUD_Mana.fillAmount = playerMana / 100;
	}

	public IEnumerator AlertNewHorde () {
		SoundManager.instance.PlayInBackground(countDownClip, 0.4f);
		hordCounter.text = "Horde " + EnemiesManager.instance.horde;
		battleStart.text = "3";
		yield return new WaitForSecondsRealtime(0.9f);
		battleStart.text = "2";
		yield return new WaitForSecondsRealtime(0.9f);
		battleStart.text = "1";
		yield return new WaitForSecondsRealtime(0.9f);
		battleStart.text = "Kill them all!";
		yield return new WaitForSecondsRealtime(1f);
		battleStart.text = "";
	}

	public void UpdateScore (int points) {
		score += points;
	}

	public void Pause () {
		inPause = true;
		mask.SetActive(true);
		gigantPauseInMountains.SetActive(true);
		bt_Menu.SetActive(true);
		Time.timeScale = 0;
	}
	public void unPause () {
		inPause = false;
		mask.SetActive(false);
		gigantPauseInMountains.SetActive(false);
		bt_Menu.SetActive(false);
		Time.timeScale = 1;
	}

	public void BackToMainMenu () {
		// Destroi instâncias
		Destroy(instance);
		Destroy(SoundManager.instance);
		Destroy(SoundManager.instance);
		Destroy(EnemiesManager.instance);
		// retira pause
		Time.timeScale = 1;
		// volta para menu princiapl
		UnityEngine.SceneManagement.SceneManager.LoadScene("mainMenu");
	}

	public void GameOver () {
		Time.timeScale = 0;
		mask.SetActive(true);
		bt_Menu.SetActive(true);
		inputPlayerName.SetActive(true);
		gameOver.SetActive(true);
	}

	public void SaveScore (string name) {
		ScoreManager.instance.AddScore(name, score);
		BackToMainMenu();
	}
}

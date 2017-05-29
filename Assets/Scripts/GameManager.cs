using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public float playerHp, playerMana;
	public bool playerDead;
	
	public UnityEngine.UI.Image HUD_Life, HUD_Mana, HUD_Power;
	public UnityEngine.UI.Text hordCounter, battleStart;

	void Awake () {
		if (instance == null) 
			instance = this;
		else Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		playerHp = 100f;
		playerMana = 100f;
		playerDead = false;
	}
	
	void Update () {
		UpdateHUD();	
	}

	void UpdateHUD () {
		HUD_Life.fillAmount = playerHp / 100;
		HUD_Mana.fillAmount = playerMana / 100;
	}

	public IEnumerator AlertNewHorde () {
		hordCounter.text = "Horde " + EnemiesManager.instance.horde;
		battleStart.text = "3";
		yield return new WaitForSecondsRealtime(0.5f);
		battleStart.text = "2";
		yield return new WaitForSecondsRealtime(0.5f);
		battleStart.text = "1";
		yield return new WaitForSecondsRealtime(0.5f);
		battleStart.text = "Kill them all!";
		yield return new WaitForSecondsRealtime(0.5f);
		battleStart.text = "";
	}

	void SetPower () {

	}
}

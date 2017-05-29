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
		UpdateHordNumber();	
	}

	void UpdateHUD () {
		HUD_Life.fillAmount = playerHp / 100;
		HUD_Mana.fillAmount = playerMana / 100;
	}

	void UpdateHordNumber () {
		hordCounter.text = "Horde " + EnemiesManager.instance.horde;
	}

	void SetPower () {

	}
}

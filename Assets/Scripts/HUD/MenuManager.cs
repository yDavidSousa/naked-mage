using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState {
	MAINMENU, CREDITS, RANKING
}

public class MenuManager : MonoBehaviour {

	public GameState gameState;
	public void Update () {

		// Caso usuário aperte ESC
		if (Input.GetKeyDown(KeyCode.Escape)) {
			switch (gameState) {
				case (GameState.MAINMENU): 
		 			Application.Quit();
					break;
				case (GameState.RANKING | GameState.CREDITS):
					gameState = GameState.MAINMENU;
					break;
			}
		}
		
	}

	// Clicou no play
	public void OnPlay () {
		SceneManager.LoadScene("battle");
	}
}

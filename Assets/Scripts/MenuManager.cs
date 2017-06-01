using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState {
	MAINMENU, CREDITS, RANKING
}

public class MenuManager : MonoBehaviour {

	public GameState gameState;
	public GameObject bt_play, bt_credits, bt_ranking, bt_close;
	public GameObject scr_credits, scr_background, scr_ranking, mask;
	public GameObject txt_ranking;


	public void Start () {
		gameState = GameState.MAINMENU;
		RenderState();
	}

	public void Update () {

		// Caso usuário aperte ESC
		if (Input.GetKeyDown(KeyCode.Escape)) {
			switch (gameState) {
				case (GameState.MAINMENU): 
		 			Application.Quit();
					break;
				default: // CREDITS OU RANKING
					gameState = GameState.MAINMENU;
					RenderState();
					break;
			}
			RenderState();
		}
		
	}

	private void RenderState () {
		switch (gameState) {
			case GameState.MAINMENU: 
				DesactiveCredits();
				DesactiveRanking();
				ActiveMainMenu();
				break;
			case GameState.CREDITS:
				DesactiveMainMenu();
				ActiveCredits();
				break;
			case GameState.RANKING:
				DesactiveMainMenu();
				ActiveRanking();
				break;
		}
	}

	/*
	BOTÕES
	*/

	// Clicou no play
	public void OnPlay () {
		SceneManager.LoadScene("battle");
	}

	// Clicou nos creditos
	public void OnCredits () {
		gameState = GameState.CREDITS;
		RenderState();
	}

	// Clicou no ranking
	public void OnRanking () {
		gameState = GameState.RANKING;
		RenderState();
	}

	// Clicou no botão de voltar
	public void OnBack () {
		gameState = GameState.MAINMENU;
		RenderState();
	}

	/*
	DESATIVAR OU ATIVAR ESTADOS
	*/

	// ativa os game objects que compõem a tela de menu
	private void ActiveMainMenu () {
		bt_play.SetActive(true);
		bt_credits.SetActive(true);
		bt_ranking.SetActive(true);
	}

	// desativa os game objects que compõem a tela de menu
	private void DesactiveMainMenu () {
		bt_play.SetActive(false);
		bt_credits.SetActive(false);
		bt_ranking.SetActive(false);
	}

	// ativa os game objects que compõem a tela de créditos
	private void ActiveCredits () {
		mask.SetActive(true);
		bt_close.SetActive(true);
		scr_credits.SetActive(true);
	}

	// desativa os game objects que compõem a tela de creditos
	private void DesactiveCredits () {
		mask.SetActive(false);
		bt_close.SetActive(false);
		scr_credits.SetActive(false);
	}

	// ativa os game objects que compõem a tela de ranking
	private void ActiveRanking () {
		mask.SetActive(true);
		bt_close.SetActive(true);
		scr_ranking.SetActive(true);
		txt_ranking.SetActive(true);
		ScoreManager.instance.ShowScores();
	}

	// desativa os game objects que compõem a tela de ranking
	private void DesactiveRanking () {
		mask.SetActive(false);
		bt_close.SetActive(false);
		scr_ranking.SetActive(false);
		txt_ranking.SetActive(false);
	}

}

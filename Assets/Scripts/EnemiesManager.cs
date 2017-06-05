using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyStates {
    IDLE, // DE BOA
    INGUARD, // EM GUARDA
    ALERT, // DETECTOU PLAYER
    AGRESSIVE, // PERSEGUE PLAYER
    ATTACK // ATACA PLAYER
}

public class EnemiesManager : MonoBehaviour {

	public static EnemiesManager instance;
	public int horde = 1;
	public bool waterEnemies = false, fireEnemies = false, flyingEnemies = false, inBattle = false;

	public GameObject enemyEarth, enemyWater, enemyFire, enemyAir;
	private Vector3 spawnPointGround1, spawnPointGround2, spawnPointAir1, spawnPointAir2, 
					actualGroundSpawnPoint, actualAirSpawnPoint;

	void Awake () {
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
		
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		inBattle = false;
		
		// define pontos de nascimento dos inimigos
		spawnPointGround1 = new Vector3(1.741f, -0.457f, 0f);
		spawnPointGround2 = new Vector3(-1.741f, -0.457f, 0f);
		spawnPointAir1 = new Vector3(1.741f, 0.79f, 0f);
		spawnPointAir2 = new Vector3(-1.741f, 0.79f, 0f);

		// inicializa os pontos de nascimento
		actualAirSpawnPoint = spawnPointAir1;
		actualGroundSpawnPoint = spawnPointGround1;

		// chama primeira horda
		StartCoroutine(SpawnEnemies());
	}

	void Update () {
		if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0 && inBattle) {
			inBattle = false;
			horde++; // próxima horda
			StartCoroutine(SpawnEnemies()); // instância os inimigos
			GameManager.instance.UpdateScore(50 * horde);
		}
	}

	IEnumerator SpawnEnemies () {
		
		// alerta ao player sobre a nova horda
		StartCoroutine(GameManager.instance.AlertNewHorde());
		yield return new WaitForSecondsRealtime(1f);

		// ativa tipo de inimigo de acordo com a horda atual
		if (horde > 1) {
			waterEnemies = true;
		} if (horde > 2) {
			fireEnemies = true;
		} if (horde > 4) {
			flyingEnemies = true;
		}

		int numberOfEnemies = 5 * horde; // adiciona 5 inimigos a mais por horda

		// instancia os inimigos de forma aleatoria e alternando a posição da tela à cada nova instância
		for (int i = 1, j = 0; i <= numberOfEnemies / 5; i++) {
			for (j = 1 ; j <= 5; j++) {
				switch (aleatoryEnemy()) {
					case 0:
						StartCoroutine(instantiateEnemy(enemyEarth, actualGroundSpawnPoint, j*i/2));
						break;
					case 1:
						StartCoroutine(instantiateEnemy(enemyWater, actualGroundSpawnPoint, j*i/2));
						break;
					case 2:
						StartCoroutine(instantiateEnemy(enemyFire, actualGroundSpawnPoint, j*i/2));
						break;
					case 3: 
						StartCoroutine(instantiateEnemy(enemyAir, actualAirSpawnPoint, j*i/2));
						break;
					default: 
						StartCoroutine(instantiateEnemy(enemyFire, actualGroundSpawnPoint, j*i/2));
						break;
				}
				ChangeSpawnPoint();
			}
		}
		
	}

	int aleatoryEnemy () {
		if (!waterEnemies && !fireEnemies && !flyingEnemies)
			return 0;
		else {
			if (flyingEnemies)
				return Mathf.RoundToInt(Random.Range(0, 4));
			else 
			return (fireEnemies) ? Mathf.RoundToInt(Random.Range(0, 3)):
								   Mathf.RoundToInt(Random.Range(0, 2));
		}
	}


	IEnumerator instantiateEnemy(GameObject enemy, Vector3 position, int delay) {
    	yield return new WaitForSeconds(delay);
		Instantiate(enemy, position, Quaternion.identity);
		if (!inBattle) inBattle = true;
 	}

	void ChangeSpawnPoint () {
		actualAirSpawnPoint = (actualAirSpawnPoint == spawnPointAir1) ? spawnPointAir2 : spawnPointAir1;
		actualGroundSpawnPoint = (actualGroundSpawnPoint == spawnPointGround1) ? 
																		spawnPointGround2 : spawnPointGround1;
	}

}
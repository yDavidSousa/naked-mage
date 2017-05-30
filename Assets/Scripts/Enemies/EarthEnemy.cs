using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthEnemy : MonoBehaviour {


	public Animator animator;
	public Animation animation;
	public bool inGround;
	public float velocity = 0.1f;
	public Vector3 playerPosition;
	public GameObject insidePrefab;

	void Start () {
		animator = gameObject.GetComponent<Animator>();
		CatchPlayer();
	}

	/*
	Pega a posição atual do player e volta a ir na direção dele
	*/
	void CatchPlayer () {
		animator.Play("Idle", 0, 0f); // Restart animation
		playerPosition = GameObject.FindWithTag("Player").transform.position;
		InvokeRepeating("Dig", 0.2f, 1f);
		inGround = true;
	}
	
	void Update () {
		if (Mathf.Abs(playerPosition.x - transform.position.x) <= 0.01f && inGround) { // caso esteja próximo ao player
			inGround = false;
			animator.Play("Attack", -1, 0f); // ativa animação de sair e entrar na terra
			CancelInvoke("Dig"); // cancela invocação da função Dig
			Invoke("CatchPlayer", 5f); // ativa a função CatchPlayer
		}
	}

	/*
		Instância o prefab da terra cavada e atualiza a posição do inimigo de acordo com a posição do player
	*/
	void Dig () {

		var difference = playerPosition.x - transform.position.x;
		if (difference > 0) {
			if (difference > velocity) transform.position += Vector3.right * velocity;
			else transform.position += Vector3.right * difference;
		} else {
			if (difference < velocity) transform.position -= Vector3.right * velocity;
			else transform.position -= Vector3.right * difference;
		}

		// instancia prefab de terra cavada
		Instantiate(insidePrefab, new Vector3(transform.position.x, -0.75f, transform.position.z), Quaternion.identity);
	}

	void OnTriggerEnter2D (Collider2D collider2D) {

		if (collider2D.gameObject.CompareTag("Power"))  {
			Destroy(this.gameObject, 0.1f);
		}

		if (collider2D.gameObject.CompareTag("Collider")) {
			if (transform.rotation.eulerAngles.y > 0)
				transform.rotation = Quaternion.identity;
			else
				transform.rotation = Quaternion.Euler(0, 180, 0);

			CatchPlayer();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum attackDirection {
	up, side, down
}

public class Player : MonoBehaviour {

	private Rigidbody2D rigidbody2D;
	private RaycastHit2D hit;
	public GameObject skill;

	public Animator playerAnimator;
	public float velocity, jumpHeigh;
	public AudioClip attackClip, jumpClip, pickUpItem;
	public bool horizontalMovement, verticalMovement, attack, jumping, inGround;

	// controls
	public KeyCode btLeft, btRight, btUp, btDown, btAttack, btJump;

	void Start () {
		playerAnimator = gameObject.GetComponent<Animator>();
		rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

		inGround = true;
	}
	
	void Update () {
		horizontalMovement = Input.GetKey(btLeft) || Input.GetKey(btRight);
		verticalMovement = Input.GetKey(btUp) || Input.GetKey(btDown);
		jumping = Input.GetKeyDown(btJump);

		inGround = Physics2D.Raycast(transform.position, Vector2.down, 0.15f, 1 << LayerMask.NameToLayer("Ground"));

		if (Input.GetKeyDown(btAttack)) { 
			if (!inGround) { // attack in air
				if (horizontalMovement) // lateral attack 
					Attack(attackDirection.side); 
				else if (Input.GetKey(btDown)) // attack in down direction
					Attack(attackDirection.down); 
				else // attack to up
					Attack(attackDirection.up); 
			} else { 
				if (Input.GetKey(btUp)) // attack up
					Attack(attackDirection.up);
				else
					Attack(attackDirection.side); // normal attack
			}
		} else {
			if (horizontalMovement) // running in horizontal
				RunInHorizontal(); 
			if (jumping && inGround) // jump
				Jump();
		}
	}

	/*
	* ATUALIZA OS COLISORES DAS PLATAFORMAS DE ACORDO COM A POSIÇÃO DO PLAYER.
	* SE ESTÁ EM CIMA DESATIVA O TRIGGER, SE ESTÁ AO LADO OU EMBAIXO, ATIVA.
	*/
	void FixedUpdate () {
		hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, 1 << LayerMask.NameToLayer("Ground"));
		if (hit) hit.collider.isTrigger = false;

		hit = Physics2D.Raycast(transform.position, Vector2.left, 0.5f, 1 << LayerMask.NameToLayer("Ground"));
		if (hit) hit.collider.isTrigger = true;
		hit = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, 1 << LayerMask.NameToLayer("Ground"));
		if (hit) hit.collider.isTrigger = true;
		hit = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, 1 << LayerMask.NameToLayer("Ground"));
		if (hit) hit.collider.isTrigger = true;
	}

	/*
	AÇÕES
	*/

	void RunInHorizontal () {
		if (Input.GetAxis("Horizontal") > 0) {
			transform.rotation = Quaternion.identity;
			transform.position += Vector3.right * velocity * Time.deltaTime;
		} else {
			transform.rotation = Quaternion.Euler(0, 180, 0);
			transform.position += Vector3.left * velocity * Time.deltaTime;
		}
		setRunningAnimation();
	}

	void Jump () {
		inGround = false;
		playerAnimator.Play("Jump");
		SoundManager.instance.PlaySingle(jumpClip);
		rigidbody2D.AddForce(Vector2.up * jumpHeigh, ForceMode2D.Impulse);
	}

	void Attack (attackDirection direction) {
		setAttackAnimation(direction);
		SoundManager.instance.PlaySingle(attackClip);
		Debug.Log("Player atacou na direção " + direction.ToString());
	}

	void TakeDamage (float damage) {
		GameManager.instance.playerHp -= damage;
		if (GameManager.instance.playerHp <= 0)
			Die();
		Debug.Log("Player tomou "+ damage + " de dano");
	}

	void Die () {
		GameManager.instance.playerDead = true;
		// LANÇAR PREFAB DE MORTE
		gameObject.SetActive(false);
		Debug.Log("Player morreu");
	}

	/*
	TRATAMENTO DE COLISÕES
	*/
	
	void OnCollisionEnter2D (Collision2D collision2D) {
		if (collision2D.gameObject.CompareTag("Enemy")) // sendo atacado
			TakeDamage(10);
	}
	void OnCollisionStay2D (Collision2D collision2D) {
		if (collision2D.gameObject.CompareTag("Enemy")) // sendo atacado
			TakeDamage(0.1f);
	}

	void OnTriggerEnter2D (Collider2D collider2D) {
		if (collider2D.gameObject.CompareTag("Enemy"))
			TakeDamage(5);
	}

	void OnTriggerStay2D (Collider2D collider2D) {
		if (collider2D.gameObject.CompareTag("Enemy"))
			TakeDamage(0.1f);
	}

	/* 
	ANIMAÇÕES
	*/

	void setIdleAnimation () {
		playerAnimator.Play("Idle");
	}
	void setRunningAnimation () {
		playerAnimator.Play("run");
	}

	void setJumpAnimation () {
		playerAnimator.Play("Jump");
	}
	void setLandingAnimation () {
		playerAnimator.Play("Landing");
	}

	void setAttackAnimation (attackDirection direction) {
		GameObject power = Instantiate(skill, transform.position, Quaternion.identity) as GameObject;
		switch (direction) {
			case attackDirection.side:
				playerAnimator.Play("sideAttack");
				if (transform.rotation.eulerAngles.y > 0)
					power.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 2f, ForceMode2D.Impulse);
				else
					power.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2f, ForceMode2D.Impulse);
				break;
			case attackDirection.down:
				playerAnimator.Play("downAttack");
				power.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 2f, ForceMode2D.Impulse);
				break;
			case attackDirection.up:
				playerAnimator.Play("topAttack");
				power.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
				break;
		}
	}
}

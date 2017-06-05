using UnityEngine;

public class FireBomb : MonoBehaviour {
    Vector2 playerPosition;
    public Animator animator;
    public float velocity = 5f;
    public bool dead;

    public void Start () {
        playerPosition = GameObject.FindWithTag("Player").transform.position;
        animator = gameObject.GetComponent<Animator>();
        dead = false;
    }

    public void Update () {

        var difference = playerPosition.x - transform.position.x;

		if (difference > 0) {
			if (difference > velocity) transform.position += Vector3.right * velocity * Time.deltaTime;
			else transform.position += Vector3.right * difference * Time.deltaTime;
		} else {
			if (difference < velocity) transform.position -= Vector3.right * velocity * Time.deltaTime;
			else transform.position -= Vector3.right * difference * Time.deltaTime;
		}

        if (Mathf.Abs(playerPosition.x - transform.position.x) <= 0.01f && !dead) { // caso esteja próximo ao player
            Explode();
		}

    }

    public void Explode () {
        dead = true;
        animator.Play("BombExplosion", 0, 0f); // ativa animação de explosão
        Destroy(gameObject, 1f);
    }

    public void OnTriggerEnter2D (Collider2D collider2D) {
        if (collider2D.gameObject.CompareTag("Player")) {
            playerPosition = GameObject.FindWithTag("Player").transform.position;
        }
    }

}

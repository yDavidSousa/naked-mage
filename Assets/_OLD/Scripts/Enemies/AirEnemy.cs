using System.Collections;
using UnityEngine;

public sealed class AirEnemy : MonoBehaviour {

    public AudioClip deathClip;
    
    public Animator animator;
	public float velocity = 0.5f, playerDistance;
    public EnemyStates actualState;

    public float attackDelay;

    // MARCADORES DE POSIÇÕES
    private Vector3 playerPosition,
                    initialPosition,
                    inGuardPosition,
                    targetPosition;

    public void Start () {
        initialPosition = transform.position;
        animator = gameObject.GetComponent<Animator>();
        inGuardPosition = new Vector3(Random.Range(0, 1.5f), transform.position.y, 0);

        targetPosition = inGuardPosition;
        SetState(EnemyStates.INGUARD);
    }

    public void SetState (EnemyStates state) {
        actualState = state;

        switch (state) {
            case EnemyStates.IDLE:
                animator.Play("Fly", 0, 0f);
                break;
            case EnemyStates.INGUARD:
                animator.Play("Fly", 0, 0f);
                break;
            case EnemyStates.AGRESSIVE:
                animator.Play("FlyFaster", 0, 0f);
                break;
            case EnemyStates.ATTACK:
                animator.Play("Attack", -1, 0f);
                break;
        }
    }

    public void Update () {

        playerPosition = GameObject.FindWithTag("Player").transform.position;
        playerDistance = Vector3.Distance(playerPosition, transform.position);
        attackDelay += Time.deltaTime;

        if (playerDistance < 0.7f && 
                        (actualState == EnemyStates.IDLE || actualState == EnemyStates.INGUARD))
            SetState(EnemyStates.AGRESSIVE); // Se a distância até o player é menor que 0.9 unidades, muda o estado para alerta

         switch (actualState) {
            case EnemyStates.IDLE:
                targetPosition = (targetPosition == inGuardPosition) ? 
                                        initialPosition : inGuardPosition; // Se chegou na posição inGuard, volta para a posição inicial
                SetState(EnemyStates.INGUARD);
                break;
            case EnemyStates.INGUARD: 
                MoveTo(targetPosition); // Só fica se movendo de um lado para o outro
                break;
            case EnemyStates.AGRESSIVE:
                if (Mathf.Abs(playerPosition.y - transform.position.y) > 1f)  
                    SetState(EnemyStates.INGUARD); 
                else if (playerDistance < 0.4f && attackDelay > 1.5f)
                    SetState(EnemyStates.ATTACK);

                MoveTo(playerPosition);
                break;
            case EnemyStates.ATTACK:
                StartCoroutine(Attack());
                break;
        }
    }

    void MoveTo (Vector3 destiny) {
        var difference = destiny.x - transform.position.x;

        if (Mathf.Abs(difference) <= 0.04f)
            return;

        if (difference > 0) {
			transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.position += (difference > velocity * Time.deltaTime) ?
                                Vector3.right * velocity * Time.deltaTime : Vector3.right * difference;
        } else {
            transform.rotation = Quaternion.identity;
            transform.position -= (difference < velocity * Time.deltaTime) ?
                                Vector3.right * velocity * Time.deltaTime : Vector3.right * difference;
        }

        if (Mathf.Abs(destiny.x - transform.position.x) <= 0.05f && actualState == EnemyStates.INGUARD)
            SetState(EnemyStates.IDLE);
    }

    IEnumerator Attack () {
        if (attackDelay > 1.5f) {
            attackDelay = 0f; 
      //       transform.DOMove(Vector3.MoveTowards(transform.position, playerPosition, 1f), 0.2f).
						// SetLoops(-1, LoopType.Yoyo).
						// SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(0.85f);
            SetState(EnemyStates.AGRESSIVE);
        }
    }

	void OnTriggerEnter2D (Collider2D collider2D) {

		if (collider2D.gameObject.CompareTag("Power"))  {
			GameManager.instance.UpdateScore(30);
            SoundManager.instance.PlaySingle(deathClip);
			Destroy(this.gameObject, 0.1f);
		}

	}
}

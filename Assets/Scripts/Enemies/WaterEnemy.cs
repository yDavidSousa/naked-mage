using System.Collections;
using UnityEngine;

public class WaterEnemy : MonoBehaviour {
    public Animator animator;
	public float velocity, playerDistance;
    public EnemyStates actualState;

    // VELOCIDADES
    public const float idleVelocity = 0.2f, alertVelocity = 0.5f;

    public float idleCount, attackDelay;

    // MARCADORES DE POSIÇÕES
    private Vector3 playerPosition,
                    initialPosition,
                    inGuardPosition,
                    targetPosition;

    public void Start () {
        initialPosition = transform.position;
        animator = gameObject.GetComponent<Animator>();
        inGuardPosition = new Vector3(Random.Range(0, 1.5f), transform.position.y, 0);

        idleCount = 0f;

        velocity = idleVelocity;
        targetPosition = inGuardPosition;
        SetState(EnemyStates.INGUARD);
    }

    public void SetState (EnemyStates state) {
        actualState = state;

        switch (state) {
            case EnemyStates.IDLE:
                animator.Play("Idle", 0, 0f);
                velocity = idleVelocity;
                break;
            case EnemyStates.INGUARD:
                animator.Play("Running", 0, 0f);
                break;
            case EnemyStates.ALERT:
                animator.Play("Alert", 0, 0f);     
                velocity = alertVelocity;     
                break;
            case EnemyStates.AGRESSIVE:
                animator.Play("Running", 0, 0f);
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

        if (playerDistance < 0.9f && 
                        (actualState == EnemyStates.IDLE || actualState == EnemyStates.INGUARD))
            SetState(EnemyStates.ALERT); // Se a distância até o player é menor que 0.9 unidades, muda o estado para alerta

         switch (actualState) {
            case EnemyStates.IDLE:
                idleCount += Time.deltaTime; // conta 2 seg Idle
                if (idleCount > 2f) {
                    idleCount = 0f;

                    targetPosition = (targetPosition == inGuardPosition) ? 
                                            initialPosition : inGuardPosition; // Se chegou na posição inGuard, volta para a posição inicial
                    SetState(EnemyStates.INGUARD);
                }
                break;
            case EnemyStates.INGUARD: 
                MoveTo(targetPosition); // Só fica se movendo de um lado para o outro
                break;
            case EnemyStates.ALERT:
                if (Mathf.Abs(playerPosition.y - transform.position.y) <= 0.2f)  // Caso o player esteja na mesma plataforma que o inimigo
                    SetState (EnemyStates.AGRESSIVE); // Muda o estado para agressivo
                else if (playerDistance >= 0.9f)
                    SetState(EnemyStates.IDLE); // Volta ao estado Idle
                break;
            case EnemyStates.AGRESSIVE:
                if (Mathf.Abs(playerPosition.y - transform.position.y) > 0.2f)  
                    SetState(EnemyStates.ALERT); // Se o player não estiver na mesma plataforma que o inimigo, volta ao estado Idle
                else if (playerDistance < 0.5f && attackDelay > 1.5f)
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
            Debug.Log(difference);
        }

        if (Mathf.Abs(destiny.x - transform.position.x) <= 0.05f)
            SetState(EnemyStates.IDLE);
    }

    IEnumerator Attack () {
        if (attackDelay > 1.5f) {
            attackDelay = 0f;
            yield return new WaitForSecondsRealtime(0.85f);
            SetState(EnemyStates.AGRESSIVE);
        }
    }

	void OnTriggerEnter2D (Collider2D collider2D) {

		if (collider2D.gameObject.CompareTag("Power"))  {
			GameManager.instance.UpdateScore(20);
			Destroy(this.gameObject, 0.1f);
		}

	}

}
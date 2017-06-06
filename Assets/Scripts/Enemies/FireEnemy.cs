using UnityEngine;

public class FireEnemy : MonoBehaviour {
    public Animator animator;
    public AudioClip deathClip;
	public float velocity, playerDistance;
	public GameObject bombPrefab;
    public EnemyStates actualState;

    // VELOCIDADES
    public const float idleVelocity = 0.3f, alertVelocity = 0.4f;

    public float idleCount, alertCount, attackDelay;

    // MARCADORES DE POSIÇÕES
    private Vector3 playerPosition,
                    initialPosition,
                    inGuardPosition,
                    targetPosition;

    public void Start () {
        initialPosition = transform.position;
        animator = gameObject.GetComponent<Animator>();
        inGuardPosition = new Vector3(Random.Range(-1.5f, 1.5f), transform.position.y, 0);

        idleCount = 0f;

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

        if (!Physics2D.Raycast(transform.position, Vector2.down, 0.15f, 1 << LayerMask.NameToLayer("Ground"))) // GRAVIDADE PARA QUEDA (:
            transform.position += Vector3.down * 4f * Time.deltaTime;

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
                if (playerDistance < 0.6f) 
                    SetState (EnemyStates.AGRESSIVE); // Muda o estado para agressivo
                else alertCount += Time.deltaTime;
                if (alertCount > 1f) { // Se estiver alerta à mais de 1 segundo e o player tiver mantido distância
                    alertCount = 0f;
                    SetState(EnemyStates.IDLE); // Volta ao estado Idle
                }
                break;
            case EnemyStates.AGRESSIVE:
                if (playerDistance > 1.2f)  
                    SetState(EnemyStates.IDLE); // Se a distância aumentar em mais que 1.2 unidades, volta ao estado IDLE
                else if (playerDistance < 1f && attackDelay > 1.2f)
                    SetState(EnemyStates.ATTACK);

                MoveTo(playerPosition);
                break;
            case EnemyStates.ATTACK:
                attack();
                SetState(EnemyStates.AGRESSIVE);
                break;
        }
    }

    public void MoveTo (Vector3 destiny) {
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

        
        if (Mathf.Abs(destiny.x - transform.position.x) <= 0.05f)
            SetState(EnemyStates.IDLE);
    }

    public void attack () {
        Instantiate(bombPrefab, transform.position, Quaternion.identity);
        attackDelay = 0f;
    }


	void OnTriggerEnter2D (Collider2D collider2D) {

		if (collider2D.gameObject.CompareTag("Power"))  {
            SoundManager.instance.PlaySingle(deathClip);
			GameManager.instance.UpdateScore(25);
			Destroy(this.gameObject, 0.1f);
		}

	}

}

using UnityEngine;

public class Enemy : MonoBehaviour 
{
    [SerializeField]
    private Transform exitPoint;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private float navigationUpdate;
    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private int rewardAmount;
    private int target;
    private Transform enemy;
    private Collider2D enemyCollider;
    private Animator anim;
    private float navigationTime;
    private bool isDead;
    public GameManager gameManager;
    public SoundManager soundManager;

    public bool IsDead => isDead;

    void Start () {
        enemy = GetComponent<Transform>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        wayPoints = gameManager.wayPoints;
        exitPoint = gameManager.exit;
    }
    
	void Update ()
    {
        if (wayPoints == null || isDead) return;
        navigationTime += Time.deltaTime;
        if (navigationTime <= navigationUpdate) return;
        enemy.position = Vector2.MoveTowards(
            enemy.position, target < wayPoints.Length 
                ? wayPoints[target].position 
                : exitPoint.position, navigationTime);
        navigationTime = 0;
    }
    
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("checkpoint"))
        {
            target += 1;
            if (target != wayPoints.Length)
            {
                if (wayPoints[target].position.x > enemy.position.x)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
            }
        }
        else if (collider2D.CompareTag("Finish"))
        {
            gameManager.escapedEnemies += 1;
            gameManager.currentCastleHealth -= 1;
            gameManager.UnregisterEnemy(this);
            gameManager.IsWaveOver();
        }
        else if(collider2D.CompareTag("Bullet"))
        {
            Bullet newP = collider2D.gameObject.GetComponent<Bullet>();
            EnemyHit(newP.AttackStrength);
            Destroy(collider2D.gameObject);
        }
    }

    private void EnemyHit(int hitPoints)
    {
        if (healthPoints - hitPoints > 0)
        {
            healthPoints -= hitPoints;
            anim.Play("TakeDamage");
            gameManager.AudioSource.PlayOneShot(soundManager.HitClip);
        }
        else
        {
            anim.Play("Death");
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        gameManager.killedEnemies += 1;
        gameManager.AudioSource.PlayOneShot(soundManager.DeathClip);
        gameManager.AddMoney(rewardAmount);
        gameManager.IsWaveOver();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float timeBetweenAttacks;
    [SerializeField]
    private float attackRange;
    [FormerlySerializedAs("projectilePrefab")] [SerializeField]
    private Bullet bulletPrefab;
    private Enemy targetEnemy;
    private float timer;
    private bool canShoot;
    public bool isDestroyed;
    public int towerLevel = 1;
    public int upgradePrice;
    public string towerType;
    private List<Vector3> shootPositions;

    public GameManager gameManager;
    public SoundManager soundManager;
    
	void Start ()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        shootPositions = new List<Vector3>()
        {
            new Vector3(transform.localPosition.x - 0.25f, transform.localPosition.y, transform.localPosition.z),
            new Vector3(transform.localPosition.x + 0.25f, transform.localPosition.y, transform.localPosition.z)
        };
    }
    
	void Update () {
        if (!isDestroyed)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                canShoot = true;
                timer = timeBetweenAttacks;
            }
            else
                canShoot = false;

            if (canShoot)
                Attack();
        }
    }

    private void Attack()
    {
        for (var i = 0; i < towerLevel; i++)
        {
            var target = GetClosestEnemyInRange();
            if (target != null)
            {
                canShoot = false;
                var bullet = Instantiate(bulletPrefab);
                
                Vector3 shootPosition = new Vector3();
                if (towerLevel == 1)
                {
                    shootPosition = transform.localPosition;
                    bullet.transform.localPosition = shootPosition;
                }

                if (towerLevel == 2)
                {
                    shootPosition = shootPositions[i];
                    bullet.transform.localPosition = shootPosition;
                }

                bullet.target = target;

                if (bullet.BulletType == BulletType.Arrow)
                    gameManager.AudioSource.PlayOneShot(soundManager.ArrowClip);
                else if (bullet.BulletType == BulletType.Fireball)
                    gameManager.AudioSource.PlayOneShot(soundManager.FireballClip);
                else if (bullet.BulletType == BulletType.Rock)
                    gameManager.AudioSource.PlayOneShot(soundManager.RockClip);

                if (bullet.target == null || bullet.target.IsDead)
                    Destroy(bullet.gameObject);
                else
                    StartCoroutine(MoveBullet(bullet, shootPosition));
            }
        }
    }

    IEnumerator MoveBullet (Bullet bullet, Vector3 shootPosition)
    {
        while (bullet != null && !bullet.target.IsDead)
        {
            var direction = bullet.target.transform.localPosition - shootPosition;
            var angleDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            
            bullet.transform.position += (bullet.target.transform.position - shootPosition).normalized * (5 * Time.deltaTime);
            var bulletLocalPosition = bullet.target.transform.localPosition;
            bullet.lastPosition = new Vector2(bulletLocalPosition.x, bulletLocalPosition.y);
            
            yield return null;
        }
        
        if (bullet != null && bullet.target.IsDead)
        {
            bullet.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(MoveForward(bullet, shootPosition));
            StartCoroutine(DestroyProjectile(bullet));
        }
        
    }

    IEnumerator MoveForward (Bullet bullet, Vector3 shootPosition)
    {
        while (bullet != null)
        {
            var direction = new Vector3(bullet.lastPosition.x, bullet.lastPosition.y, 0) -
                            transform.localPosition;
            var angleDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            
            bullet.transform.position += (new Vector3(bullet.lastPosition.x, bullet.lastPosition.y, 0) - shootPosition).normalized * (5 * Time.deltaTime);
            
            yield return null;
        }
    }
    
    IEnumerator DestroyProjectile(Bullet bullet)
    {
        yield return new WaitForSeconds(2);
        Destroy(bullet.gameObject);
    }

    private List<Enemy> GetEnemiesInRange()
    {
        var enemiesInRange = new List<Enemy>();
        foreach(var enemy in gameManager.enemyList)
        {
            if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRange && !enemy.IsDead)
                enemiesInRange.Add(enemy);
        }
        return enemiesInRange;
    }

    private Enemy GetClosestEnemyInRange()
    {
        Enemy closestEnemy = null;
        var smallestDistance = float.PositiveInfinity;

        foreach (var enemy in GetEnemiesInRange())
        {
            if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }
}

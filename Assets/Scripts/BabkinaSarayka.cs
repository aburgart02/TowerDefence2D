using System.Collections;
using UnityEngine;

public class BabkinaSarayka : MonoBehaviour
{
    public bool isActive;
    private float timer;
    private bool canShoot;
    public float timeBetweenAttacks;
    public Bullet[] bulletPrefabs;

    public Vector3[] directions = new[]
    {
        new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0.5f, 0),
        new Vector3(1, 0, 0), new Vector3(1, -0.5f, 0), new Vector3(1, -1, 0), new Vector3(0.5f, -1, 0),
        new Vector3(0, -1, 0), new Vector3(-0.5f, -1, 0), new Vector3(-1, -1, 0), new Vector3(-1, -0.5f, 0),
        new Vector3(-1, 0, 0), new Vector3(-1, 0.5f, 0), new Vector3(-1, 1, 0), new Vector3(-0.5f, 1, 0),
    };
    
    void Update () {
        if (isActive)
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
        foreach (var direction in directions)
        {
            canShoot = false;
            var bullet = Instantiate(bulletPrefabs[Random.Range(0, bulletPrefabs.Length)]);
            
            Vector3 shootPosition = new Vector3();

            bullet.transform.localPosition = transform.position;
            
            bullet.targetPosition = 100 * direction;
            
            StartCoroutine(MoveBullet(bullet, shootPosition));
        }
    }
    
    IEnumerator MoveBullet (Bullet bullet, Vector3 shootPosition)
    {
        while (bullet != null)
        {
            var direction = bullet.targetPosition - shootPosition;
            var angleDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            
            bullet.transform.position += (bullet.targetPosition - shootPosition).normalized * (5 * Time.deltaTime);
            var bulletLocalPosition = bullet.targetPosition;
            bullet.lastPosition = new Vector2(bulletLocalPosition.x, bulletLocalPosition.y);
            
            yield return null;
        }
        
        if (bullet != null)
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
}

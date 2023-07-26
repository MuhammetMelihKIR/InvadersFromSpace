using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;

    Camera cam;
    public float width;

    bool isShooting;
    [SerializeField] private ObjectPool objectPool = null;

    public ShipStats shipStats;
    private Vector2 offScreenPos = new Vector2(0, -20);
    private Vector2 startPos = new Vector2(0, -6);
    private float dirx;

    public static bool camOn = true;
    [SerializeField] float _speed;
    [SerializeField] Vector3 _moveToStart;
    [SerializeField] Vector3 _nextToStart = new Vector3(0, -13, 0);


    private void Awake()
    {
        cam = Camera.main;
        width = ((1 /(cam.WorldToViewportPoint(new Vector3(1,1,0)).x - .5f) / 2) - 0.25f);
    }

    void Start()
    {
        shipStats.currentHealth = shipStats.maxHealth;
        shipStats.currentLifes = shipStats.maxLifes;
        //transform.position = startPos;
        UIManager.UpdatehealtBar(shipStats.currentHealth);
        UIManager.UpdateLives(shipStats.currentLifes);
    }


    void Update()
    {

   

        if (camOn==true)
        {
            MoveStartPoint();
        }

#if UNITY_EDITOR

        if (Input.GetKey(KeyCode.A) && transform.position.x > -width)
        {
            transform.Translate(Vector2.left * Time.deltaTime * shipStats.shipSpeed);
        }
        if (Input.GetKey(KeyCode.D) && transform.position.x < width)
        {
            transform.Translate(Vector2.right * Time.deltaTime * shipStats.shipSpeed);
        }
        if (Input.GetKey(KeyCode.Space) && !isShooting)
        {
            StartCoroutine(Shoot());
        }
#endif

        dirx = Input.acceleration.x;
        
        if (dirx <= -0.1f && transform.position.x > -width)
        {
            transform.Translate(Vector2.left * Time.deltaTime * shipStats.shipSpeed);
        }
        if (dirx >= 0.1f && transform.position.x < width)
        {
            transform.Translate(Vector2.right * Time.deltaTime * shipStats.shipSpeed);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("Player Hit!");
            collision.gameObject.SetActive(false);
            TakeDamage();
        }
       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("stop"))
        {
            camOn = false;
        }

        
    }
    public void ShootButton()
    {
        if (!isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    public void AddHealth()
    {
        if (shipStats.currentHealth == shipStats.maxHealth)
        {
            UIManager.UpdateScore(250);
        }
        else
        {
            shipStats.currentHealth++;
            UIManager.UpdatehealtBar(shipStats.currentHealth);
        }
    }

    public void AddLife()
    {
        if (shipStats.currentLifes == shipStats.maxLifes)
        {
            UIManager.UpdateScore(1000);
        }
        else
        {
            shipStats.currentLifes++;
            UIManager.UpdateLives(shipStats.currentLifes);
        }
    }

    public void TakeDamage()
    {
        shipStats.currentHealth--;
        UIManager.UpdatehealtBar(shipStats.currentHealth);

        if (shipStats.currentHealth <= 0)
        {
            shipStats.currentLifes--;
            UIManager.UpdateLives(shipStats.currentLifes);

            if (shipStats.currentLifes <= 0)
            {
                Debug.Log("Game Over");
            }
            else
            {
                
                StartCoroutine(Respawn());
            }
        }
    }

    private IEnumerator Shoot()
    {
        isShooting = true;
        GameObject obj = objectPool.GetPooledObject();
        obj.transform.position = gameObject.transform.position;
        yield return new WaitForSeconds(shipStats.fireRate);
        isShooting = false;
    }

    private IEnumerator Respawn()
    {
        transform.position = offScreenPos;

        yield return new WaitForSeconds(2);

        shipStats.currentHealth = shipStats.maxHealth;

        transform.position = startPos;
        UIManager.UpdatehealtBar(shipStats.currentHealth);
    }


    public void MoveStartPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, _moveToStart, _speed * Time.deltaTime);

    }

  

}

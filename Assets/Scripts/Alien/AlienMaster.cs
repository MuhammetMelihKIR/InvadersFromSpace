using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienMaster : MonoBehaviour
{
    Player _playerSC;
    private float width;

    [Header("POOLS")]
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private ObjectPool motherShipObjectPool;
    public GameObject bulletPrefab;
    

    [Header("Alien")]
    private Vector3 hMoveDistance = new Vector3(0.05f, 0, 0);
    private Vector3 vMoveDistance = new Vector3(0, 0.15f, 0);    
    private const float MAX_MOVE_SPEED = 0.02f;
    public static List<GameObject> allAliens = new List<GameObject>();
    private bool moveingRight;
    private float moveTimer = 0.01f;
    private float moveTime = 0.005f;
    private float shootTimer = 3f;
    private const float ShootTime = 3f;

    [Header("MOTHER SHIP")]
    public GameObject motherShipPrefab;
    private Vector3 motherShipSpawnPos = new Vector3(6,5.66f, 0);
    private float motherShipTimer = 60f;
    private const float MOTHERSHIP_MIN = 16f;
    private const float MOTHERSHIP_MAX = 60f;

    private const float START_Y = 1.7f;
    private bool entering = true;
    
    void Start()
    {
        _playerSC = GameObject.Find("PlayerShip").GetComponent<Player>();

        width = _playerSC.width - 0.15f;
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Alien"))
        {
            allAliens.Add(go);
        }
    }

    void Update()
    {
        if (entering)
        {
            transform.Translate(Vector2.down * Time.deltaTime * 10);

            if (transform.position.y <= START_Y)
            {
                entering = false;
            }
        }
        else
        {
            if (moveTimer <= 0)
            {
                MoveEnemies();
            }
            if (shootTimer <= 0)
            {
                Shoot();
            }
            if (motherShipTimer <= 0)
            {
                SpawnMotherShip();
            }
            moveTimer -= Time.deltaTime;
            shootTimer -= Time.deltaTime;
            motherShipTimer -= Time.deltaTime;
        }
    }

    private void MoveEnemies()
    {
        int hitMax = 0;
        for (int i = 0; i < allAliens.Count; i++)
        {
            if (moveingRight)
            {
                allAliens[i].transform.position += hMoveDistance;
            }
            else
            {
                allAliens[i].transform.position -= hMoveDistance;
            }
            if (allAliens[i].transform.position.x > width || allAliens[i].transform.position.x < -width)
            {
                hitMax++;
            }
        }
        if (hitMax > 0)
        {
            for (int i = 0; i < allAliens.Count; i++)
            {
                allAliens[i].transform.position -= vMoveDistance;
            }
            moveingRight = !moveingRight;
        }
        moveTimer = GetMovedSpeed();
    }

    private void SpawnMotherShip()
    {
        GameObject obj = motherShipObjectPool.GetPooledObject();
        obj.transform.position = motherShipSpawnPos;
        motherShipPrefab.SetActive(true);
        motherShipTimer = Random.Range(MOTHERSHIP_MIN,MOTHERSHIP_MAX);
    }

    private void Shoot()
    {
        Vector2 pos = allAliens[Random.Range(0, allAliens.Count)].transform.position;

        GameObject obj = objectPool.GetPooledObject();
        obj.transform.position = pos;

        shootTimer = ShootTime;
    }

    private float GetMovedSpeed()
    {
        float f = allAliens.Count * moveTime;

        if (f < MAX_MOVE_SPEED)
        {
            return MAX_MOVE_SPEED;
        }
        else
        {
            return f;
        }
    }
}

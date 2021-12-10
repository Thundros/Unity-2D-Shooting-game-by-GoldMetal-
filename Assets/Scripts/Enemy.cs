using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;

    public Sprite[] sprites;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Enemy shoot
        Fire();

        // Bullet reload;
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "A")
        {
            GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            // Direction of target(Player) = target position - enemy position
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 4, ForceMode2D.Impulse);
        }
        else if(enemyName == "B")
        {
            GameObject bulletLeft = Instantiate(bulletObjA, transform.position + Vector3.left * 0.3f, transform.rotation);
            GameObject bulletRight = Instantiate(bulletObjA, transform.position + Vector3.right * 0.3f, transform.rotation);

            Rigidbody2D rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidRight = bulletRight.GetComponent<Rigidbody2D>();

            Vector3 dirVecLeft = player.transform.position - (transform.position + Vector3.left * 0.3f);
            Vector3 dirVecRight = player.transform.position - (transform.position + Vector3.right * 0.3f);

            // Change unit vector : .normalized
            rigidLeft.AddForce(dirVecLeft.normalized * 3, ForceMode2D.Impulse);
            rigidRight.AddForce(dirVecRight.normalized * 3, ForceMode2D.Impulse);
        }
        else if(enemyName == "C")
        {
            GameObject bulletLeft = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);
            GameObject bulletRight = Instantiate(bulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation);

            Rigidbody2D rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidRight = bulletRight.GetComponent<Rigidbody2D>();

            Vector3 dirVecLeft = player.transform.position - (transform.position + Vector3.left * 0.3f);
            Vector3 dirVecRight = player.transform.position - (transform.position + Vector3.right * 0.3f);

            rigidLeft.AddForce(dirVecLeft.normalized * 2, ForceMode2D.Impulse);
            rigidRight.AddForce(dirVecRight.normalized * 2, ForceMode2D.Impulse);
        }

        // Reload
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if (health <= 0) return;

        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        
        // Delay to sprite change
        Invoke("ReturnSprite", 0.1f);

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            // Random Ratio Item Drop
            int ran = Random.Range(0, 10);
            if (ran < 3)
                Debug.Log("Not Item");
            else if (ran < 6) // Drop Coin
                Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
            else if (ran < 8) // Drop Power
                Instantiate(itemPower, transform.position, itemPower.transform.rotation);
            else // Drop Boom
                Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);

            Destroy(gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
            Destroy(gameObject);
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            Destroy(collision.gameObject);
        }
    }
}

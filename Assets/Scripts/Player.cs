using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;
    public int speed;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;

    public bool isHit;
    public bool isBoomTime;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;

    public GameManager gameManager;

    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchLeft;
    private bool isTouchRight;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Player movement
        Move();

        // Player shoot
        Fire();

        // Bullet reload;
        Reload();

        // Player Use Boom
        Boom();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Player Movement Restrictions
        // when player try move left or right or top or bottom
        // resetting move data, when player position is edge of the map
        if ((h == 1 && isTouchRight) || (h == -1 && isTouchLeft)) h = 0;
        if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom)) v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        // Player Movement Animation
        anim.SetInteger("Input", (int)h);
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1") || curShotDelay < maxShotDelay)
            return;

        // Create bullet object
        switch(power)
        {
            case 1:
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigidbody2D = bullet.GetComponent<Rigidbody2D>();
                rigidbody2D.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletLeft = Instantiate(bulletObjA, transform.position + Vector3.left * 0.2f, transform.rotation);
                GameObject bulletRight = Instantiate(bulletObjA, transform.position + Vector3.right * 0.2f, transform.rotation);

                Rigidbody2D rigidbody2D_left = bulletLeft.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidbody2D_right = bulletRight.GetComponent<Rigidbody2D>();

                rigidbody2D_left.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidbody2D_right.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletCenter = Instantiate(bulletObjB, transform.position, transform.rotation);
                Rigidbody2D rigidbody2D_center = bulletCenter.GetComponent<Rigidbody2D>();
                rigidbody2D_center.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        // Reload
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2") || isBoomTime || boom == 0) return;

        isBoomTime = true;
        gameManager.UpdateBoomIcon(--boom);

        // Boom Effect visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 2.0f);

        // Destroy Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; ++i)
        {
            Enemy enemyLogic = enemies[i].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }

        // Remove Enemy Bullet
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < bullets.Length; ++i)
            Destroy(bullets[i]);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit) return;

            isHit = true;
            gameManager.UpdateLifeIcon(--life);

            if (life == 0)
                gameManager.GameOver();
            else
                gameManager.Invoke("RespawnPlayer", 2.0f);

            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower) score += 500;
                    else ++power;
                    break;
                case "Boom":
                    if (boom == maxBoom) score += 500;
                    else
                        gameManager.UpdateBoomIcon(++boom);
                    break;
            }

            Destroy(collision.gameObject);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}

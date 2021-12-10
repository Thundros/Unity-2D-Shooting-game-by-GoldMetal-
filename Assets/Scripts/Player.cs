using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;
    public int speed;
    public int power;

    public bool isHit;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;

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

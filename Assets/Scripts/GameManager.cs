using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }

        // UI score update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);

        GameObject enemy = Instantiate(enemyObjs[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;

        // Change enemy movement according to spawn point
        if (ranPoint == 5 || ranPoint == 6) // Left spawn
        {
            enemy.transform.Rotate(Vector3.forward * 45);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else if(ranPoint == 7 || ranPoint == 8) // Right spawn
        {
            enemy.transform.Rotate(Vector3.back * 45);
            rigid.velocity = new Vector2(enemyLogic.speed * -1, -1);
        }
        else // Center spawn
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1);
    }

    public void UpdateLifeIcon(int life)
    {
        for (int i = 0; i < 3; ++i)
            lifeImage[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < life; ++i)
            lifeImage[i].color = new Color(1, 1, 1, 1);
    }

    public void UpdateBoomIcon(int boom)
    {
        for (int i = 0; i < 3; ++i)
            boomImage[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < boom; ++i)
            boomImage[i].color = new Color(1, 1, 1, 1);
    }

    public void RespawnPlayer()
    {
        // Respawn and Resetting player position
        player.transform.position = Vector3.down * 4.0f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}

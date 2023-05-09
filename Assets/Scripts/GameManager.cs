using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus
{
    Start, Next, GameOver, Win
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text moneyLabel;
    [SerializeField]
    private Text currentWaveLabel;
    [SerializeField]
    private Text enemiesCountLabel;
    [SerializeField]
    private Text castleHealthLabel;
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    public Enemy[] enemies;
    [SerializeField]
    private int enemiesPerSpawn;
    [SerializeField]
    private Text playButtonLabel;
    [SerializeField]
    private Button playButton;

    public int startMoney;
    public int startCastleHealth;
    public List<int> enemiesWaves = new();
    
    private int waveNumber;
    public int currentMoney;
    public int currentCastleHealth;
    public int killedEnemies;
    public int escapedEnemies;
    public int enemiesCount;
    
    private GameStatus currentState = GameStatus.Start;
    private AudioSource audioSource;

    public List<Enemy> enemyList = new();
    const float SpawnDelay = 2f;
    
    public TowerManager towerManager;
    public SoundManager soundManager;

    public AudioSource AudioSource => audioSource;

    void Start () {
        playButton.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        towerManager = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        ShowMenu();
	}
    
	void Update () {
        HandleEscape();
	}
    
    IEnumerator Spawn()
    {
        if (enemiesPerSpawn > 0 && enemyList.Count < enemiesCount)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (enemyList.Count < enemiesCount)
                {
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemies.Length)]);
                    newEnemy.transform.position = spawnPoint.transform.position;
                    enemyList.Add(newEnemy);
                }
            }
            yield return new WaitForSeconds(SpawnDelay);
            StartCoroutine(Spawn());
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemy.GetComponent<SpriteRenderer>().enabled = false;
        enemyList.Remove(enemy);
    }

    public void DestroyAllEnemies()
    {
        foreach(Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
    }

    public void DestroyAllBullets()
    {
        foreach (var bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(bullet.gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        moneyLabel.text = currentMoney.ToString();
    }

    public void SubtractMoney(int amount)
    {
        currentMoney -= amount;
        moneyLabel.text = currentMoney.ToString();
    }

    public void IsWaveOver()
    {
        castleHealthLabel.text = "Castle health: " + currentCastleHealth;
        enemiesCountLabel.text = "Enemies killed: " + $"{killedEnemies}/{enemiesCount}";
        SetCurrentGameState();
    }

    public void SetCurrentGameState()
    {
        if (currentCastleHealth <= 0)
        {
            Time.timeScale = 0;
            currentState = GameStatus.GameOver;
            ShowMenu();
        }
        else if (waveNumber == enemiesWaves.Count - 1 && killedEnemies + escapedEnemies == enemiesCount)
        {
            Time.timeScale = 0;
            currentState = GameStatus.Win;
            ShowMenu();
        }
        else if (killedEnemies + escapedEnemies == enemiesCount)
        {
            Time.timeScale = 0;
            currentState = GameStatus.Next;
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        switch (currentState)
        {
            case GameStatus.Start:
                playButtonLabel.text = "Start game";
                break;
            case GameStatus.Win:
                playButtonLabel.text = "You won. Play again";
                break;
            case GameStatus.GameOver:
                playButtonLabel.text = "Game Over. Play Again";
                AudioSource.PlayOneShot(soundManager.GameOverClip);
                break;
            case GameStatus.Next:
                playButtonLabel.text = "Start next wave";
                break;
        }
        playButton.gameObject.SetActive(true);
    }
    
    public void PlayButtonPressed()
    {
        if (currentState == GameStatus.Next)
        {
            waveNumber += 1;
        }
        else
        {
            waveNumber = 0;
            currentMoney = startMoney;
            currentCastleHealth = startCastleHealth;
            towerManager.RenameTagsBuildSites();
            AudioSource.PlayOneShot(soundManager.NewGameClip);
            if (currentState != GameStatus.Start)
                towerManager.DestroyAllTower();
        }

        Time.timeScale = 1;
        DestroyAllEnemies();
        DestroyAllBullets();
        killedEnemies = 0;
        escapedEnemies = 0;
        enemiesCount = enemiesWaves[waveNumber];
        moneyLabel.text = currentMoney.ToString();
        currentWaveLabel.text = "Wave " + (waveNumber + 1);
        castleHealthLabel.text = "Castle health: " + currentCastleHealth;
        enemiesCountLabel.text = "Enemies killed: " + $"{killedEnemies}/{enemiesCount}";
        StartCoroutine(Spawn());
        playButton.gameObject.SetActive(false);
    }
    
    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            towerManager.DisableDragSprite();
            towerManager.TowerButtonPressed = null;
        }
    }
}
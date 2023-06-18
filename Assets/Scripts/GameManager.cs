using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameStatus
{
    Start, Next, GameOver, Win
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int levelNumber;
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
    public Transform[] wayPoints;
    [SerializeField]
    public Transform exit;
    [SerializeField]
    private Text playButtonLabel;
    [SerializeField]
    private Button playButton;

    public int startMoney;
    public int startCastleHealth;
    
    [SerializeField]
    public List<Wave> waves;
    
    private List<Wave> enemiesWaves = new List<Wave>();
    
    private int waveNumber;
    public int currentMoney;
    public int currentCastleHealth;
    public int killedEnemies;
    public int escapedEnemies;
    public int enemiesCount;

    private GameStatus currentState = GameStatus.Start;
    private AudioSource audioSource;

    public List<Enemy> enemyList = new();
    public List<float> spawnDelays = new ();
    public float spawnDelay;
    
    public TowerManager towerManager;
    public SoundManager soundManager;

    public AudioSource AudioSource => audioSource;

    void Start () {
        playButton.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.2f;
        towerManager = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        foreach (var wave in waves)
            enemiesWaves.Add(new Wave(wave.Light, wave.Medium, wave.Heavy));
        ShowMenu();
	}
    
	void Update () {
        HandleEscape();
	}
    
    IEnumerator Spawn()
    {
        if (enemiesPerSpawn > 0 && enemiesWaves[waveNumber].TotalCount > 0)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (enemiesWaves[waveNumber].TotalCount > 0)
                {
                    var available = new List<int>();
                    
                    if (enemiesWaves[waveNumber].Light > 0)
                        available.Add(0);
                    if (enemiesWaves[waveNumber].Medium > 0)
                        available.Add(1);
                    if (enemiesWaves[waveNumber].Heavy > 0)
                        available.Add(2);
                    
                    var rand = Random.Range(0, available.Count);
                    var enemyType = available[rand];
                    var newEnemy = Instantiate(enemies[enemyType]);
                    if (enemyType == 0)
                        enemiesWaves[waveNumber].Light -= 1;
                    if (enemyType == 1)
                        enemiesWaves[waveNumber].Medium -= 1;
                    if (enemyType == 2)
                        enemiesWaves[waveNumber].Heavy -= 1;
                    newEnemy.transform.position = spawnPoint.transform.position;
                    enemyList.Add(newEnemy);
                }
            }
            yield return new WaitForSeconds(spawnDelay);
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
                playButtonLabel.text = "You won";
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
        if (currentState == GameStatus.Win)
        {
            foreach (var o in FindObjectsOfType(typeof(GameObject)))
            {
                if (o.name == "Main Camera")
                    continue;
                Destroy(o);
            }
            if (levelNumber == 1)
                SceneManager.LoadScene("Level2");
            if (levelNumber == 2)
                SceneManager.LoadScene("Level1");
        }
        if (currentState == GameStatus.Next)
        {
            waveNumber += 1;
        }
        else
        {
            waveNumber = 0;
            currentMoney = startMoney;
            currentCastleHealth = startCastleHealth;
            AudioSource.PlayOneShot(soundManager.NewGameClip);
            if (currentState != GameStatus.Start)
                towerManager.DestroyAllTower();
            enemiesWaves = new List<Wave>();
            foreach (var wave in waves)
                enemiesWaves.Add(new Wave(wave.Light, wave.Medium, wave.Heavy));
        }

        Time.timeScale = 1;
        DestroyAllEnemies();
        DestroyAllBullets();
        killedEnemies = 0;
        escapedEnemies = 0;
        spawnDelay = spawnDelays[waveNumber];
        enemiesCount = enemiesWaves[waveNumber].TotalCount;
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
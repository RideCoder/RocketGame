using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public Player player;
    public TMP_Text timeText;
    public TMP_Text attemptsText;
    public TMP_Text deathText;
    public TMP_Text winText;
    public int attempts;

    public float elapsedTime;
    private bool timerRunning = true;
    public static bool playerDead = false;
    public AudioSource music;
   
    public LoadLevel level;
    public SpawnPointObject spawnPoint;
    public GameObject levelParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    void Start()
    {
        //player.rocket.transform.position = level.GetSpawnPosition();
        playerDead = false;
        music.time = 0f;
        music.Play();
        elapsedTime = 0f;
        attempts = 0;
        if (levelParent != null)
        {
            foreach (Transform t in levelParent.transform)
            {
                if (t.gameObject.TryGetComponent<SpawnPointObject>(out SpawnPointObject spawn))
                {
                    spawnPoint = spawn;
                }
            }
        }
    }

    public void SetSpawnPoint()
    {
        if (levelParent != null)
        {
            foreach (Transform t in levelParent.transform)
            {
                if (t.gameObject.TryGetComponent<SpawnPointObject>(out SpawnPointObject spawn))
                {
                    spawnPoint = spawn;
                }
            }
        }
    }
    void OnEnable()
    {
        Player.OnPlayerDeath += PlayerDeath;
        Player.OnPlayerTouchGoal += PlayerWin;
    }

    void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
        Player.OnPlayerTouchGoal -= PlayerWin;
    }

    public void SpawnPlayer()
    {
        Rigidbody rb = player.rocket.GetComponent<Rigidbody>();

        // Temporarily disable physics
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 spawnPos;

        if (PracticeMode.practiceCheckpoints.Count <= 0)
        {
            if (level == null)
            {
                if (levelParent != null && spawnPoint != null)
                    spawnPos = spawnPoint.transform.position;
                else
                    spawnPos = new Vector3(2.4f, 4.18f, -6.77f);
            }
            else
            {
                spawnPos = level.GetSpawnPosition();
            }
        }
        else
        {
            PracticeMode.practiceCheckpoints.TryGetValue(
                PracticeMode.practiceCheckpoints.Count,
                out GameObject practiceCheckpoint
            );

            spawnPos = practiceCheckpoint.transform.position;
            rb.velocity = practiceCheckpoint.GetComponent<Checkpoint>().velocity;
        }

        player.rocket.transform.position = spawnPos;

        // Re-enable physics on next physics step
        rb.isKinematic = false;
    }

    void Update()
    {
        if (!UIManager.IsPaused)
        {
            if (timerRunning)
            {
                elapsedTime += Time.deltaTime;
                timeText.text = FormatTime(elapsedTime);
            }




            if (Mouse.current.leftButton.wasPressedThisFrame && playerDead)
            {
                Debug.Log(playerDead);
                playerDead = false;
                player.rocket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                SpawnPlayer();

                
                deathText.enabled = false;
                elapsedTime = 0f;     // Reset timer on death
                attempts += 1;
                Debug.Log("DIED");
               
                attemptsText.text = "Attempts: " + attempts.ToString();
                timerRunning = true;
               
                music.Play();
                music.time = 0f;
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int hundredths = Mathf.FloorToInt((time * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}.{hundredths:00}";
    }

    public void PlayerDeath()
    {


        // OPTIONAL behaviors (choose one)
        Debug.Log("DIEDAGAIN");
        deathText.enabled = true;
        playerDead = true;
        timerRunning = false;
        music.Stop();
        // timerRunning = false; // Stop timer on death
    }

    public void PlayerWin()
    {
        timerRunning = false;
        winText.enabled = true;
        player.rocket.GetComponent<Rigidbody>().isKinematic = true;

        winText.text = "<Time: "+ FormatTime(elapsedTime)+">";
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public Player player;
    public TMP_Text timeText;
    public TMP_Text attemptsText;
    public TMP_Text deathText;
    public TMP_Text winText;
    public AudioSource music;

    public LoadLevel level;
    public SpawnPointObject spawnPoint;
    public GameObject levelParent;

    [Header("State")]
    public int attempts;
    public float elapsedTime;

    private bool timerRunning = true;
    private bool pendingRespawn = false;

    public static bool playerDead = false;

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

    private void OnEnable()
    {
        Player.OnPlayerDeath += PlayerDeath;
        Player.OnPlayerTouchGoal += PlayerWin;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
        Player.OnPlayerTouchGoal -= PlayerWin;
    }

    private void Start()
    {
        playerDead = false;
        timerRunning = true;
        pendingRespawn = false;

        elapsedTime = 0f;
        attempts = 0;

        if (music != null)
        {
            music.time = 0f;
            music.Play();
        }

        ResolveSpawnPoint();

        if (attemptsText != null)
            attemptsText.text = "Attempts: 0";

        if (deathText != null)
            deathText.enabled = false;

        if (winText != null)
            winText.enabled = false;
    }

    private void Update()
    {
        if (UIManager.IsPaused)
            return;

        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            if (timeText != null)
                timeText.text = FormatTime(elapsedTime);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && playerDead)
        {
            pendingRespawn = true;
        }

        if (pendingRespawn)
        {
            pendingRespawn = false;
            Respawn();
        }
    }

    private void Respawn()
    {
        playerDead = false;

        Rigidbody rb = player.rocket.GetComponent<Rigidbody>();

        // Fully reset physics state
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        SpawnPlayer();

        Physics.SyncTransforms();

        rb.isKinematic = false;

        if (deathText != null)
            deathText.enabled = false;

        elapsedTime = 0f;
        attempts += 1;

        if (attemptsText != null)
            attemptsText.text = "Attempts: " + attempts;

        timerRunning = true;

        if (music != null)
        {
            music.time = 0f;
            music.Play();
        }
    }

    public void SpawnPlayer()
    {
        Rigidbody rb = player.rocket.GetComponent<Rigidbody>();

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
    }

    public void SetSpawnPoint()
    {
        ResolveSpawnPoint();
    }

    private void ResolveSpawnPoint()
    {
        if (levelParent == null)
            return;

        foreach (Transform t in levelParent.transform)
        {
            if (t.gameObject.TryGetComponent<SpawnPointObject>(out SpawnPointObject spawn))
            {
                spawnPoint = spawn;
                break;
            }
        }
    }

    public void PlayerDeath()
    {
        if (playerDead)
            return;

        playerDead = true;
        timerRunning = false;

        if (deathText != null)
            deathText.enabled = true;

        if (music != null)
            music.Stop();
    }

    public void PlayerWin()
    {
        timerRunning = false;

        if (winText != null)
        {
            winText.enabled = true;
            winText.text = "<Time: " + FormatTime(elapsedTime) + ">";
        }

        Rigidbody rb = player.rocket.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int hundredths = Mathf.FloorToInt((time * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}.{hundredths:00}";
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Player player;
    public TMP_Text timeText;
    public TMP_Text attemptsText;
    public TMP_Text deathText;
    public TMP_Text winText;
    public int attempts;

    private float elapsedTime;
    private bool timerRunning = true;
    public static bool playerDead = false;
    public AudioSource music;
   
    public LoadLevel level;
    void Start()
    {
        //player.rocket.transform.position = level.GetSpawnPosition();
        music.time = 60f;
        music.Play();
        elapsedTime = 0f;
        attempts = 0;
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
               
                playerDead = false;
                player.rocket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                if (PracticeMode.practiceCheckpoints.Count <= 0)
                {
                 if (level == null)
                    {
                        player.rocket.transform.position = new Vector3(2.4f, 4.18f, -6.77f);
                    }
                    else
                    {
                        player.rocket.transform.position = level.GetSpawnPosition();
                    }
                        
                }
                else
                {
                    PracticeMode.practiceCheckpoints.TryGetValue(PracticeMode.practiceCheckpoints.Count, out GameObject practiceCheckpoint);
                    player.rocket.transform.position = practiceCheckpoint.transform.position;
                   
                    player.rocket.GetComponent<Rigidbody>().linearVelocity = practiceCheckpoint.GetComponent<Checkpoint>().velocity;
                }

                
                deathText.enabled = false;
                elapsedTime = 0f;     // Reset timer on death
                attempts += 1;
                attemptsText.text = "Attempts: " + attempts.ToString();
                timerRunning = true;
                music.time = 60f;
                music.Play();
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

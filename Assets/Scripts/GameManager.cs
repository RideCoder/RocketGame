using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Player player;
    public TMP_Text timeText;
    public TMP_Text attemptsText;
    public TMP_Text deathText;
    public int attempts;

    private float elapsedTime;
    private bool timerRunning = true;
    public static bool playerDead = false;
    void Start()
    {
        Player.OnPlayerDeath += PlayerDeath;
        Player.OnPlayerTouchGoal += PlayerWin;
        elapsedTime = 0f;
        attempts = 0;
    }

    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            timeText.text = FormatTime(elapsedTime);
        }
           

       

        if (Mouse.current.leftButton.wasPressedThisFrame && playerDead)
        {
            Debug.Log("TEST");
            playerDead = false;
             player.rocket.transform.position = new Vector3(0.5f, 4.35f, -6.5f);
         player.rocket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            deathText.enabled = false;
            elapsedTime = 0f;     // Reset timer on death
            attempts += 1;
            attemptsText.text = "Attempts: " + attempts.ToString();
            timerRunning = true;
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
        // timerRunning = false; // Stop timer on death
    }

    public void PlayerWin()
    {

    }
}

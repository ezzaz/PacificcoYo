using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    private float points;
    private float highScore;

    private const string SCORE_KEY = "PlayerScore";
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string LIVES_KEY = "CurrentLives";

    private int maxLives = 3;
    public int currentLives;
    private TextMeshProUGUI vidasText;
    private TextMeshProUGUI scoreText;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
            UpdateLivesDisplay();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindScoreTextInScene();
        currentLives = PlayerPrefs.GetInt(LIVES_KEY, maxLives);
        UpdateLivesDisplay();
    }
    void OnEnable()
    {

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindScoreTextInScene();
        UpdateLivesDisplay();
        ResetScore();
        if (scene.name == "Menu")
        {
            
            ResetLives();
        }
    }

    public void LoseLife()
    {
        currentLives--;
        PlayerPrefs.SetInt(LIVES_KEY, currentLives);
        PlayerPrefs.Save();
        UpdateLivesDisplay();

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            RestartLevel();
        }
    }

    

   

    private void UpdateLivesDisplay()
    {
        if (vidasText != null)
        {
            vidasText.text = $"Lives: {currentLives}";
        }

    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameOver()
    {
        if (vidasText != null)
        {
            vidasText.text = "GAME OVER!";
        }

        PlayerPrefs.DeleteKey(LIVES_KEY);
        PlayerPrefs.Save();
        StartCoroutine(GoToGameOverScreen());
    }
    private void ResetLives()
    {
        currentLives = maxLives;

        PlayerPrefs.SetInt(LIVES_KEY, currentLives);
        PlayerPrefs.Save();

        UpdateLivesDisplay();
    }
    private System.Collections.IEnumerator GoToGameOverScreen()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
        SceneManager.LoadScene("Menu");
    }
    

   

    private void FindScoreTextInScene()
    {
       
        GameObject vidasUI = GameObject.Find("Vidas");
        if (vidasUI != null)
        {

            vidasText = vidasUI.GetComponent<TextMeshProUGUI>();

        }

        GameObject scoreUI = GameObject.Find("Score");
        if (scoreUI != null)
        {
            scoreText = scoreUI.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
       
        if (scoreText != null)
        {
            scoreText.text = "Score: " + points.ToString("0");
        }
    }

    public void AddScore(float points)
    {
        this.points += points;
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        if (points > highScore)
        {
            highScore = points;
        }
    }

    public float GetScore()
    {
        return points;
    }
    public int GetCurrentLives()
    {
        return currentLives;
    }

    public int GetMaxLives()
    {
        return maxLives;
    }
    public float GetHighScore()
    {
        return highScore;
    }

    public void SaveScores()
    {
        PlayerPrefs.SetFloat(SCORE_KEY, points);
        PlayerPrefs.SetFloat(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
    }

    public void LoadScores()
    {
        points = PlayerPrefs.GetFloat(SCORE_KEY, 0);
        highScore = PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0);
    }
    public void GainLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            PlayerPrefs.SetInt(LIVES_KEY, currentLives);
            PlayerPrefs.Save();
            UpdateLivesDisplay();
        }
    }
    public void ResetScore()
    {
        points = 0;
    }
}
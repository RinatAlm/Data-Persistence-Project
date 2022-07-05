using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text MaxScoreText;
    public GameObject GameOverText;
    public GameObject StartButton;
    public GameObject RestartButton;
    public GameObject ExitButton;
    public GameObject InputField;
    public Text InputText;
    public GameObject SaveButton;
    
    private bool m_Started = false;
    private int m_Points;
    public int maxPoints;
    public string maxName;
   

    private bool m_GameOver = false;

    [System.Serializable]
    class SaveData
    {
        public int savedPoints;
       public string maxName;
    }


   public void SaveScore()
    {
        maxName = InputText.text;
        SaveData data = new SaveData();
        data.savedPoints = m_Points;
        data.maxName = maxName;
     

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", json);
    }


    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/SaveData.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            maxPoints = data.savedPoints;
            maxName = data.maxName;
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        StartButton = GameObject.Find("StartButton");
        RestartButton = GameObject.Find("RestartButton");
        ExitButton = GameObject.Find("ExitButton");
        InputField = GameObject.Find("InputField");
        SaveButton = GameObject.Find("SaveButton");
        InputText = GameObject.Find("Input").GetComponent<Text>();
        MaxScoreText = GameObject.Find("ScoreText (1)").GetComponent<Text>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        RestartButton.SetActive(false);
        InputField.SetActive(false);
        SaveButton.SetActive(false);
        LoadScore();

        MaxScoreText.text = "Best Score: " + maxName + ":" + maxPoints;

        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        if(m_Points > maxPoints)
        {
            InputField.SetActive(true);
            SaveButton.SetActive(true);
        }
        GameOverText.SetActive(true);
        RestartButton.SetActive(true);
        
    }

    public void GameStart()
    {
        m_Started = true;
        StartButton.SetActive(false);
        ExitButton.SetActive(false);
        
    }

    public void GameRestart()
    {
        SceneManager.LoadScene("main");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
Application.Quit();
#endif
    }
}

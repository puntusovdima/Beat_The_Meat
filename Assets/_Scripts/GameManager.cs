using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [SerializeField] private float score;
    [SerializeField] private TMP_Text scoreText;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateScore(score);
    }

    public void AddScore(float score)
    {
        this.score += score;
        UpdateScore(this.score);
    }

    private void UpdateScore(float score)
    {
        if (scoreText != null)
            scoreText.text = $"ENEMIES SLAIN: {score:00}";
    }

    public int GetScore()
    {
        return((int)score);
    }
}

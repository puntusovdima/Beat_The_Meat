using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject darkFilter;
    // Start is called before the first frame update
    void Start()
    {
        darkFilter.SetActive(true);
        gameObject.SetActive(false);
        
    }

    public void ReloadGame()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}

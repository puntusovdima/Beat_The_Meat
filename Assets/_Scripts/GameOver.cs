using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Health>().currentHealth == 0)
        {
            gameObject.SetActive(true);
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateEnemies : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject restrictColliders;
    [SerializeField] private GameObject enemies;
    [SerializeField] private int continueScore;
    private bool isRestricting = false;

    private void Start()
    {
        restrictColliders.SetActive(false);
        enemies.SetActive(false);
    }

    private void Update()
    {
        if(GameManager.Instance.GetScore() == continueScore)
        {
            restrictColliders.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isRestricting)
        {
            HitByPlayer(collision.gameObject);
            isRestricting = true;
        }
    }

    public void HitByPlayer(GameObject player)
    {
        restrictColliders.SetActive(true);
        Debug.Log("RESTRICTING PLAYER MOVEMENT UNTIL ALL ENEMIES ARE SLAIN.");
        enemies.SetActive(true);
    }

    public void HitByEnemy(GameObject enemy)
    {
        //Do nothing
    }
}

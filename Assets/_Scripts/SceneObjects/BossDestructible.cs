using System;
using System.Collections;
using UnityEngine;

public class BossDestructible : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject runeGlow;
    [SerializeField] private GameObject pillarShadow;

    private Vector3 bossRoomPlayerPos = new Vector3(140, 0, 0);
    private Vector3 bossRoomCamPos = new Vector3(143, 0, -10);

    private int continueScore = 15;

    private void Start()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        runeGlow.GetComponent<SpriteRenderer>().enabled = false;
        pillarShadow.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        if(GameManager.Instance.GetScore() >= continueScore)
        {
            Debug.Log("I, THE BOSS DESTRUCTIBLE, HAVE BEEN ACTIVATED");
            gameObject.GetComponent<Collider2D>().enabled = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            runeGlow.GetComponent<SpriteRenderer>().enabled = true;
            pillarShadow.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void HitByPlayer(GameObject player)
    {
        StartCoroutine(TransportDelay());
    }

    public void HitByEnemy(GameObject enemy)
    {
        //Do nothing
    }

    private IEnumerator TransportDelay()
    {
        runeGlow.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        //Take the player to the boss room
        player.transform.position = bossRoomPlayerPos;
        cam.GetComponent<FollowPlayer>().SetCamTrackStatus(false);
        cam.transform.position = bossRoomCamPos;
        Destroy(gameObject, 3.0f);
        

    }
}

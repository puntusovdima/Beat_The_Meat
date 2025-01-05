using System;
using System.Collections;
using UnityEngine;

public class BossDestructible : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject runeGlow;
    [SerializeField] private GameObject pillarShadow;

    private Vector3 bossRoomPlayerPos = new Vector3(120, 0, 0);
    private Vector3 bossRoomCamPos = new Vector3(123, 0, -10);

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
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        runeGlow.SetActive(false);
        pillarShadow.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        //Take the player to the boss room or spawn the boss
        Debug.Log("HERE HERE HERE HERE HERE HERE HERE HERE HERE HERE HERE HERE HERE HERE HERE");
        player.transform.position = bossRoomPlayerPos;
        cam.transform.position = bossRoomCamPos;
        

    }
}

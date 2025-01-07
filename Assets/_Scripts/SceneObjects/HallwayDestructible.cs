using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayDestructible : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cam;

    private Vector3 hallwayRoomPlayerPos = new Vector3(100, 0, 0);
    private Vector3 hallwayRoomCamPos = new Vector3(103, 0, -10);

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
        yield return new WaitForSeconds(0.7f);
        //Take the player to the hallway room
        player.transform.position = hallwayRoomPlayerPos;
        cam.GetComponent<FollowPlayer>().SetCamTrackStatus(false);
        cam.transform.position = hallwayRoomCamPos;


    }
}

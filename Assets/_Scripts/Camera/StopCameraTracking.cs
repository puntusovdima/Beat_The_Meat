using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCameraTracking : MonoBehaviour, ITriggerEnter
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject cam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            HitByPlayer(collision.gameObject);
        }
    }

    public void HitByPlayer(GameObject player)
    {
        cam.GetComponent<FollowPlayer>().SetCamTrackStatus(false);
        Debug.Log("PLAYER IS INSIDE, PLAYER HAS MADE CONTACT WITH A NON-TRACKING ZONE. CAM TRACKING: " + cam.GetComponent<FollowPlayer>().GetCamTrackStatus());
        
    }

    public void HitByEnemy(GameObject enemy)
    {
        //Do nothing
    }
}

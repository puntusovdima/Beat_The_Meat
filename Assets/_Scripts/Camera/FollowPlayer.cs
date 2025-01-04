using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float dampTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;
    private bool trackPlayer = true;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target && trackPlayer)
        {
            Vector3 destination = new Vector3(target.position.x, transform.position.y, transform.position.z);
            //transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            transform.position = destination;
        }

    }

    public bool GetCamTrackStatus()
    {
        return trackPlayer;
    }

    public void SetCamTrackStatus (bool newStatus)
    {
        trackPlayer = newStatus;
    }
}

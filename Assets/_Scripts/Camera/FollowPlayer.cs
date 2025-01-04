using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float dampTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;
    private bool end = false;

    // Update is called once per frame
    void Update()
    {
        if (target && !end)
        {
            //Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
            //Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            //Vector3 destination = transform.position + delta;

            Vector3 destination = new Vector3(target.position.x, transform.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }

    }
}

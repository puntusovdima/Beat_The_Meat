using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : PickUp
{
    [SerializeField]
    private float speedAdded;
    public override void ApplyPickUp()
    {
        if (_player != null)
        {
            _player.GetComponent<PlayerBeatController>().Boost(speedAdded);
        }
        base.ApplyPickUp();
    }
}

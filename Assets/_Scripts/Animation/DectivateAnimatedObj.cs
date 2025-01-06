using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DectivateAnimatedObj : MonoBehaviour
{
    public GameObject[] objectsToDeactivate;

    public void DeactivateObjects()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }
    }
}


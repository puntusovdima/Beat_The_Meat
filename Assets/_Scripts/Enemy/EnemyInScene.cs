using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInScene : MonoBehaviour
{
    public GameObject m_enemyObject;
    public float m_timeToInitBehavour = 0;
    public bool m_isHiddenAtStart = false;

    private float m_currentTime = 0;
    private EnemyGroup m_myGroup;
    private bool m_isStarted = false;

    public void SetMyGroup(EnemyGroup group)
    {
        m_myGroup = group;

        if (m_isHiddenAtStart)
        {
            m_enemyObject.SetActive(false);
        }
        else
        {
            m_enemyObject.GetComponent<EnemyBeatController>().enabled = false;
        }
    }

    public void AddTime(float timeDelta)
    {
        m_currentTime += timeDelta;

        if (m_currentTime >= m_timeToInitBehavour && !m_isStarted)
        {
            if (!gameObject.CompareTag("Dancer"))
            {
                m_enemyObject.GetComponent<EnemyBeatController>().enabled = true;
                m_myGroup.InitEnemy(m_enemyObject.GetComponent<EnemyBeatController>());
            }
            else
            {
                m_enemyObject.GetComponent<Boss>().enabled = true;
                m_myGroup.InitBoss(m_enemyObject.GetComponent<Boss>());
            }
            m_isStarted = true;
        }
    }
    
}

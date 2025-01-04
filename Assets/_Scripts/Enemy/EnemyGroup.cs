using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour, ITriggerObject
{
    [SerializeField] private List<EnemyInScene> m_enemyList;
    
    bool m_enableGroup = false;
    private int m_currentEnemy = 0;

    public void InitEnemy(EnemyBeatController enemy)
    {
        enemy.gameObject.SetActive(true);
        m_currentEnemy++;
    }

    private void Start()
    {
        for (int i = 0; i < m_enemyList.Count; i++)
        {
            m_enemyList[i].SetMyGroup(this);
        }
    }

    private void Update()
    {
        if (m_enableGroup)
        {
            for (int i = 0; i < m_enemyList.Count; i++)
            {
                m_enemyList[i].AddTime(Time.deltaTime);
            }
        }
    }

    public void TriggerByPlayer(CharacterBeatController player)
    {
        m_enableGroup = true;
    }
}

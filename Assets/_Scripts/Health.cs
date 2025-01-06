using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    
    [SerializeField] private Image healthBar;
        
    private float _currentHealthInverse;
        
    [SerializeField] public float currentHealth;

    protected void Start()
    {
        currentHealth = maxHealth;
        _currentHealthInverse = 1f / currentHealth;
    }
    

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        CheckDeath();

        UpdateHealthBar();
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        
        CheckHealth();

        UpdateHealthBar();
    }
    

    protected void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth * _currentHealthInverse;
    }

    protected void CheckHealth()
    {
        if (currentHealth <= maxHealth) return;
        
        currentHealth = maxHealth;
        
    }

    protected void CheckDeath()
    {
        if (currentHealth > 0) return;
        
        currentHealth = 0;

        Death();
    }

    protected virtual void Death()
    {
        //Muerte del jugador
    }
}

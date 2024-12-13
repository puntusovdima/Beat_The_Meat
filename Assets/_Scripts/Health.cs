using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    
    [SerializeField] private Image healthBar;
        
    private float _currentHealthInverse;
        
    [SerializeField] int _currentHealth;

    protected void Start()
    {
        _currentHealth = maxHealth;
        _currentHealthInverse = 1f / _currentHealth;
    }
    

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        CheckDeath();

        UpdateHealthBar();
    }

    public void Heal(int heal)
    {
        _currentHealth += heal;
        
        CheckHealth();

        UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        healthBar.fillAmount = _currentHealth * _currentHealthInverse;
    }

    protected void CheckHealth()
    {
        if (_currentHealth <= maxHealth) return;
        
        _currentHealth = maxHealth;
        
    }

    protected void CheckDeath()
    {
        if (_currentHealth > 0) return;
        
        _currentHealth = 0;

        Death();
    }

    protected virtual void Death()
    {
        //Muerte del jugador
    }
}

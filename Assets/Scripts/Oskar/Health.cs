using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [Header("Health stats")]
    [SerializeField] int maxHealth = 100;
    [SerializeField][SyncVar] int currentHealth = 100;

    
    public void takeDamage(int damage)
    {
        //if (!isServer) return;
        currentHealth -= damage;
        Debug.Log("Current Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Player Died");
            currentHealth = maxHealth;
            // Handle player death (e.g., respawn, disable controls, etc.)
        }
    }

   
}

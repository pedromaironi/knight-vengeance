using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    public int maxHealth = 100;       
    public int currentHealth;       
    public int attackDamage = 5;       
    public float attackCooldown = 2f; 
    public float attackRange = 1.5f;   

    private Animator animator;
    private Transform player;
    private bool isAttacking = false;
    private bool isDead = false;
    public AudioClip attackSound;
    private AudioSource audioSource;   

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Añadir AudioSource si no existe
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
    }

    IEnumerator AttackPlayer()
    {


        isAttacking = true;
        animator.SetTrigger("Attack");
        if (audioSource != null && attackSound != null)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
        }
        yield return new WaitForSeconds(attackCooldown);

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
        }

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("isDead", true);
        Destroy(gameObject, 2f); // Delay to allow death animation to play

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.OnEnemyKilled("Golem");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDead)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }
}

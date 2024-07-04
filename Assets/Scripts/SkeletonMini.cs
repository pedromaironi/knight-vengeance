using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMini : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    public int attackDamage = 10; // Puedes ajustar el daño según sea necesario
    public float attackCooldown = 2f;
    public float attackRange = 1.5f;

    private Animator animator;
    private Transform player;
    private bool isAttacking = false;
    private bool isAlive = true;

    private AudioSource audioSource; // Para reproducir sonidos
    public AudioClip attackSound; // Sonido de ataque del Skeleton

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isAlive && player != null)
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
        animator.SetTrigger("AttackSkeleton");

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
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    void Die()
    {
        if (isAlive) return;
        animator.SetBool("DeathSkeleton", true);
        Destroy(gameObject, 2f);

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.OnEnemyKilled("SkeletonMini");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }
}
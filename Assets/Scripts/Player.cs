using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    public float strengthJump;
    public float walkSpeed;
    private bool isAttacking;
    private bool isGrounded;
    private Rigidbody2D rg;
    private Animator animator;
    private Vector3 originalScale;
    public int maxHealth = 100;
    public int currentHealth;
    public float fallLimitY = -10f;
    public int playerAttackDamage = 20;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public TMP_Text levelUpText;

    private int golemsKilled = 0;
    private int batsKilled = 0;
    public AudioClip attackSound;
    private AudioSource audioSource;

    private int skeletonsKilled = 0;

    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetBool("isJumping", true);
            rg.AddForce(new Vector2(0, strengthJump), ForceMode2D.Impulse);
            isGrounded = false;
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        if (moveHorizontal != 0)
        {
            animator.SetBool("isWalking", true);
            transform.Translate(new Vector2(moveHorizontal * walkSpeed * Time.deltaTime, 0));

            if (moveHorizontal < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else if (moveHorizontal > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.F) && !isAttacking)
        {
            StartCoroutine(AttackWithSound());
        }

        if (transform.position.y < fallLimitY)
        {
            Die();
        }
    }

    IEnumerator AttackWithSound()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        for (int i = 0; i < 3; i++)
        {
            if (audioSource != null && attackSound != null)
            {
                audioSource.clip = attackSound;
                audioSource.Play();
            }

            yield return new WaitForSeconds(.1f);
        }

        DealDamageToEnemies();

        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("isDead");
        StartCoroutine(LoadGameOverAfterDelay(3f));
    }

    IEnumerator LoadGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Game over");
    }

    public void DealDamageToEnemies()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Golem"))
            {
                Golem golem = enemy.GetComponent<Golem>();
                if (golem != null)
                {
                    golem.TakeDamage(playerAttackDamage);
                }
            }
            else if (enemy.CompareTag("Bat"))
            {
                Bat bat = enemy.GetComponent<Bat>();
                if (bat != null)
                {
                    bat.TakeDamage(playerAttackDamage);
                }
            }
            else if (enemy.CompareTag("Skeleton"))
            {
                SkeletonMini skeleton = enemy.GetComponent<SkeletonMini>();
                if (skeleton != null)
                {
                    skeleton.TakeDamage(playerAttackDamage);
                }
            }
            else if (enemy.CompareTag("SkeletonWarrior"))
            {
                SkeletonWarrior skeletonWarrior = enemy.GetComponent<SkeletonWarrior>();
                if (skeletonWarrior != null)
                {
                    skeletonWarrior.TakeDamage(playerAttackDamage);
                }
            }
        }

        isAttacking = false;
    }

    public void OnEnemyKilled(string enemyType)
    {
        if (enemyType == "Golem")
        {
            golemsKilled++;
        }
        else if (enemyType == "Bat")
        {
            batsKilled++;
        }
        else if (enemyType == "Skeleton" || enemyType == "SkeletonWarrior")
        {
            skeletonsKilled++;

            if (skeletonsKilled >= 2)
            {
                StartCoroutine(LoadGameOverAfterDelay(3f));
            }
        }

        if (SceneManager.GetActiveScene().name == "Level1" && golemsKilled >= 2)
        {
            StartCoroutine(AdvanceToNextLevel("Level2"));
        }
        else if (SceneManager.GetActiveScene().name == "Level2" && batsKilled >= 1)
        {
            StartCoroutine(AdvanceToNextLevel("Level3"));
        }
    }

    IEnumerator AdvanceToNextLevel(string nextLevel)
    {
        if (levelUpText != null)
        {
            levelUpText.text = "Avanzando al siguiente nivel!";
        }
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(nextLevel);
    }
}

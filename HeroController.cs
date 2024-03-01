using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using TMPro;
using UnityEngine.Audio;

public class HeroController : MonoBehaviour
{
    public NavMeshAgent agent;
    Animator animator;
    public Material newSword;
    public Renderer swordRender;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip healSound, entranceSound;
    [SerializeField]
    private AudioClip[] attackSounds;
    [SerializeField]
    private AudioClip[] damageSounds;
    public Transform player;
    public PlayerController playerController;
    private HeroManager manager;
    private UIManager uiManager;
    [SerializeField]
    private GameObject healEffect;

    public LayerMask whatIsGround, whatIsPlayer;

    //Retreat/Heal
    public Vector3 walkPoint;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public bool heroAttacking;

    //States
    public float attackRange, defendRange, sightRange;
    public bool playerInHealRange, playerInDefendRange, playerInAttackRange, playerInSightRange;

    public bool isDead;

    [Header("Hero Attributes")]
    //Attributes
    public int maxHeroHealth;
    public int currentHeroHealth;
    public int heroDamage;
    public int heroDodgeChance;
    public int maxNumOfHeals;
    public int currentNumOfHeals;
    public bool canHeal;
    public bool wantToHeal;
    private bool runningAway;
    public int chanceToDodge;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerController = FindAnyObjectByType<PlayerController>();
        player = GameObject.FindGameObjectWithTag("BossTag").GetComponent<Transform>();
        manager = FindAnyObjectByType<HeroManager>();
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager.heroController = this;
        manager.heroController = this;
        playerController.hero = this;
        //Recieve Attribute Info
        Debug.Log("Spawned");
        maxHeroHealth = manager.maxHeroHealthSet;
        maxNumOfHeals = manager.maxNumOfHealsSet;
        heroDamage = manager.heroDamageSet;
        heroDodgeChance = manager.heroDodgeChanceSet;
        audioSource.PlayOneShot(entranceSound);

        if(manager.heroLevel >= 10)
        {
            swordRender.material = newSword;
        }
}
    private void Start()
    {

        currentHeroHealth = maxHeroHealth;
        currentNumOfHeals = maxNumOfHeals;
    }
    private void Update()
    {
        uiManager.currentHeroHealth = currentHeroHealth;
        uiManager.setMaxHeroHealth = maxHeroHealth;
        manager.currentHeroHealth = currentHeroHealth;
        manager.setMaxHeroHealth = maxHeroHealth;
        manager.numOfHeals = currentNumOfHeals;
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(110);
        }
        // check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInDefendRange = Physics.CheckSphere(transform.position, defendRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(currentNumOfHeals <= 0)
        {
            canHeal = false;
        }
        else
        {
            canHeal = true;
        }

        if(currentHeroHealth >= 1)
        {
            if (playerInSightRange && !playerInAttackRange && !wantToHeal)
            {
                ChasePlayer();
            }
            if (playerInSightRange && playerInAttackRange && !wantToHeal)
            {
                AttackPlayer();
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }
            if ((float)currentHeroHealth / maxHeroHealth <= 0.5 && canHeal)
            {
                wantToHeal = true;
                runningAway = true;
                Retreat();
            }
        }
        Debug.Log("Chance to Dodge: " + chanceToDodge);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlayerDamage") && playerController.playerAttacking == true && !isDead)
        {
            ChanceToDodge();
        }
    }
    public void ChanceToDodge()
    {
        chanceToDodge = Random.Range(0, 9) + heroDodgeChance;
        if (chanceToDodge >= 10)
        {
            Dodge();
        }
        else
        {
            TakeDamage(playerController.playerDamage);
        }
    }
    private void Dodge()
    {
        chanceToDodge = 0;
        animator.SetBool("IsRolling", true);
        Invoke(nameof(StopRolling), 1f);
    }
    private void StopRolling()
    {
        animator.SetBool("IsRolling", false);
    }
    private void Retreat()
    {
        Vector3 awayFromPlayer = transform.position - player.transform.position;
        awayFromPlayer.Normalize();

        if(runningAway == true)
        agent.SetDestination(transform.position + awayFromPlayer);

        //walkpoint reached
        if (!playerInDefendRange)
        {
            Debug.Log("Should Heal now");
            Heal();
        }
    }
    private void Heal()
    {
        Instantiate(healEffect, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(healSound);
        currentNumOfHeals--;
        currentHeroHealth += 150;
        if (currentHeroHealth > maxHeroHealth)
        {
            currentHeroHealth = maxHeroHealth;
        }
        Invoke(nameof(WaitForHeal), 2f);
    }
    private void WaitForHeal()
    {
        agent.SetDestination(transform.position);
        wantToHeal = false;
        runningAway = false;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            ///Attack Code
            Debug.Log("Attacking the boss");
            animator.SetBool("IsAttacking", true);
            heroAttacking = true;
            AudioClip attackClip = attackSounds[Random.Range(0, attackSounds.Length)];
            audioSource.PlayOneShot(attackClip);
            ///

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        animator.SetBool("IsAttacking", false);
        alreadyAttacked = false;
        heroAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHeroHealth -= damage;
        AudioClip hurtClip = damageSounds[Random.Range(0, damageSounds.Length)];
        audioSource.PlayOneShot(hurtClip);
        if(currentHeroHealth <= 0)
        {
            KillHero();
        }
    }

    private void KillHero()
    {
        isDead = true;
        //play animation of death
        animator.SetBool("Death", true);
        agent.SetDestination(transform.position);
        //then destroy after 5 seconds
        Invoke(nameof(DestroyHero), 5f);
    }
    private void DestroyHero()
    {
        manager.targetGroup.RemoveMember(manager.heroController.transform);
        Destroy(this.gameObject);
        manager.SpawnProcess();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, defendRange);
    }
}

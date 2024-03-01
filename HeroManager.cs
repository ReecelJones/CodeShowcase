using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.Audio;

public class HeroManager : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    private AudioSource managerSource;
    [SerializeField] private AudioClip deathSound;
    public HeroController heroController;
    private PlayerController playerController;
    public Vector3 heroSpawn;
    public GameObject heroPrefab;
    public int heroLevel;

    [Header("Hero Attributes")]
    public int maxHeroHealthSet;
    public int heroDamageSet;
    public int heroDodgeChanceSet;
    public int maxNumOfHealsSet;
    public TextMeshProUGUI heroHealthUI;
    public TextMeshProUGUI heroHeals;
    public Animator heroSlain;
    public int currentHeroHealth = 200;
    public int setMaxHeroHealth = 200;
    public int numOfHeals = 2;
    public bool heroIsCurrentlyDead;
    public AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        heroSlain.SetBool("Die", false);
        heroLevel = 1;
        Invoke(nameof(SpawnHero), 10f);
        playerController = FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>();
        managerSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        heroHealthUI.text = currentHeroHealth + " / " + setMaxHeroHealth;
        heroHeals.text = numOfHeals.ToString();
        switch (heroLevel)
        {
            case 1:
                maxHeroHealthSet = 200;
                heroDamageSet = 2;
                heroDodgeChanceSet = 1;
                maxNumOfHealsSet = 2;
                break;
            case 2:
                maxHeroHealthSet = 250;
                heroDamageSet = 5;
                heroDodgeChanceSet = 2;
                maxNumOfHealsSet = 2;
                break;
            case 3:
                maxHeroHealthSet = 300;
                heroDamageSet = 8;
                heroDodgeChanceSet = 2;
                maxNumOfHealsSet = 2;
                break;
            case 4:
                maxHeroHealthSet = 500;
                heroDamageSet = 16;
                heroDodgeChanceSet = 3;
                maxNumOfHealsSet = 2;
                break;
            case 5:
                maxHeroHealthSet = 655;
                heroDamageSet = 29;
                heroDodgeChanceSet = 3;
                maxNumOfHealsSet = 2;
                break;
            case 6:
                maxHeroHealthSet = 788;
                heroDamageSet = 46;
                heroDodgeChanceSet = 3;
                maxNumOfHealsSet = 3;
                break;
            case 7:
                maxHeroHealthSet = 999;
                heroDamageSet = 69;
                heroDodgeChanceSet = 4;
                maxNumOfHealsSet = 3;
                break;
            case 8:
                maxHeroHealthSet = 1100;
                heroDamageSet = 77;
                heroDodgeChanceSet = 4;
                maxNumOfHealsSet = 3;
                break;
            case 9:
                maxHeroHealthSet = 1275;
                heroDamageSet = 99;
                heroDodgeChanceSet = 4;
                maxNumOfHealsSet = 3;
                break;
            case 10:
                maxHeroHealthSet = 1402;
                heroDamageSet = 117;
                heroDodgeChanceSet = 5;
                maxNumOfHealsSet = 3;
                break;
            case 11:
                maxHeroHealthSet = 1500;
                heroDamageSet = 150;
                heroDodgeChanceSet = 5;
                maxNumOfHealsSet = 3;
                break;
            case 12:
                maxHeroHealthSet = 1600;
                heroDamageSet = 199;
                heroDodgeChanceSet = 5;
                maxNumOfHealsSet = 3;
                break;
            case 13:
                maxHeroHealthSet = 1789;
                heroDamageSet = 222;
                heroDodgeChanceSet = 6;
                maxNumOfHealsSet = 4;
                break;
            case 14:
                maxHeroHealthSet = 2001;
                heroDamageSet = 290;
                heroDodgeChanceSet = 6;
                maxNumOfHealsSet = 4;
                break;
            case 15:
                maxHeroHealthSet = 2222;
                heroDamageSet = 350;
                heroDodgeChanceSet = 6;
                maxNumOfHealsSet = 4;
                break;
            case 16:
                maxHeroHealthSet = 2500;
                heroDamageSet = 375;
                heroDodgeChanceSet = 7;
                maxNumOfHealsSet = 4;
                break;
            case 17:
                maxHeroHealthSet = 2700;
                heroDamageSet = 400;
                heroDodgeChanceSet = 7;
                maxNumOfHealsSet = 4;
                break;
            case 18:
                maxHeroHealthSet = 2900;
                heroDamageSet = 425;
                heroDodgeChanceSet = 7;
                maxNumOfHealsSet = 4;
                break;
            case 19:
                maxHeroHealthSet = 3100;
                heroDamageSet = 425;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
            case 20:
                maxHeroHealthSet = 3300;
                heroDamageSet = 450;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
            case 21:
                maxHeroHealthSet = 3500;
                heroDamageSet = 450;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
            case 22:
                maxHeroHealthSet = 3700;
                heroDamageSet = 475;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
            case 23:
                maxHeroHealthSet = 3700;
                heroDamageSet = 475;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
            case 24:
                maxHeroHealthSet = 3700;
                heroDamageSet = 475;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
            case 25:
                maxHeroHealthSet = 3700;
                heroDamageSet = 475;
                heroDodgeChanceSet = 8;
                maxNumOfHealsSet = 4;
                break;
        }
        if(heroLevel >= 26)
        {
            maxHeroHealthSet = 3800;
            heroDamageSet = 500;
            heroDodgeChanceSet = 8;
            maxNumOfHealsSet = 4;
        }
    }

    public void SpawnProcess()
    {
        heroSlain.SetBool("Die", true);
        managerSource.PlayOneShot(deathSound);
        heroIsCurrentlyDead = true;
        GameObject[] healEffects = GameObject.FindGameObjectsWithTag("HealEffect");
        foreach(GameObject healEffect in healEffects)
        {
            GameObject.Destroy(healEffect);
        }
        playerController.ResetAttributes();
        float timeOfSpawn = Random.Range(4, 12);
        Invoke(nameof(SpawnHero), timeOfSpawn);
    }

    public void SpawnHero()
    {
        heroSlain.SetBool("Die", false);
        heroIsCurrentlyDead = false;
        Debug.Log("Spawning Hero");
        heroLevel++;
        Instantiate(heroPrefab, heroSpawn, Quaternion.identity);
        targetGroup.AddMember(heroController.transform, 0.01f, 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterHandler : MonoBehaviour
{
    private WormholeManager manager;
    private EnvironmentManager envManager;

    public List<Encounter> encounters;

    public bool requirementsMet;

    public Encounter currentSetEncounter;

    public List<Encounter> metRequirementsEncounters; // Store encounters that meet requirements

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<WormholeManager>();
        envManager = FindObjectOfType<EnvironmentManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DetermineEncounter()
    {
        metRequirementsEncounters.Clear(); // Clear the list before checking requirements

        foreach (var encounter in encounters)
        {
            if (CheckJumpsRequirement(encounter) && 
                CheckColorIndexRequirement(encounter) && 
                CheckNoiseColourRequirement(encounter) &&
                CheckNoiseMagnitudeRequirement(encounter) &&
                CheckTwistMagnitudeRequirement(encounter) &&
                CheckTwistFrequencyRequirement(encounter) &&
                CheckLightningRequirement(encounter) &&
                CheckLightningJiggleSpeedRequirement(encounter) &&
                CheckDisruptionRequirement(encounter) &&
                CheckDisruptionFrequencyRequirement(encounter) &&
                CheckStarsRequirement(encounter))
            {
                // If the requirements are met, add the encounter to the list
                metRequirementsEncounters.Add(encounter);
            }
            else
            {
                ResetRequirements();
            }
        }

        // List of encounters that meet requirements
        HandleMetRequirementsEncounters();
    }

    private bool CheckJumpsRequirement(Encounter encounter)
    {
        return manager.wormholeCount >= encounter.requiredJumps;
    }

    private bool CheckColorIndexRequirement(Encounter encounter)
    {
        if (encounter.requiredWormholeColorIndex.Length == 0)
        {
            // No color index requirement, so return true
            return true;
        }

        return ContainsColorIndex(encounter.requiredWormholeColorIndex, manager.currentWormholeColourIndex);
    }

    private bool CheckNoiseColourRequirement(Encounter encounter)
    {
        if(encounter.hasNoiseColourRequirement)
        {
            return manager.noiseMagColour >= encounter.requiredNoiseMagColourMIN && manager.noiseMagColour <= encounter.requiredNoiseMagColourMAX;
        }
        else
        {
            return true;
        }
    }
    private bool CheckNoiseMagnitudeRequirement(Encounter encounter)
    {
        if (encounter.hasNoiseMagnitudeRequirement)
        {
            return manager.noiseMagnitude >= encounter.requiredNoiseMagnitudeMIN && manager.noiseMagnitude <= encounter.requiredNoiseMagnitudeMAX;
        }
        else
        {
            return true;
        }
    }
    private bool CheckTwistMagnitudeRequirement(Encounter encounter)
    {
        if (encounter.hasTwistMagnitudeRequirement)
        {
            return manager.twistMagnitude >= encounter.requiredTwistMagnitudeMIN && manager.twistMagnitude <= encounter.requiredTwistMagnitudeMAX;
        }
        else
        {
            return true;
        }
    }

    private bool CheckTwistFrequencyRequirement(Encounter encounter)
    {
        if (encounter.hasTwistFrequencyRequirement)
        {
            return manager.twistFrequency >= encounter.requiredTwistFrequencyMIN && manager.twistFrequency <= encounter.requiredTwistFrequencyMAX;
        }
        else
        {
            return true;
        }
    }

    private bool CheckLightningRequirement(Encounter encounter)
    {
        if (encounter.requiredLightningOff)
        {
            return manager.lightningEnabled == false;
        }
        else if (encounter.requiredLightningOn)
        {
            return manager.lightningEnabled == true;
        }
        else
        {
            return true;
        }
    }

    private bool CheckLightningJiggleSpeedRequirement(Encounter encounter)
    {
        if (encounter.hasLightningJiggleSpeedRequirement)
        {
            return manager.lightningJiggleSpeed >= encounter.requiredLightningJiggleSpeedMIN && manager.lightningJiggleSpeed <= encounter.requiredLightningJiggleSpeedMAX;

        }
        else
        {
            return true;
        }
    }

    private bool CheckDisruptionRequirement(Encounter encounter)
    {
        if (encounter.requiredDisruptionOff)
        {
            return manager.disruptionEnabled == false;
        }
        else if (encounter.requiredDisruptionOn)
        {
            // Check if disruption is enabled
            if (manager.disruptionEnabled == true)
            {
                // Check for additional condition (e.g., waves off or waves on)
                if (encounter.requiredDisruptionWavesOff)
                {
                    return manager.disruptionWavesEnabled == false;
                }
                else if (encounter.requiredDisruptionWavesOn)
                {
                    return manager.disruptionWavesEnabled == true;
                }
                else
                {
                    // disruption is on
                    return true;
                }
            }
            else
            {
                // Disruption is not enabled
                return false;
            }
        }
        else
        {
            // No disruption requirement
            return true;
        }
    }

    private bool CheckDisruptionFrequencyRequirement(Encounter encounter)
    {
        if (encounter.hasDisruptionFrequencyRequirement)
        {
            return manager.disruptionFrequency >= encounter.requiredDisruptionFrequencyMIN && manager.disruptionFrequency <= encounter.requiredDisruptionFrequencyMAX;

        }
        else
        {
            return true;
        }
    }

    private bool CheckStarsRequirement(Encounter encounter)
    {
        if (encounter.requiredStarsOff)
        {
            return manager.starsEnabled == false;
        }
        else if (encounter.requiredStarsOn)
        {
            return manager.starsEnabled == true;
        }
        else
        {
            return true;
        }
    }

    private void ResetRequirements()
    {
        requirementsMet = false;
        currentSetEncounter = null;
    }

    private bool ContainsColorIndex(int[] colorIndices, int targetIndex)
    {
        foreach (var index in colorIndices)
        {
            if (index == targetIndex)
            {
                return true;
            }
        }
        return false;
    }

    private void HandleMetRequirementsEncounters()
    {
        // Check if there are any encounters to handle
        if (metRequirementsEncounters.Count > 0)
        {
            // Find the encounter with the highest priority
            Encounter highestPriorityEncounter = metRequirementsEncounters[0];

            foreach (var encounter in metRequirementsEncounters)
            {
                // Compare priorities and update if the current encounter has a higher priority
                if (encounter.priority > highestPriorityEncounter.priority)
                {
                    highestPriorityEncounter = encounter;
                }
            }

            // Handle the encounter with the highest priority
            HandleEncounter(highestPriorityEncounter);
        }
    }

    public void HandleEncounter(Encounter encounter)
    {
        // Handle the encounter based on the provided requirements
        requirementsMet = true;
        currentSetEncounter = encounter;
        Debug.Log("Handling Encounter with " + encounter.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event 
{

    GameManager gm;

    public Sprite image;

    public int sound = -1;

    public int repeatStopper = 0;

    public string name, description, lowriskoption, mediumriskoption, highriskoption, lowrisksuccess, lowriskfailure, medrisksuccess, medriskfailure, highrisksuccess, highriskfailure;

    //MOST DISGUSTING CODE I'VE EVER WRITTEN PLEASE FORGIVE ME========================================================================================================================
    public int loyaltyAffectLowSuccess, loyaltyAffectMedSuccess, loyaltyAffectHighSuccess, loyaltyAffectLowFailure, loyaltyAffectMedFailure, loyaltyAffectHighFailure = 0;
    public int energyAffectLowSuccess, energyAffectMedSuccess, energyAffectHighSuccess, energyAffectLowFailure, energyAffectMedFailure, energyAffectHighFailure = 0;
    public int skillAffectLowSuccess, skillAffectMedSuccess, skillAffectHighSuccess, skillAffectLowFailure, skillAffectMedFailure, skillAffectHighFailure = 0;

    public int structureAffectLowSuccess, structureAffectMedSuccess, structureAffectHighSuccess, structureAffectLowFailure, structureAffectMedFailure, structureAffectHighFailure = 0;
    public int fuelAffectLowSuccess, fuelAffectMedSuccess, fuelAffectHighSuccess, fuelAffectLowFailure, fuelAffectMedFailure, fuelAffectHighFailure = 0;
    public int foodAffectLowSuccess, foodAffectMedSuccess, foodAffectHighSuccess, foodAffectLowFailure, foodAffectMedFailure, foodAffectHighFailure = 0;

    public int allLoyaltyAffectLowSuccess, allLoyaltyAffectMedSuccess, allLoyaltyAffectHighSuccess, allLoyaltyAffectLowFailure, allLoyaltyAffectMedFailure, allLoyaltyAffectHighFailure = 0;
    public int allEnergyAffectLowSuccess, allEnergyAffectMedSuccess, allEnergyAffectHighSuccess, allEnergyAffectLowFailure, allEnergyAffectMedFailure, allEnergyAffectHighFailure = 0;
    public int allSkillAffectLowSuccess, allSkillAffectMedSuccess, allSkillAffectHighSuccess, allSkillAffectLowFailure, allSkillAffectMedFailure, allSkillAffectHighFailure = 0;

    public string lowIndicator, medIndicator, highIndicator;

    public string[] mainaffects = new string[2];
    //================================================================================================================================================================================

    public float lowDificulty, medDifficulty, highDifficulty;

    //IF EVENT IS A DIALOGUE ONLY EVENT
    public bool dialogueonly, allaffecting;

    public string description2, description3;

    public Event(GameManager gameman)
    {
        gm = gameman;
    }

    public bool LowRisk()
    {
        bool success;

        if (SuccessCheck(0))
        {
            for (int i = 0; i < gm.companions.Count; i++)
            {
                int totalchangeEnergy = 0;
                int totalchangeSkill = 0;
                int totalchangeLoyalty = 0;
                if (gm.companions[i].tasked)
                {
                    totalchangeEnergy += -5 + energyAffectLowSuccess;
                    totalchangeSkill += skillAffectLowSuccess;
                    totalchangeLoyalty += loyaltyAffectLowSuccess;
                    gm.companions[i].AddEnergy(energyAffectLowSuccess - 10);
                    gm.companions[i].AddSkill(skillAffectLowSuccess);
                    gm.companions[i].AddLoyalty(loyaltyAffectLowSuccess);
                }
                totalchangeEnergy += allEnergyAffectLowSuccess;
                totalchangeSkill += allSkillAffectLowSuccess;
                totalchangeLoyalty += allLoyaltyAffectLowSuccess;
                gm.companions[i].AddEnergy(allEnergyAffectLowSuccess);
                gm.companions[i].AddSkill(allSkillAffectLowSuccess);
                gm.companions[i].AddLoyalty(allLoyaltyAffectLowSuccess);

                if(totalchangeEnergy < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeEnergy + " energy\n";
                }
                else if (totalchangeEnergy > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeEnergy + " energ\n";
                }

                if (totalchangeLoyalty < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeLoyalty + " loyalty\n";
                }
                else if (totalchangeLoyalty > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeLoyalty + " loyalty\n";
                }

                if (totalchangeSkill < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeSkill + " skill\n";
                }
                else if (totalchangeSkill > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeSkill + " skill\n";
                }

            }
            gm.SetFood(foodAffectLowSuccess);
            gm.SetFuel(fuelAffectLowSuccess);
            gm.SetStructure(structureAffectLowSuccess);

            success = true;
        }
        else
        {
            for (int i = 0; i < gm.companions.Count; i++)
            {
                int totalchangeEnergy = 0;
                int totalchangeSkill = 0;
                int totalchangeLoyalty = 0;
                if (gm.companions[i].tasked)
                {
                    totalchangeEnergy += -5 + energyAffectLowFailure;
                    totalchangeSkill += skillAffectLowFailure;
                    totalchangeLoyalty += loyaltyAffectLowFailure;
                    gm.companions[i].AddEnergy(energyAffectLowFailure - 10);
                    gm.companions[i].AddSkill(skillAffectLowFailure);
                    gm.companions[i].AddLoyalty(loyaltyAffectLowFailure);
                }
                totalchangeEnergy += allEnergyAffectLowFailure;
                totalchangeSkill += allSkillAffectLowFailure;
                totalchangeLoyalty += allLoyaltyAffectLowFailure;
                gm.companions[i].AddEnergy(allEnergyAffectLowFailure);
                gm.companions[i].AddSkill(allSkillAffectLowFailure);
                gm.companions[i].AddLoyalty(allLoyaltyAffectLowFailure);

                if (totalchangeEnergy < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeEnergy + " energy\n";
                }
                else if (totalchangeEnergy > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeEnergy + " energ\n";
                }

                if (totalchangeLoyalty < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeLoyalty + " loyalty\n";
                }
                else if (totalchangeLoyalty > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeLoyalty + " loyalty\n";
                }

                if (totalchangeSkill < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeSkill + " skill\n";
                }
                else if (totalchangeSkill > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeSkill + " skill\n";
                }
            }
            gm.SetFood(foodAffectLowFailure);
            gm.SetFuel(fuelAffectLowFailure);
            gm.SetStructure(structureAffectLowFailure);
            success = false;
        }
        return success;
    }

    public bool MedRisk()
    {
        bool success;

        if (SuccessCheck(1))
        {
            
            for (int i = 0; i < gm.companions.Count; i++)
            {
                int totalchangeEnergy = 0;
                int totalchangeSkill = 0;
                int totalchangeLoyalty = 0;
                if (gm.companions[i].tasked)
                {
                    totalchangeEnergy += -5 + energyAffectMedSuccess;
                    totalchangeLoyalty += loyaltyAffectMedSuccess;
                    totalchangeSkill += skillAffectMedSuccess;
                    gm.companions[i].AddEnergy(energyAffectMedSuccess - 10);
                    gm.companions[i].AddSkill(skillAffectMedSuccess);
                    gm.companions[i].AddLoyalty(loyaltyAffectMedSuccess);
                }
                totalchangeEnergy += allEnergyAffectMedSuccess;
                totalchangeSkill += allSkillAffectMedSuccess;
                totalchangeLoyalty += allLoyaltyAffectMedSuccess;
                gm.companions[i].AddEnergy(allEnergyAffectMedSuccess);
                gm.companions[i].AddSkill(allSkillAffectMedSuccess);
                gm.companions[i].AddLoyalty(allLoyaltyAffectMedSuccess);

                if (totalchangeEnergy < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeEnergy + " energy\n";
                }
                else if (totalchangeEnergy > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeEnergy + " energ\n";
                }

                if (totalchangeLoyalty < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeLoyalty + " loyalty\n";
                }
                else if (totalchangeLoyalty > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeLoyalty + " loyalty\n";
                }

                if (totalchangeSkill < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeSkill + " skill\n";
                }
                else if (totalchangeSkill > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeSkill + " skill\n";
                }

            }
            gm.SetFood(foodAffectMedSuccess);
            gm.SetFuel(fuelAffectMedSuccess);
            gm.SetStructure(structureAffectMedSuccess);
            success = true;
        }
        else
        {
            for (int i = 0; i < gm.companions.Count; i++)
            {
                int totalchangeEnergy = 0;
                int totalchangeSkill = 0;
                int totalchangeLoyalty = 0;
                if (gm.companions[i].tasked)
                {
                    totalchangeEnergy += -5 + energyAffectMedFailure;
                    totalchangeLoyalty += loyaltyAffectMedFailure;
                    totalchangeSkill += skillAffectMedFailure;

                    gm.companions[i].AddEnergy(energyAffectMedFailure - 10);
                    gm.companions[i].AddSkill(skillAffectMedFailure);
                    gm.companions[i].AddLoyalty(loyaltyAffectMedFailure);
                }
                totalchangeEnergy += allEnergyAffectMedFailure;
                totalchangeSkill += allSkillAffectMedFailure;
                totalchangeLoyalty += allLoyaltyAffectMedFailure;
                gm.companions[i].AddEnergy(allEnergyAffectMedFailure);
                gm.companions[i].AddSkill(allSkillAffectMedFailure);
                gm.companions[i].AddLoyalty(allLoyaltyAffectMedFailure);
                if (totalchangeEnergy < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeEnergy + " energy\n";
                }
                else if (totalchangeEnergy > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeEnergy + " energ\n";
                }

                if (totalchangeLoyalty < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeLoyalty + " loyalty\n";
                }
                else if (totalchangeLoyalty > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeLoyalty + " loyalty\n";
                }

                if (totalchangeSkill < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeSkill + " skill\n";
                }
                else if (totalchangeSkill > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeSkill + " skill\n";
                }
            }
            gm.SetFood(foodAffectMedFailure);
            gm.SetFuel(fuelAffectMedFailure);
            gm.SetStructure(structureAffectMedFailure);
            success = false;
        }
        return success;
    }

    public bool HighRisk()
    {

        bool success;
        if (SuccessCheck(2))
        {
            //apply effects to companions
            for (int i = 0; i < gm.companions.Count; i++)
            {
                int totalchangeEnergy = 0;
                int totalchangeSkill = 0;
                int totalchangeLoyalty = 0;
                if (gm.companions[i].tasked)
                {
                    totalchangeEnergy += -5 + energyAffectHighSuccess;
                    totalchangeLoyalty += loyaltyAffectHighSuccess;
                    totalchangeSkill += skillAffectHighSuccess;
                    gm.companions[i].AddEnergy(energyAffectHighSuccess-10);
                    gm.companions[i].AddSkill(skillAffectHighSuccess);
                    gm.companions[i].AddLoyalty(loyaltyAffectHighSuccess);
                }
                totalchangeEnergy += allEnergyAffectHighSuccess;
                totalchangeSkill += allSkillAffectHighSuccess;
                totalchangeLoyalty += allLoyaltyAffectHighSuccess;
                gm.companions[i].AddEnergy(allEnergyAffectHighSuccess);
                gm.companions[i].AddSkill(allSkillAffectHighSuccess);
                gm.companions[i].AddLoyalty(allLoyaltyAffectHighSuccess);
                if (totalchangeEnergy < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeEnergy + " energy\n";
                }
                else if (totalchangeEnergy > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeEnergy + " energ\n";
                }

                if (totalchangeLoyalty < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeLoyalty + " loyalty\n";
                }
                else if (totalchangeLoyalty > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeLoyalty + " loyalty\n";
                }

                if (totalchangeSkill < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeSkill + " skill\n";
                }
                else if (totalchangeSkill > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeSkill + " skill\n";
                }
            }
            gm.SetFood(foodAffectHighSuccess);
            gm.SetFuel(fuelAffectHighSuccess);
            gm.SetStructure(structureAffectHighSuccess);

            success = true;

        }
        else
        {
            for (int i = 0; i < gm.companions.Count; i++)
            {
                int totalchangeEnergy = 0;
                int totalchangeSkill = 0;
                int totalchangeLoyalty = 0;
                if (gm.companions[i].tasked)
                {
                    totalchangeEnergy += -5 + energyAffectHighFailure;
                    totalchangeLoyalty += loyaltyAffectHighFailure;
                    totalchangeSkill += skillAffectHighFailure;
                    gm.companions[i].AddEnergy(energyAffectHighFailure - 10);
                    gm.companions[i].AddSkill(skillAffectHighFailure);
                    gm.companions[i].AddLoyalty(loyaltyAffectHighFailure);
                }
                totalchangeEnergy += allEnergyAffectHighFailure;
                totalchangeSkill += allSkillAffectHighFailure;
                totalchangeLoyalty += allLoyaltyAffectHighFailure;
                gm.companions[i].AddEnergy(allEnergyAffectHighFailure);
                gm.companions[i].AddSkill(allSkillAffectHighFailure);
                gm.companions[i].AddLoyalty(allLoyaltyAffectHighFailure);
                if (totalchangeEnergy < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeEnergy + " energy\n";
                }
                else if (totalchangeEnergy > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeEnergy + " energ\n";
                }

                if (totalchangeLoyalty < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeLoyalty + " loyalty\n";
                }
                else if (totalchangeLoyalty > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeLoyalty + " loyalty\n";
                }

                if (totalchangeSkill < 0)
                {
                    gm.changes += gm.companions[i].companionName + " lost " + totalchangeSkill + " skill\n";
                }
                else if (totalchangeSkill > 0)
                {
                    gm.changes += gm.companions[i].companionName + " gained " + totalchangeSkill + " skill\n";
                }
            }
            gm.SetFood(foodAffectHighFailure);
            gm.SetFuel(fuelAffectHighFailure);
            gm.SetStructure(structureAffectHighFailure);
            success = false;

        }
        return success;

    }

    private bool SuccessCheck(int risk) //0 is low risk option, 1 = med risk option, 2 = high risk option
    {
        bool success = false;
        int roll = Random.Range(1, 101);

        if (gm.blessed)
        {
            gm.blessed = false;
            return true;
        }
        if (gm.cursed)
        {
            gm.cursed = false;
            return false;
        }

        //This is where we'd have modifiers 

        if(roll <= GameManager.instance.odds)
        {
            success = true;
        }
        else
        {
            success = false;
        }



        return success;

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Companion
{
    public GameManager gm;
    public string companionName;
    public int energy, loyalty, skill;
    private Sprite portrait;
    public bool tasked;
    public bool cursed;

    public Companion(string name, GameManager gamemanager)
    {
        gm = gamemanager;
        companionName = name;
        energy = 100;
        loyalty = 100;
        skill = 0;
        tasked = false;
    }

    public void AddEnergy(int e)
    {
        if(energy + e > 100)
        {
            energy = 100;
        }
        else if(energy + e <= 0)
        {
            energy = 0;
            gm.RemoveCompanion(companionName, 0);
        }
        else
        {
            energy += e;
        }
        /*
        if(e < 0)
        {
            GameManager.instance.changes += companionName + " lost " + e + " energy\n";
        }
        else if(e > 0)
        {
            GameManager.instance.changes += companionName + " gained " + e + " energy\n";
        }
        */
    }

    public void AddLoyalty(int e)
    {
        if (loyalty + e > 100)
        {
            loyalty = 100;
        }
        else if (loyalty + e <= 0)
        {
            loyalty = 0;
            gm.RemoveCompanion(companionName, 1);
        }
        else
        {
            loyalty += e;
        }
        /*
        if (e < 0)
        {
            GameManager.instance.changes += companionName + " lost " + e + " loyalty\n";
        }
        else if (e > 0)
        {
            GameManager.instance.changes += companionName + " gained " + e + " loyalty\n";
        }
        */
    }

    public void AddSkill(int e)
    {
        if (skill + e >= 100)
        {
            skill = 100;
            gm.RemoveCompanion(companionName, 2);
        }
        else if (skill + e <= 0)
        {
            skill = 0;
        }
        else
        {
            skill += e;
        }
        /*
        if (e < 0)
        {
            GameManager.instance.changes += companionName + " lost " + e + " skill\n";
        }
        else if (e > 0)
        {
            GameManager.instance.changes += companionName + " gained " + e + " skill\n";
        }
        */

    }



}

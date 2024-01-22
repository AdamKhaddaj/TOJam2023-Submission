using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Security.Cryptography;
using Unity.VisualScripting;
using System.Drawing;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int state; //0 = travelling, 1 = dialogue and what companions to bring, 2 = options

    public int structure, fuel, food, eventsEncountered;
    public List<Companion> companions;

    public UIManager ui;
    public UIPortrait uiportrait;
    public Island island;

    public BoatMovement airship;
    
    private List<string> names = new List<string>();
    private List<Event> events = new List<Event>();
    private List<Event> dialogueevents = new List<Event>();

    private bool[] selected = new bool[4] { false, false, false, false };

    public Event curevent;

    public int companionSelectionsMade, riskoption, checkpointsreached, dialogueeventstep;

    public int odds;

    public bool cursed, blessed, fortuneavailable, starving, empty, broken, firstload, travelling;

    public string cursedcompanionname, changes;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        eventsEncountered = 0;
        structure = 3;
        fuel = 100;
        food = 100;
        blessed = false;
        cursed = false;
        starving = false;
        empty = false;
        broken = false;
        travelling = false;
        firstload = true;
        fortuneavailable = true;
        companionSelectionsMade = 0;
        checkpointsreached = 0;
        dialogueeventstep = 0;
        companions = new List<Companion>();

        GameSetup();
        ReachCheckpoint();
    }

    IEnumerator EventTravel()
    {

        travelling = true;
        for (int i = 0; i < 1; i++)
        {
            if (empty)
            {
                FuelEmpty();
                empty = false;
            }
            if (starving)
            {
                FoodEmpty();
                starving = false;
            }
            if (broken)
            {
                StructureEmpty();
            }

            yield return new WaitForSeconds(2.5f);
            PassiveResourceDrain();
            yield return new WaitForSeconds(2.5f);
        }
        if(eventsEncountered == 5)
        {
            travelling = false;
            ReachCheckpoint();
        }
        else
        {
            travelling = false;
            RandomEvent();
        }

    }

    private void PopulateCompanions()
    {
        for(int i = 0; i < 4; i++)
        {
            Companion c = new Companion(names[i], this);
            companions.Add(c);
        }
    }

    private void PassiveResourceDrain()
    {
        SetFood( -1 * companions.Count);
        SetFuel(-2);
        if (food==0)
        {
            for(int i = 0; i < companions.Count; i++)
            {
                companions[i].AddEnergy(-3);
                companions[i].AddLoyalty(-3);
            }
        }
    }

    private void FoodEmpty()
    {
        Time.timeScale = 0;
        ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = "You've run out of food! Your companions will suffer decreases to their loyalty and energy until you restock";
        ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
        ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
    }

    public void FuelEmpty()
    {
        Time.timeScale = 0;
        ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = "You've run out of fuel! You dig into your ship's auxiliary power to keep yourselves afloat, but you strain the ship in the process";
        ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
        ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
        SetStructure(-1);
        SetFuel(50);
    }

    public void StructureEmpty()
    {
        Time.timeScale = 0;
        ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = "With a large, brittle crack, your vessel shatters into peices... as do your dreams. Better luck next time, Captain";
        ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
        ui.UI.transform.Find("NotificationPannel").Find("NotificationButton").gameObject.SetActive(false);
    }

    public void FailedCurseCompanion()
    {
        Time.timeScale = 0;
        ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = "You failed to remove the companion, invoking the wrath of the fates. A sudden bolt of lightning strikes your ship, dealing one structural damage";
        ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
        ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
        SetStructure(-1);
    }

    public void NoCompanions()
    {
        Time.timeScale = 0;
        ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = "You have no companions remaining left to be cursed. With no one to take on your bad fortune, the fates turn their eyes towards you... and your ship explodes. Your journey ends here, captain.";
        ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
        ui.UI.transform.Find("NotificationPannel").Find("NotificationButton").gameObject.SetActive(false);
    }

    public void RemoveCompanion(string name, int statzero)
    {
        int killindex = -1;
        for (int i = 0; i < companions.Count; i++)
        {
            if (companions[i].companionName == name)
            {
                killindex = i;
            }
        }

        if (statzero == 0) //energy gone
        {
            Time.timeScale = 0;
            ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = name + " has collapsed from exhaustion. They are unable to continue and have left your ship";
            ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
            ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
        }
        else if (statzero == 1) //loyalty gone
        {
            Time.timeScale = 0;
            ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = name + " is fed up with your innaptitude! They leave the ship";
            ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
            ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
        }
        else if (statzero == 2) //skill maxed
        {
            Time.timeScale = 0;
            ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = name + " has developed incredible skills, and has left your ship to pursue their dreams. They leave you 30 food as a thank you gift";
            ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
            ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
            SetFood(30);
        }

        if (companions.Count == 1)
        {
            companions.RemoveAt(killindex);
            uiportrait.displaycompanion = null;
        }
        else
        {
            companions.RemoveAt(killindex);
            uiportrait.displaycompanion = companions[0];
        }

    }

    public void Unpause()
    {

        if (companions.Count == 0)
        {
            NoCompanions();
        }
        

        Time.timeScale = 1;
        if(structure == 0)
        {
            StructureEmpty();
            return;
        }
        ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(false);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    private void RandomEvent()
    {

        eventsEncountered++;
        ui.pannelslidein = true;
        ui.pannelslideout = false;

        while (true)
        {
            int roll = Random.Range(0, events.Count);

            if (events[roll].repeatStopper == 0)
            {
                curevent = events[roll];
                curevent.repeatStopper = 4;
                break;
            }
        }

        for(int i = 0; i < events.Count; i++)
        {
            if (events[i].repeatStopper > 0) 
            {
                events[i].repeatStopper -= 1;
            }
        }

        DisplayEvent();

    }

    private void ReachCheckpoint()
    {

        GameObject.Find("Airship").gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        fortuneavailable = true;

        if (firstload)
        {
            firstload = false;
            ui.UI.transform.Find("EventPannel").GetComponent<RectTransform>().anchoredPosition = new Vector2(-535, ui.UI.transform.Find("EventPannel").GetComponent<RectTransform>().anchoredPosition.y);
        }
        else
        {
            ui.pannelslidein = true;
            ui.pannelslideout = false;
            SoundManager.instance.PlaySoundEffect(1);
        }

        eventsEncountered = 0;

        if (checkpointsreached == 0) //play intro
        {
            curevent = dialogueevents[0];
            island.On();
            DialogueEvent();

        }
        else if(checkpointsreached == 1) //first checkpoint event
        {

            int cursedcompanion = Random.Range(0, companions.Count);
            companions[cursedcompanion].cursed = true;
            cursedcompanionname = companions[cursedcompanion].companionName;

            string newdescription1 = dialogueevents[1].description2.Replace("CURSEDNAME", cursedcompanionname);
            string newdescription2 = dialogueevents[1].description3.Replace("CURSEDNAME", cursedcompanionname);

            dialogueevents[1].description2 = newdescription1;
            dialogueevents[1].description3 = newdescription2;

            curevent = dialogueevents[1];

            island.On();

            //turn on ball
            ui.UI.transform.Find("FateBall").gameObject.SetActive(true);

            DialogueEvent();

        }
        else if (checkpointsreached == 2)
        {
            for (int i = 0; i < companions.Count; i++)
            {
                if (companions[i].cursed)
                {
                    FailedCurseCompanion();
                }
            }


            int cursedcompanion = Random.Range(0, companions.Count);
            companions[cursedcompanion].cursed = true;
            cursedcompanionname = companions[cursedcompanion].companionName;
            string newdescription = dialogueevents[2].description2.Replace("CURSEDNAME", cursedcompanionname);
            dialogueevents[2].description2 = newdescription;
            curevent = dialogueevents[2];

            island.On();

            DialogueEvent();
        }
        else if (checkpointsreached == 3)
        {

            for (int i = 0; i < companions.Count; i++)
            {
                if (companions[i].cursed)
                {
                    FailedCurseCompanion();
                }
            }
            int cursedcompanion = Random.Range(0, companions.Count);
            companions[cursedcompanion].cursed = true;
            cursedcompanionname = companions[cursedcompanion].companionName;
            curevent = dialogueevents[3];

            island.On();

            DialogueEvent();
        }
        else if (checkpointsreached == 4)
        {
            if (companions.Count != 0)
            {
                for (int i = 0; i < companions.Count; i++)
                {
                    if (companions[i].cursed)
                    {
                    }
                }
            }

            curevent = dialogueevents[4];

            island.SetFinalIsland();
            island.On();

            DialogueEvent();
        }

        checkpointsreached++;
    }

    public void TellFortune()
    {
        Time.timeScale = 0;

        ui.fortuneslidein = true;
        ui.fortuneslideout = false;
    }

    public void Curse()
    {
        cursed = true;
        blessed = false;
        fortuneavailable = false;

        ui.fortuneslideout = true;
        ui.fortuneslidein = false;

        SoundManager.instance.PlaySoundEffect(0);

        Time.timeScale = 1;
    }

    public void Bless()
    {
        blessed = true;
        cursed = false;
        fortuneavailable = false;

        ui.fortuneslideout = true;
        ui.fortuneslidein = false;

        SoundManager.instance.PlaySoundEffect(4);

        Time.timeScale = 1;
    }

    public void SetFood(int val)
    {
        if(food + val <= 0)
        {
            food = 0;
            starving = true;
        }
        else if (food + val > 100)
        {
            food = 100;
            starving = false;
        }
        else
        {
            food += val;
            starving = false;
        }

        if (val < 0)
        {
            changes += "The Stellarix 9000 lost " + val + " food\n";
        }
        else if (val > 0)
        {
            changes += "The Stellarix 9000 gained " + val + " food\n";
        }

    }
    
    public void SetFuel(int val)
    {
        if (fuel + val <= 0)
        {
            fuel = 0;
            empty = true;
        }
        else if (fuel + val > 100)
        {
            fuel = 100;
            empty = false;
        }
        else
        {
            fuel += val;
            empty = false;
        }

        if (val < 0)
        {
            changes += "The Stellarix 9000 lost " + val + " fuel\n";
        }
        else if (val > 0)
        {
            changes += "The Stellarix 9000 gained " + val + " fuel\n";
        }

    }

    public void SetStructure(int val)
    {
        if(structure+val > 3)
        {
            structure = 3;
            broken = false;
        }
        else if (structure+val <= 0)
        {
            structure = 0;
            broken = true;
        }
        else
        {
            structure += val;
            broken = false;
        }

        if (val < 0)
        {
            changes += "The Stellarix 9000 lost " + val + " structure\n";
        }
        else if (val > 0)
        {
            changes += "The Stellarix 9000 gained " + val + " structure\n";
        }
        airship.UpdateShipSprite();
    }

    public void StartTravel()
    {
        //set unactive to most UI stuff
        foreach (Transform child in ui.UI.transform.Find("EventPannel").gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }

        ui.pannelslidein = false;
        ui.pannelslideout = true;

        curevent = null;

        companionSelectionsMade = 0;

        blessed = false;
        cursed = false;

        dialogueeventstep = 0;

        island.Off();
        GameObject.Find("Airship").gameObject.transform.localScale = new Vector3(0.74f, 0.74f, 0.74f);

        for (int i = 0; i < selected.Length; i++)
        {
            selected[i] = false;
        }

        for (int i = 0; i < companions.Count; i++)
        {
            companions[i].tasked = false;
        }

        StartCoroutine("EventTravel");

    }


    //DIALOGUE EVENT HANDLING ===================================================================================================================================================================

    public void DialogueEvent()
    {
        ui.UI.transform.Find("EventPannel").Find("DialogueTitle").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.name;
        ui.UI.transform.Find("EventPannel").Find("DialogueTitle").gameObject.SetActive(true);
        ui.UI.transform.Find("EventPannel").Find("DialogueDescription").gameObject.SetActive(true);
        ui.UI.transform.Find("EventPannel").Find("DialogueButton").gameObject.SetActive(true);

        if(dialogueeventstep == 0)
        {
            ui.UI.transform.Find("EventPannel").Find("DialogueDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.description;
        }
        if(dialogueeventstep == 1)
        {
            ui.UI.transform.Find("EventPannel").Find("DialogueDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.description2;
        }
        if (dialogueeventstep == 2)
        {
            ui.UI.transform.Find("EventPannel").Find("DialogueDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.description3;
        }
        if (dialogueeventstep == 3)
        {
            ui.UI.transform.Find("EventPannel").Find("DialogueTitle").gameObject.SetActive(false);
            ui.UI.transform.Find("EventPannel").Find("DialogueDescription").gameObject.SetActive(false);
            ui.UI.transform.Find("EventPannel").Find("DialogueButton").gameObject.SetActive(false);

            if(checkpointsreached == 4)
            {
                Time.timeScale = 0;
                ui.UI.transform.Find("NotificationPannel").Find("NotificationText").GetComponent<TMPro.TextMeshProUGUI>().text = "Thank you for playing our game! This was made during a 72 hour game jam event with TOJam2023. We hope you enjoyed!\n" +
                    "Credits:\n" +
                    "Adam Khaddaj: Programming, Design, Writing, Direction\n" +
                    "Hazel Gifford: Art and Writing\n" +
                    "Daniel Khaddaj: Sound and Game Design\n" +
                    "Leo McCarthy-Kennedy: Music\n" +
                    "Luka Andjelic: Game Design\n" +
                    "Some art assets that were used were created by the following artist:\n" +
                    "Madame Zoya: Zelda Devon";
                ui.UI.transform.Find("NotificationPannel").gameObject.SetActive(true);
                ui.UI.transform.Find("NotificationPannel").Find("GameOverButton").gameObject.SetActive(false);
            }

            StartTravel();
        }
        dialogueeventstep++;

    }

    //EVENT HANDLING====================================================================================================================================================================
    public void SelectButton(int c)
    {

        if (selected[c])
        {
            selected[c] = false;
            ui.UI.transform.Find("EventPannel").Find("Companion" + c).GetComponent<Image>().color = new Color32(223, 191, 243, 255);

            for (int i = 0; i < companions.Count; i++)
            {
                if (companions[i].companionName == ui.UI.transform.Find("EventPannel").Find("Companion" + c).Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text)
                {
                    companions[i].tasked = false;
                }
            }

            companionSelectionsMade--;
        }
        else
        {
            selected[c] = true;
            ui.UI.transform.Find("EventPannel").Find("Companion" + c).GetComponent<Image>().color = new Color32(168, 114, 203, 255);

            for (int i = 0; i < companions.Count; i++)
            {
                if (companions[i].companionName == ui.UI.transform.Find("EventPannel").Find("Companion" + c).Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text)
                {
                    companions[i].tasked = true;
                }
            }

            companionSelectionsMade++;
        }

        //calculate odds of success:
        if (riskoption == 0)
        {
            odds = 100 - (int)curevent.lowDificulty;
        }
        else if (riskoption == 1)
        {
            odds = 100 - (int)curevent.medDifficulty;
        }
        else
        {
            odds = 100 - (int)curevent.highDifficulty;
        }
        for (int i = 0; i < companions.Count; i++)
        {
            if (companions[i].tasked)
            {
                odds += 7 + (int)Mathf.Round((companions[i].skill * 0.20f));
            }
        }
        if (blessed)
        {
            odds = 100;
        }
        else if (cursed)
        {
            odds = 0;
        }
        ui.UI.transform.Find("EventPannel").Find("SuccessBar").GetComponent<Bar>().SetValue(odds);
    }

    private void DisplayEvent()
    {
        if(curevent.sound != -1)
        {
            SoundManager.instance.PlaySoundEffect(curevent.sound);
        }

        ui.UI.transform.Find("EventPannel").Find("EventTitle").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.name;
        ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.description;
        ui.UI.transform.Find("EventPannel").Find("EventImage").GetComponent<Image>().sprite = curevent.image;

        ui.UI.transform.Find("EventPannel").Find("EventTitle").gameObject.SetActive(true);
        ui.UI.transform.Find("EventPannel").Find("EventDescription").gameObject.SetActive(true);
        ui.UI.transform.Find("EventPannel").Find("EventImage").gameObject.SetActive(true);

        //display options

        ui.UI.transform.Find("EventPannel").Find("LowRiskOption").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.lowriskoption;
        ui.UI.transform.Find("EventPannel").Find("MedRiskOption").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.mediumriskoption;
        ui.UI.transform.Find("EventPannel").Find("HighRiskOption").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.highriskoption;

        ui.UI.transform.Find("EventPannel").Find("LowRiskOption").gameObject.SetActive(true);
        ui.UI.transform.Find("EventPannel").Find("MedRiskOption").gameObject.SetActive(true);
        ui.UI.transform.Find("EventPannel").Find("HighRiskOption").gameObject.SetActive(true);

    }

    public void DisplayCompanions(int choice)
    {
        ui.UI.transform.Find("EventPannel").Find("GoButton").gameObject.SetActive(true);

        ui.UI.transform.Find("EventPannel").Find("Indicator").GetComponent<TMPro.TextMeshProUGUI>().text = " ";

        ui.UI.transform.Find("EventPannel").Find("Indicator").gameObject.SetActive(false);

        //set choice to which risk option the player chose
        riskoption = choice;

        odds = 0;

        if (riskoption == 0)
        {
            odds = 100 - (int)curevent.lowDificulty;
        }
        else if (riskoption == 1)
        {
            odds = 100 - (int)curevent.medDifficulty;
        }
        else
        {
            odds = 100 - (int)curevent.highDifficulty;
        }

        if (blessed)
        {
            ui.UI.transform.Find("EventPannel").Find("SuccessBar").Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = "BLESSED";
            odds = 100;
        }
        else if (cursed)
        {
            ui.UI.transform.Find("EventPannel").Find("SuccessBar").Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = "CURSED";
            odds = 0;
        }
        else
        {
            ui.UI.transform.Find("EventPannel").Find("SuccessBar").Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = "SUCCESS CHANCE";
        }

        ui.UI.transform.Find("EventPannel").Find("SuccessBar").gameObject.SetActive(true);
        

        ui.UI.transform.Find("EventPannel").Find("SuccessBar").GetComponent<Bar>().SetValue(odds);

        //un-display choices
        ui.UI.transform.Find("EventPannel").Find("LowRiskOption").gameObject.SetActive(false);
        ui.UI.transform.Find("EventPannel").Find("MedRiskOption").gameObject.SetActive(false);
        ui.UI.transform.Find("EventPannel").Find("HighRiskOption").gameObject.SetActive(false);

        for (int i = 0; i < companions.Count; i++)
        {
            ui.UI.transform.Find("EventPannel").Find("Companion" + i).Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = companions[i].companionName;
            ui.UI.transform.Find("EventPannel").Find("Companion" + i).gameObject.SetActive(true);
        }
        ui.UI.transform.Find("EventPannel").Find("SuccessBar").GetComponent<Bar>().SetValue(odds);

    }

    public void ExecuteEvent()
    {

        changes = "";

        ui.UI.transform.Find("EventPannel").Find("SuccessBar").gameObject.SetActive(false);

        for (int i = 0; i < companions.Count; i++)
        {
            ui.UI.transform.Find("EventPannel").Find("Companion" + i).Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = companions[i].companionName;
            ui.UI.transform.Find("EventPannel").Find("Companion" + i).GetComponent<Image>().color = new Color32(223, 191, 243, 255);
            ui.UI.transform.Find("EventPannel").Find("Companion" + i).gameObject.SetActive(false);
        }

        ui.UI.transform.Find("EventPannel").Find("GoButton").gameObject.SetActive(false);

        ui.UI.transform.Find("EventPannel").Find("MoveOnButton").gameObject.SetActive(true);

        if (riskoption == 0)
        {
            if (curevent.LowRisk())
            {
                ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.lowrisksuccess;
            }
            else
            {
                ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.lowriskfailure;
            }
        }
        else if (riskoption == 1)
        {
            if (curevent.MedRisk())
            {
                ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.medrisksuccess;
            }
            else
            {
                ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.medriskfailure;
            }
        }
        else if (riskoption == 2)
        {
            if (curevent.HighRisk())
            {
                ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.highrisksuccess;
            }
            else
            {
                ui.UI.transform.Find("EventPannel").Find("EventDescription").GetComponent<TMPro.TextMeshProUGUI>().text = curevent.highriskfailure;
            }
        }

        ui.UI.transform.Find("EventPannel").Find("StatsSummary").GetComponent<TMPro.TextMeshProUGUI>().text = changes;
        ui.UI.transform.Find("EventPannel").Find("StatsSummary").gameObject.SetActive(true);

    }

    //===============================================================================================================================================================================

    //This probably should be imported as a text file instead of all being written in code but.... oh well!
    private void GameSetup()
    {
        //Create Companions
        names.Add("Octavia");
        names.Add("Bimley");
        names.Add("Giovanni");
        names.Add("Jacques");
        names.Add("Jiao");
        names.Add("Eddie");

        PopulateCompanions();

        ui.UI.transform.Find("EventPannel").Find("SuccessBar").GetComponent<Bar>().SetMaxValue(100);
        ui.UI.transform.Find("EventPannel").Find("SuccessBar").GetComponent<Bar>().SetValue(0);

        //Create dialogue events
        Event introevent = new Event(this);

        introevent.name = "Introduction - The Journey's Beginning";
        introevent.description = "Welcome, Captain! You have begun your journey to the elusive and fabled Temple of Euz, seeking refuge from viscious " +
            "debt collectors. It is a journey so treacherous, so terrible, that fate itself forbids all from making safe passage there. \nLuckily, with the aid of the esteemed fortune teller Madame Zoya"+
            ", you've found a loophole. Should you make others fail on their journey to the temple... the fates might look the" +
            " other way. Madame Zoya will tell you more of this when you reach the first checkpoint. ";
        introevent.description2 = "As the captain, you will need to balance your ship's fuel, food, and structural integrity, all while keeping your" +
            " companions healthy and loyal.\nAt least, until you need them gone.\nYou will encounter strange events along your journey, " +
            "and it will be your duty to handle them effectively, as well as determine which sorry fools will have to go deal with it." +
            "But fret not! With your leadership, and a bit of fortune, fate can be woo'd your way. ";
        introevent.description3 = "A few tips before you begin:\nEvents usually have low, medium, and high risk options.\nSending a companion on an event will ALWAUS cost them 5 energy, regardless of the outcome.\nCompanion Skill infleunces the success rate when you send them to handle events \n\n" +
            "Now go, and blaze a trail on the Stellarix 9000!";
        dialogueevents.Add(introevent);

        Event firstcheckpointevent = new Event(this);

        firstcheckpointevent.name = "Checkpoint One - Fate's Ire";
        firstcheckpointevent.description = "You've successfully reached the first checkpoint in your journey. It is a peaceful little skytown, quiet and humble, with soft clouds " +
            "rolling over the town floors. \n" +
            "You and your companions have finally earned some rest and resupply. However, Madame Zoya speaks of ill tidings...";
        firstcheckpointevent.description2 = "The Fortune Teller speaks in a low and intentional voice: 'It seems the fates has finally caught notice of our little outing. They've drawn " +
            "the debt collectors our way, and the winds have begun acting against us. Now we must begin applying... preventative measures.'\n" +
            "Her eyes roll back into her skull, and she takes a sharp breath.\n'I see... they ask for CURSEDNAME to fail this journey. Get them off " +
            "the ship, by any means necessary. You have my blessings, and curses, should you need them.'\nShe gives a short bow, and leaves.";
        firstcheckpointevent.description3 = "Before reaching the next checkpoint, you must have CURSEDNAME leave your ship and fail the journey, " +
            "lest the fates point their ill wills towards you instead. This can be achieved by having their Energy or Loyalty reach 0.\n" +
            "By using the crystal ball on the bottom left corner of your screen, you may request Madame Zoya to bless or curse the next event you encounter, guaranteeing either success of failure, depending on what you wish upon your companions...\n" +
            "There are 5 events between each checkpoint. Stay sharp, captain.";
        dialogueevents.Add(firstcheckpointevent);

        Event secondcheckpointevent = new Event(this);

        secondcheckpointevent.name = "Checkpoint Two - Storm";
        secondcheckpointevent.description = "You've made it to the second checkpoint on your journey, and you're not quite sure how happy you are about it.\n" +
            "In contrast to the previous town, this place could only be descriped as grey and cold. The reek of rotting skyfish hangs in the air, and you doubt that " +
            "you've seen a single smile since making it to port.\n But it is a town nonetheless, and so you refill on supplies, and get some rest.";
        secondcheckpointevent.description2 = "Madame Zoya approaches you.\n'CURSEDNAME must fail the journey'\nShe slips back into her quarters. Nothing else needed to be said.";
        secondcheckpointevent.description3 = "The morning comes, and the Stellarix 9000 sets sail once more.";
        dialogueevents.Add(secondcheckpointevent);

        Event thirdcheckpoint = new Event(this);

        thirdcheckpoint.name = "Checkpoint Three - Fog";
        thirdcheckpoint.description = "You made it to the third and final town on your journey. The great temple of Eux looms over the horizon, your safety, your new home. " +
            "But the clouds here are dark, and your pursuers feel closer than they've ever been before. Now is the time for haste, captain!";
        thirdcheckpoint.description2 = "Madame Zoya approaches you. Even she seems nervous.\n'CURSEDNAME must fail the journey. This is the final test.'\nShe slips back into her quarters. It's now or never, captain.";
        thirdcheckpoint.description3 = "Set sail!";
        dialogueevents.Add(thirdcheckpoint);

        Event fourthcheckpoint = new Event(this);

        fourthcheckpoint.name = "Epilogue - The Great Temple of Euz";
        fourthcheckpoint.description = "You stand before the Great Temple of Euz, but you can hardly believe you're really here. It's beauty is unmathced, in every conceivable way. \n" +
            "Waters clearer than the sky herself, towering stone pillars that are as magnificent as they are daunting. You feel like you will finally be safe here.";
        fourthcheckpoint.description2 = "Madame Zoya gives you a small bow and walks away. Seems she already has other business to attend to. You give her a bow yourself.\n";
        fourthcheckpoint.description3 = "Finally, you turn to the Stellarix 9000. The great vessel stands there, resting ontop the clouds.\nYou unhook it from the dock, and let it sail away. Maybe it'll find the next poor soul with debt collectors at their heel, just as it found you.\n ";
        dialogueevents.Add(fourthcheckpoint);

        //Create Events

        //EVENT ONE SCURVY

        Event testevent1 = new Event(this);
        testevent1.name = "Scurvy";
        testevent1.description = "But damn, have you been feeling tired lately. Lethargy, achiness, your face hurts, your bones hurt, and your gums sag. You haven't been eating your vegetables, have you? Didn't your parents teach you any better?";
        testevent1.mainaffects[0] = "energy";
        testevent1.mainaffects[1] = "loyalty";

        testevent1.sound = 2;

        testevent1.image = Resources.Load<Sprite>("eventIcons/scurvy");

        testevent1.mediumriskoption = "Have them whip up a strong vitamin C concoction";
        testevent1.medIndicator = "Success: AllEnergy+ AllLoyalty+ Skill+\nFailure: Food- AllEnergy-";

        testevent1.highriskoption = "Ignore it. Get some sailors together to play scatterjacks with their teeth, it'll raise morale at least.";
        testevent1.highIndicator = "Success: AllEnergy- AllLoyalty+++ AllSkill+\nFailure: AllEnergy---";

        testevent1.lowriskoption = "Have them dig through the food piles and hand out oranges.";
        testevent1.lowIndicator = "Success: Food-- AllLoyalty++ AllEnergy+\nFailure: Food- AllEnergy-";


        testevent1.medrisksuccess = "SUCCESS: What's in it? Doesn't matter. You and your crew feel fully rejuvinated, and that's all that matters.";
        testevent1.medriskfailure = "FAILURE: Do you even know what foods contain enough vitamin C? This is just a milky soup, and a gross one at that.";


        testevent1.highrisksuccess = "SUCCESS: They may still be exhausted, but they sure are having fun. Nice choice captain.";
        testevent1.highriskfailure = "FAILURE: Seems like scatterjacks didn't help distract from the fact they'd be sucking down porridge for the next few months. Shame.";

        testevent1.lowrisksuccess = "SUCCESS: Tasty and refreshing! The crew will be better in no time.";
        testevent1.lowriskfailure = "FAILURE: They couldn't find a single orange, let alone a single fruit. Who the hell was in charge of provisions?";

        testevent1.lowDificulty = 45f;
        testevent1.medDifficulty = 60f;
        testevent1.highDifficulty = 75f;

        testevent1.allEnergyAffectMedSuccess = 8;
        testevent1.allLoyaltyAffectMedSuccess = 8;
        testevent1.skillAffectMedSuccess = 10;

        testevent1.foodAffectMedFailure = -8;
        testevent1.allEnergyAffectMedFailure = -8;

        testevent1.foodAffectLowSuccess = -18;
        testevent1.allLoyaltyAffectLowSuccess = 23;
        testevent1.allEnergyAffectLowSuccess = 10;

        testevent1.foodAffectLowFailure = -8;
        testevent1.allEnergyAffectLowFailure = -8;

        testevent1.allEnergyAffectHighSuccess = -8;
        testevent1.allLoyaltyAffectHighSuccess = 35;
        testevent1.allSkillAffectHighSuccess = 10;

        testevent1.allEnergyAffectHighFailure = -30;

        events.Add(testevent1);


        //EVENT TWO LEAKY HULL

        Event testevent3 = new Event(this);
        testevent3.name = "Leaky Hull";
        testevent3.description = "Uh oh, the ship (which must stay in the air) has a hole in it! Someone’s gotta do something, because engineering and structural integrity are complicated, and it’s probably a bad idea to do nothing.";
        testevent3.image = Resources.Load<Sprite>("eventIcons/burst_engine");

        

        testevent3.mainaffects[0] = "fuel";
        testevent3.mainaffects[1] = "structure";

        testevent3.lowriskoption = "Repair it, we don't shy away from a little elbow grease!";
        testevent3.lowIndicator = "Success: Energy- Fuel+ Skill+\nFailure: Energy- Loyalty-";


        testevent3.mediumriskoption = "Set up fans near the hole. If we blow the air back out, it'll basically balance out.";
        testevent3.medIndicator = "Success: Fuel++\nFailure: Fuel- Loyalty-";


        testevent3.highriskoption = "Carve out more of the hull and turn it into a window. It'll be beautiful, and aerodynamic!";
        testevent3.highIndicator = "Success: Fuel++ AllEnergy++ \nFailure: Fuel-- Structure-";

        testevent3.lowrisksuccess = "SUCCESS: That patch isn’t perfect, but it’ll do the job. Good work.";
        testevent3.lowriskfailure = "FAILURE: Taping up a piece of paper to the hull isn’t really what I had in mind when I said “repair it.”";

        testevent3.medrisksuccess = "SUCCESS: Pressure differences, buoyancy, all just fancy talk. She's working great!.";
        testevent3.medriskfailure = "FAILURE: Big mistake bucko, the Stellarix is looking tilted, and her noise is definitely pointing down.";

        testevent3.highrisksuccess = "SUCCESS: The lower decks now have some much needed sunlight, and the extra airflow was surprisingly helpful!";
        testevent3.highriskfailure = "FAILURE: You made the hole bigger. What did you think was gonna happen?";

        testevent3.lowDificulty = 45f;
        testevent3.medDifficulty = 60f;
        testevent3.highDifficulty = 75f;

        testevent3.fuelAffectLowSuccess = 7;
        testevent3.skillAffectLowSuccess = 10;
        testevent3.energyAffectLowSuccess = -7;

        testevent3.energyAffectLowFailure = -7;
        testevent3.loyaltyAffectLowFailure = -7;

        testevent3.fuelAffectMedSuccess = 23;

        testevent3.fuelAffectMedFailure = -9;
        testevent3.loyaltyAffectMedFailure = -12;

        testevent3.fuelAffectHighSuccess = 23;
        testevent3.allEnergyAffectHighSuccess = 23;

        testevent3.fuelAffectHighFailure = -19;
        testevent3.structureAffectHighFailure = -1;


        events.Add(testevent3);

        //EVENT THREE COMPASS OVERBOARD

        Event testevent4 = new Event(this);
        testevent4.name = "Compass Overboard!";
        testevent4.description = "You were just strolling along the deck, taking in the sights, when oops, an apple rolls out of its crate! One thing leads to the next, and now the compass has fallen overboard. It wasn’t like a cherished heirloom or anything, but it certainly was useful for not getting wildly lost.";
        testevent4.image = Resources.Load<Sprite>("eventIcons/compass_overboard");

        testevent4.mainaffects[0] = "fuel";
        testevent4.mainaffects[1] = "loyalty";

        testevent4.highriskoption = "Loop a rope around some crew members and send them overboard!";
        testevent4.highIndicator = "Success: Fuel++ Skill+++\nFailure: Energy-- Loyalty---";

        testevent4.mediumriskoption = "Orient yourself using the stars instead, like our forefathers did!";
        testevent4.medIndicator = "Success: Fuel+ Skill+ Loyalty+\nFailure: Fuel- Loyalty-";

        testevent4.lowriskoption = "The thing was a finnicky mess anyway, just have the sailors keep us in the same direction we were going.";
        testevent4.lowIndicator = "Success: Fuel+\nFailure: Fuel-";

        testevent4.highrisksuccess = "SUCCESS: In a feat of acrobatic prowess, they actually managed to catch it! Well done!";
        testevent4.highriskfailure = "FAILURE: It's a miracle they're still alive, but their ribs won't grow back.";

        testevent4.medrisksuccess = "SUCCESS: The compass was unnecessary, the cosmos have guided us just fine.";
        testevent4.medriskfailure = "FAILURE: So many stars. How the hell is anyone supposed to keep track?";

        testevent4.lowrisksuccess = "SUCCESS: Look, the town reamins just over the horizon! No harm done.";
        testevent4.lowriskfailure = "FAILURE: Well, we're still heading in the right direction, it's just much, much longer.";

        testevent4.lowDificulty = 45f;
        testevent4.medDifficulty = 60f;
        testevent4.highDifficulty = 75f;

        testevent4.fuelAffectLowSuccess = 8;
        testevent4.fuelAffectLowFailure = -8;

        testevent4.fuelAffectMedSuccess = 8;
        testevent4.skillAffectMedSuccess = 10;
        testevent4.loyaltyAffectMedSuccess = 10;

        testevent4.fuelAffectMedFailure = -8;
        testevent4.loyaltyAffectMedFailure = -8;

        testevent4.fuelAffectHighSuccess = 22;
        testevent4.skillAffectHighSuccess = 30;

        testevent4.energyAffectHighFailure = -22;
        testevent4.loyaltyAffectHighFailure = -33;

        events.Add(testevent4);

        //
        // EVENT FIVE ENORMOUS CLOUD

        Event testevent5 = new Event(this);
        testevent5.name = "Enormous Cloud";
        testevent5.description = "You are of course intimately aware of what it is like to fly through a cloud, given that you are riding on an airship, but those were faint plumes compared to the mammoth you’ve found yourself facing. You’ve been sailing without vision for well over an hour, and fear has begun creaping in.";
        testevent5.image = Resources.Load<Sprite>("eventIcons/enormous_cloud");

        testevent5.mainaffects[0] = "energy";
        testevent5.mainaffects[1] = "fuel";
    
        testevent5.lowriskoption = "Have them fold the sails, and turn us around. There can never be too much caution.";
        testevent5.lowIndicator = "Success: Fuel- Loyalty+\nFailure: Fuel-";

        testevent5.mediumriskoption = "Get a few sailors to set up cloud-catching buckets. We won't let this freshwater go to waste!";
        testevent5.medIndicator = "Success: AllEnergy+++ AllLoyalty+ Fuel-\nFailure: AllEnergy- Fuel-";

        testevent5.highriskoption = "Make them row! We sail straight through it! The quicker the better!";
        testevent5.highIndicator = "Success: Fuel+++ Loyalty++ Energy-\nFailure: Energy-- Loyalty- Fuel-";

        testevent5.lowrisksuccess = "SUCCESS: It may have taken a while, but testing nature would have been unwise indeed.";
        testevent5.lowriskfailure = "FAILURE: You know that picture of the dude mining and he turns around right before he finds all the diamonds? That";

        testevent5.medrisksuccess = "SUCCESS: Delicious, cool, and crisp. How rejuvenating. The crew barely notices how lost we are.";
        testevent5.medriskfailure = "FAILURE: It tasted like polution. And defeat. And we're still lost.";

        testevent5.highrisksuccess = "SUCCESS: You soar out of the clouds, leaving a trail of mist behind you. Great work captain.";
        testevent5.highriskfailure = "FAILURE: It's been hours. Everything is wet and cold. At least everyone gets free showers.";

        testevent5.lowDificulty = 45f;
        testevent5.medDifficulty = 60f;
        testevent5.highDifficulty = 75f;

        testevent5.fuelAffectLowSuccess = -6;
        testevent5.loyaltyAffectLowSuccess = 8;

        testevent5.fuelAffectLowFailure = -6;

        testevent5.allEnergyAffectMedSuccess = 30;
        testevent5.allLoyaltyAffectMedSuccess = 10;
        testevent5.fuelAffectMedSuccess = -10;

        testevent5.allEnergyAffectMedFailure = -8;
        testevent5.fuelAffectMedFailure = -7;

        testevent5.fuelAffectHighSuccess = 30;
        testevent5.loyaltyAffectHighSuccess = 20;
        testevent5.energyAffectHighSuccess = -7;

        testevent5.energyAffectHighFailure = -12;
        testevent5.loyaltyAffectHighFailure = -6;
        testevent5.fuelAffectHighFailure = -8;

        events.Add(testevent5);

        // EVENT SIX GOAT ON A STICK

        Event testevent7 = new Event(this);
        testevent7.name = "Goat On a Stick";
        testevent7.description = "There’s a goat on a stick. You have a feeling it's referring to something. Something to do with... toes?";
        testevent7.image = Resources.Load<Sprite>("eventIcons/goat_on_stick");

        testevent7.mainaffects[0] = "???";
        testevent7.mainaffects[1] = "???";

        testevent7.highriskoption = "Ask: What does it mean to live a good life?";
        testevent7.mediumriskoption = "Ask: What is freedom?";
        testevent7.lowriskoption = "Ask: Why are you standing on a stick?";
        testevent7.lowIndicator = "???";
        testevent7.medIndicator = "???";
        testevent7.highIndicator = "???";

        testevent7.highrisksuccess = "SUCCESS: 'Oh child, now that is a question. Perhaps, to live a good life means to be proud. Perhaps, it means to savour love, and create joy. Perhaps, it means none of that at all. Perhaps it simply means standing on a stick.'";
        testevent7.highriskfailure = "SUCCESS: 'Oh child, now that is a question. Perhaps, to live a good life means to be proud. Perhaps, it means to savour love, and create joy. Perhaps, it means none of that at all. Perhaps it simply means standing on a stick.'";

        testevent7.medrisksuccess = "SUCCESS: Certainly not what you’re enjoying, debt-laden servant of fates. When people speak of freedom they often talk about it in relation to choice, and the ability to make such choice unrestrained. I think in your case, that is not a very helpful description, because the mental chains you struggle against blind you to those alternative choices. Much as you tell yourself that you are on the path to personal fulfillment, is that really true?'";
        testevent7.medriskfailure = "SUCCESS: Certainly not what you’re enjoying, debt-laden servant of fates. When people speak of freedom they often talk about it in relation to choice, and the ability to make such choice unrestrained. I think in your case, that is not a very helpful description, because the mental chains you struggle against blind you to those alternative choices. Much as you tell yourself that you are on the path to personal fulfillment, is that really true?'";

        testevent7.lowrisksuccess = "SUCCESS: Why not? Why are you standing on an airship? Most things, when looked at from a certain angle at least, are freakish, strange, or at least odd. If you go about questioning everything you see that seems out of place to you, then in time you will fear those things, and your world will begin to shrink, becoming cramped and dull. Learning to accept the strange opens your mind to a world of beauty and intrigue, and the better you get at it, the more that you’ll find. Maybe you ought to consider standing on a stick. You might find that you like it.";
        testevent7.lowriskfailure = "SUCCESS: Why not? Why are you standing on an airship? Most things, when looked at from a certain angle at least, are freakish, strange, or at least odd. If you go about questioning everything you see that seems out of place to you, then in time you will fear those things, and your world will begin to shrink, becoming cramped and dull. Learning to accept the strange opens your mind to a world of beauty and intrigue, and the better you get at it, the more that you’ll find. Maybe you ought to consider standing on a stick. You might find that you like it.";

        testevent7.lowDificulty = 0f;
        testevent7.medDifficulty = 0f;
        testevent7.highDifficulty = 0f;

        testevent7.fuelAffectMedSuccess = 15;
        testevent7.fuelAffectMedFailure = 15;

        testevent7.foodAffectHighFailure = 15;
        testevent7.foodAffectHighSuccess = 15;

        testevent7.structureAffectLowFailure = 1;
        testevent7.structureAffectLowSuccess = 1;

        testevent7.energyAffectHighFailure = 10;
        testevent7.energyAffectHighSuccess = 10;
        testevent7.energyAffectMedFailure = 10;
        testevent7.energyAffectMedSuccess = 10;
        testevent7.energyAffectLowFailure = 10;
        testevent7.energyAffectLowSuccess = 10;

        events.Add(testevent7);



        // EVENT SEVEN FLYING RATS

        Event testevent15 = new Event(this);
        testevent15.name = "Flying Rats";
        testevent15.description = "Nibbles in our bread, squeaks underneath the boards, droppings along the door frames… and worst of all, they can fly. Send someone down there to get rid of the soaring pests!";
        testevent15.image = Resources.Load<Sprite>("eventIcons/rats");

        testevent15.mainaffects[0] = "energy";
        testevent15.mainaffects[1] = "food";

        testevent15.lowriskoption = "Get some sailors to throw some honeyed bread overboard, the rats won't be able to resist.";
        testevent15.lowIndicator = "Success: Food- Skill+ \nFailure: Food--";

        testevent15.mediumriskoption = "Have them bring Luna, the ship's cat, below deck. She'll make quick work of them";
        testevent15.medIndicator = "Success: Food+ Loyalty+ \nFailure: Energy--";

        testevent15.highriskoption = "Bring Fire to them! We'll have rat kebabs tonight, boys!";
        testevent15.highIndicator = "Success: Food+++ Skill++ \nFailure: Energy-- Food--";


        testevent15.lowrisksuccess = "SUCCESS: The gluttonous little creatures couldn't resist it, and soared overboard after the bread.";
        testevent15.lowriskfailure = "FAILURE: The rats stare at you, wondering why you would throw such perfectly fine bread overboard. They leave the ship, but only after having their fill.";

        testevent15.medrisksuccess = "SUCCESS: Luna couldn't be purring louder. Her belly is full, and now our bellies will be too. Good kitten.";
        testevent15.medriskfailure = "FAILURE: LUNA DOES NOT LIKE BEING PICKED UP. Luckily, the screams of crew members had the rats running.";

        testevent15.highrisksuccess = "SUCCESS: Whipping out the flamethrower, you expertly roast them while avoiding the food. RatKebabs!";
        testevent15.highriskfailure = "FAILURE: We forgot to account for the fact that the food (and the sailors) are also flammable... ";

        testevent15.lowDificulty = 45f;
        testevent15.medDifficulty = 60f;
        testevent15.highDifficulty = 75f;

        testevent15.foodAffectLowSuccess = -4;
        testevent15.skillAffectLowSuccess = 10;

        testevent15.foodAffectLowFailure = -10;

        testevent15.foodAffectMedSuccess = 8;
        testevent15.loyaltyAffectMedSuccess = 8;

        testevent15.energyAffectMedFailure = -18;

        testevent15.foodAffectHighSuccess = 35;
        testevent15.skillAffectHighSuccess = 20;

        testevent15.energyAffectHighFailure = -18;
        testevent15.foodAffectHighFailure = -18;

        events.Add(testevent15);

        // EVENT NINE Siren's Call

        Event testevent8 = new Event(this);
        testevent8.name = "Siren's Call";
        testevent8.description = "Through the clouds, you hear voices as sweet as honey. A song echoes around your ship, beckoning you and your crew above deck. You can faintly see silouhettes resting elegantly along some nearby floating rocks.";
        testevent8.image = Resources.Load<Sprite>("eventIcons/siren");

        testevent8.mainaffects[0] = "Loyalty";
        testevent8.mainaffects[1] = "Structure";

        testevent8.lowriskoption = "Lock some companions below deck. Put them in horny jail NOW!";
        testevent8.lowIndicator = "Success: Loyalty- Energy+ Skill+ \n Failure: Energy-";

        testevent8.mediumriskoption = "Let some lucky companions hear the song... but bring the ship no closer";
        testevent8.medIndicator = "Success: Loyalty++ \nFailure: Loyalty-- Fuel-";

        testevent8.highriskoption = "Who are we to say they're evil? Get some companions to park the ship near the rocks, and invite them onboard!";
        testevent8.highIndicator = "Success: AllLoyalty+++ Energy+ \nFailure: Structure-";


        testevent8.lowrisksuccess = "SUCCESS: They're pissy about it, but they came to their senses rather quickly.";
        testevent8.lowriskfailure = "FAILURE: They resisted, and a small brawl broke out. At least it distracted them from the Sirens.";

        testevent8.medrisksuccess = "SUCCESS: They stood there, eyes closed, soaking the beautiful song in. Seems as if it was harmless after all.";
        testevent8.medriskfailure = "FAILURE: One of your companions tried 'casually' leaning against the wheel in their direction. Put us off course by quite a bit.";

        testevent8.highrisksuccess = "SUCCESS: Turns out Sirens aren't evil at all! They're actually great company!";
        testevent8.highriskfailure = "FAILURE: The Sirens were actualy quite friendly, but in eager recklesness, we steered the ship too damn close to the rocks. Shame.";

        testevent8.lowDificulty = 45f;
        testevent8.medDifficulty = 60f;
        testevent8.highDifficulty = 75f;

        testevent8.loyaltyAffectLowSuccess = -7;
        testevent8.energyAffectLowSuccess = 20;
        testevent8.skillAffectLowSuccess = 10;

        testevent8.energyAffectLowFailure = -7;

        testevent8.loyaltyAffectMedSuccess = 20;
        testevent8.loyaltyAffectMedFailure = -20;

        testevent8.fuelAffectMedFailure = -8;

        testevent8.allLoyaltyAffectHighSuccess = 35;
        testevent8.energyAffectHighSuccess = 8;

        testevent8.structureAffectHighFailure = -1;

        events.Add(testevent8);


        //EVENT TEN FLOATING CARNIVAL
        Event testevent9 = new Event(this);
        testevent9.name = "Floating Carnival";
        testevent9.description = "Walking under the bright canopies and lanterns, alongside the vendors holding out skewers and sweets, you and your crew weave through the floating carnival. At the center of it all is a large food donation, intended to be distributed to poor families around town.";
        testevent9.image = Resources.Load<Sprite>("eventIcons/floating_fair");

        testevent9.mainaffects[0] = "Loyalty";
        testevent9.mainaffects[1] = "Food";

        testevent9.lowriskoption = "Have a few companions bring over some food barrels. They need it more than we do (and it'll make you seem very generous).";
        testevent9.lowIndicator = "Success: AllLoyalty+ Loyalty++ Food-- \n Failure: Food-";

        testevent9.mediumriskoption = "Have some companions steal some food. You're making an impossible journey, and you need every edge you can get.";
        testevent9.medIndicator = "Success: Food++ Loyalty--\nFailure: Energy-- Loyalty-";

        testevent9.highriskoption = "Enter some companions into the pie eating contest! Winner gets free pies!";
        testevent9.highIndicator = "Success: Energy+ Skill+++ Food++ \nFailure: Energy-- Loyalty--";


        testevent9.lowrisksuccess = "SUCCESS: Your companions looks at you with a sense of respect and admiration. You are such a good person.";
        testevent9.lowriskfailure = "FAILURE: A particularly jaded companion made sure to only donate a near-rotting barrel of saltfish, refusing to 'waste' perfectly good food.";

        testevent9.medrisksuccess = "SUCCESS: You made out like a bandit. To hell with the bad karma, your ship must get to the temple at any cost.";
        testevent9.medriskfailure = "FAILURE: Your crew was caught. The local townsfolk were less than pleased. You all book it out of the carnival with some bruises.";

        testevent9.highrisksuccess = "SUCCESS: Your companions sucked those pies down with unparalleled gluttonous fury! That's what we pay them for!";
        testevent9.highriskfailure = "FAILURE: Not only did your companions lose, but now they vomit at the near sight of a pie. And they hate you for it.";

        testevent9.lowDificulty = 45f;
        testevent9.medDifficulty = 60f;
        testevent9.highDifficulty = 75f;

        testevent9.loyaltyAffectLowSuccess = 22;
        testevent9.allLoyaltyAffectLowSuccess = 6;
        testevent9.foodAffectLowSuccess = -18;

        testevent9.foodAffectLowFailure = -8;

        testevent9.foodAffectMedSuccess = 22;
        testevent9.loyaltyAffectMedSuccess = -20;

        testevent9.energyAffectMedFailure = -20;
        testevent9.loyaltyAffectMedFailure = -7;

        testevent9.energyAffectHighSuccess = 8;
        testevent9.skillAffectHighSuccess = 25;
        testevent9.foodAffectHighSuccess = 20;

        testevent9.energyAffectHighFailure = -22;
        testevent9.loyaltyAffectHighFailure = -22;


        events.Add(testevent9);

        //EVENT ELEVEN

        Event testevent10 = new Event(this);
        testevent10.name = "The Great Storm";

        testevent10.description = "What would a treacherous journey be without a little wind, eh? Or maybe, a lot of wind? Or maybe, a lot of wind and rain and lightning and peril?";
        testevent10.image = Resources.Load<Sprite>("eventIcons/broken_hull");

        testevent10.mainaffects[0] = "Fuel";
        testevent10.mainaffects[1] = "Energy";

        testevent10.lowriskoption = "Have your companions bring the sails down. Only fools test the weather, and only bigger fools test fate-twisted weather";
        testevent10.lowIndicator = "Success: Fuel+ \nFailure: Food-";

        testevent10.mediumriskoption = "Have them steer us towards the eye of the storm, we've no other options!";
        testevent10.medIndicator = "Success: Fuel++ Skill+ Loyalty+\nFailure: Fuel--";

        testevent10.highriskoption = "Get your companions above deck with metal wire, the lightning will power us for days!";
        testevent10.highIndicator = "Success: Fuel+++ Skill++ \nFailure: Structure- Energy- Fuel+";


        testevent10.lowrisksuccess = "SUCCESS: The rain waterlogged some of our food, but letting the storm carry us saved some fuel. A good trade, captain!";
        testevent10.lowriskfailure = "FAILURE: The heavy rain waterlogged some of our food, and it was a slow exit out of the storm. At least we're alive, though.";

        testevent10.medrisksuccess = "SUCCESS: The center of the storm is beautiful, but more importantly, it's safe. The storm only managed to bring you closer to your next checkpoint!";
        testevent10.medriskfailure = "FAILURE: Turns out there is no 'eye of the storm'. Turns out the center of the storm is, in fact, just as bad as the rest of it.";

        testevent10.highrisksuccess = "SUCCESS: Power courses through the ship! She's never sounded happier! Onward and upwards, captain!";
        testevent10.highriskfailure = "FAILURE: Drawing lightning to your ship is, perhaps, the very last thing a captain should aim to do. Your ship and crew are hurt, but some fuel was still gained, at least.";

        testevent10.lowDificulty = 45f;
        testevent10.medDifficulty = 60f;
        testevent10.highDifficulty = 75f;

        testevent10.fuelAffectLowSuccess = 12;
        testevent10.foodAffectLowFailure = -8;

        testevent10.fuelAffectMedSuccess = 24;
        testevent10.skillAffectMedSuccess = 10;
        testevent10.loyaltyAffectMedSuccess = 8;
        
        testevent10.fuelAffectMedFailure = -18;

        testevent10.fuelAffectHighSuccess = 40;
        testevent10.skillAffectHighSuccess = 25;

        testevent10.structureAffectHighFailure = -1;
        testevent10.energyAffectHighFailure = -10;
        testevent10.fuelAffectHighFailure = 12;
        


        events.Add(testevent10);

        Event testevent11 = new Event(this);
        testevent11.name = "Horrors Beyond Comprehension";

        testevent11.description = "Gods... what is that thing??? The crew can't look away. People have already started crying. Captain, do something!";
        testevent11.image = Resources.Load<Sprite>("eventIcons/horrors_beyond_comprehension");


        testevent11.mainaffects[0] = "Fuel";
        testevent11.mainaffects[1] = "Energy";

        testevent11.lowriskoption = "Get your companions to send everyone below deck, NOW!";
        testevent11.lowIndicator = "Success: None \nFailure: Energy-";

        testevent11.mediumriskoption = "Have your companions shoot it. Shoot it with guns.";
        testevent11.medIndicator = "Success: Skill++ Food++\nFailure: Loyalty-- Energy-";

        testevent11.highriskoption = "Have them look at it. Interpret it. Do not let anyone look away.";
        testevent11.highIndicator = "Success: Skill+++ Loyalty+++ AllEnergy+ \nFailure: Loyalty--- Energy--";


        testevent11.lowrisksuccess = "SUCCESS: That... thing eventually left. Hopefully everyone can just forget about it now.";
        testevent11.lowriskfailure = "FAILURE: Someone made the faintest eye contact with... 'it' on their way down. Things didn't go so pretty after that.";

        testevent11.medrisksuccess = "SUCCESS: Turns out even unfathomable horrors die to bullets. Now we have extra meat for the stew.";
        testevent11.medriskfailure = "FAILURE: You thought bullets would best that thing? You thought wrong. Before it left, it gave everyone a splitting headache.";

        testevent11.highrisksuccess = "SUCCESS: All along, it was only trying to show you. Thank you, creature.";
        testevent11.highriskfailure = "FAILURE: THEY WERE NOT READY.";

        testevent11.lowDificulty = 45f;
        testevent11.medDifficulty = 60f;
        testevent11.highDifficulty = 75f;

        testevent11.energyAffectLowFailure = -8;

        testevent11.skillAffectMedSuccess = 20;
        testevent11.foodAffectMedSuccess = 20;
        testevent11.energyAffectMedFailure = -8;
        testevent11.loyaltyAffectMedFailure = -17;

        testevent11.skillAffectHighSuccess = 26;
        testevent11.loyaltyAffectHighSuccess = 30;
        testevent11.allEnergyAffectHighSuccess = 10;
        testevent11.loyaltyAffectHighFailure = -28;
        testevent11.energyAffectHighFailure = -16;

        events.Add(testevent11);


    }



}


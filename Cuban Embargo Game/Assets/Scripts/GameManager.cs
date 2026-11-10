
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Menu,
        Help,
        Credits,
        Playing,
        EventChoice,
        GameOver,
        Victory
    }

    [Header("Game Systems")]
    [SerializeField] private ResourceManager resources;
    [SerializeField] private EventManager events;
    [SerializeField] private UIManager ui;

    private GameState state = GameState.Menu;

    private int level;
    private int day;

    private const int DaysPerLevel = 7;
    private const float EventChance = 0.45f;

    void Start()
    {
        if (resources == null)
            resources = GetComponent<ResourceManager>();

        if (events == null)
            events = GetComponent<EventManager>();

        if (ui == null)
            ui = GetComponent<UIManager>();

        if (resources == null || events == null || ui == null)
        {
            Debug.LogError(
                "GameManager is missing one or more systems!");
            enabled = false;
            return;
        }

        if (!ui.Initialize(this))
        {
            Debug.LogError(
                "GameManager could not initialize the UI.");
            enabled = false;
            return;
        }

        ShowMenu();
    }

    // ======================================
    // MENU / NAVIGATION
    // ======================================

    void ShowMenu()
    {
        state = GameState.Menu;
        ui.ShowMenu();
    }

    void ShowHelp()
    {
        state = GameState.Help;
        ui.ShowHelp();
    }

    void ShowCredits()
    {
        state = GameState.Credits;
        ui.ShowCredits();
    }

    // ======================================
    // BUTTON INPUT
    // ======================================

    public void OnActionButton(int buttonNumber)
    {
        if (state == GameState.Menu)
        {
            switch (buttonNumber)
            {
                case 1:
                    StartNewGame();
                    break;

                case 2:
                    ShowHelp();
                    break;

                case 3:
                    ShowCredits();
                    break;
            }

            return;
        }

        if (state == GameState.Playing)
        {
            TakeAction(buttonNumber);
        }
    }

    public void OnChoiceButton(int choice)
    {
        if (state == GameState.Help ||
            state == GameState.Credits)
        {
            if (choice == 1)
                ShowMenu();

            return;
        }

        if (state == GameState.GameOver ||
            state == GameState.Victory)
        {
            if (choice == 1)
                StartNewGame();
            else if (choice == 2)
                ShowMenu();

            return;
        }

        if (state == GameState.EventChoice)
        {
            ResolveEncounter(choice);
        }
    }

    // ======================================
    // GAME START / DAILY ACTIONS
    // ======================================

    void StartNewGame()
    {
        level = 1;
        day = 1;

        resources.ResetResources();

        Debug.Log("Starting Level 1: Havana");

        ShowDay();
    }

    void ShowDay()
    {
        state = GameState.Playing;

        ui.ShowDay(resources, level, day);

        Debug.Log("Level " + level + " - Day " + day);
    }

    void TakeAction(int action)
    {
        switch (action)
        {
            case 1:
                if (!resources.SpendMoney(12))
                {
                    Debug.Log("Not enough money for food.");
                    return;
                }

                resources.AddFood(4);
                Debug.Log("Purchased 4 food.");
                break;

            case 2:
                if (!resources.SpendMoney(10))
                {
                    Debug.Log("Not enough money for fuel.");
                    return;
                }

                resources.AddFuel(2);
                Debug.Log("Purchased 2 fuel.");
                break;

            case 3:
                if (!resources.SpendMoney(15))
                {
                    Debug.Log("Not enough money for medicine.");
                    return;
                }

                resources.AddMedicine(2);
                Debug.Log("Purchased 2 medicine.");
                break;

            case 4:
                resources.AddMoney(18);
                resources.ChangeHealth(-5);

                Debug.Log("Worked and earned $18.");
                break;

            case 5:
                resources.ChangeHealth(12);

                Debug.Log("Rested and recovered health.");
                break;

            case 6:
                if (Random.value < 0.5f)
                {
                    resources.AddFood(3);
                    Debug.Log("Neighbors provided food.");
                }
                else
                {
                    Debug.Log("No assistance available.");
                }
                break;

            case 7:
                if (level != 2 ||
                    !resources.UseMedicine(1))
                {
                    Debug.Log("Unable to treat patients.");
                    return;
                }

                resources.ChangeClinic(12);

                Debug.Log("Patients treated.");
                break;

            default:
                return;
        }

        resources.ApplyDailyCosts(level, day);

        if (CheckGameOver())
            return;

        // Chance of an encounter after daily activity
        if (Random.value < EventChance)
        {
            BeginEncounter();
        }
        else
        {
            FinishDay();
        }
    }

    // ======================================
    // RANDOM ENCOUNTERS
    // ======================================

    void BeginEncounter()
    {
        state = GameState.EventChoice;

        events.GenerateEvent(level);

        ui.ShowEncounter(events, resources, level, day);

        Debug.Log(
            "Encounter: " + events.GetEventDescription());
    }

    void ResolveEncounter(int choice)
    {
        events.ResolveEvent(choice, resources);

        FinishDay();
    }

    // ======================================
    // DAY / LEVEL PROGRESSION
    // ======================================

    bool CheckGameOver()
    {
        if (resources.Health <= 0)
        {
            GameOver("Your family's health reached zero.");
            return true;
        }

        if (level == 2 && resources.ClinicCondition <= 0)
        {
            GameOver("The clinic could no longer operate.");
            return true;
        }

        return false;
    }

    void FinishDay()
    {
        if (CheckGameOver())
            return;

        Debug.Log("End of Day " + day);

        day++;

        if (day > DaysPerLevel)
        {
            if (level == 1)
                StartLevelTwo();
            else
                WinGame();

            return;
        }

        ShowDay();
    }

    void StartLevelTwo()
    {
        level = 2;
        day = 1;

        resources.AddMoney(20);
        resources.AddMedicine(2);

        Debug.Log("Level 2: Rural Community Clinic");

        ShowDay();
    }

    // ======================================
    // ENDGAME
    // ======================================

    void GameOver(string reason)
    {
        state = GameState.GameOver;

        ui.ShowGameOver(reason, level, day);

        Debug.Log("Game Over: " + reason);
    }

    void WinGame()
    {
        state = GameState.Victory;

        ui.ShowVictory(resources);

        Debug.Log("Victory! Both levels completed.");
    }
}


using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Menu,
        Intro,
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

    [Header("Introduction")]
    [SerializeField] private IntroTransition introTransition;

    private GameState state = GameState.Menu;

    private int level;
    private int day;

    private const int DaysPerLevel = 7;
    private const float EventChance = 0.45f;

    private string pendingActionDescription;

    // Stores resource values before each daily action.
    private struct ResourceSnapshot
    {
        public int money;
        public int food;
        public int fuel;
        public int medicine;
        public int health;
        public int clinic;
    }

    private ResourceSnapshot dayStartResources;

    // ======================================
    // INITIALIZATION
    // ======================================

    void Start()
    {
        if (resources == null)
            resources = GetComponent<ResourceManager>();

        if (events == null)
            events = GetComponent<EventManager>();

        if (ui == null)
            ui = GetComponent<UIManager>();

        if (introTransition == null)
            introTransition = GetComponent<IntroTransition>();

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

        Time.timeScale = 1f;
    }

    // ======================================
    // MENU / NAVIGATION
    // ======================================

    void ShowMenu()
    {
        state = GameState.Menu;
        ui.ShowMenu();
    }

    public void ShowHelp()
    {
        state = GameState.Help;
        ui.ShowHelp();
    }

    public void ShowCredits()
    {
        state = GameState.Credits;
        ui.ShowCredits();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ======================================
    // INTRODUCTION SEQUENCE
    // ======================================

    public void StartIntro()
    {
        ui.ToggleMenuButtons(false);

        // Prevent gameplay inputs during the introduction.
        state = GameState.Intro;

        Debug.Log("Starting cinematic introduction...");

        if (introTransition != null)
        {
            // Play the introduction, then start the game.
            introTransition.PlayIntro(StartNewGame);
        }
        else
        {
            // If no transition exists, still allow gameplay.
            Debug.LogWarning(
                "IntroTransition not found. " +
                "Starting gameplay directly.");

            StartNewGame();
        }
    }

    // ======================================
    // BUTTON INPUT
    // ======================================

    public void OnActionButton(int buttonNumber)
    {
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
    // GAME START
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

    // ======================================
    // RESOURCE SNAPSHOTS / FEEDBACK
    // ======================================

    ResourceSnapshot CaptureResources()
    {
        return new ResourceSnapshot
        {
            money = resources.Money,
            food = resources.Food,
            fuel = resources.Fuel,
            medicine = resources.Medicine,
            health = resources.Health,
            clinic = resources.ClinicCondition
        };
    }

    static void AddChange(
        List<string> changes,
        string label,
        int amount,
        bool currency = false)
    {
        if (amount == 0)
            return;

        string sign = amount > 0 ? "+" : "-";
        string prefix = currency ? "$" : "";

        changes.Add(
            label + " " + sign +
            prefix + Mathf.Abs(amount));
    }

    string GetNetResourceChanges()
    {
        List<string> changes = new List<string>();

        AddChange(
            changes,
            "Money",
            resources.Money - dayStartResources.money,
            true);

        AddChange(
            changes,
            "Food",
            resources.Food - dayStartResources.food);

        AddChange(
            changes,
            "Fuel",
            resources.Fuel - dayStartResources.fuel);

        AddChange(
            changes,
            "Medicine",
            resources.Medicine - dayStartResources.medicine);

        AddChange(
            changes,
            "Health",
            resources.Health - dayStartResources.health);

        if (level == 2)
        {
            AddChange(
                changes,
                "Clinic",
                resources.ClinicCondition -
                dayStartResources.clinic);
        }

        if (changes.Count == 0)
            return "No net resource changes.";

        return "Net changes: " +
               string.Join(" | ", changes);
    }

    // ======================================
    // DAILY ACTIONS
    // ======================================

    void TakeAction(int action)
    {
        // Save resources before the daily action.
        dayStartResources = CaptureResources();

        switch (action)
        {
            case 1:
                if (!resources.SpendMoney(12))
                {
                    ui.ShowFeedback(
                        "Not enough money to buy food.");
                    return;
                }

                resources.AddFood(4);

                pendingActionDescription =
                    "Purchased 4 food for $12.";

                Debug.Log("Purchased 4 food.");
                break;

            case 2:
                if (!resources.SpendMoney(10))
                {
                    ui.ShowFeedback(
                        "Not enough money to buy fuel.");
                    return;
                }

                resources.AddFuel(2);

                pendingActionDescription =
                    "Purchased 2 fuel for $10.";

                Debug.Log("Purchased 2 fuel.");
                break;

            case 3:
                if (!resources.SpendMoney(15))
                {
                    ui.ShowFeedback(
                        "Not enough money to buy medicine.");
                    return;
                }

                resources.AddMedicine(2);

                pendingActionDescription =
                    "Purchased 2 medicine for $15.";

                Debug.Log("Purchased 2 medicine.");
                break;

            case 4:
                resources.AddMoney(18);
                resources.ChangeHealth(-5);

                pendingActionDescription =
                    "Worked a long shift and earned $18.";

                Debug.Log("Worked and earned $18.");
                break;

            case 5:
                resources.ChangeHealth(12);

                pendingActionDescription =
                    "Rested to recover your family's health.";

                Debug.Log("Rested and recovered health.");
                break;

            case 6:
                if (Random.value < 0.5f)
                {
                    resources.AddFood(3);

                    pendingActionDescription =
                        "Neighbors shared 3 food with your family.";

                    Debug.Log("Neighbors provided food.");
                }
                else
                {
                    pendingActionDescription =
                        "No community assistance was available.";

                    Debug.Log("No assistance available.");
                }

                break;

            case 7:
                if (level != 2 ||
                    !resources.UseMedicine(1))
                {
                    ui.ShowFeedback(
                        "Unable to treat patients. " +
                        "Medicine may be unavailable.");
                    return;
                }

                resources.ChangeClinic(12);

                pendingActionDescription =
                    "Treated patients at the community clinic.";

                Debug.Log("Patients treated.");
                break;

            default:
                return;
        }

        // Apply daily resource consumption.
        resources.ApplyDailyCosts(level, day);

        if (CheckGameOver())
            return;

        // Determine whether an encounter occurs.
        if (Random.value < EventChance)
        {
            BeginEncounter();
        }
        else
        {
            FinishDay(pendingActionDescription);
        }
    }

    // ======================================
    // RANDOM ENCOUNTERS
    // ======================================

    void BeginEncounter()
    {
        state = GameState.EventChoice;

        events.GenerateEvent(level);

        ui.ShowEncounter(
            events,
            resources,
            level,
            day);

        Debug.Log(
            "Encounter: " +
            events.GetEventDescription());
    }

    void ResolveEncounter(int choice)
    {
        string eventName =
            events.GetEventDescription().Split('\n')[0];

        string selectedChoice =
            events.GetChoiceText(choice);

        events.ResolveEvent(choice, resources);

        string feedback =
            eventName + ": " + selectedChoice;

        FinishDay(feedback);
    }

    // ======================================
    // DAY / LEVEL PROGRESSION
    // ======================================

    bool CheckGameOver()
    {
        if (resources.Health <= 0)
        {
            GameOver(
                "Your family's health reached zero.");

            return true;
        }

        if (level == 2 &&
            resources.ClinicCondition <= 0)
        {
            GameOver(
                "The clinic could no longer operate.");

            return true;
        }

        return false;
    }

    void FinishDay(string feedbackDescription)
    {
        if (CheckGameOver())
            return;

        string dailyFeedback =
            feedbackDescription + "\n" +
            GetNetResourceChanges();

        Debug.Log("End of Day " + day);

        day++;

        if (day > DaysPerLevel)
        {
            if (level == 1)
            {
                StartLevelTwo();
            }
            else
            {
                WinGame();
            }

            return;
        }

        ShowDay();

        // Display the previous day's results.
        ui.ShowFeedback(dailyFeedback);
    }

    void StartLevelTwo()
    {
        level = 2;
        day = 1;

        // Carry over resources and provide some
        // additional supplies for the clinic.
        resources.AddMoney(20);
        resources.AddMedicine(2);

        Debug.Log(
            "Level 2: Rural Community Clinic");

        ShowDay();



        /*
        ui.ShowFeedback(
            "Level 1 complete! You have reached " +
            "the rural clinic. Your remaining resources " +
            "have carried over.");
        */

        ui.LevelTwoIntroPanel();
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

        Debug.Log(
            "Victory! Both levels completed.");
    }
}

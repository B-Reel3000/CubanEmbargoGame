
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Progress Text")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text dayText;

    [Header("Resource Text")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text medicineText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text clinicText;

    [Header("Action Buttons")]
    [SerializeField] private Button buyFoodButton;
    [SerializeField] private Button buyFuelButton;
    [SerializeField] private Button buyMedicineButton;
    [SerializeField] private Button workButton;
    [SerializeField] private Button restButton;
    [SerializeField] private Button assistanceButton;
    [SerializeField] private Button treatPatientsButton;

    [Header("Event UI")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text eventDescriptionText;
    [SerializeField] private TMP_Text menuPopupText;
    [SerializeField] private Button choice1Button;
    [SerializeField] private Button choice2Button;
    [SerializeField] private Button choice3Button;

    [Header("Screen Backgrounds")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Image topHUDBackground;
    [SerializeField] private Image bottomActionBackground;
    [SerializeField] private GameObject playButton, helpButton, creditButton;

    [Header("Player Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMP_Text feedbackText;

    private Button[] actionButtons;
    private GameObject[] menuButtons;
    private GameManager gameManager;

    public bool Initialize(GameManager manager)
    {
        gameManager = manager;

        actionButtons = new Button[]
        {
            buyFoodButton,
            buyFuelButton,
            buyMedicineButton,
            workButton,
            restButton,
            assistanceButton,
            treatPatientsButton
        };

        menuButtons = new GameObject[]
        {
            playButton,
            helpButton,
            creditButton
        };

        if (!ValidateReferences())
            return false;

        for (int i = 0; i < actionButtons.Length; i++)
        {
            int buttonNumber = i + 1;

            actionButtons[i].onClick.AddListener(
                () => gameManager.OnActionButton(buttonNumber));
        }

        choice1Button.onClick.AddListener(
            () => gameManager.OnChoiceButton(1));

        choice2Button.onClick.AddListener(
            () => gameManager.OnChoiceButton(2));

        choice3Button.onClick.AddListener(
            () => gameManager.OnChoiceButton(3));

        return true;
    }

    bool ValidateReferences()
    {
        if (gameManager == null ||
            levelText == null || dayText == null ||
            moneyText == null || foodText == null ||
            fuelText == null || medicineText == null ||
            healthText == null || clinicText == null ||
            eventPanel == null ||
            eventDescriptionText == null ||
            choice1Button == null ||
            choice2Button == null ||
            choice3Button == null ||
            menuPanel == null ||
            topHUDBackground == null ||
            bottomActionBackground == null ||
            feedbackPanel == null ||
            feedbackText == null)
        {
            Debug.LogError(
                "UIManager has missing Inspector references!");
            return false;
        }

        foreach (Button button in actionButtons)
        {
            if (button == null)
            {
                Debug.LogError(
                    "UIManager is missing an action button!");
                return false;
            }
        }

        return true;
    }

    // ======================================
    // UI HELPERS
    // ======================================

    void SetGameplayAppearance(bool gameplay)
    {
        menuPanel.SetActive(!gameplay);
        topHUDBackground.enabled = gameplay;
        bottomActionBackground.enabled = gameplay;
    }

    void SetButton(Button button, bool visible, string label)
    {
        button.gameObject.SetActive(visible);

        if (!visible)
            return;

        button.interactable = true;

        TMP_Text buttonText =
            button.GetComponentInChildren<TMP_Text>(true);

        if (buttonText != null)
            buttonText.text = label;
    }

    void HideActionButtons()
    {
        foreach (Button button in actionButtons)
            button.gameObject.SetActive(false);
    }

    void HideResources()
    {
        moneyText.gameObject.SetActive(false);
        foodText.gameObject.SetActive(false);
        fuelText.gameObject.SetActive(false);
        medicineText.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
        clinicText.gameObject.SetActive(false);
    }

    public void ToggleMenuButtons(bool state)
    {
        foreach (GameObject button in menuButtons) { button.SetActive(state); }
    }

    // ======================================
    // PLAYER FEEDBACK
    // ======================================

    public void ShowFeedback(string message)
    {
        feedbackText.text = message;
        feedbackPanel.SetActive(true);

        Debug.Log("PLAYER FEEDBACK: " + message);
    }

    public void HideFeedback()
    {
        feedbackPanel.SetActive(false);
    }

    // ======================================
    // RESOURCE DISPLAY
    // ======================================

    public void UpdateResources(
        ResourceManager resources, int level, int day)
    {
        moneyText.gameObject.SetActive(true);
        foodText.gameObject.SetActive(true);
        fuelText.gameObject.SetActive(true);
        medicineText.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
        clinicText.gameObject.SetActive(level == 2);

        levelText.text = level == 1
            ? "Level 1: Havana"
            : "Level 2: Rural Clinic";

        dayText.text = "Day " + day + " / 7";

        moneyText.text = "Money: $" + resources.Money;
        foodText.text = "Food: " + resources.Food;
        fuelText.text = "Fuel: " + resources.Fuel;
        medicineText.text =
            "Medicine: " + resources.Medicine;
        healthText.text = "Health: " + resources.Health;

        clinicText.text =
            "Clinic: " + resources.ClinicCondition;
    }

    // ======================================
    // MENU
    // ======================================

    public void ShowMenu()
    {
        SetGameplayAppearance(false);
        HideFeedback();
        eventPanel.SetActive(false);

        HideResources();
        HideActionButtons();

        levelText.text = "";
        dayText.text = "";

        foreach(GameObject button in menuButtons) { button.SetActive(true); }
    }

    public void ShowHelp()
    {
        SetGameplayAppearance(false);
        HideFeedback();
        HideActionButtons();
        HideResources();

        levelText.text = "";
        dayText.text = "";

        popupPanel.SetActive(true);

        menuPopupText.text =
            "HOW TO PLAY\n\n" +
            "Survive 7 days in each of two levels.\n\n" +
            "Manage money, food, fuel, medicine and health.\n\n" +
            "Choose one daily action using the buttons.\n\n" +
            "Random encounters require decisions.\n\n" +
            "In Level 2, maintain the community clinic.\n\n" +
            "Keep health above zero to survive.";

        foreach (GameObject button in menuButtons) { button.SetActive(false); }
    }

    public void ShowCredits()
    {
        SetGameplayAppearance(false);
        HideFeedback();
        HideActionButtons();
        HideResources();

        levelText.text = "";
        dayText.text = "";

        popupPanel.SetActive(true);

        menuPopupText.text =
            "GD 135 - Serious Games Project\n\n" +
            "Cuban Embargo: Journey of Survival\n\n" +
            "Developed by:\n" +
            "Ian Woodard\n" +
            "Braeden Allaman\n" +
            "Jane Puryear\n" +
            "Tyler A Prince\n\n" +
            "Educational Resource Management Game";

        foreach (GameObject button in menuButtons) { button.SetActive(false); }
    }

    // ======================================
    // GAMEPLAY
    // ======================================

    public void ShowDay(
        ResourceManager resources, int level, int day)
    {
        SetGameplayAppearance(true);
        HideFeedback();
        eventPanel.SetActive(false);

        UpdateResources(resources, level, day);

        SetButton(actionButtons[0], true, "Buy Food\n($12)");
        SetButton(actionButtons[1], true, "Buy Fuel\n($10)");
        SetButton(actionButtons[2], true, "Buy Medicine\n($15)");
        SetButton(actionButtons[3], true, "Work\n(+$18)");
        SetButton(actionButtons[4], true, "Rest\n(+12 Health)");
        SetButton(actionButtons[5], true, "Seek Assistance\n");

        SetButton(actionButtons[6], level == 2, "Treat Patients\n(-1 Med)");
    }

    // ======================================
    // RANDOM ENCOUNTERS
    // ======================================

    public void ShowEncounter(
        EventManager events,
        ResourceManager resources,
        int level,
        int day)
    {
        SetGameplayAppearance(true);
        HideFeedback();
        HideActionButtons();

        UpdateResources(resources, level, day);

        eventPanel.SetActive(true);

        eventDescriptionText.text =
            events.GetEventDescription();

        SetButton(
            choice1Button, true, events.GetChoiceText(1));

        SetButton(
            choice2Button, true, events.GetChoiceText(2));

        SetButton(
            choice3Button, true, events.GetChoiceText(3));
    }

    // ======================================
    // GAME OVER / VICTORY
    // ======================================

    public void ShowGameOver(
        string reason, int level, int day)
    {
        SetGameplayAppearance(false);
        HideFeedback();
        HideActionButtons();
        HideResources();
        
        //levelText.text = "GAME OVER";
        dayText.text = "Level " + level + " - Day " + day;

        eventPanel.SetActive(true);

        eventDescriptionText.text =
            "GAME OVER\n\n" + reason +
            "\n\nYour journey has ended.";

        SetButton(choice1Button, true, "Restart");
        SetButton(choice2Button, true, "Main Menu");
        SetButton(choice3Button, false, "");
    }

    public void ShowVictory(ResourceManager resources)
    {
        SetGameplayAppearance(false);
        HideFeedback();
        HideActionButtons();
        HideResources();
        /*
        levelText.text = "VICTORY!";
        dayText.text = "Both Levels Complete";
        */
        eventPanel.SetActive(true);

        eventDescriptionText.text =
            "YOU SURVIVED!\n\n" +
            "You completed both levels.\n\n" +
            "Final Health: " + resources.Health + "\n" +
            "Remaining Food: " + resources.Food + "\n" +
            "Remaining Money: $" + resources.Money + "\n" +
            "Clinic Condition: " +
            resources.ClinicCondition;

        SetButton(choice1Button, true, "Play Again");
        SetButton(choice2Button, true, "Main Menu");
        SetButton(choice3Button, false, "");
    }
}

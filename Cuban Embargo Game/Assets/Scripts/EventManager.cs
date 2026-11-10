
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private int currentEvent;

    public void GenerateEvent(int level)
    {
        currentEvent = level == 1
            ? Random.Range(0, 2)
            : Random.Range(2, 4);
    }

    public string GetEventDescription()
    {
        switch (currentEvent)
        {
            case 0:
                return
                    "FOOD SHIPMENT DELAYED\n\n" +
                    "A local vendor has limited food " +
                    "available at an increased price.";

            case 1:
                return
                    "NEIGHBORHOOD BLACKOUT\n\n" +
                    "An electrical outage has " +
                    "affected your neighborhood.";

            case 2:
                return
                    "MEDICAL SUPPLY SHORTAGE\n\n" +
                    "The clinic has received fewer " +
                    "medical supplies than expected.";

            case 3:
                return
                    "GENERATOR FAILURE\n\n" +
                    "The clinic generator has stopped. " +
                    "Patients need electricity.";
        }

        return "";
    }

    public string GetChoiceText(int choice)
    {
        switch (currentEvent)
        {
            case 0:
                if (choice == 1) return "Buy Food ($25)";
                if (choice == 2) return "Ration Food";
                return "Ask Neighbors";

            case 1:
                if (choice == 1) return "Use Fuel (2)";
                if (choice == 2) return "Endure Outage";
                return "Help Repairs";

            case 2:
                if (choice == 1) return "Buy Supplies ($20)";
                if (choice == 2) return "Ration Supplies";
                return "Request Help";

            case 3:
                if (choice == 1) return "Repair ($18)";
                if (choice == 2) return "Use Fuel (2)";
                return "No Power";
        }

        return "";
    }

    public void ResolveEvent(
        int choice, ResourceManager resources)
    {
        switch (currentEvent)
        {
            case 0:
                ResolveFoodEvent(choice, resources);
                break;

            case 1:
                ResolveBlackoutEvent(choice, resources);
                break;

            case 2:
                ResolveMedicineEvent(choice, resources);
                break;

            case 3:
                ResolveGeneratorEvent(choice, resources);
                break;
        }
    }

    void ResolveFoodEvent(
        int choice, ResourceManager resources)
    {
        switch (choice)
        {
            case 1:
                if (resources.SpendMoney(25))
                {
                    resources.AddFood(5);
                    Debug.Log("Purchased 5 food.");
                }
                else
                {
                    resources.ChangeHealth(-8);
                    Debug.Log("Could not afford food. -8 Health");
                }
                break;

            case 2:
                resources.ChangeHealth(-8);
                Debug.Log("Rationed food. -8 Health");
                break;

            case 3:
                if (Random.value < 0.5f)
                {
                    resources.AddFood(3);
                    Debug.Log("Neighbors shared 3 food.");
                }
                else
                {
                    resources.ChangeHealth(-5);
                    Debug.Log("No assistance. -5 Health");
                }
                break;
        }
    }

    void ResolveBlackoutEvent(
        int choice, ResourceManager resources)
    {
        switch (choice)
        {
            case 1:
                if (resources.UseFuel(2))
                {
                    Debug.Log("Backup power restored.");
                }
                else
                {
                    resources.ChangeHealth(-12);
                    Debug.Log("Not enough fuel. -12 Health");
                }
                break;

            case 2:
                resources.ChangeHealth(-12);
                Debug.Log("Endured outage. -12 Health");
                break;

            case 3:
                resources.ChangeHealth(-5);
                resources.AddMoney(5);

                Debug.Log("Helped with repairs.");
                Debug.Log("Earned $5. Lost 5 Health.");
                break;
        }
    }

    void ResolveMedicineEvent(
        int choice, ResourceManager resources)
    {
        switch (choice)
        {
            case 1:
                if (resources.SpendMoney(20))
                {
                    resources.AddMedicine(3);
                    Debug.Log("Purchased 3 medicine.");
                }
                else
                {
                    resources.ChangeClinic(-12);
                    Debug.Log("Could not afford supplies.");
                }
                break;

            case 2:
                resources.ChangeClinic(-12);
                Debug.Log("Rationed medical supplies.");
                break;

            case 3:
                if (Random.value < 0.5f)
                {
                    resources.AddMedicine(2);
                    Debug.Log("Received 2 medicine.");
                }
                else
                {
                    resources.ChangeClinic(-8);
                    Debug.Log("No supplies delivered.");
                }
                break;
        }
    }

    void ResolveGeneratorEvent(
        int choice, ResourceManager resources)
    {
        switch (choice)
        {
            case 1:
                if (resources.SpendMoney(18))
                {
                    Debug.Log("Generator repaired.");
                }
                else
                {
                    resources.ChangeClinic(-15);
                    Debug.Log("Could not afford repairs.");
                }
                break;

            case 2:
                if (resources.UseFuel(2))
                {
                    Debug.Log("Emergency power activated.");
                }
                else
                {
                    resources.ChangeClinic(-15);
                    Debug.Log("Not enough emergency fuel.");
                }
                break;

            case 3:
                resources.ChangeClinic(-15);
                Debug.Log("Clinic operated without power.");
                break;
        }
    }
}


using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int Money { get; private set; }
    public int Food { get; private set; }
    public int Fuel { get; private set; }
    public int Medicine { get; private set; }
    public int Health { get; private set; }
    public int ClinicCondition { get; private set; }

    public void ResetResources()
    {
        Money = 75;
        Food = 8;
        Fuel = 4;
        Medicine = 3;
        Health = 85;
        ClinicCondition = 60;
    }

    public bool SpendMoney(int amount)
    {
        if (Money < amount)
            return false;

        Money -= amount;
        return true;
    }

    public void AddMoney(int amount)
    {
        Money = Mathf.Max(0, Money + amount);
    }

    public void AddFood(int amount)
    {
        Food = Mathf.Max(0, Food + amount);
    }

    public void AddFuel(int amount)
    {
        Fuel = Mathf.Max(0, Fuel + amount);
    }

    public bool UseFuel(int amount)
    {
        if (Fuel < amount)
            return false;

        Fuel -= amount;
        return true;
    }

    public void AddMedicine(int amount)
    {
        Medicine = Mathf.Max(0, Medicine + amount);
    }

    public bool UseMedicine(int amount)
    {
        if (Medicine < amount)
            return false;

        Medicine -= amount;
        return true;
    }

    public void ChangeHealth(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, 100);
    }

    public void ChangeClinic(int amount)
    {
        ClinicCondition = Mathf.Clamp(
            ClinicCondition + amount, 0, 100);
    }

    public void ApplyDailyCosts(int level, int day)
    {
        // Daily family food consumption
        int remainingFood = Food - 2;

        if (remainingFood < 0)
        {
            Food = 0;
            ChangeHealth(-12);

            Debug.Log("Food shortage! -12 Health");
        }
        else
        {
            Food = remainingFood;
        }

        // Fuel consumption
        if (level == 2 || day % 2 == 0)
        {
            if (!UseFuel(1))
            {
                ChangeHealth(-7);

                Debug.Log("Fuel shortage! -7 Health");
            }
        }

        // Clinic condition deteriorates each day
        if (level == 2)
        {
            ChangeClinic(-5);
        }
    }
}


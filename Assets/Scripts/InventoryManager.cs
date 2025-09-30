using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Public fields for Money and AI Credits
    public int AiCredits;
    public float Money;
    public float HappinessMeter = 0;

    public float houseCount = 0;
    public bool isHappy = false;

    // UI Text elements for displaying money and AI credits
    public Text moneyText;
    public Text aiCreditsText;
    public Text happinessMeterText;

    private void Start()
    {
        // Initialize the UI display
        UpdateMoneyDisplay();
        UpdateAiCreditsDisplay();
        UpdateHappinessDisplay();
    }

    public void UpdateHappiness()
    {
        houseCount++;
        HappinessMeter = houseCount / 20 * 100;
        UpdateHappinessDisplay();
        if (HappinessMeter >= 50 && !isHappy)
        {
            AddAiCredits(100);
            AddMoney(1000);
            isHappy = true;
        }
        else if (HappinessMeter < 50)
        {
            isHappy = false;
        }
    }

    // Method to check if the player can afford an item based on money
    public bool CanBuy(float cost)
    {
        return Money >= cost;
    }

    public bool CanBuyAi(float cost)
    {
        return AiCredits >= cost;
    }

    // Method to purchase an item by deducting money
    public void Buy(float cost)
    {
        Money -= cost;
        UpdateMoneyDisplay();
    }

    public void ClearMoney(int percent = 100)
    {
        Money *= 1 - percent / 100;
        UpdateMoneyDisplay();
    }

    // Method to add money to the player's balance
    public void AddMoney(float money)
    {
        Money += money;
        UpdateMoneyDisplay();
    }

    // Method to add AI credits
    public void AddAiCredits(int credits)
    {
        AiCredits += credits;
        UpdateAiCreditsDisplay();
    }

    // Method to spend AI credits
    public bool SpendAiCredits(int credits)
    {
        if (AiCredits >= credits)
        {
            AiCredits -= credits;
            UpdateAiCreditsDisplay();
            return true; // Successfully spent AI credits
        }
        else
        {
            return false; // Not enough AI credits
        }
    }

    // Method to update the Money display in the UI
    private void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + Money.ToString(""); // Show 2 decimal points for money
        }
    }

    // Method to update the AI Credits display in the UI
    private void UpdateAiCreditsDisplay()
    {
        if (aiCreditsText != null)
        {
            aiCreditsText.text = AiCredits.ToString(); // Display AI credits
        }
    }

    private void UpdateHappinessDisplay()
    {
        if (happinessMeterText != null)
        {
            happinessMeterText.text = HappinessMeter.ToString() + "%";
        }
    }
}

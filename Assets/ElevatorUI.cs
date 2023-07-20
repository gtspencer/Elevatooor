using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject panelUI;
    [SerializeField] private const float startingUpgradeAreaPosition = 241.0789f;

    private Dictionary<int, Image> floorButtons = new Dictionary<int, Image>();

    private Color litColor = Color.yellow;
    private Color defaultColor = Color.white;
    
    // Start is called before the first frame update
    void Start()
    {
        EventRepository.Instance.OnElevatorSelected += OnElevatorSelected;
        EventRepository.Instance.OnElevatorUnSelected += OnElevatorUnSelected;
    }

    private void OnElevatorUnSelected()
    {
        selectedElevator.OnLitButtonsChanged -= UpdateUI;
        DestroyAllButtons();
        DestroyUpgradeUI();
        
        panelUI.SetActive(false);
        upgradeContainer.SetActive(false);
        
        selectedElevator = null;
    }

    private ElevatorV2 selectedElevator;
    private void OnElevatorSelected(ElevatorV2 elevator)
    {
        selectedElevator = elevator;
        elevator.OnLitButtonsChanged += UpdateUI;
        CreateUI();
    }

    // TODO call this when adding a floor while this is open
    private void UpdateUI()
    {
        foreach (KeyValuePair<int, Image> kv in floorButtons)
        {
            if (selectedElevator.LitButtons.Contains(kv.Key))
                kv.Value.color = litColor;
            else
                kv.Value.color = defaultColor;
        }
    }

    private void DestroyAllButtons()
    {
        foreach (Image i in floorButtons.Values)
            Destroy(i.transform.parent.gameObject);
        
        floorButtons.Clear();
    }
    
    private void DestroyUpgradeUI()
    {
        foreach (GameObject child in upgradeContainer.transform)
        {
            Destroy(child);
        }
    }

    // TODO refactor to reuse already created buttons
    public void CreateUI()
    {
        DestroyAllButtons();
        
        // Create Buttons UI
        for (int i = 1; i <= Building.Instance.Floors; i++)
        {
            var button = GameObject.Instantiate(buttonPrefab, panelUI.transform);

            var image = button.GetComponentInChildren<Image>();
            image.color = selectedElevator.LitButtons.Contains(i) ? litColor : defaultColor;
            floorButtons.Add(i, image);
            button.GetComponentInChildren<Text>().text = i.ToString();
        }
        
        // Create Upgrade UI
        upgradeContainer.SetActive(true);
        
        SetupSpeedUpgrades();

        panelUI.SetActive(true);
    }

    #region Upgrades

    [SerializeField] private GameObject upgradeAreaPrefab;
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private GameObject upgradeContainer;

    private Dictionary<int, Button> speedLevelButtons = new Dictionary<int, Button>();
    public void SetupSpeedUpgrades(/*Dictionary<int, Button> leveButtons, int currentLevel*/)
    {
        currentSpeedLevel = selectedElevator.ElevatorSpeedLevel;
        
        var upgradeArea = Instantiate(this.upgradeAreaPrefab, upgradeContainer.transform);
        upgradeArea.gameObject.name = "Speed Upgrade";

        upgradeArea.transform.GetChild(0).GetComponentInChildren<Text>().text = "Speed";

        speedLevelButtons = new Dictionary<int, Button>();

        for (int i = 0; i < ElevatorUpgrades.ElevatorSpeedUpgrades.Count; i++)
        {
            var upgradeButton = Instantiate(upgradeButtonPrefab, upgradeArea.transform.GetChild(1).transform);
            var button = upgradeButton.GetComponentInChildren<Button>();
            var text = upgradeButton.GetComponentInChildren<Text>();
            
            int buttonLevelNumber = i + 1;
            // button level is below or at current level
            if (buttonLevelNumber > selectedElevator.ElevatorSpeedLevel + 1)
            {
                button.interactable = false;
            }

            text.text = ElevatorUpgrades.ElevatorSpeedUpgrades[buttonLevelNumber].value + "\n$" + ElevatorUpgrades.ElevatorSpeedUpgrades[buttonLevelNumber].cost;
            
            button.onClick.AddListener(() =>
            {
                SpeedUpgradeSelected(buttonLevelNumber);
            });

            speedLevelButtons.Add(buttonLevelNumber, button);
            
            var colors = speedLevelButtons[currentSpeedLevel].colors;
            colors.normalColor = Color.green;
            colors.selectedColor = Color.green;
            speedLevelButtons[currentSpeedLevel].colors = colors;
        }
    }

    private int currentSpeedLevel;

    private void SpeedUpgradeSelected(int upgrade)
    {
        if (currentSpeedLevel == upgrade)
            return;
        
        // need to buy upgrade
        if (upgrade > selectedElevator.ElevatorSpeedLevel)
        {
            // check cost, if not able to buy, return
            var upgradeCost = ElevatorUpgrades.ElevatorSpeedUpgrades[upgrade].cost;
            if (GoldManager.Instance.CurrentGold < upgradeCost)
                return;

            if (upgrade < ElevatorUpgrades.ElevatorSpeedUpgrades.Count)
                speedLevelButtons[upgrade + 1].interactable = true;
            
            GoldManager.Instance.RemoveGold(upgradeCost);
        }

        var colors = speedLevelButtons[currentSpeedLevel].colors;
        colors.normalColor = Color.white;
        colors.selectedColor = Color.white;
        speedLevelButtons[currentSpeedLevel].colors = colors;
        
        var newColors = speedLevelButtons[upgrade].colors;
        newColors.normalColor = Color.green;
        newColors.selectedColor = Color.green;
        speedLevelButtons[upgrade].colors = newColors;

        currentSpeedLevel = upgrade;
        selectedElevator.ElevatorSpeedLevel = upgrade;
    }

    #endregion
}

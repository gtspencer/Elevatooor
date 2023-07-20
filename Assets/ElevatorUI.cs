using System;
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
        
        speedLevelButtons.Clear();
    }

    private ElevatorV2 selectedElevator;
    private void OnElevatorSelected(ElevatorV2 elevator)
    {
        if (elevator.Equals(selectedElevator))
            return;

        if (selectedElevator != null)
        {
            OnElevatorUnSelected();
        }
        
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
        foreach (Transform child in upgradeContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // TODO refactor to reuse already created buttons
    public void CreateUI()
    {
        DestroyAllButtons();
        
        // Create Lit Buttons UI
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
        
        speedLevelButtons = SetupUpgradeUI(ElevatorUpgrades.ElevatorSpeedUpgrades, selectedElevator.ElevatorSpeedLevel, selectedElevator.elevatorMaxSpeedLevelReached, "Speed", 1);
        accelLevelButtons = SetupUpgradeUI(ElevatorUpgrades.ElevatorAccelerationUpgrades, selectedElevator.ElevatorAccelLevel, selectedElevator.elevatorMaxAccelLevelReached, "Accel", 2);
        weightLimitLevelButtons = SetupUpgradeUI(ElevatorUpgrades.ElevatorWeightLimitUpgrades, selectedElevator.ElevatorWeightLimitLevel, selectedElevator.elevatorMaxWeightLimitLevelReached, "Weight", 3);

        panelUI.SetActive(true);
    }

    #region Upgrades

    [SerializeField] private GameObject upgradeAreaPrefab;
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private GameObject upgradeContainer;

    private Dictionary<int, Button> speedLevelButtons = new Dictionary<int, Button>();
    private Dictionary<int, Button> accelLevelButtons = new Dictionary<int, Button>();
    private Dictionary<int, Button> weightLimitLevelButtons = new Dictionary<int, Button>();

    public Dictionary<int, Button> SetupUpgradeUI(Dictionary<int, Upgrade> upgrades, int currentLevel, int maxLevelReached, string upgradeName, int upgradeUIIndex)
    {
        var upgradeArea = Instantiate(this.upgradeAreaPrefab, upgradeContainer.transform);
        upgradeArea.gameObject.name = $"{upgradeName} Upgrade";
        var yPosition = (startingUpgradeAreaPosition) - (upgradeArea.GetComponent<RectTransform>().rect.height * (upgradeUIIndex - 1));
        upgradeArea.transform.localPosition = new Vector3(0, yPosition, 0);

        upgradeArea.transform.GetChild(0).GetComponentInChildren<Text>().text = upgradeName;

        var levelButtons = new Dictionary<int, Button>();

        for (int i = 0; i < upgrades.Count; i++)
        {
            var upgradeButton = Instantiate(upgradeButtonPrefab, upgradeArea.transform.GetChild(1).transform);
            var button = upgradeButton.GetComponentInChildren<Button>();
            var text = upgradeButton.GetComponentInChildren<Text>();
            
            int buttonLevelNumber = i + 1;
            // button level is below or at current level, or the next one
            if (buttonLevelNumber > maxLevelReached + 1)
            {
                button.interactable = false;
            }
            
            text.text = upgrades[buttonLevelNumber].value.ToString();
            
            if (buttonLevelNumber > maxLevelReached)
                text.text += "\n$" + upgrades[buttonLevelNumber].cost;

            button.onClick.AddListener(() =>
            {
                UpgradeSelected(buttonLevelNumber, upgradeName);
            });

            levelButtons.Add(buttonLevelNumber, button);

            if (levelButtons.ContainsKey(currentLevel))
            {
                var colors = levelButtons[currentLevel].colors;
                colors.normalColor = Color.green;
                colors.selectedColor = Color.green;
                levelButtons[currentLevel].colors = colors;
            }
        }

        return levelButtons;
    }

    // TODO refactor these so its 1 success method
    private void WasSpeedUpgradeSuccessful(bool success, int levelUpgraded, int previousLevel)
    {
        if (success)
        {
            if (levelUpgraded < ElevatorUpgrades.ElevatorSpeedUpgrades.Count)
                speedLevelButtons[levelUpgraded + 1].interactable = true;
            
            if (speedLevelButtons.ContainsKey(previousLevel))
                ChangeButtonColor(speedLevelButtons[previousLevel], Color.white);
        
            ChangeButtonColor(speedLevelButtons[levelUpgraded], Color.green);
            speedLevelButtons[levelUpgraded].GetComponentInChildren<Text>().text = ElevatorUpgrades.ElevatorSpeedUpgrades[levelUpgraded].value.ToString();
        }
        else
        {
            if (levelUpgraded != previousLevel)
            {
                if (speedLevelButtons.ContainsKey(previousLevel))
                    ChangeButtonColor(speedLevelButtons[previousLevel], Color.white);
            }
        }
    }
    
    private void WasWeightUpgradeSuccessful(bool success, int levelUpgraded, int previousLevel)
    {
        if (success)
        {
            if (levelUpgraded < ElevatorUpgrades.ElevatorWeightLimitUpgrades.Count)
                weightLimitLevelButtons[levelUpgraded + 1].interactable = true;
            
            if (weightLimitLevelButtons.ContainsKey(previousLevel))
                ChangeButtonColor(weightLimitLevelButtons[previousLevel], Color.white);
        
            ChangeButtonColor(weightLimitLevelButtons[levelUpgraded], Color.green);
            weightLimitLevelButtons[levelUpgraded].GetComponentInChildren<Text>().text = ElevatorUpgrades.ElevatorWeightLimitUpgrades[levelUpgraded].value.ToString();
        }
        else
        {
            if (levelUpgraded != previousLevel)
            {
                if (weightLimitLevelButtons.ContainsKey(previousLevel))
                    ChangeButtonColor(weightLimitLevelButtons[previousLevel], Color.white);
            }
        }
    }
    
    private void WasAccelUpgradeSuccessful(bool success, int levelUpgraded, int previousLevel)
    {
        if (success)
        {
            if (levelUpgraded < ElevatorUpgrades.ElevatorAccelerationUpgrades.Count)
                accelLevelButtons[levelUpgraded + 1].interactable = true;
            
            if (accelLevelButtons.ContainsKey(previousLevel))
                ChangeButtonColor(accelLevelButtons[previousLevel], Color.white);
        
            ChangeButtonColor(accelLevelButtons[levelUpgraded], Color.green);
            accelLevelButtons[levelUpgraded].GetComponentInChildren<Text>().text = ElevatorUpgrades.ElevatorAccelerationUpgrades[levelUpgraded].value.ToString();
        }
        else
        {
            if (levelUpgraded != previousLevel)
            {
                if (accelLevelButtons.ContainsKey(previousLevel))
                    ChangeButtonColor(accelLevelButtons[previousLevel], Color.white);
            }
        }
    }

    private void UpgradeSelected(int upgrade, string upgradeType)
    {
        switch (upgradeType)
        {
            case "Speed":
                selectedElevator.UpgradeSpeed(upgrade, WasSpeedUpgradeSuccessful);
                break;
            case "Weight":
                selectedElevator.UpgradeWeight(upgrade, WasWeightUpgradeSuccessful);
                break;
            case "Accel":
                selectedElevator.UpgradeAccel(upgrade, WasAccelUpgradeSuccessful);
                break;
        }
    }

    private void ChangeButtonColor(Button button, Color newColor)
    {
        var colors = button.colors;
        colors.normalColor = newColor;
        colors.selectedColor = newColor;
        button.colors = colors;
    }

    #endregion
}

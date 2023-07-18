using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject panelUI;

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
        panelUI.SetActive(false);
        selectedElevator = null;
    }

    private ElevatorV2 selectedElevator;
    private void OnElevatorSelected(ElevatorV2 elevator)
    {
        selectedElevator = elevator;
        elevator.OnLitButtonsChanged += UpdateUI;
        CreateUI(Building.Instance.Floors, elevator.LitButtons);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyAllButtons()
    {
        foreach (Image i in floorButtons.Values)
            Destroy(i.transform.parent.gameObject);
        
        floorButtons.Clear();
    }

    // TODO refactor to reuse already created buttons
    public void CreateUI(int totalFloors, List<int> litFloors)
    {
        DestroyAllButtons();
        
        for (int i = 1; i <= totalFloors; i++)
        {
            var button = GameObject.Instantiate(buttonPrefab, panelUI.transform);

            var image = button.GetComponentInChildren<Image>();
            image.color = litFloors.Contains(i) ? litColor : defaultColor;
            floorButtons.Add(i, image);
            button.GetComponentInChildren<Text>().text = i.ToString();
        }

        panelUI.SetActive(true);
    }

    public void ToggleFloorButton(int floor, bool lit)
    {
        floorButtons[floor].color = lit ? litColor : defaultColor;
    }
}
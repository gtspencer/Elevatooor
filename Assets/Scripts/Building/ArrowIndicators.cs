using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicators : MonoBehaviour
{
    [SerializeField] private Text upArrow;
    [SerializeField] private Text downArrow;

    private Color defaultColor;
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = upArrow.color;
    }

    public void ToggleHighlightUp(bool shouldHighlight = true)
    {
        upArrow.color = shouldHighlight ? Color.blue : defaultColor;
    }
    
    public void ToggleHighlightDown(bool shouldHighlight = true)
    {
        downArrow.color = shouldHighlight ? Color.blue : defaultColor;
    }

    public void ResetIndicators()
    {
        upArrow.color = defaultColor;
        downArrow.color = defaultColor;
    }
}

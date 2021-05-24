using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Output : MonoBehaviour
{
    private TextMeshProUGUI OutputField;
    public static Output Component { private set; get; }
    // Start is called before the first frame update
    void Awake()
    {
        OutputField = GetComponent<TextMeshProUGUI>();
        Component = this;
    }

    public void AddText(string text)
    {
        OutputField.text += text +'\n';
    }
    public void Reset()
    {
        OutputField.text = "";
    }
}

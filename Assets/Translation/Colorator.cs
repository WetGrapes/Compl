using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Colorator : MonoBehaviour
{
   public Color[] Colors;
   public static Colorator Instance { private set; get; }
   void Start() => Instance = this;
   public List<string> GetColors() => Colors.Select(color => ColorUtility.ToHtmlStringRGB(color)).ToList();

    public static string RemoveColors(TMP_InputField Input)
    {
        var rgx = new Regex(@"<(.|\n)*?>");
        return rgx.Replace(Input.text, "");
    }
}

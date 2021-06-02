using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Colorator : MonoBehaviour
{
   public Color[] Colors;
   public static Colorator Instance { private set; get; }
   void Start() => Instance = this;
   public List<string> GetColors() => Colors.Select(color => ColorUtility.ToHtmlStringRGB(color)).ToList();
}

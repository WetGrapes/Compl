using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum States { S = 0, INT, FLOAT, F, ID, ER, CON, FUNC, TYPE, LOOPFOR, WORD, DLM }

public class WordTable : MonoBehaviour
{
    public static (int, string) WideSearch( string buf)
    {
        for (var i = startIndex; i < count; i++)
        {
            var srch = StateSearch((States) i, buf);
            if (srch.Item1 != -1) return srch;
        }
        return (-1,"");
    }
    public static (int, string) StateSearch(States _state, string buf)
    {
        var a = -1;
        var b = "";
        foreach (var list in WordTable.val[_state])
        {
            (a, b) = SearchLex(list.Value.ToArray(), buf);
            if (a == -1) continue;
            a = (int) _state;
            b = list.Key;
            break;
        }
        return (a, b);
    }
    public static  (int, string) SearchLex(string[] lexes, string buf)
    {
        var srh = Array.FindIndex(lexes, s => s.Equals(buf));
        return srh != -1 ? (srh, buf) : (-1, "");
    }
    
    static int count = Enum.GetNames(typeof(States)).Length;
    private static int startIndex = 6;
    
    public static Dictionary<States, Dictionary<string,List<string>>> val 
        = new Dictionary<States, Dictionary<string,List<string>>>
    {
        {
        States.CON, new Dictionary<string,List<string>>
        {
            {"if",new List<string>{"если"}},
            {"}\nelse",new List<string>{",иначе"}},
        }
    },
    {
        States.FUNC, new Dictionary<string,List<string>>
        {
            {"",new List<string>{"функция", "подпрограмма", "вызвать", "вызовем", "вызываем"}},
            {")\n{",new List<string>{"которая"}},
        }
    },
    {
        States.TYPE, new Dictionary<string,List<string>>
        {
            {"int",new List<string>{"целое", "целочис", "целочисленное"}},
            {"float",new List<string>{"дробное", "веществ", "вещественное"}},
            {"char",new List<string>{"символ", "символьная", "символьный"}},
            {"bool",new List<string>{"логическая", "логич"}},
        }
    },
    {
        States.LOOPFOR, new Dictionary<string,List<string>>
        {
            {"for(",new List<string>{"для"}},
            {";",new List<string>{",", "веществ", "вещественное"}},
            {"", new List<string>{"пока", "до","тех", "пор"}},
            {")\n{",new List<string>{"выполнить", "сделать"}},
            {"break", new List<string>{"остановить"}},
            {"continue", new List<string>{"следующий"}},
        }
    },
    {
        States.WORD, new Dictionary<string,List<string>>
        {
            {"break", new List<string>{"остановить"}},
            {"continue", new List<string>{"следующий"}},
            {"return", new List<string>{"вернуть", "возвращает"}},
        }
    },
    {
        States.DLM, new Dictionary<string,List<string>>
        {
            {"1", new List<string>{"один"}},
            {"+",new List<string>{"сложить", "прибавить","увеличить", "увеличивает", "плюс"}}, 
            {"-",new List<string>{"уменьшить", "уменьшает"}},
            {"*",new List<string>{"умножить", "умножаем"}},
            {"/",new List<string>{"разделить", "делит"}},
            {">",new List<string>{"больше"}}, 
            {"<",new List<string>{"меньше"}},
            {"==",new List<string>{"эквивалентно", "эквивалентен"}},
            {"&&",new List<string>{"и"}},
            {"||",new List<string>{"или"}},
            {"!",new List<string>{"не"}},
            {"=",new List<string>{"равно", "равняется", "равном"}},
            {")\n{",new List<string>{"то", "тогда", "которая"}},
            {"}", new List<string>{".", "..."}},
            {";", new List<string>{";"}},
            {"(",new List<string>{"от", "при", "с", ":"}},
            {";\n",new List<string>{"\n"}},
            {",",new List<string>{","}},
            {"\"",new List<string>{"\""}},
        }
    },
    };
}
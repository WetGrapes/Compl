using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum States { S = 0, INT, FLOAT, F, ID, ER, CON, FUNC, TYPE, LOOPFOR, WORD, DLM, NL }

public class WordTable : MonoBehaviour
{
    public static (int, string) WideSearch(string buf)
    {
        for (var i = startIndex; i < count; i++)
        {
            var srch = StateSearch((States)i, buf);
            if (srch.Item1 != -1) return srch;
        }
        return (-1, "");
    }
    public static (int, string) StateSearch(States _state, string buf)
    {
        var a = -1;
        var b = "";
        foreach (var list in WordTable.val[_state])
        {
            (a, b) = SearchLex(list.Value.ToArray(), buf);
            if (a == -1) continue;
            a = (int)_state;
            b = list.Key;
            break;
        }
        return (a, b);
    }
    public static (int, string) SearchLex(string[] lexes, string buf)
    {
        var srh = Array.FindIndex(lexes, s => 
        {
            if (buf.StartsWith(s))
            {
                return true;
            }
            else return false;
        });
        return srh != -1 ? (srh, buf) : (-1, "");
    }

    static int count = Enum.GetNames(typeof(States)).Length;
    private static int startIndex = 6;

    public static Dictionary<States, Dictionary<string, List<string>>> val
        = new Dictionary<States, Dictionary<string, List<string>>>
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
            {"",new List<string>{"функц", "подпрограмм", "вызва", "вызов", "вызыв","парамет"}},
            {"\n{",new List<string>{"котор"}},
            {")",new List<string>{";"} }
        }
    },
    {
        States.TYPE, new Dictionary<string,List<string>>
        {
            {"int",new List<string>{"целоч","цела"}},
            {"float",new List<string>{"дробн", "веществ"}},
            {"char",new List<string>{"символ"}},
            {"bool",new List<string>{"логич"}},
        }
    },
    {
        States.LOOPFOR, new Dictionary<string,List<string>>
        {
            {"for(",new List<string>{"для"}},
            {";",new List<string>{",", "веществ"}},
            {"", new List<string>{"пока", "до","тех", "пор"}},
            {")\n{",new List<string>{"выполн", "сдела"}},
            {"break", new List<string>{"останови"}},
            {"continue", new List<string>{"следующ"}},
        }
    },
    {
        States.WORD, new Dictionary<string,List<string>>
        {
            {"break", new List<string>{"останови"}},
            {"continue", new List<string>{"следующ"}},
            {"return", new List<string>{"вернуть", "возвра"}},
        }
    },
    {
        States.DLM, new Dictionary<string,List<string>>
        {
            {"1", new List<string>{"один"}},
            {"+",new List<string>{"слож", "прибав","увеличи", "плюс"}},
            {"-",new List<string>{"уменьш","минус"}},
            {"*",new List<string>{"умнож"}},
            {"/",new List<string>{"раздел", "делит"}},
            {">",new List<string>{"больше"}},
            {"<",new List<string>{"меньше"}},
            {"==",new List<string>{"эквивалент"}},
            {"&&",new List<string>{"и"}},
            {"||",new List<string>{"или"}},
            {"!",new List<string>{"не"}},
            {"=",new List<string>{"равн"}},
            {")\n{",new List<string>{"то", "тогда", "котор"}},
            {"}", new List<string>{"."}},
            {";", new List<string>{";"}},
            {"(",new List<string>{"от", "при", "с", ":","без"}},
            {",",new List<string>{","}},
            {"\"",new List<string>{"\""}},
        }
    },
    { 
        States.NL, new Dictionary<string,List<string>>
        {
            {";\n",new List<string>{"\n"}},
        }
    },
    };
}
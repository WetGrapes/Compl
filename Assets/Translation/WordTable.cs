using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum States { S = 0, INT, FLOAT, F, ID, ER, CON, FUNC, TYPE, LOOPFOR, LOOPWHILE, WORD, DLM, STR }

public class WordTable : MonoBehaviour
{
    public static (int, string) WideSearch(string buf)
    {
        try
        {
            for (var i = startIndex; i < count; i++)
            {
                var srch = StateSearch((States)i, buf);
                if (srch.Item1 != -1) return srch;
            }
        }
        catch (Exception e)
        {
            Debug.Log(buf);
            throw;
        }
        return (-1, "");
    }
    public static (int, string) StateSearch(States _state, string buf)
    {
        var a = -1;
        var b = "";
        foreach (var list in val[_state])
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
        var srh = Array.FindIndex(lexes, s 
            => buf.StartsWith(s) && s.Length + 2 <= buf.Length);
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
            {"\nelse",new List<string>{"иначе"}}
        }
    },
    {
        States.FUNC, new Dictionary<string,List<string>>
        {
            {"",new List<string>{"функц", "подпрограмм", "вызва", "вызов", "вызыв","парамет"}}
        }
    },
    {
        States.TYPE, new Dictionary<string,List<string>>
        {
            {"int",new List<string>{"целоч","цела","целое", "инт"}},
            {"float",new List<string>{"дробн", "веществ", "флоат"}},
            {"char",new List<string>{"символ","чар"}},
            {"bool",new List<string>{"логич", "бул"}}
        }
    },
    {
        States.LOOPFOR, new Dictionary<string,List<string>>
        {
            {"for",new List<string>{"для"}}
        }
    },
      {
        States.LOOPWHILE, new Dictionary<string,List<string>>
        {
            {"while", new List<string>{"пока"}}
        }
    },
    {
        States.WORD, new Dictionary<string,List<string>>
        {
            {"break", new List<string>{"останови"}},
            {"continue", new List<string>{"следующ"}},
            {"return", new List<string>{"вернуть", "возвра"}},
            {"#include", new List<string>{"подключ", "добави"}},
            {"int", new List<string>{"программа_"}}
        }
    },
    {
        States.DLM, new Dictionary<string,List<string>>
        {
            {"+",new List<string>{"слож", "прибав","увеличи", "плюс", "+"}},
            {"-",new List<string>{"уменьш","минус", "-", "вычит", "вычесть"}},
            {"*",new List<string>{"умнож", "*","указатeл"}},
            {"/",new List<string>{"раздел", "делит", "/"}},
            {">",new List<string>{"больш"}},
            {"<",new List<string>{"меньш"}},
            {"==",new List<string>{"эквивалент"}},
            {"&&",new List<string>{"и"}},
            {"||",new List<string>{"или"}},
            {"!",new List<string>{"не", "!"}},
            {"|",new List<string>{"|"}},
            {"&",new List<string>{"&","ссылк"}},
            {"=",new List<string>{"=","равн"}},
            {")\n{\n",new List<string>{"то", "тогда", "повтор", "выполн", "сдела"}},
            {"}", new List<string>{"."}},
            {";", new List<string>{":"}},
            {")", new List<string>{";",")"}},
            {"(",new List<string>{"от","при", "с","без", "("}},
            {"",new List<string>{"на", "в","до","тех", "пор"}},
            {",",new List<string>{","}},
            {"%", new List<string>{"%", "процент"}},
            {"\n{\n",new List<string>{"котор"}}
        }
    },
    {
        States.STR, new Dictionary<string,List<string>>
        {
            {"\"",new List<string>{"\""}}
        }
    },
    };
}


/*
подключ "iostream"

веществ функция F с инт a = 5, бул m; которая
printf от "%d", a плюс равно 1;
.

программа_
целое a = 4
целочисленное b равн 8
вещественное g равен 7.656564561
целая c = b раздел на a
printf от "%d", c;
если c больше 1 то c равно b минус a
.
для инт i = 0: i меньш 5: i ++ повторяй printf от "%d", i;
.
.

*/
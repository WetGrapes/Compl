using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LexicalAnalyzer 
{
    public virtual string CreateTokens(string code)
    {
        Addresses.SymbolTable = new SymbolTable();
        var bf = "";
        var bfOut = "";
        foreach (var c in code)
        {
            if (itCanBeName(c, bf))
            {
                bf += c;
            }
            else
            {
                if (bf != "") bfOut += Addresses.SymbolTable + bf;
                bf = "";
                bfOut += c;
            }
        }
        Debug.Log(bfOut);
        return bfOut;
    }

    static bool itCanBeName(char o, string nowName) => 
        (nowName != "" || !char.IsDigit(o)) && (char.IsLetterOrDigit(o) || o == '_');
}

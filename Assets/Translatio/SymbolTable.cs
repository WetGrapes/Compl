using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SymbolTable
{
    private static int lst = 'a';
    private Dictionary<char, string> _chsd = new Dictionary<char, string>();
    private static Dictionary<char, string> standart = new Dictionary<char, string>
    {
        {(char)0,"if"},
        {(char)1,"else"},
        {(char)2,"while"},
        {(char)3,"do"},
        {(char)4,"for"},
        {(char)5,"float"},
        {(char)6,"int"},
    };
    private static Dictionary<char, string> symbols = new Dictionary<char, string>
    {
        {(char)20,"+"},
        {(char)21,"-"},
        {(char)22,"*"},
        {(char)23,"/"},
        {(char)24,"("},
        {(char)25,")"},
        {(char)26,"{"},
        {(char)27,"}"},
        {(char)28,"["},
        {(char)29,"]"},
        {(char)30,">"},
        {(char)31,"<"},
        {(char)32,"="},
    };
    string str = "";

    public SymbolTable()
    {
        _chsd = standart;
        foreach (var symbol in symbols)
        {
            if(_chsd.ContainsKey(symbol.Key)) continue;
            _chsd.Add(symbol.Key, symbol.Value);
        }
        lst = 'a';
    }

    private string GetValue(char key)
    {
        try
        {
            if (_chsd.TryGetValue(key, out str)) return str;
            throw new Exception();
        } catch(Exception e) {
            return "--";
        }
    }

    private void SetValue(char key, string value)
    {
        _chsd.Add(key, value);
    }

    public string this[char key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    public string LastValue => this[LastKey];
    public char LastKey => (char) (lst - 1);

    public char Add(string value)
    {
        if (_chsd.ContainsValue(value))
        {
            foreach (var pair in _chsd)
            {
                if (value == pair.Value) return pair.Key;
            }
        }
        else
        {
            _chsd.Add((char)lst, value);
            lst++;
        }

        return LastKey;
    }
    public static implicit operator char(SymbolTable t) => t.LastKey;
    public static char operator +(SymbolTable a, string b) => a.Add(b);
}

public static class Addresses
{
    public static SymbolTable SymbolTable = new SymbolTable();
}
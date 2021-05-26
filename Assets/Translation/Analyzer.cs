using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Analyzer : MonoBehaviour
{
    private string[] Words = { "program", "var", "integer", "real", "bool", "begin", "end", "if", "then", "else", "while", "do", "read", "write", "true", "false" };
    private string[] Delimiter = { ".", ";", ",", "(", ")", "+", "-", "*", "/", "=", ">", "<" };
    public List<Lex> Lexemes = new List<Lex>();

    private string buf = ""; // буфер для хранения лексемы
    private char[] sm = new char[1];
    private int dt = 0;
    private enum States { S, NUM, DLM, FIN, ID, ER, ASGN, COM } // состояния state-машины
    private States state = States.S; // хранит текущее состояние
    private StringReader sr; // позволяет посимвольно считывать строку
    private string[] TNUM;
    private string[] TID;
    private int tics = 0;
    
    private Dictionary<int, string> typesOfLexem = new Dictionary<int, string>()
    {
        {1, "служебные слова"},
        {2, "ограничители"},
        {3, "числа"},
        {4, "идентификатор"},
        {-1, "не опознано"},
    };

    public TMP_InputField Input;
    public void StartAnalyze(){
        Lexemes = new List<Lex>();
        TNUM = new string[]{};
        TID = new string[]{};
        state = States.S;
        buf = "";
        dt = 0;
        
        Analysis(Input.text);
        
        foreach (var lexeme in Lexemes) Debug.Log(lexeme.val + "\t" + typesOfLexem[lexeme.id]);
        Debug.Log("F");
    }
    private (int, string) SearchLex(string[] lexes)
    {
        var srh = Array.FindIndex(lexes, s => s.Equals(buf));
        return srh != -1 ? (srh, buf) : (-1, "");
    }

    private (int, string) PushLex(string[] lexes, string buf)
    {
        lexes ??= new string[] { };
        var srh = Array.FindIndex(lexes, s => s.Equals(buf));
        if (srh != -1) return (-1, "");
        
        Array.Resize(ref lexes, lexes.Length + 1);
        lexes[lexes.Length - 1] = buf;
        return (lexes.Length - 1, buf);
        
    }
    public void Analysis(string text)
    {
        if(text == null || text.Length <= 0) return;
        sr = new StringReader(text);
        tics = text.Length*2;
        while (state != States.FIN && tics>0)
        {
            tics--;
            switch (state)
            {

                case States.S:
                    if (sm[0] == ' ' || sm[0] == '\n' || sm[0] == '\t' || sm[0] == '\0' || sm[0] == '\r')
                        GetNext();
                    else if (char.IsLetter(sm[0]))
                    {
                        buf = "";
                        buf += sm[0];
                        state = States.ID;
                        GetNext();
                    }
                    else if (char.IsDigit(sm[0]))
                    {
                        dt = (int)(sm[0] - '0');
                        GetNext();
                        state = States.NUM;

                    }
                    else if (sm[0] == '{')
                    {
                        state = States.COM;
                        GetNext();
                        
                    }
                    else if (sm[0] == ':')
                    {
                        state = States.ASGN;
                        buf = "";
                        buf += sm[0];
                        GetNext();
                    }
                    else if (sm[0] == '.')
                    {
                        Lexemes.Add(new Lex(2, 0, sm[0].ToString()));
                        state = States.FIN;
                    }
                    else
                    {
                        state = States.DLM;
                    }

                    break;
                case States.ID:
                    if (char.IsLetterOrDigit(sm[0]))
                    {
                        buf += sm[0];
                        GetNext();
                    }
                    else
                    {
                        var srch = SearchLex(Words);
                        if (srch.Item1 != -1)
                            Lexemes.Add(new Lex(1, srch.Item1, srch.Item2));
                        else
                        {
                            var j = PushLex(TID, buf);
                            Lexemes.Add(new Lex(4, j.Item1, j.Item2));
                        }
                        state = States.S;
                    }
                    break;

                case States.NUM:
                    if (char.IsDigit(sm[0]))
                    {
                        dt = dt * 10 + (int)(sm[0] - '0');
                        GetNext();
                    }
                    else
                    {
                        var (item1, item2) = PushLex(TNUM, dt.ToString());
                        Lexemes.Add(new Lex(3, item1, item2));
                        state = States.S;
                    }
                    break;
                case States.DLM:
                    
                    buf = "";
                    buf += sm[0];

                    var (int_val, str_val) = SearchLex(Delimiter);
                    if (int_val != -1)
                    {
                        Lexemes.Add(new Lex(2, int_val, str_val));
                        state = States.S;
                        GetNext();
                    }
                    else
                        state = States.ER;
                    break;
                case States.ASGN:
                    if (sm[0] == '=')
                    {
                        buf += sm[0];
                        Lexemes.Add(new Lex(2, 4, buf));
                        buf = "";
                        GetNext();
                    }
                    else
                        Lexemes.Add(new Lex(2, 3, buf));
                    state = States.S;

                    break;
                case States.ER:
                    Debug.Log("Ошибка в программе");
                    state = States.FIN;
                    break;
                case States.FIN:
                    Debug.Log("Лексический анализ закончен");
                    break;
            }
        }
    }
    private void GetNext()
    {
        var res = sr.Read(sm, 0, 1);
        if (res == -1) sm[0] = '.';
    }

}

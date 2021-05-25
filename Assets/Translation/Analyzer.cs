using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private States state; // хранит текущее состояние
    private StringReader sr; // позволяет посимвольно считывать строку
    private string[] TNUM;
    private string[] TID;
    private (int, string) SerchLex(string[] lexes)
    {
        var srh = Array.FindIndex(lexes, s => s.Equals(buf));
        if (srh != -1)
            return (srh, buf);
        else return (-1, "");
    }

    private (int, string) PushLex(string[] lexes, string buf)
    {
        var srh = Array.FindIndex(lexes, s => s.Equals(buf));
        if (srh != -1)
            return (-1, "");
        else
        {
            Array.Resize(ref lexes, lexes.Length + 1);
            lexes[lexes.Length - 1] = buf;
            return (lexes.Length - 1, buf);
        }
    }
    public void Analysis(string text)
    {
        sr = new StringReader(text);
        while (state != States.FIN)
        {
            switch (state)
            {

                case States.S:
                    if (sm[0] == ' ' || sm[0] == '\n' || sm[0] == '\t' || sm[0] == '\0' || sm[0] == '\r')
                        GetNext();
                    else if (Char.IsLetter(sm[0]))
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
                    if (Char.IsLetterOrDigit(sm[0]))
                    {
                        buf += sm[0];
                        GetNext();
                    }
                    else
                    {
                        var srch = SerchLex(Words);
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
                    if (Char.IsDigit(sm[0]))
                    {
                        dt = dt * 10 + (int)(sm[0] - '0');
                        GetNext();
                    }
                    else
                    {

                        var j = PushLex(TNUM, dt.ToString());
                        Lexemes.Add(new Lex(3, j.Item1, j.Item2));
                        state = States.S;
                    }
                    break;
                case States.DLM:
                    
                    buf = "";
                    buf += sm[0];

                    var r = SerchLex(Delimiter);
                    if (r.Item1 != -1)
                    {
                        Lexemes.Add(new Lex(2, r.Item1, r.Item2));
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
        sr.Read(sm, 0, 1);
    }

}

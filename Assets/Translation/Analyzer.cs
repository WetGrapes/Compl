using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


public class Analyzer : MonoBehaviour
{
    public List<Lex> Lexemes = new List<Lex>();
    public TextMeshProUGUI output;
    private string buf = ""; // буфер для хранения лексемы
    private string prevBuf = ""; // буфер для хранения прошлой лексемы
    private char[] sm = new char[1];
    private int dt = 0, mantis = 1;
    private float fl = 0;
    // состояния state-машины
    private States globalState = States.S; // хранит текущее состояние
    private States localState = States.S; // хранит текущее состояние
    private StringReader sr; // позволяет посимвольно считывать строку
    private string[] TNUM;
    private string[] TID;
    private int tics = 0;
    bool stop = false;
    private Dictionary<int, string> typesOfLexem = new Dictionary<int, string>()
    {
        {1, "служебные слова"},
        {2, "ограничители"},
        {3, "числа"},
        {4, "идентификатор"},
        {-1, "не опознано"},
    };

    public TMP_InputField Input;
    public void StartAnalyze()
    {
        Lexemes = new List<Lex>();
        TNUM = new string[] { };
        TID = new string[] { };
        globalState = States.S;
        buf = "";
        dt = 0;

        Analysis(Input.text);

        foreach (var lexeme in Lexemes) Debug.Log(lexeme.val + "\t" + typesOfLexem[lexeme.id]);
        Debug.Log("F");
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
        if (text == null || text.Length <= 0) return;
        sr = new StringReader(text);
        tics = text.Length + 1;
        Debug.LogWarning(text.Length);
        while (globalState != States.F && tics > 0)
        {
            
            switch (globalState)
            {

                case States.S:
                    if (sm[0] == ' ' || sm[0] == '\t' || sm[0] == '\0' || sm[0] == '\r')
                        GetNext();
                    else if (sm[0] == '\n')
                    {
                        Lexemes.Add(new Lex(2, (int)States.DLM, ";\n"));
                        GetNext();
                    }
                    else if (char.IsLetter(sm[0]) || sm[0] == '_')
                    {
                        buf = "";
                        buf += sm[0];
                        globalState = States.ID;
                        GetNext();
                    }
                    else if (char.IsDigit(sm[0]))
                    {
                        dt = (int)(sm[0] - '0');
                        GetNext();
                        globalState = States.INT;

                    }
                    else
                    {
                        globalState = States.DLM;
                    }
                    break;
                case States.ID:
                    if (char.IsLetterOrDigit(sm[0]) || sm[0] == '_')
                    {
                        buf += sm[0];
                        GetNext();
                    }
                    else
                    {
                        var srch = WordTable.WideSearch(buf);
                        if (srch.Item1 != -1)
                        {
                            ///если нашли стандартное слово
                            globalState = (States)srch.Item1;
                            Lexemes.Add(new Lex(1, srch.Item1, srch.Item2));
                        }
                        else
                        {
                            var j = PushLex(TID, buf);
                            Lexemes.Add(new Lex(4, j.Item1, j.Item2));

                        }
                        globalState = States.S;
                    }
                    break;

                case States.INT:
                    if (char.IsDigit(sm[0]))
                    {
                        dt = dt * 10 + (int)(sm[0] - '0');
                        GetNext();
                    }
                    else if (sm[0] == '.' || sm[0] == ',')
                    {
                        fl = dt;
                        globalState = States.FLOAT;
                        GetNext();
                    }
                    else
                    {
                        var (item1, item2) = PushLex(TNUM, dt.ToString());
                        Lexemes.Add(new Lex(3, item1, item2));
                        globalState = States.S;
                    }
                    break;
                case States.FLOAT:
                    if (char.IsDigit(sm[0]))
                    {
                        fl += (float)(sm[0] - '0') / Mathf.Pow(10, mantis++);
                        GetNext();
                    }
                    else
                    {
                        mantis = 1;
                        var (a, b) = PushLex(TNUM, dt.ToString());
                        Lexemes.Add(new Lex(3, a, b));
                        globalState = States.S;
                    }
                    break;
                case States.DLM:

                    buf = "";
                    buf += sm[0];

                    var (c, d) = WordTable.StateSearch(States.DLM, buf);
                    if (c != -1)
                    {
                        Lexemes.Add(new Lex(2, c, d));
                        globalState = States.S;
                        GetNext();
                    }
                    else
                        globalState = States.ER;
                    break;
                case States.ER:
                    Debug.Log("Ошибка в программе");
                    globalState = States.F;
                    break;
                case States.F:
                    Debug.Log("Лексический анализ закончен");
                    break;

            }
        }
        Output();
    }

    private bool str;
    private void Output()
    {
        foreach (var lex in Lexemes)
        {
            var c = ' ';
            if (lex.val == "\"") str = !str;
            if (str) c = '\0';
            output.text += lex.val + (str ? "": c.ToString());

        }
    }
    private void GetNext()
    {
        tics--;
        var res = sr.Read(sm, 0, 1);
        if(res == -1 ) Debug.LogWarning("ATAS");
        if (res != -1 || globalState == States.F) return;
        Debug.LogWarning("ATAS");
        Lexemes.Add(new Lex(2, (int)States.DLM, "}"));
        globalState = States.F;
    }

}

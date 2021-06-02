using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;


public class Analyzer : MonoBehaviour
{
    public TMP_InputField Input, Output; // ввод данных и вывод

    //буферы
    private string buf = "";             // буфер для хранения лексемы
    private char[] sm = new char[1];     //буфер для последнего символа
    private int dt = 0, mantis = 1;      //целая часть, знак мантиссы
    private float fl = 0;                //вещественное число


    //лексемы
    public List<Lex> Lexemes = new List<Lex>();    //все лексемы
    private Lex previous;                          //буфер для прошлой лексемы для обработки особых состояний


    // состояния state-машины
    private States globalState = States.S;   // хранит текущее состояние

    private StringReader sr;                 // позволяет посимвольно считывать строку

    private int tics = 0;                    // предохранение от вечного цикла
    private bool str;                        // требуется для корректного распознавания "строк" 


    private List<string> hexColor => Colorator.Instance.GetColors();   // цвета для синтаксиса
    private string HexColor(int state) => hexColor[str ? (int)States.STR : state];

    private readonly string colin = "<COLOR=#";
    private readonly string colout = "</COLOR>";
    private string reverseBuf;                                         // окрашенный ввод

    private void Cleaning()
    {
        Lexemes = new List<Lex>();
        globalState = States.S;
        buf = "";
        dt = 0;
        fl = 0;
        mantis = 1;
        Output.text = "";
        reverseBuf = "";
        sm[0] = '\0';
    }

    public void StartAnalyze()
    {
        Cleaning();
        Input.text = Colorator.RemoveColors(Input);
        Analysis(Input.text);
        Debug.Log("F");
    }

    public void Analysis(string text)
    {
        if (text == null || text.Length <= 0) return;
        sr = new StringReader(text);
        tics = text.Length + 1;
        while (globalState != States.F && tics > 0)
        {

            switch (globalState)
            {

                case States.S:
                    StartState();
                    break;
                case States.ID:
                    IDState();
                    break;

                case States.INT:
                    IntState();
                    break;
                case States.FLOAT:
                    FloatState();
                    break;
                case States.DLM:
                    DLMState();
                    break;
                case States.ER:
                    Debug.Log("Ошибка в программе");
                    globalState = States.F;
                    break;
                case States.F:
                    Debug.Log("Лексический анализ закончен");
                    break;

                case States.CON:
                    if (previous.val == "if") Lexemes.Add(new Lex((int)States.DLM, "("));
                    globalState = States.S;
                    break;
                case States.FUNC:
                    globalState = States.S;
                    break;
                case States.STR:
                    
                    if (!str)
                    {
                        Debug.Log(buf);
                        if (buf == "\"") str = !str;
                    }
                    else
                    {
                        buf += sm[0];
                        if (sm[0] == '"')
                        {
                            str = !str;
                            Lexemes.Add(new Lex((int)States.STR, buf));
                            reverseBuf += ColoredLex(buf, HexColor((int)States.STR));
                            globalState = States.S;
                            buf = "";
                        }
                    }
                    GetNext();
                    break;
                case States.TYPE:
                    globalState = States.S;
                    break;
                case States.LOOPFOR:
                    if (previous.val == "for") Lexemes.Add(new Lex((int)States.DLM, "("));
                    globalState = States.S;
                    break;
                case States.LOOPWHILE:
                    if (previous.val == "while") Lexemes.Add(new Lex((int)States.DLM, "("));
                    globalState = States.S;
                    break;
                case States.WORD:
                    if (previous.val == "int")
                    {
                        Lexemes.Add(new Lex((int)States.ID, "main"));
                        Lexemes.Add(new Lex((int)States.DLM, "(){\n"));
                    }
                    globalState = States.S;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        OutputLexemes();
        Input.text = reverseBuf;
    }
    private void OutputLexemes()
    {
        for (var i = 0; i < Lexemes.Count; i++)
        {
            var lex = Lexemes[i];
            var c = ' ';
            if (lex.val == "\"") str = !str;
            var hex = HexColor(lex.state);
            if (i < Lexemes.Count - 1 && lex.state == (int)States.DLM && Lexemes[i + 1].state == (int)States.DLM || lex.val=="") c = '\0';
            Output.text += ColoredLex(lex.val, hex) + (str ? "" : c.ToString());
        }
    }
    private void GetNext()
    {
        tics--;
        var res = sr.Read(sm, 0, 1);
        if (res == -1) Debug.LogWarning("ATAS");
        if (res != -1 || globalState == States.F) return;
        Debug.LogWarning("ATAS");
        Lexemes.Add(new Lex((int)States.DLM, "}"));
        globalState = States.F;
    }

    private string ColoredLex(string val, string col) => colin + col + ">" + val + colout;

    public void StartState()
    {
        if (sm[0] == ' ' || sm[0] == '\t' || sm[0] == '\0' || sm[0] == '\r')
        {
            reverseBuf += sm[0];
            GetNext();
        }
        else if (sm[0] == '\n')
        {
            if (!Lexemes[Lexemes.Count - 1].val.Contains(";") && !Lexemes[Lexemes.Count - 1].val.Contains("{"))
            {
                Lexemes.Add(new Lex((int)States.DLM, ";\n"));
            }
            reverseBuf += sm[0];
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
    }
    public void IDState()
    {
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
                reverseBuf += ColoredLex(buf, HexColor((int)globalState));
                if (globalState == States.DLM) globalState = States.S;
                previous = new Lex(srch.Item1, srch.Item2);
                Lexemes.Add(new Lex(srch.Item1, srch.Item2));
            }
            else
            {
                reverseBuf += ColoredLex(buf, HexColor((int)globalState));
                Lexemes.Add(new Lex((int)States.ID, buf));
                globalState = States.S;
            }

        }
    }
    public void IntState()
    {
        if (char.IsDigit(sm[0]))
        {
            dt = dt * 10 + (sm[0] - '0');
            reverseBuf += sm[0];
            GetNext();
        }
        else if (sm[0] == '.')
        {
            fl = dt;
            globalState = States.FLOAT;
            GetNext();
        }
        else
        {
            reverseBuf += ColoredLex(dt.ToString(), HexColor((int)States.INT));
            Lexemes.Add(new Lex((int)States.INT, dt.ToString()));
            globalState = States.S;
        }
    }
    public void FloatState()
    {
        if (char.IsDigit(sm[0]))
        {
            fl += (float)(sm[0] - '0') / Mathf.Pow(10, mantis++);
            GetNext();
        }
        else
        {
            mantis = 1;
            reverseBuf += ColoredLex(fl.ToString(CultureInfo.InvariantCulture), HexColor((int)States.FLOAT));
            Lexemes.Add(new Lex((int)States.FLOAT, fl.ToString()));
            globalState = States.S;
        }
    }
    public void DLMState()
    {
        buf = "";
        buf += sm[0];
        var (c, d) = WordTable.StateSearch(States.DLM, buf);
        if (c != -1)
        {
            Lexemes.Add(new Lex(c, d));
            reverseBuf += ColoredLex(buf, HexColor((int) States.DLM));
            globalState = States.S;
            GetNext();
        }
        else globalState = States.STR;
    }
}




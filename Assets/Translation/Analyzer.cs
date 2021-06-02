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
    public TMP_InputField Input, Output; // ���� ������ � �����
    
    //������
    private string buf = "";             // ����� ��� �������� �������
    private char[] sm = new char[1];     //����� ��� ���������� �������
    private int dt = 0, mantis = 1;      //����� �����, ���� ��������
    private float fl = 0;                //������������ �����
    
    
    //�������
    public List<Lex> Lexemes = new List<Lex>();    //��� �������
    private Lex previous;                          //����� ��� ������� ������� ��� ��������� ������ ���������
    
    
    // ��������� state-������
    private States globalState = States.S;   // ������ ������� ���������
    private States localState = States.S;    // ������ ������� ���������
    
    private StringReader sr;                 // ��������� ����������� ��������� ������
    
    private string[] TNUM;                   // ������� ����������� �����
    private string[] TID;                    // ������� ����������� ���������������
    
    
    private int tics = 0;                    // ������������� �� ������� �����
    private bool str;                        // ��������� ��� ����������� ������������� "�����" 
    
    
    private List<string> hexColor => Colorator.Instance.GetColors();   // ����� ��� ����������
    private string HexColor(int state) => hexColor[str ? (int) States.STR : state];
    private string colin = "<COLOR=#", colout = "</COLOR>";            // ��������������� ����������
    
    
    
    private string reverseBuf;                                         // ���������� ����

    private Dictionary<int, string> typesOfLexem = new Dictionary<int, string>()
    {
        {1, "��������� �����"},
        {2, "������������"},
        {3, "�����"},
        {4, "�������������"},
        {-1, "�� ��������"},
    };

    public void RemoveColors()
    {
        var rgx = new Regex(@"<(.|\n)*?>");
        Input.text = rgx.Replace(Input.text, "");
    }
    public void StartAnalyze()
    {
        Lexemes = new List<Lex>();
        TNUM = new string[] { };
        TID = new string[] { };
        globalState = States.S;
        buf = "";
        dt = 0;
        fl = 0;
        mantis = 1;
        Output.text = "";
        reverseBuf = "";
        sm[0] = '\0';
        RemoveColors();
        Analysis(Input.text);

        //foreach (var lexeme in Lexemes) Debug.Log(lexeme.val + "\t" + typesOfLexem[lexeme.id]);
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
        while (globalState != States.F && tics > 0)
        {

            switch (globalState)
            {

                case States.S:
                    if (sm[0] == ' ' || sm[0] == '\t' || sm[0] == '\0' || sm[0] == '\r')
                    {
                        reverseBuf += sm[0];
                        GetNext();
                    }
                    else if (sm[0] == '\n')
                    {
                        if (!Lexemes[Lexemes.Count - 1].val.Contains(";") && !Lexemes[Lexemes.Count - 1].val.Contains("{"))
                        {
                            Lexemes.Add(new Lex(2, (int)States.DLM, ";\n"));
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
                            ///���� ����� ����������� �����
                            globalState = (States)srch.Item1;
                            reverseBuf += ColoredLex(buf, HexColor((int)globalState));
                            if (globalState == States.DLM) globalState = States.S;
                            previous = new Lex(1, srch.Item1, srch.Item2);
                            Lexemes.Add(new Lex(1, srch.Item1, srch.Item2));
                        }
                        else
                        {
                            var j = PushLex(TID, buf);
                            reverseBuf += ColoredLex(buf, HexColor((int)globalState));
                            Lexemes.Add(new Lex(4, j.Item1, j.Item2));
                            globalState = States.S;
                        }

                    }
                    break;

                case States.INT:
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
                        var (item1, item2) = PushLex(TNUM, dt.ToString());
                        reverseBuf += ColoredLex(dt.ToString(), HexColor((int)States.INT));
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
                        var (a, b) = PushLex(TNUM, fl.ToString(CultureInfo.InvariantCulture));
                        reverseBuf += ColoredLex(fl.ToString(CultureInfo.InvariantCulture), HexColor((int)States.FLOAT));
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
                        reverseBuf += ColoredLex(buf, HexColor((int)States.DLM));
                        globalState = States.S;
                        GetNext();
                    }
                    else
                    {
                        (c, d) = WordTable.StateSearch(States.STR, buf);
                        if (c != -1)
                        {
                            str = !str;
                            Lexemes.Add(new Lex(2, c, d));
                            reverseBuf += ColoredLex(buf, HexColor((int)States.STR));
                            globalState = States.S;
                            GetNext();
                        }
                        else
                            globalState = States.ER;
                    }

                    break;
                case States.ER:
                    Debug.Log("������ � ���������");
                    globalState = States.F;
                    break;
                case States.F:
                    Debug.Log("����������� ������ ��������");
                    break;

                case States.CON:
                    if (previous.val == "if") Lexemes.Add(new Lex(2, (int)States.DLM, "("));
                    globalState = States.S;
                    break;
                case States.FUNC:
                    globalState = States.S;
                    break;
                case States.TYPE:
                    globalState = States.S;
                    break;
                case States.LOOPFOR:
                    if (previous.val == "for") Lexemes.Add(new Lex(2, (int)States.DLM, "("));
                    globalState = States.S;
                    break;
                case States.LOOPWHILE:
                    if (previous.val == "while") Lexemes.Add(new Lex(2, (int)States.DLM, "("));
                    globalState = States.S;
                    break;
                case States.WORD:
                    if (previous.val == "int")
                    {
                        Lexemes.Add(new Lex(3, (int)States.ID, "main"));
                        Lexemes.Add(new Lex(2, (int)States.DLM, "(){\n"));
                    }
                    globalState = States.S;
                    break;
                case States.NL:
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
            var hex = HexColor(lex.lex) ;
            if (i<Lexemes.Count-1 && lex.lex == (int) States.DLM && Lexemes[i+1].lex == (int) States.DLM) c = '\0';
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
        Lexemes.Add(new Lex(2, (int)States.DLM, "}"));
        globalState = States.F;
    }
    
    private string ColoredLex(string val, string col) => colin + col + ">" + val + colout;

}




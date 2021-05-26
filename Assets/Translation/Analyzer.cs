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

    private string buf = ""; // ����� ��� �������� �������
    private string prevBuf = ""; // ����� ��� �������� ������� �������
    private char[] sm = new char[1];
    private int dt = 0, mantis=1;
    private float fl = 0;
    // ��������� state-������
    private States state = States.S; // ������ ������� ���������
    private StringReader sr; // ��������� ����������� ��������� ������
    private string[] TNUM;
    private string[] TID;
    private int tics = 0;
    
    private Dictionary<int, string> typesOfLexem = new Dictionary<int, string>()
    {
        {1, "��������� �����"},
        {2, "������������"},
        {3, "�����"},
        {4, "�������������"},
        {-1, "�� ��������"},
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
        while (state != States.F && tics>0)
        {
            tics--;
            switch (state)
            {

                case States.S:
                    if (sm[0] == ' ' || sm[0] == '\n' || sm[0] == '\t' || sm[0] == '\0' || sm[0] == '\r')
                        GetNext();
                    else if (char.IsLetter(sm[0]) || sm[0] == '_')
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
                        state = States.INT;

                    }
                    else if (sm[0] == '.')
                    {
                        Lexemes.Add(new Lex(2, 0, sm[0].ToString()));
                        state = States.F;
                    }
                    else
                    {
                        state = States.DLM;
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
                            state = (States) srch.Item1;
                        }
                        else
                        {
                            var j = PushLex(TID, buf);
                            Lexemes.Add(new Lex(4, j.Item1, j.Item2));
                        }
                        state = States.S;
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
                        state = States.FLOAT;
                        GetNext();
                    } else
                    {
                        var (item1, item2) = PushLex(TNUM, dt.ToString());
                        Lexemes.Add(new Lex(3, item1, item2));
                        state = States.S;
                    }
                    break;
                case States.FLOAT:
                    if (char.IsDigit(sm[0]))
                    {
                        fl += (float)(sm[0] - '0')/Mathf.Pow(10,mantis++);
                        GetNext();
                    }else
                    {
                        mantis = 1;
                        var (a, b) = PushLex(TNUM, dt.ToString());
                        Lexemes.Add(new Lex(3, a, b));
                        state = States.S;
                    }
                    break;
                case States.DLM:
                    
                    buf = "";
                    buf += sm[0];

                    var (c, d) = WordTable.SearchLex(Delimiter, buf);
                    if (c != -1)
                    {
                        Lexemes.Add(new Lex(2, c, d));
                        state = States.S;
                        GetNext();
                    }
                    else
                        state = States.ER;
                    break;
                case States.ER:
                    Debug.Log("������ � ���������");
                    state = States.F;
                    break;
                case States.F:
                    Debug.Log("����������� ������ ��������");
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

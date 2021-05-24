using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Input : MonoBehaviour
{
    private TMP_InputField InputField;
    private string outputBuf, tempBuf;
    private SemanticType typeBuf = SemanticType.nulltype;
    private List<KeyValuePair<SemanticType, string>> _tree;
    private int mLine;
    private bool er;
    LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
    
    void Awake()
    {
        InputField = GetComponent<TMP_InputField>();
    }
    void Update()
    {
        
        if (er)
        {
            if (InputField.text[InputField.text.Length - 1] != '=') return;
            Output.Component.Reset();
            InputField.text = "";
            mLine = 0;
            er = false;
            return;
        }
        if (InputField.text.Length==0)  return;
        if (!EndOfLine(InputField.text[InputField.text.Length-1]) ) return;


        var buffer = lexicalAnalyzer.CreateTokens(InputField.text.Trim(' '));
        
        outputBuf = "";
        tempBuf = "";
        typeBuf = SemanticType.nulltype;
        _tree = new List<KeyValuePair<SemanticType, string>>();
       
       
        for (var i = mLine; i < buffer.Length; i++)
        {
            var c = buffer[i];
            if (EndOfLine(c))
            {
                _tree.Add(new KeyValuePair<SemanticType, string>(typeBuf, tempBuf));
                mLine = i+2;
                if (typeBuf == SemanticType.oper)
                {
                    er = true;
                    outputBuf = Exp;
                }
                break;
            }
            if ( WhichType(c) != SemanticType.oper)
            {
                typeBuf = WhichType(c);
                tempBuf += buffer[i];
            }
            else 
            {
                if (typeBuf == SemanticType.oper)
                {
                    er = true;
                    outputBuf = Exp;
                    break;
                }
                _tree.Add(new KeyValuePair<SemanticType, string>(typeBuf, tempBuf));
                typeBuf = WhichType(c);
                tempBuf = ""+c;
                _tree.Add(new KeyValuePair<SemanticType, string>(typeBuf, tempBuf));
                tempBuf = "";
            }
        }
       
        
        
        if (!er) outputBuf = DecodingTree(_tree);
        Output.Component.AddText(outputBuf);
        AddEnd();
    }

    private const string Exp = "Invalid expression \nInput \'=\' to restart";

    private static bool EndOfLine(char c)
    {
        return c == '=' || c == ';' || c == '.'|| c == '\t'|| c == '`';
    }

    private void AddEnd()
    {
        InputField.text += '\n';
        InputField.ActivateInputField();
        InputField.caretPosition = InputField.text.Length;
    }

    private static SemanticType WhichType(char c)
    {
        if (char.IsDigit(c)) return SemanticType.digit;
        if (c == '+' || c == '-' || c == '*' || c == '/') return SemanticType.oper;
        return SemanticType.nulltype;
    }

    private string CreatingTree()
    {
        return "";
    }

    private static string DecodingTree(IReadOnlyList<KeyValuePair<SemanticType, string>> tree)
    {
        var str = "";
        for (var i = 0; i < tree.Count; i++)
        {
            if(tree[i].Key == SemanticType.oper){
                str += tree[i+1].Value + " ";
                str += tree[i].Value + " ";
                i++;
            } else
                str += tree[i].Value + " ";
        }

        return str;
    }
}
public enum SemanticType
{
    digit,
    oper,
    expr,
    nulltype
}

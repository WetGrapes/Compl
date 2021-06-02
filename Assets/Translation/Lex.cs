using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lex 
{
        public int state;
        public string val;

        public Lex( int _stateID, string _val)
        {
            state = _stateID;
            val = _val;
        }
}

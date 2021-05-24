using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;

namespace AnalyzerViewer
{
    public abstract class AnalyzerView
    {
        public abstract string Name { get; }

        public virtual string GetView(TokenStream tokenStream, out int numberOfTokens)
        {
            StringBuilder sb = new StringBuilder();

            Token token = tokenStream.Next();

            numberOfTokens = 0;

            while (token != null)
            {
                numberOfTokens++;
                sb.Append(GetTokenView(token));
                token = tokenStream.Next();
            }

            return sb.ToString();
        }

        protected abstract string GetTokenView(Token token);
    }

    public class TermAnalyzerView : AnalyzerView
    {
        public override string Name
        {
            get { return "Terms"; }
        }

        protected override string GetTokenView(Token token)
        {
            return "[" + token.TermText() + "]   ";
        }
    }

    public class TermWithOffsetsView : AnalyzerView
    {
        public override string Name
        {
            get { return "Terms With Offsets"; }
        }

        protected override string GetTokenView(Token token)
        {
            return token.TermText() + "   Start: " + token.StartOffset().ToString().PadLeft(5) + "  End: " + token.EndOffset().ToString().PadLeft(5) + "\r\n";
        }
    }

    public class TermFrequencies : AnalyzerView
    {
        public override string Name
        {
            get { return "Term Frequencies"; }
        }

        Dictionary<string, int> termDictionary = new Dictionary<string, int>();

        public override string GetView(TokenStream tokenStream, out int numberOfTokens)
        {
            StringBuilder sb = new StringBuilder();

            Token token = tokenStream.Next();

            numberOfTokens = 0;

            while (token != null)
            {
                numberOfTokens++;

                if (termDictionary.Keys.Contains(token.TermText()))
                    termDictionary[token.TermText()] = termDictionary[token.TermText()] + 1;
                else
                    termDictionary.Add(token.TermText(), 1);

                token = tokenStream.Next();
            }

            foreach (var item in termDictionary.OrderBy(x => x.Key))
            {
                sb.Append(item.Key + " [" + item.Value + "]   ");
            }

            termDictionary.Clear();

            return sb.ToString();
        }

        protected override string GetTokenView(Token token)
        {
            throw new NotImplementedException();
        }
    }



}

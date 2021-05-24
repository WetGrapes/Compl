using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;

namespace AnalyzerViewer
{
    /// <summary>
    /// Provides the analyzer and some information about a lucene analyzer.
    /// </summary>
    public class AnalyzerInfo
    {
        /// <summary>
        /// A private field used to hold the value for LuceneAnalyzer property.
        /// </summary>
        private Analyzer _LuceneAnalyzer;

        /// <summary>
        /// Gets or sets the Lucene.Net Analyzer object.
        /// </summary>
        public Analyzer LuceneAnalyzer
        {
            get
            {
                return _LuceneAnalyzer;
            }
            set
            {
                _LuceneAnalyzer = value;
            }
        }


        /// <summary>
        /// A private field used to hold the value for Name property.
        /// </summary>
        private string _Name;

        /// <summary>
        /// Gets or sets the name of the analyzer.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        /// <summary>
        /// A private field used to hold the value for Description property.
        /// </summary>
        private string _Description;

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }




        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerInfo"/> class.
        /// </summary>
        /// <param name="name">the name of the analyzer.</param>
        /// <param name="analyzer">The Lucene.Net Analyzer to use.</param>
        public AnalyzerInfo(string name, string description,  Analyzer analyzer)
        {
            this.Name = name;
            this.Description = description;
            this.LuceneAnalyzer = analyzer;
        }
    }
}

﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;

namespace OpenXmlPowerTools
{
    class Program
    {
        static string CapText(Match m) {
            // Get the matched string.
            string x = m.ToString();
            // If the first char is lower case...
            if (char.IsLower(x[0]))
            {
                // Capitalize it.
                return char.ToUpper(x[0]) + x.Substring(1, x.Length - 1);
            }
            return x;
        }
        static string EmptyText(Match m)
        {
            return String.Empty;
        }
        static string NullText(Match m)
        {
            return null;
        }
        private static MatchEvaluator GetEvaluator() {
            return (tokenMatch) => {
               var token = TokenExcMatch.Match(tokenMatch.Value);
               if (!token.Success) { return "ERROR: Did not recognize token"; }
               var nameMatch = TokenNameMatch.Match(token.Value);
               if (!nameMatch.Success) { return "ERROR: Could not match token name in token: " + token.Value; }

               var formatMatch = FormatMatch.Match(token.Value);
               var format = formatMatch.Success ? formatMatch.Value : "";
               return nameMatch.Value;
            };
        }


        private static readonly Regex TokenIncMatch  = new Regex(@"\[\[(.*?)\]?\]\]"      , RegexOptions.Compiled);
        private static readonly Regex TokenExcMatch  = new Regex(@"(?<=\[\[)(.*)(?=\]\])" , RegexOptions.Compiled);
        private static readonly Regex FormatMatch    = new Regex(@"(?<=\{)(.*)(?=\})"     , RegexOptions.Compiled);
        private static readonly Regex TokenNameMatch = new Regex(@"^(.*?)(?=[\s|\{|\[]|$)", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var n = DateTime.Now;
            var tempDi = new DirectoryInfo(string.Format("ExampleOutput-{0:00}-{1:00}-{2:00}-{3:00}{4:00}{5:00}", n.Year - 2000, n.Month, n.Day, n.Hour, n.Minute, n.Second));
            tempDi.Create();

            DirectoryInfo di2 = new DirectoryInfo("../../../");
            foreach (var file in di2.GetFiles("*.docx"))
                file.CopyTo(Path.Combine(tempDi.FullName, file.Name));

            using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test01.docx"), true))
                TextReplacer.SearchAndReplace(doc, TokenIncMatch, GetEvaluator());

            //try
            //{
            //    using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test02.docx"), true))
            //        TextReplacer.SearchAndReplace(doc, "the", "this", false);
            //}
            //catch (Exception) { }
            //try
            //{
            //    using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test03.docx"), true))
            //        TextReplacer.SearchAndReplace(doc, "the", "this", false);
            //}
            //catch (Exception) { }
            //using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test04.docx"), true))
            //    TextReplacer.SearchAndReplace(doc, "the", "this", true);
            //using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test05.docx"), true))
            //    TextReplacer.SearchAndReplace(doc, "is on", "is above", true);
            //using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test06.docx"), true))
            //    TextReplacer.SearchAndReplace(doc, "the", "this", false);
            //using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test07.docx"), true))
            //    TextReplacer.SearchAndReplace(doc, "the", "this", true);
            //using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test08.docx"), true))
            //    TextReplacer.SearchAndReplace(doc, "the", "this", true);
            //using (WordprocessingDocument doc = WordprocessingDocument.Open(Path.Combine(tempDi.FullName, "Test09.docx"), true))
            //    TextReplacer.SearchAndReplace(doc, "===== Replace this text =====", "***zzz***", true);
        }
    }
}

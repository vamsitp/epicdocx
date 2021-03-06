﻿namespace EpicDoc
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class Extensions
    {
        internal static readonly string HeadersColor = ConfigurationManager.AppSettings["HeadersColor"];
        internal static readonly string[] ColorReplacements = ConfigurationManager.AppSettings["ColorReplacements"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        private const int Max = 200;

        public static string GetFullPath(this string file)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase.Replace("file:///", string.Empty)), file);
        }

        public static bool ContainsArg(this string[] args, string arg)
        {
            return args?.Contains($"/{arg}", StringComparer.OrdinalIgnoreCase) == true || args?.Contains($"-{arg}", StringComparer.OrdinalIgnoreCase) == true;
        }

        // Credit: https://stackoverflow.com/a/5276721
        public static string StripHtml(this string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", Environment.NewLine);

            //get rid of multiple blank lines
            output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

            return "- " + string.Join(Environment.NewLine + "- ", HttpUtility.HtmlDecode(output).Split(Environment.NewLine).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())).Trim();
        }

        public static string TrimEx(this string content)
        {
            content = Regex.Replace(content, "</?(font|span)[^>]*>", string.Empty, RegexOptions.IgnoreCase).Trim('\n');
            // content = Regex.Replace(content, "(border-color)[^;]*", $"border-color:{HeadersColor}", RegexOptions.IgnoreCase);
            // content.Replace("rgb(0, 0, 0)", HeadersColor).Replace("black", HeadersColor).Replace("#f0f0f0", HeadersColor).Replace("windowtext", HeadersColor);
            foreach (var replace in ColorReplacements)
            {
                content = content.Replace(replace, HeadersColor);
            }

            return content;
        }

        // Credit: https://stackoverflow.com/a/11463800
        public static IEnumerable<List<T>> SplitList<T>(this List<T> list, int limit = Max)
        {
            if (list?.Any() == true)
            {
                for (var i = 0; i < list.Count; i += limit)
                {
                    yield return list.GetRange(i, Math.Min(limit, list.Count - i));
                }
            }
        }

        public static void NAR(this object o)
        {
            try
            {
                if (o != null)
                {
                    Marshal.FinalReleaseComObject(o);
                }
            }
            finally
            {
                o = null;
            }
        }
    }
}

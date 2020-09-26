using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NsnbcGenerator
{
    internal class Program
    {
        private XDocument x;
        
        public static void Main(string[] args)
        {
            XDocument xdoc = XDocument.Load("nsnbc.xml");
            var comics = xdoc.Descendants("comic").ToList();
            int index = 1;
            int last = comics.Count;
            string template = File.ReadAllText("template.html");
            string obsah = File.ReadAllText("obsah.html");
            string listContents = "";
            string rss = File.ReadAllText("nsnbc.rss");
            List<string> rssItems = new List<string>();
            foreach (XElement element in comics)
            {
                string title = element.Element("title").Value;
                DateTime date = DateTime.Parse(element.Element("date").Value);
                string simpletext = element.Element("simpletext")?.Value;
                string pureAftertext = simpletext ?? element.Element("aftertext").Value;
                string aftertext = element.Element("aftertext").Value.Replace("\n", "<br>");

                if (aftertext.Contains("<br>"))
                {
                    aftertext = "<div style='text-align: center'><div style='display: inline-block; margin-left: 10px; margin-right: 10px; text-align: left;'><i>"
                                + aftertext + "</i></div></div>";
                }
                else
                {
                    aftertext = "<i>" + aftertext + "</i>";
                }
                
                string buttonrow = CreateButtonRow(index, last);
                string html = template.Replace("[[NUMBER]]", index.ToString())
                    .Replace("[[TITLE]]", title)
                    .Replace("[[AFTERTEXT]]", aftertext)
                    .Replace("[[BUTTON ROW]]", buttonrow)
                    .Replace("[[COPYRIGHT]]", "<span style='color: darkgrey;'>" + date.ToString("d. MMMM yyyy", new CultureInfo("cs")) + "</span>")
                    .Replace("[[FACEBOOK DESCRIPTION]]", FacebookIze(pureAftertext));
                rssItems.Add($"<item>\n <title>{title}</title>\n <link>https://nsnbc.neocities.org/{index.ToString()}.html</link>\n <description>{FacebookIze(pureAftertext)}</description>\n</item>");
                File.WriteAllText("output\\" + index + ".html", html);
                listContents = "<span style='color: darkgrey;'>(" + date.Year + ")</span>&nbsp;<a href='" + index + ".html'>" + "#" + index + " " + title + "</a><br>" + listContents;
                if (index == last)
                {
                    File.WriteAllText("output\\index.html", html);
                }
                index++;
            }

            rssItems.Reverse();
            rss = rss.Replace("[[ITEMS]]", string.Join("\n", rssItems));
            obsah = obsah.Replace("[[LIST]]", listContents);
            File.WriteAllText("output\\obsah.html", obsah);
            File.WriteAllText("output\\nsnbc.rss", rss);
        }

        private static string FacebookIze(string aftertext)
        {
            aftertext = Regex.Replace(aftertext, "<.*?>", "").Replace("\"", "'");
            int br = aftertext.IndexOf("\n");
            if (br == -1)
            {
                return aftertext;
            }
            else
            {
                return aftertext.Substring(0, br);
            }
        }

        private static string CreateButtonRow(int index, int last)
        {
            int first = 1;
            int previous = index - 1;
            int next = index + 1;
            return $@"<a href=""{first}.html"" class=""btn btn-primary{(index == first ? " disabled" : "")}"">První</a>
                <a href=""{previous}.html"" class=""btn btn-primary{(index == first ? " disabled" : "")}"">Předchozí</a>
                <a href=""obsah.html"" class=""btn btn-primary"">Seznam</a>
                <a href=""{next}.html"" class=""btn btn-primary{(index == last ? " disabled" : "")}"">Následující</a>
                <a href=""{last}.html"" class=""btn btn-primary{(index == last ? " disabled" : "")}"">Poslední</a>";
        }
    }
}
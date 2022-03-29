using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TestArea
{
    internal class WebExchange
    {
        static void Main(string[] args)
        {

            GetPage();
        }

        static void GetPage()
        {

            //string s = "";
            string pageHtmlText = "";

            for (int i = 0; i < 4; i++)
            {
                //s += GetInf("pr", i);
                pageHtmlText += GetPageText("pr", i);
            }

            string openTag = "<tr>", closeTag = "</tr>";
            //List<string> namesPr = Find("<tr>", "</tr>", s);
            List<string> namesAndPointsFromPracticsPages = GetDataWithoutTags(openTag, closeTag, pageHtmlText);
            // s = "";
            pageHtmlText = "";

            for (int i = 0; i < 4; i++)
            {
                //s += GetInf("lb", i);
                pageHtmlText += GetPageText("lb", i);
            }
            
            //List<string> namesLab = Find("<tr>", "</tr>", s);
            List<string> namesAndPointsFromLabPages = GetDataWithoutTags(openTag, closeTag, pageHtmlText);

            //string s1 = namesAndPointsFromPracticsPages[0].Trim() + namesAndPointsFromLabPages[0].Trim().Substring(3);
            string tableHead = namesAndPointsFromPracticsPages[0].Trim() + namesAndPointsFromLabPages[0].Trim().Substring(3);
            tableHead = tableHead.Replace(" ", "\t");
            Console.WriteLine(tableHead);

            //Dictionary<string, List<int>> names = CombineLists(namesAndPointsFromLabPages, namesAndPointsFromPracticsPages);
            Dictionary<string, List<int>> studentsList = CombineLists(namesAndPointsFromLabPages, namesAndPointsFromPracticsPages);

            //SumIndex(names);
            GetTotalScoreColumn(studentsList);

            //foreach (var item in studentsList.OrderBy(item => item.Value[item.Value.Count - 1]))
            foreach (var student in studentsList.OrderBy(item => item.Value[item.Value.Count - 1])) //sort by total score
            {
                string result = student.Key;
                for (int i = 0; i < student.Value.Count; i++)
                {
                    result += "\t" + student.Value[i];
                }
                Console.WriteLine(result);
            }
            Console.ReadKey();

        }

        static string GetPageText(string type, int pageNumber)
        {
            //string s = "";
            string currentPageText = "";
            string path = "http://playpit.ru/220/tableResultGET.php?type=" + type + "&page=" + pageNumber;
           
            WebRequest rqs = WebRequest.Create(path);
            WebResponse rsp = rqs.GetResponse();
            Stream stream = rsp.GetResponseStream();
           
            using (StreamReader reader = new StreamReader(stream))
            {
                //s = reader.ReadToEnd();
                currentPageText = reader.ReadToEnd();
            }

            stream.Close();
            rsp.Close();
            return currentPageText;
        }

        //static List<string> Find(string tegOpen, string tegClose, string s)
        static List<string> GetDataWithoutTags(string tagOpen, string tagClose, string htmlPageText)
        {
            //string[] arr = s.Split(new string[] { tegClose, tegOpen }, StringSplitOptions.None);
            string[] pageStrings = htmlPageText.Split(new string[] { tagClose, tagOpen }, StringSplitOptions.None);

            //List<string> data = new List<string>();
            List<string> resultStrings = new List<string>();
            for (int i = 0; i < pageStrings.Length; i++)
            {
                //string a = arr[i];
                string currentString = pageStrings[i];
                if ((currentString.Contains("<td>") || currentString.Contains("<th>")) && !resultStrings.Contains(currentString))
                {
                    //data.Add(a);
                    resultStrings.Add(currentString);
                }
            }
            for (int i = 0; i < resultStrings.Count; i++)
            {
                //data[i] = data[i].Replace("</td><td>", " ").Replace("<td>", "").Replace("</td>", "");
                //data[i] = data[i].Replace("</th><th>", " ").Replace("<th>", "").Replace("</th>", "");
                resultStrings[i] = ReplaceTags(resultStrings[i]);
            }
            return resultStrings;
        }

        static string ReplaceTags(string data)
        {
            data = data.Replace("</td><td>", " ").Replace("<td>", "").Replace("</td>", "");
            data = data.Replace("</th><th>", " ").Replace("<th>", "").Replace("</th>", "");

            return data;
        }

        static Dictionary<string, List<int>> CombineLists(List<string> namesLab, List<string> namesPr)
        {
            //Dictionary<string, List<int>> names = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> studentsList = new Dictionary<string, List<int>>();

            for (int i = 1; i < namesPr.Count; i++)
            {
               // string[] arrPr = namesPr[i].Trim().Split(new char[] { ' ' });
                string[] practicsResults = namesPr[i].Trim().Split(new char[] { ' ' });
                //string[] arrLab = namesLab[i].Trim().Split(new char[] { ' ' });
                string[] labResults = namesLab[i].Trim().Split(new char[] { ' ' });
                
                string name = practicsResults[0]+" "+practicsResults[1];
                //List<int> num = new List<int>();
                List<int> points = new List<int>();

                GetPoints(points, practicsResults);
                GetPoints(points, labResults);

                studentsList.Add(name, points);
            }
            return studentsList;
        }

        private static void GetPoints(List<int> points, string[] results)
        {
            for (int j = 3; j < results.Length; j++)
            {
                points.Add(Convert.ToInt32(results[j]));
            }
        }


        //private static Dictionary<string, List<int>> SumIndex(Dictionary<string, List<int>> names)
        private static void GetTotalScoreColumn(Dictionary<string, List<int>> studentsList)
        {

            //foreach (var i in names.Values)
            foreach (var student in studentsList.Values)
            {
                //int num = 0;
                int totalScore = 0;
                //foreach (var item in i)
                foreach (var points in student)
                {
                    //num += item;
                    totalScore += points;
                }
                //i.Add(num);
                student.Add(totalScore);
            }
        }

    }
}

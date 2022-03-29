using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestArea
{
    internal class WebExchangeDecompose
    {
        static void Main(string[] args)
        {
            HtmlPageData pageData = new HtmlPageData();
            DataProcessing processing = new DataProcessing(pageData);
        }
    }

    public interface IStudents
    {
        List<string> GetLabResults();
        List<string> GetPracticeResults();
    }

    public class HtmlPageData :IStudents
    {
        string pageHtmlText = "";
        List<string> namesAndPointsFromPracticsPages, namesAndPointsFromLabPages;

        public HtmlPageData()
        {
            GetPage();
        }

        private void GetPage()
        {
            for (int i = 0; i < 4; i++)
            {
                pageHtmlText += GetPageText("pr", i);
            }

            string openTag = "<tr>", closeTag = "</tr>";
            namesAndPointsFromPracticsPages = GetDataWithoutTags(openTag, closeTag, pageHtmlText);
            pageHtmlText = "";

            for (int i = 0; i < 4; i++)
            {
                pageHtmlText += GetPageText("lb", i);
            }

            namesAndPointsFromLabPages = GetDataWithoutTags(openTag, closeTag, pageHtmlText);
        }

        string GetPageText(string type, int pageNumber)
        {
            string currentPageText = "";
            string path = "http://playpit.ru/220/tableResultGET.php?type=" + type + "&page=" + pageNumber;

            WebRequest rqs = WebRequest.Create(path);
            WebResponse rsp = rqs.GetResponse();
            Stream stream = rsp.GetResponseStream();

            using (StreamReader reader = new StreamReader(stream))
            {
                currentPageText = reader.ReadToEnd();
            }

            stream.Close();
            rsp.Close();
            return currentPageText;
        }

        List<string> GetDataWithoutTags(string tagOpen, string tagClose, string htmlPageText)
        {
            string[] pageStrings = htmlPageText.Split(new string[] { tagClose, tagOpen }, StringSplitOptions.None);

            List<string> resultStrings = new List<string>();
            for (int i = 0; i < pageStrings.Length; i++)
            {
                string currentString = pageStrings[i];
                if ((currentString.Contains("<td>") || currentString.Contains("<th>")) && !resultStrings.Contains(currentString))
                {
                    resultStrings.Add(currentString);
                }
            }
            for (int i = 0; i < resultStrings.Count; i++)
            {
                resultStrings[i] = ReplaceTags(resultStrings[i]);
            }
            return resultStrings;
        }

        string ReplaceTags(string data)
        {
            data = data.Replace("</td><td>", " ").Replace("<td>", "").Replace("</td>", "");
            data = data.Replace("</th><th>", " ").Replace("<th>", "").Replace("</th>", "");

            return data;
        }

        public List<string> GetLabResults()
        {
            return namesAndPointsFromLabPages;
        }

        public List<string> GetPracticeResults()
        {
            return namesAndPointsFromPracticsPages;
        }
    }

    public class DataProcessing
    {
        List<string> namesAndPointsFromPracticsPages, namesAndPointsFromLabPages;

        public DataProcessing(IStudents students)
        {
            namesAndPointsFromLabPages = students.GetLabResults();
            namesAndPointsFromPracticsPages = students.GetPracticeResults();

            string tableHead = namesAndPointsFromPracticsPages[0].Trim() + namesAndPointsFromLabPages[0].Trim().Substring(3);
            tableHead = tableHead.Replace(" ", "\t");
            Console.WriteLine(tableHead);

            Dictionary<string, List<int>> studentsList = CombineLists(namesAndPointsFromLabPages, namesAndPointsFromPracticsPages);

            GetTotalScoreColumn(studentsList);

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

        static Dictionary<string, List<int>> CombineLists(List<string> namesLab, List<string> namesPr)
        {
            Dictionary<string, List<int>> studentsList = new Dictionary<string, List<int>>();

            for (int i = 1; i < namesPr.Count; i++)
            {
                string[] practicsResults = namesPr[i].Trim().Split(new char[] { ' ' });
                string[] labResults = namesLab[i].Trim().Split(new char[] { ' ' });

                string name = practicsResults[0] + " " + practicsResults[1];
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

        private static void GetTotalScoreColumn(Dictionary<string, List<int>> studentsList)
        {

            foreach (var student in studentsList.Values)
            {
                int totalScore = 0;
                foreach (var points in student)
                {
                    totalScore += points;
                }
                student.Add(totalScore);
            }
        }
    }

    
}

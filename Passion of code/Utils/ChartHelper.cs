using Passion_of_code.Controllers;
using QuickChart;

namespace Passion_of_code.Utils.ChartHelper
{
    public class ChartHelper
    {

        public static string getGraphUrl(List<myTup>input , string label,string type)
        {

            var Labels = "";
            var Counts = "";
            foreach (var i in input)
            {
                if (i.name == "")
                {
                    Labels += ",'No Survey'";
                }
                else
                {
                    Labels += ",'" + i.name + "'";
                }

                Counts += ",'" + i.tally.ToString() + "'";
            }
            try
            {
                Labels = Labels.Substring(1);
                Counts = Counts.Substring(1);
            }
            catch {
                Labels = "";
                Counts = "";
            }

            var myChart = new Chart();
            myChart.Width = 500;
            myChart.Height = 300;
            myChart.Version = "2.9.4";
            myChart.Config = @"{ type: '" + type +"', data: { labels: [ " +   Labels + "]" +
            @", datasets: [{ label: '" + label + "', data: [" + Counts + "] }] } }";

            Console.WriteLine(myChart.Config);
            return myChart.GetUrl();
        }
        public static string getDiff(int idx)
        {

    var diff = new List<string>(["Very Easy", "Easy", "Medium",
                    "Hard", "Very Hard", "No Survey"]);
            return diff[idx];
        }
    }
}


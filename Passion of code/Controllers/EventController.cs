using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Passion_of_code.Models;
using Passion_of_code.Utils.Content;


using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Web;
using Microsoft.Identity.Client;
using Mysqlx.Crud;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mysqlx.Prepare;
using System.Data;

namespace Passion_of_code.Controllers
{

    public class Tally {
        public int day { get; set; }
        public int tally { get; set; } = 0;
    }
    public class DaysInfo {
        public int yr;
        public List<Tally> days = new List<Tally>();
        public int total = 0;
    }

    public class Statistic {
        public List<Tuple<string,string>> Languages = new List<Tuple<string,string>>();
        public List<Tuple<string,string>> Difficulty = new List<Tuple<string,string>>();
        public List<Tuple<string,string>> Approach = new List<Tuple<string,string>>();
        public List<Tuple<string,string>> Paradigm = new List<Tuple<string,string>>();
    }

    public class ChallangeInfo {
        public int yr;
        public int day;
        public bool show_input = true;
        public string? part1_problem;
        public bool part1_solved=false;
        public string part1_solution;
        public string? part2_problem;
        public bool part2_solved=false;
        public bool part3_solved=false;
        public string part2_solution;
        public Models.Task task;
    }

    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }
        public IActionResult Year(int year)
        {
            ViewData["Title"] = String.Format("{0}", year);
            List<string> event_dirs = Directory.GetDirectories(Path.Join([Environment.CurrentDirectory ,"Views", "Event",year.ToString()])).ToList();
            List<Tally> dirs_for_year = [];
            int total = 0;
            int i = 0;

            foreach (var f in event_dirs)
            {
                dirs_for_year.Insert(i, new Tally { day = i + 1, tally = 0 } );
                i++; 
            }

            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    var task = db_context.Tasks
                                   .Where(t => t.Year == year && t.Username == uname)
                                   .ToList();
                    foreach (var t in task) {
                        if (t.Day != null)
                        {
                            for (int j = 0; j < dirs_for_year.Count;j++)
                            {
                                if (dirs_for_year[j].day == t.Day.Value) {
                                    if (t.Part1_solved.HasValue)
                                        dirs_for_year[j].tally += t.Part1_solved.Value;
                                        total += t.Part1_solved.Value;
                                    if (t.Part2_solved.HasValue)
                                        dirs_for_year[j].tally += t.Part2_solved.Value;
                                        total += t.Part2_solved.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return View(new DaysInfo{yr = year, days = dirs_for_year, total = total });
        }

        [HttpPost]
        public IActionResult Survey(int year, int day, string hours, string Language, string Approach, string Paradigm, string Difficulty) {
            if (Language != null && Approach != null && Paradigm != null && Difficulty != null)
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    var task = db_context.Tasks
                                   .Where(t => t.Year == year && t.Day == day && t.Username == uname)
                                   .ToList();
                    var crnt_task = task.First();
                    crnt_task.Language = Language;
                    crnt_task.Approach = Approach;
                    crnt_task.Paradigm = Paradigm;
                    crnt_task.Difficulty = ConstTables.Difficulty.IndexOf(Difficulty) + 1;

                    db_context.Update(crnt_task);
                    db_context.SaveChanges();
                    ViewBag.usr_msg = "Survey response saved";
                }
                return Survey(year, day);
            }
            return Survey(year, day);
        }
        public IActionResult Survey(int year, int day)
        {
         ViewData["Title"] = String.Format("Survey for {0} - Day {1}", year, day);
            string uname = HttpContext.Session.GetString("uname");
            ChallangeInfo crnt_day = new ChallangeInfo { yr = year, day = day };
            string event_dirs = Path.Join(
                [Environment.CurrentDirectory,
                "Views",
                "Event",
                year.ToString(),
                "day" + day.ToString()]
            );
            using (var db_context = new PassionOfCodeContext())
            {
                var task = db_context.Tasks
                               .Where(t => t.Year == year && t.Day == day && t.Username == uname)
                               .ToList();
                var crnt_task = task.First();

                ContentManager cm = ContentManager.Instance;
                crnt_day.part1_problem = cm.GetFile(Path.Join(event_dirs, "p1"));
                crnt_day.part1_solution = crnt_task.Part1_solution;
                crnt_day.part2_problem = cm.GetFile(Path.Join(event_dirs, "p2"));
                crnt_day.part2_solution = crnt_task.Part2_solution;
                crnt_day.task = crnt_task;

                ViewBag.lang = ConstTables.langs.ConvertAll(obj => new SelectListItem { Text = obj, Value = obj });
                if (crnt_task.Language != "") {
                    int idx = ConstTables.langs.IndexOf(crnt_task.Language);
                    if (idx > -1) {
                        ViewBag.lang[idx].Selected = true;   
                    }
                }
                ViewBag.appr = ConstTables.Approach.ConvertAll(obj => new SelectListItem { Text = obj, Value = obj });
                if (crnt_task.Approach != "") {
                    int idx = ConstTables.Approach.IndexOf(crnt_task.Approach);
                    if (idx > -1) {
                        ViewBag.appr[idx].Selected = true;   
                    }
                }
                ViewBag.para = ConstTables.Paradigm.ConvertAll(obj => new SelectListItem { Text = obj, Value = obj });
                if (crnt_task.Paradigm != "") {
                    int idx = ConstTables.Paradigm.IndexOf(crnt_task.Paradigm);
                    if (idx > -1) {
                        ViewBag.para[idx].Selected = true;   
                    }
                }
                ViewBag.diff = ConstTables.Difficulty.ConvertAll(obj => new SelectListItem { Text = obj, Value = obj });

                if (crnt_task.Difficulty != null  )
                {
                    int idx = crnt_task.Difficulty.Value  - 1;
                    if (idx > -1) {
                        ViewBag.diff[idx].Selected = true;
                    }
                }
            }
            return View(crnt_day);
        }

        [HttpPost]
        public IActionResult Day(int year,int day,int part, string answer)
        {
            if (HttpContext.Session.GetInt32("day") == day
                && HttpContext.Session.GetInt32("year") == year)
                if (part == 1 || part == 2)
                {
                    string answer_file = Path.Join(
                        [Environment.CurrentDirectory,
                    "Views",
                    "Event",
                    year.ToString(),
                    "day" + day.ToString(),
                    "ans" + part.ToString()]
                    );
                    ContentManager cm = ContentManager.Instance;
                    string expected_answer = cm.GetFile(answer_file);
                    if (expected_answer == answer)
                    {
                        Models.Task status;
                        if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
                        {
                            string uname = HttpContext.Session.GetString("uname");
                            using (var db_context = new PassionOfCodeContext())
                            {
                                var task = db_context.Tasks
                                               .Where(t => t.Year == year && t.Day == day && t.Username == uname)
                                               .ToList();
                                if (task.Count > 0)
                                {
                                    status = task.First();
                                    if (part == 1) {
                                        status.Part1_solved = 1;
                                        status.Part1_solution = answer;
                                    }
                                    if (part == 2) {
                                        status.Part2_solved = 1;
                                        status.Part2_solution = answer;
                                    }
                                    db_context.Update(status);
                                    db_context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            return Day(year, day);
        }

        public IActionResult Day(int year, int day)
        {
            ViewData["Title"] = String.Format("{0} - Day {1}", year, day);
            ChallangeInfo crnt_day = new ChallangeInfo { yr = year, day = day };
            HttpContext.Session.SetInt32("day", day);
            HttpContext.Session.SetInt32("year", year);
            string event_dirs = Path.Join(
                [Environment.CurrentDirectory,
                "Views",
                "Event",
                year.ToString(),
                "day" + day.ToString()]
            );
            ContentManager cm = ContentManager.Instance;
            crnt_day.part1_problem = cm.GetFile(Path.Join(event_dirs, "p1"));
            Models.Task status;
            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    var task = db_context.Tasks
                                   .Where(t => t.Year == year && t.Day == day && t.Username == uname)
                                   .ToList();
                    if (task.Count > 0)
                    {
                        status = task.First();
                    }
                    else { 
                        status = new Models.Task {
                            Day = day,
                            Username = uname,
                            Year = year,
                            Part1_solved = 0,
                            Part1_solution = "",
                            Part2_solved = 0,
                            Part2_solution = "",
                            Difficulty = 0,
                            Language = "",
                            Paradigm = "",
                            Approach = ""
                        };
                        db_context.Add(status);
                        db_context.SaveChanges();
                    }
                }
            }
            else {
                crnt_day.show_input = false;
                return View(crnt_day);
            }

            if (status.Part1_solved == 1) {
                crnt_day.part1_solved = true;
                crnt_day.part1_solution = status.Part1_solution;
            }

            crnt_day.part2_problem = cm.GetFile(Path.Join(event_dirs, "p2"));
            if (status.Part2_solved == 1) {
                crnt_day.part2_solved = true;
                crnt_day.part2_solution = status.Part2_solution;
            }

            if(status.Part1_solved == 1 && status.Part2_solved == 1) {
                crnt_day.task = status;

            }

            return View(crnt_day);
        }
        [Produces("text/plain")]
        public IActionResult InputForDay(int year, int day) {
            string event_dirs = Path.Join(
               [Environment.CurrentDirectory,
                "Views",
                "Event",
                year.ToString(),
                "day" + day.ToString()]
            );
            string input_path = Path.Join(event_dirs, "input");
            ContentManager cm = ContentManager.Instance;
            return Content(cm.GetFile(input_path));
        }
        public IActionResult Archive()
        {
            // TODO: add a tally for each year 
            // and for all years

            ViewData["Title"] = "Events";
            string event_dirs = Path.Join(Environment.CurrentDirectory ,"Views", "Event");

            DaysInfo years = new DaysInfo();
            foreach (string pth in Directory.GetDirectories(event_dirs).ToList() ){
                int yr = Int32.Parse(pth.Split(Path.DirectorySeparatorChar).Last());
                years.days.Add(new Tally { day = yr });
            }
            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    var task = db_context.Tasks
                                   .Where(t => t.Username == uname)
                                   .ToList();
                    foreach (var t in task) {
                        if (t.Day != null && t.Year != null)
                        {
                            for (int i = 0; i < years.days.Count;i++)
                            {
                                if (years.days[i].day == t.Year.Value) {
                                    if (t.Part1_solved.HasValue)
                                        years.days[i].tally += t.Part1_solved.Value;
                                        years.total += t.Part1_solved.Value;
                                    if (t.Part2_solved.HasValue)
                                        years.days[i].tally += t.Part2_solved.Value;
                                        years.total += t.Part2_solved.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            years.days.Reverse();
            return View(years);
        }
        
        public IActionResult StatsEvent()
        {
            ViewData["Title"] = "Event Stats";
            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    ViewBag.Used_lang = db_context.Tasks
                                        .GroupBy(p => p.Language)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10)
                                        .ToList();
                    ViewBag.count = db_context.Tasks.Count();

                    ViewBag.Approach = db_context.Tasks
                                        .GroupBy(p => p.Approach)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Paradigm = db_context.Tasks
                                        .GroupBy(p => p.Paradigm)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Difficulty = db_context.Tasks
                                        .GroupBy(p => p.Difficulty)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .ToList();

                }
            }
            return View();
        }
        public IActionResult StatsDay(int year,int day)
        {
            ViewData["Title"] = String.Format("Stats for {0} - Day {1}", year, day);
            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    ViewBag.Used_lang = db_context.Tasks
                                        .Where(t => t.Day == day && t.Year == year)
                                        .GroupBy(p => p.Language)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10)
                                        .ToList();
                    ViewBag.count = db_context.Tasks
                                        .Where(t => t.Day == day && t.Year == year)
                                        .Count();

                    ViewBag.Approach = db_context.Tasks
                                        .Where(t => t.Day == day && t.Year == year)
                                        .GroupBy(p => p.Approach)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Paradigm = db_context.Tasks
                                        .Where(t => t.Day == day && t.Year == year)
                                        .GroupBy(p => p.Paradigm)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Difficulty = db_context.Tasks
                                        .Where(t => t.Day == day && t.Year == year)
                                        .GroupBy(p => p.Difficulty)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .ToList();

                }
            }
            return View();
        }

        public IActionResult StatsYear(int year)
        {
            ViewData["Title"] = String.Format("Stats for year {0}", year);
            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    ViewBag.Used_lang = db_context.Tasks
                                        .Where(t => t.Year == year)
                                        .GroupBy(p => p.Language)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10)
                                        .ToList();
                    ViewBag.count = db_context.Tasks
                                        .Where(t =>  t.Year == year)
                                        .Count();

                    ViewBag.Approach = db_context.Tasks
                                        .Where(t => t.Year == year)
                                        .GroupBy(p => p.Approach)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Paradigm = db_context.Tasks
                                        .Where(t => t.Year == year)
                                        .GroupBy(p => p.Paradigm)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Difficulty = db_context.Tasks
                                        .Where(t => t.Year == year)
                                        .GroupBy(p => p.Difficulty)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .ToList();

                }
            }
            return View();
        }

        public IActionResult StatsUser()
        {
            ViewData["Title"] = "User Stats";
            if (HttpContext.Session.GetString("uname") != null) // we are logged in, we dont query unless we are registered 
            {
                string uname = HttpContext.Session.GetString("uname");
                using (var db_context = new PassionOfCodeContext())
                {
                    ViewBag.Used_lang = db_context.Tasks
                                        .Where(t => t.Username == uname)
                                        .GroupBy(p => p.Language)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10)
                                        .ToList();
                    ViewBag.count = db_context.Tasks
                                        .Where(t => t.Username == uname)
                                        .Count();

                    ViewBag.Approach = db_context.Tasks
                                        .Where(t => t.Username == uname)
                                        .GroupBy(p => p.Approach)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Paradigm = db_context.Tasks
                                        .Where(t => t.Username == uname)
                                        .GroupBy(p => p.Paradigm)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .Take(10).ToList();

                    ViewBag.Difficulty = db_context.Tasks
                                        .Where(t => t.Username == uname)
                                        .GroupBy(p => p.Difficulty)
                                        .Select(g => new { name = g.Key, count = g.Count() })
                                        .ToList();

                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


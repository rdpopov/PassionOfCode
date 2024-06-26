using Microsoft.EntityFrameworkCore;

namespace Passion_of_code.Models
{

    [PrimaryKey(nameof(Username))]
    public class User
    {
        public string Username { get; set; }
        public string Passwd_hash { get; set; }
        public string Eail { get; set; }

    }
    [PrimaryKey(nameof(Year),nameof(Day), nameof(Username))]
    public class Task
    {
        public int? Year { get; set; }
        public int? Day { get; set; }
        public string? Username { get; set; }
        public int? Part1_solved { get; set; }
        public string? Part1_solution { get; set; }
        public int? Part2_solved { get; set; }
        public string? Part2_solution { get; set; }
        public int? Difficulty { get; set; }
        public string? Language { get; set; }
        public string? Paradigm { get; set; }
        public string? Approach { get; set; }

    }
}

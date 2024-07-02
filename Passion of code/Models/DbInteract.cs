using Microsoft.EntityFrameworkCore;

namespace Passion_of_code.Models
{
    public class ConstTables
    {
        public static List<string> langs = new List<string>{
       "ABAP", "Ada", "Agilent" , "VEE", "Algol", "APL", "Assembly", "ATLAS",
       "AutoHotkey", "AutoIt", "Awk", "Bash", "Basic", "Brainfuck", "C", "C#",
       "C++", "Clojure", "COBOL", "Crystal", "D", "Dart", "Elixir", "Erlang",
       "Excel", "F#", "Forth", "Fortran", "GNU Octave", "Go", "Groovy", "Hack",
       "Haskell", "Haxe", "INTERCAL", "J", "Java", "Julia", "Kotlin", "Lisp",
       "Logo", "Lua", "Mojo", "Nim", "Nix", "OCaml", "Pascal", "Perl", "PHP",
       "Pliant", "PowerShell", "Python", "R", "Racket", "Raku", "Scala",
       "Scheme", "Scratch", "sed", "SQL", "Tcl", "Tex", "Vala", "VBScript",
       "VHDL", "Vim", "Visual Basic", "Wolfram Mathematica", "XSLT", "yacc",
       "Yorick", "Zig", "Other" };

        public static List<string> Paradigm = new List<string> {
            "Array",
            "Logic",
            "Procedural",
            "OO",
            "Functional",
            "Reactive",
            "Maths",
            "Declerative",
        };

        public static List<string> Approach = new List<string> {
            "Brute Force",
            "Known Algoritm",
            "Analytical Solution",
            "Approximation",
        };

        public static List<string> Difficulty = new List<string> {
                    "Very Easy",
                    "Easy",
                    "Medium",
                    "Hard",
                    "Very Hard"
        };
    }

    public class PassionOfCodeContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }

        // public PassionOfCodeContext(DbContextOptions options) :base(options){ 
        // 
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            optionsBuilder.UseSqlServer(config["ConnectionStrings"]);
        }
    }
}

using System;
using System.IO;

namespace Hydra_compiler
{
    public class Driver
    {
        const string VERSION = "0.2";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis"
        };

        //-----------------------------------------------------------
        void PrintAppHeader()
        {
            Console.WriteLine("Hydra compiler, version " + VERSION);
            Console.WriteLine("Copyright \u00A9 2020-2020 Ariel Ortiz, ITESM CEM\nmodified by: Jesús Perea, Jorge López, Gerardo Galván.");
            Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This program has absolutely no warranty.");
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes()
        {
            Console.WriteLine("Included in this release:");
            foreach (var phase in ReleaseIncludes)
            {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
        void Run(string[] args)
        {

            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length != 1)
            {
                Console.Error.WriteLine(
                    "Please specify the name of the input file.");
                Environment.Exit(1);
            }

            if (args.Length == 1)
            {
                try
                {
                    var inputPath = args[0];
                    var input = File.ReadAllText(inputPath);
                    var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                    var program = parser.Prog();
                    Console.WriteLine(program.ToStringTree());

                }
                catch (Exception e)
                {

                    if (e is FileNotFoundException || e is SyntaxError)
                    {
                        Console.Error.WriteLine(e.Message);
                        Environment.Exit(1);
                    }

                    throw;
                }
            }
            else
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program = parser.Prog();
                Console.WriteLine(program.ToStringTree());
            }
        }

        //-----------------------------------------------------------
        public static void Main(string[] args)
        {
            new Driver().Run(args);
        }
    }
}
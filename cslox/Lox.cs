using System;
using System.Collections.Generic;
using System.Collections;

namespace cslox
{
    internal class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();
        static Boolean hadError = false;
        static Boolean hadRuntimeError = false;
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }
        private static void RunFile(string path)
        {
            string source = System.IO.File.ReadAllText(path);
            Run(source);

            if (hadError)
                System.Environment.Exit(65);
            if (hadRuntimeError)
                System.Environment.Exit(65);
        }
        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null)
                {
                    // System.Environment.Exit(65);
                    break;
                }
                Run(line);
                hadError = false;
            }
        }
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt?> statements = parser.Parse();

            if (hadError) return;

            interpreter.Interpret(statements);
        }

        public static void Error(int line, String message)
        {
            Report(line, "", message);
        }
        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
        public static void Error(Token token, String message)
        {
            if (token.type == TokenType.EOF)
            {
                Report(token.line, " at end", message);
            }
            else
            {
                Report(token.line, " at '" + token.lexeme + "'", message);
            }
        }
        public static void RuntimeError(RuntimeError error)
        {
            if (error.token != null)
            {
                Console.Error.WriteLine(error.GetMessage() + "\n[line " + error.token.line + "]");
            } else
            {
                Console.Error.WriteLine(error.GetMessage());
            }
            
            hadRuntimeError = true;
        }
    }
}
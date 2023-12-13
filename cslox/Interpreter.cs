using System;
using System.Collections.Generic;
using System.Text;

namespace cslox
{
    public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        private cslox.Environment environment = new cslox.Environment();
        public void Interpret(List<Stmt?> statements)
        {
            try
            {
                foreach (Stmt? statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }
        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }
        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            object? left = Evaluate(expr.left);

            if (expr.op.type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
        }
        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            object? right = Evaluate(expr.right);

            switch (expr.oper.type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.oper, right);
                    return -(double?)right;
            }

            return null;
        }
        public object? VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.name);
        }
        private void CheckNumberOperand(Token oper, object? operand)
        {
            if (operand is double) return;
            throw new RuntimeError(oper, "Operand must be a number.");
        }
        private void CheckNumberOperand(Token oper, object? left, object? right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(oper, "Operand must be a number.");
        }
        private bool IsTruthy(object? obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }
        private bool IsEqual(object? a, object? b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }
        private string Stringify(object? obj)
        {
            if (obj == null) return "nil";

            object objNN = (object)obj;

            if (objNN is double || objNN is double?)
            {
                double dblObj = (double)objNN;
                string text = dblObj.ToString();
                if ((text).EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            if (objNN is bool || objNN is bool?)
            {
                bool boolObj = (bool)objNN;
                string text = boolObj.ToString();
                return text.ToLower();
            }

            return (string)objNN;
        }
        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }
        private object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }
        private void Execute(Stmt? stmt)
        {
            if (stmt == null) { } else stmt.Accept(this);
        }
        void ExecuteBlock(List<Stmt?> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Stmt? statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }
        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new cslox.Environment(environment));
            return null;
        }
        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }
        public object? VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }
            return null;
        }
        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            object? value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }
        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);
            return null;
        }
        public object? VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
            return null;
        }
        public object? VisitAssignExpr(Expr.Assign expr)
        {
            object? value = Evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
        }
        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            object? left = Evaluate(expr.left);
            object? right = Evaluate(expr.right);

            switch (expr.oper.type)
            {
                case TokenType.GREATER:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left > (double?)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left >= (double?)right;
                case TokenType.LESS:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left < (double?)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left <= (double?)right;
                case TokenType.MINUS:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left - (double?)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.oper, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left / (double?)right;
                case TokenType.STAR:
                    CheckNumberOperand(expr.oper, left, right);
                    return (double?)left * (double?)right;
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
            }

            return null;
        }
    }
}
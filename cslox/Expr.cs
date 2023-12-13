using System;
using System.Collections.Generic;

namespace cslox
{
  public abstract class Expr
  {
      public abstract R Accept<R>(Visitor<R> visitor);
      public interface Visitor<R>
      {
          public R VisitAssignExpr(Assign expr);
          public R VisitBinaryExpr(Binary expr);
          public R VisitGroupingExpr(Grouping expr);
          public R VisitLiteralExpr(Literal expr);
          public R VisitLogicalExpr(Logical expr);
          public R VisitUnaryExpr(Unary expr);
          public R VisitVariableExpr(Variable expr);
     }
      public class Assign : Expr
      {
          public Assign(Token name, Expr value)
          {
              this.name = name;
              this.value = value;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitAssignExpr(this);
          }

          public readonly Token name;
          public readonly Expr value;
      }
      public class Binary : Expr
      {
          public Binary(Expr left, Token oper, Expr right)
          {
              this.left = left;
              this.oper = oper;
              this.right = right;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitBinaryExpr(this);
          }

          public readonly Expr left;
          public readonly Token oper;
          public readonly Expr right;
      }
      public class Grouping : Expr
      {
          public Grouping(Expr expression)
          {
              this.expression = expression;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitGroupingExpr(this);
          }

          public readonly Expr expression;
      }
      public class Literal : Expr
      {
          public Literal(Object? value)
          {
              this.value = value;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitLiteralExpr(this);
          }

          public readonly Object? value;
      }
      public class Logical : Expr
      {
          public Logical(Expr left, Token op, Expr right)
          {
              this.left = left;
              this.op = op;
              this.right = right;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitLogicalExpr(this);
          }

          public readonly Expr left;
          public readonly Token op;
          public readonly Expr right;
      }
      public class Unary : Expr
      {
          public Unary(Token oper, Expr right)
          {
              this.oper = oper;
              this.right = right;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitUnaryExpr(this);
          }

          public readonly Token oper;
          public readonly Expr right;
      }
      public class Variable : Expr
      {
          public Variable(Token name)
          {
              this.name = name;
          }

          public override R Accept<R>(Visitor<R> visitor)
         {
              return visitor.VisitVariableExpr(this);
          }

          public readonly Token name;
      }
  }
}

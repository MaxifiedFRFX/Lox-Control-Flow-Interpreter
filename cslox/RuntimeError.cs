using System;
using System.Collections.Generic;
using System.Text;

namespace cslox
{
    public class RuntimeError : Exception
    {
        public readonly Token? token;
        readonly string message;
        public RuntimeError(Token token, string message) : base(message)
        {
            this.token = token;
            this.message = message;
            //throw new Exception(message);
        }
        public RuntimeError(string message) : base(message)
        {
            this.token = null;
            this.message = message;
            //throw new Exception(message);
        }
        public string GetMessage()
        {
            return this.message;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Common.Errors;

public class Error
{
    public abstract class Error
    {
        public string Message { get; set; }

        public virtual Exception AsException()
        {
            return new Exception(Message);
        }

        public virtual Exception AsException(Exception innerException)
        {
            return new Exception(Message, innerException);
        }

        public void Throw()
        {
            throw AsException();
        }

        public void Throw(Exception innerException)
        {
            throw AsException(innerException);
        }
    }
}
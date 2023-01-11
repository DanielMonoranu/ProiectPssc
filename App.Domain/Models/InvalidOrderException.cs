using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    [Serializable]
    public class InvalidOrderException : Exception
    {
        public InvalidOrderException() { 
        

        }

        public InvalidOrderException(string message) : base(message) { 
        

        }

        public InvalidOrderException(string? message, Exception? innerException) : base(message, innerException){
        
        
        }

        protected InvalidOrderException(SerializationInfo info, StreamingContext context) : base(info, context){


        }
    }
}

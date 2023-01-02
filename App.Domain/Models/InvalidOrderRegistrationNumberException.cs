using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    [Serializable]
    public class InvalidOrderRegistrationNumberException : Exception
    {
        public InvalidOrderRegistrationNumberException() { 
        

        }

        public InvalidOrderRegistrationNumberException(string? message) : base(message) { 
        

        }

        public InvalidOrderRegistrationNumberException(string? message, Exception? innerException) : base(message, innerException){


        }

        protected InvalidOrderRegistrationNumberException(SerializationInfo info, StreamingContext context) : base(info, context){


        }
    }
}

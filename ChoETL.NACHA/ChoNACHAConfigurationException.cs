using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL
{
    [Serializable]
    public class ChoNACHAConfigurationException : ApplicationException
    {
        public ChoNACHAConfigurationException()
            : base()
        {
        }

        public ChoNACHAConfigurationException(string message)
            : base(message)
        {
        }

        public ChoNACHAConfigurationException(string message, Exception e)
            : base(message, e)
        {
        }

        protected ChoNACHAConfigurationException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}

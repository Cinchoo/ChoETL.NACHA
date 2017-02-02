using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHAConfiguration : ChoManifoldRecordConfiguration
    {
        private string PriorityCode
        {
            get;
            set;
        }

        public string RoutingNumber
        {
            get;
            set;
        }

        public string ACHIdentificationNumber
        {
            get;
            set;
        }

        public char FileIDModifier
        {
            get;
            set;
        }

        public int BlockingFactor
        {
            get;
            set;

        }

        public uint FormatCode { get; set; }

        public ChoNACHAConfiguration()
        {
            PriorityCode = "01";
            FileIDModifier = 'A';
            BlockingFactor = 10;
            FormatCode = 1;
        }
        public void Validate()
        {
            if (PriorityCode.IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("Priority Code must be not empty.");
            if (RoutingNumber.IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("Routing Number must be not empty.");
            if (ACHIdentificationNumber.IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("ACH Identification Number must be not empty.");
            if (FileIDModifier.ToString().IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("FileIDModifier must be not empty.");
        }
    }
}

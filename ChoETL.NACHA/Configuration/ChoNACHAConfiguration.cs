using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHAConfiguration : ChoManifoldRecordConfiguration
    {
        public string PriorityCode
        {
            get;
            set;
        }

        public string DestinationBankRoutingNumber
        {
            get;
            set;
        }

        public string OriginatingCompanyId
        {
            get;
            set;
        }

        public char FileIDModifier
        {
            get;
            set;
        }

        public uint BlockingFactor
        {
            get;
            set;

        }

        public uint FormatCode { get; set; }

        public string DestinationBankName
        {
            get;
            set;
        }

        public string OriginatingCompanyName
        {
            get;
            set;
        }

        public string ReferenceCode { get; set; }
		public uint BatchNumber { get; set; }
		public Func<uint> BatchNumberGenerator { get; set; }

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
            if (DestinationBankRoutingNumber.IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("Destination Bank Routing Number must be not empty.");
            string v = DestinationBankRoutingNumber;
            if (!(((v.Length == 9 && !v.Where(c => !Char.IsDigit(c)).Any()) /*|| (v.Length == 10 && v[0] == ' ' && !v.Skip(1).Where(c => !Char.IsDigit(c)).Any())*/)))
            {
                throw new ChoNACHAConfigurationException("Invalid Destination Bank Routing Number specified.");
            }
            if (OriginatingCompanyId.IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("Originating Company Id must be not empty.");
            v = OriginatingCompanyId;
            if (!(((v.Length == 9 && !v.Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && v[0] == ' ' && !v.Skip(1).Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && !v.Where(c => !Char.IsDigit(c)).Any()))))
            {
                throw new ChoNACHAConfigurationException("Invalid Originating Company Id specified.");
            }
            if (FileIDModifier.ToString().IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("FileIDModifier must be not empty.");
            if (DestinationBankName.IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("Destination Bank Name must be not empty.");
            if (OriginatingCompanyName.ToString().IsNullOrWhiteSpace())
                throw new ChoNACHAConfigurationException("Originating Company Name must be not empty.");

        }
    }
}

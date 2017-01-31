using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    [ChoFixedLengthRecordObject(94)]
    public class ChoBatchHeaderRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 5.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.BatchHeader)]
        [ChoFixedLengthRecordField(0, 1)]
        public ChoRecordTypeCode RecordTypeCode { get; private set; }

        [ChoFixedLengthRecordField(1, 3)]
        public int ServiceClassCode { get; set; }

        [ChoFixedLengthRecordField(4, 16)]
        public string CompanyName { get; set; }

        [ChoFixedLengthRecordField(20, 20)]
        public string CompanyDiscretionaryData { get; set; }

        [ChoFixedLengthRecordField(40, 10)]
        public string CompanyID { get; set; }

        [ChoFixedLengthRecordField(50, 3)]
        public string StandardEntryClassCode { get; set; }

        [ChoFixedLengthRecordField(53, 10)]
        public string CompanyEntryDescription { get; set; }

        [ChoFixedLengthRecordField(63, 6)]
        [ChoTypeConverter(typeof(ChoDateTimeConverter), Parameters = "yyMMdd")]
        public DateTime? CompanyDescriptiveDate { get; set; }

        [ChoFixedLengthRecordField(69, 6)]
        [ChoTypeConverter(typeof(ChoDateTimeConverter), Parameters = "yyMMdd")]
        public DateTime? EffectiveEntryDate { get; set; }

        [ChoFixedLengthRecordField(75, 3)]
        public string JulianSettlementDate { get; set; }

        [DefaultValue('1')]
        [ChoFixedLengthRecordField(78, 1)]
        public char OriginatorStatusCode { get; set; }

        [ChoFixedLengthRecordField(79, 8)]
        public string OriginatingDFIID { get; set; }

        [ChoFixedLengthRecordField(87, 7)]
        [Range(1, Int32.MaxValue, ErrorMessage = "Batch number must be > 0.")]
        public int BatchNumber { get; set; }
    }
}

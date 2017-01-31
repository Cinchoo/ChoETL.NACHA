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

        /// <summary>
        /// Identifies the type of entries in the batch
        ///     200 – ACH Entries Mixed Debits and Credits
        ///     220 – ACH Credits Only
        ///     225 – ACH Debits Only
        /// </summary>
        [ChoFixedLengthRecordField(1, 3)]
        public int ServiceClassCode { get; set; }

        /// <summary>
        /// Put the name of your company here. 
        /// </summary>
        [ChoFixedLengthRecordField(4, 16)]
        public string CompanyName { get; set; }

        /// <summary>
        /// This field allows you to include codes for internal purposes.
        /// </summary>
        [ChoFixedLengthRecordField(20, 20)]
        public string CompanyDiscretionaryData { get; set; }

        /// <summary>
        /// A 10-digit number assigned to you by the ODFI once they approve you to originate ACH files through them. 
        /// This is the same as the "Immediate origin" field in File Header Record
        /// </summary>
        [ChoFixedLengthRecordField(40, 10)]
        public string CompanyID { get; set; }

        /// <summary>
        /// If the entries are PPD (credits/debits towards consumer account), use "PPD". 
        /// If the entries are CCD (credits/debits towards corporate account), use "CCD". 
        /// The difference between the 2 class codes are outside of the scope of this post, 
        /// but generally most ACH transfers to consumer bank accounts should use "PPD"
        /// </summary>
        [ChoFixedLengthRecordField(50, 3)]
        [DefaultValue("PPD")]
        public string StandardEntryClassCode { get; set; }

        /// <summary>
        /// Your description of the transaction. This text will appear on the receivers’ bank statement. 
        /// For example: "Payroll   "
        /// </summary>
        [ChoFixedLengthRecordField(53, 10)]
        public string CompanyEntryDescription { get; set; }

        /// <summary>
        /// The date you choose to identify the transactions in YYMMDD format. 
        /// This date may be printed on the receivers’ bank statement by the RDFI
        /// </summary>
        [ChoFixedLengthRecordField(63, 6)]
        [ChoTypeConverter(typeof(ChoDateTimeConverter), Parameters = "yyMMdd")]
        public DateTime? CompanyDescriptiveDate { get; set; }

        /// <summary>
        /// Date transactions are to be posted to the receivers’ account. 
        /// You almost always want the transaction to post as soon as possible, so put tomorrow's date in YYMMDD format
        /// </summary>
        [ChoFixedLengthRecordField(69, 6)]
        [ChoTypeConverter(typeof(ChoDateTimeConverter), Parameters = "yyMMdd")]
        public DateTime? EffectiveEntryDate { get; set; }

        /// <summary>
        /// Always blank (just fill with spaces)
        /// </summary>
        [ChoFixedLengthRecordField(75, 3)]
        public string JulianSettlementDate { get; set; }

        [ChoFixedLengthRecordField(78, 1)]
        [DefaultValue('1')]
        [ChoGenericValidator("v => v != '0' && Char.IsDigit(v)", ErrorMessage = "OriginatorStatusCode must be 1-9.")]
        public char OriginatorStatusCode { get; set; }

        /// <summary>
        /// Your ODFI's routing number without the last digit. 
        /// The last digit is simply a checksum digit, which is why it is not necessary
        /// </summary>
        [ChoFixedLengthRecordField(79, 8)]
        public string OriginatingDFIID { get; set; }

        /// <summary>
        /// Sequential number of this Batch Header Record. 
        /// For example, put "1" if this is the first Batch Header Record in the file
        /// </summary>
        [ChoFixedLengthRecordField(87, 7)]
        [Range(1, Int32.MaxValue, ErrorMessage = "Batch number must be > 0.")]
        public int BatchNumber { get; set; }
    }
}

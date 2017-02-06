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
    [ChoRecordTypeCode(ChoRecordTypeCode.BatchControl)]
    public class ChoNACHABatchControlRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 8.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.BatchControl)]
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
        /// The total of all Entry Detail and Addenda Records in the batch.
        /// </summary>
        [ChoFixedLengthRecordField(4, 6)]
        [Range(0, ulong.MaxValue, ErrorMessage = "Entry/Addenda count must be >= 0.")]
        public ulong EntryAddendaCount { get; set; }

        /// <summary>
        /// This is the sum of all the RDFI routing numbers in each Entry Detail Record in the batch.
        /// Sum the first 8 digits only and truncate to the left.
        /// Total of all positions 4-11 on each 6 record (Detail)
        /// </summary>
        [ChoFixedLengthRecordField(10, 10)]
        public ulong EntryHash { get; set; }

        /// <summary>
        /// This is the total dollar amount of all debit entries contained in the batch.
        /// </summary>
        [ChoFixedLengthRecordField(20, 12)]
        public decimal TotalDebitEntryDollarAmount { get; set; }

        /// <summary>
        /// This is the total dollar amount of all debit entries contained in the batch.
        /// </summary>
        [ChoFixedLengthRecordField(32, 12)]
        public decimal TotalCreditEntryDollarAmount { get; set; }

        /// <summary>
        /// This should match the company identification number used in the corresponding batch header record, field 5.
        /// </summary>
        [ChoFixedLengthRecordField(44, 10)]
        public string CompanyID { get; set; }

        /// <summary>
        /// This is an optional field. Please leave this field blank.
        /// </summary>
        [ChoFixedLengthRecordField(54, 19)]
        public string MessageAuthenticationCode { get; set; }

        /// <summary>
        /// This field is reserved for Federal Reserve use. Please leave this field blank.
        /// </summary>
        [ChoFixedLengthRecordField(73, 6)]
        public string Reserved { get; set; }

        /// <summary>
        /// This is the first 8 digits of Bank's Routing Number.
        /// </summary>
        [ChoFixedLengthRecordField(79, 8)]
        public string OriginatingDFIID { get; set; }

        [ChoFixedLengthRecordField(87, 7)]
        [Range(1, ulong.MaxValue, ErrorMessage = "Batch number must be > 0.")]
        public ulong BatchNumber { get; set; }
    }
}

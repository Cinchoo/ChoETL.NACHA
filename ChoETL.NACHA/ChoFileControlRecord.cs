using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChoETL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChoETL.NACHA
{
    [ChoFixedLengthRecordObject(94)]
    public partial class ChoFileControlRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 1.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.FileControl)]
        [ChoFixedLengthRecordField(0, 1)]
        public ChoRecordTypeCode RecordTypeCode { get; private set; }

        /// <summary>
        /// This is the total number of ACH batches included in the file.
        /// </summary>
        [ChoFixedLengthRecordField(1, 6)]
        [Range(1, Int32.MaxValue, ErrorMessage = "Batch count must be > 0.")]
        public int BatchCount { get; set; }

        /// <summary>
        /// This is the total number of blocks included in the file.The block count x
        /// 10 = total number of records
        /// </summary>
        [ChoFixedLengthRecordField(7, 6)]
        [Range(1, Int32.MaxValue, ErrorMessage = "Block count must be > 0.")]
        public int BlockCount { get; set; }

        /// <summary>
        /// The total of all Entry Detail and Addenda Records in the batch.
        /// </summary>
        [ChoFixedLengthRecordField(13, 8)]
        [Range(1, Int32.MaxValue, ErrorMessage = "Entry/Addenda count must be > 0.")]
        public int EntryAddendaCount { get; set; }

        /// <summary>
        /// This is the sum of all the RDFI routing numbers in each Entry Detail Record in the batch.
        /// Sum the first 8 digits only and truncate to the left.
        /// Total of all positions 4-11 on each 6 record (Detail)
        /// </summary>
        [ChoFixedLengthRecordField(21, 10)]
        public int EntryHash { get; set; }

        /// <summary>
        /// This is the total dollar amount of all debit entries contained in the batch.
        /// </summary>
        [ChoFixedLengthRecordField(31, 12)]
        public decimal TotalDebitEntryDollarAmount { get; set; }

        /// <summary>
        /// This is the total dollar amount of all debit entries contained in the batch.
        /// </summary>
        [ChoFixedLengthRecordField(43, 12)]
        public decimal TotalCreditEntryDollarAmount { get; set; }

        /// <summary>
        /// This field is reserved for Federal Reserve use. Please leave this field blank.
        /// </summary>
        [ChoFixedLengthRecordField(55, 39)]
        public string Reserved { get; set; }
    }
}

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
    [ChoRecordTypeCode(ChoRecordTypeCode.FileHeader)]
    public partial class ChoFileHeaderRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 1.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.FileHeader)]
        [ChoFixedLengthRecordField(0, 1)]
        public ChoRecordTypeCode RecordTypeCode { get; private set; }

        /// <summary>
        /// The Lower the number, the higher processing priority. Currently, only 01 is used.
        /// </summary>
        [DefaultValue("01")]
        [ChoFixedLengthRecordField(1, 2)]
        public string PriorityCode { get; set; }

        /// <summary>
        /// Bank routing number preceded by a space.
        /// </summary>
        [ChoFixedLengthRecordField(3,10)]
        public string ImmediateDestination { get; set; }

        /// <summary>
        /// This is the ACH identification number preceded by a space.
        /// </summary>
        [ChoFixedLengthRecordField(13, 10)]
        public string ImmediateOrigin { get; set; }

        /// <summary>
        /// The date you created the input file
        /// </summary>
        [ChoFixedLengthRecordField(23, 6)]
        [ChoDefaultValue("() => DateTime.Today")]
        [ChoTypeConverter(typeof(ChoDateTimeConverter), Parameters = "yyMMdd")]
        public DateTime FileCreationDate { get; private set; }

        /// <summary>
        /// Time of day you created the input file.
        /// </summary>
        [ChoFixedLengthRecordField(29, 4)]
        [ChoDefaultValue("() => DateTime.Now")]
        [ChoTypeConverter(typeof(ChoDateTimeConverter), Parameters = "HHmm")]
        public DateTime FileCreationTime { get; private set; }

        /// <summary>
        /// This helps identify multiple files created on the same date. A is the first file; B is the second, etc.
        /// </summary>
        [ChoFixedLengthRecordField(33, 1)]
        [DefaultValue("A")]
        public char FileIDModifier { get; set; }

        /// <summary>
        /// This is the total number of characters contained in each record within your NACHA formatted file.
        /// Default is 94.
        /// </summary>
        [ChoFixedLengthRecordField(34, 3)]
        [DefaultValue(94)]
        public uint RecordSize { get; private set; }

        /// <summary>
        /// This is the number of records that will be imported into the ACH system at one time.Please do not change.
        /// If the total number of records contained in your file are not a multiple of 10, the remaining records must be ‘9’ filled.
        /// Block at 10.
        /// </summary>
        [ChoFixedLengthRecordField(37, 2)]
        [DefaultValue(10)]
        [Range(1, uint.MaxValue, ErrorMessage = "Blocking Factor must be > 0.")]
        public uint BlockingFactor { get; set; }

        /// <summary>
        /// Currently there is only one code. Enter 1.
        /// </summary>
        [ChoFixedLengthRecordField(39, 1)]
        [DefaultValue(1)]
        [Range(1, 9, ErrorMessage = "Format Code must be 1-9.")]
        public uint FormatCode { get; set; }

        /// <summary>
        /// Enter Destination Bank Name. This is where the file is going, the name of your bank
        /// </summary>
        [ChoFixedLengthRecordField(40, 23)]
        public string ImmediateDestinationName { get; set; }

        /// <summary>
        /// Your company’s name, up to 23 characters
        /// </summary>
        [ChoFixedLengthRecordField(63, 23)]
        public string ImmediateOriginName { get; set; }

        /// <summary>
        /// Optional field you may use to describe input file for internal accounting purposes
        /// </summary>
        [ChoFixedLengthRecordField(86, 8)]
        public string ReferenceCode { get; set; }
    }
}

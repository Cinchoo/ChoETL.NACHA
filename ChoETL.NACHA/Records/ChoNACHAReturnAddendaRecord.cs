using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    [ChoFixedLengthRecordObject(94, ObjectValidationMode = ChoObjectValidationMode.ObjectLevel)]
    [ChoRecordTypeCode(ChoRecordTypeCode.Addenda)]
    public class ChoNACHAReturnAddendaRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 7.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.Addenda)]
        [ChoFixedLengthRecordField(0, 1)]
        public ChoRecordTypeCode RecordTypeCode { get; set; }

        /// <summary>
        /// Two digit code identifying the type of information contained in the addenda record:
        /// Code from this list:
        ///     02 � Used for the POS, MTE and SHR standard entry classes.The addenda information is used for terminal location information.
        ///     05 � Used for CCD, CTX, and PPD standard entry classes.The Addenda information contains additional payment related information.
        ///     98 � Used for notification of Change entries. The addenda record contains the correct information.
        ///     99 � Used for Return Entries
        /// </summary>
        [ChoFixedLengthRecordField(1, 2)]
        public uint AddendaTypeCode { get; set; }

        [ChoFixedLengthRecordField(3, 3)]
        public string ReturnReasonCode { get; set; }

        [ChoFixedLengthRecordField(6, 15)]
        public string OriginalEntryTraceNumber { get; set; }

        [ChoFixedLengthRecordField(21, 6)]
        public string DateOfDeath { get; set; }

        [ChoFixedLengthRecordField(27, 8)]
        public string OriginalReceivingDFIIdentification { get; set; }


        [ChoFixedLengthRecordField(35, 44)]
        public string AddendaInformation { get; set; }

        /// <summary>
        /// The Bank will assign a trace number.
        /// </summary>
        [ChoFixedLengthRecordField(79, 15)]
        [Range(1, ulong.MaxValue, ErrorMessage = "Entry Detail Sequence Number must be > 0.")]
        public ulong EntryDetailSequenceNumber { get; set; }
    }
}

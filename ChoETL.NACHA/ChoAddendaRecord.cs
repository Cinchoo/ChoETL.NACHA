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
    [ChoRecordTypeCode(ChoRecordTypeCode.Addenda)]
    public class ChoAddendaRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 7.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.Addenda)]
        [ChoFixedLengthRecordField(0, 1)]
        public ChoRecordTypeCode RecordTypeCode { get; private set; }

        /// <summary>
        /// Two digit code identifying the type of information contained in the addenda record:
        /// Code from this list:
        ///     02 – Used for the POS, MTE and SHR standard entry classes.The addenda information is used for terminal location information.
        ///     05 – Used for CCD, CTX, and PPD standard entry classes.The Addenda information contains additional payment related information.
        ///     98 – Used for notification of Change entries. The addenda record contains the correct information.
        ///     99 – Used for Return Entries
        /// </summary>
        [ChoFixedLengthRecordField(1, 2)]
        public uint AddendaTypeCode { get; set; }

        /// <summary>
        /// This is where you place the payment information, such as invoice number contract number, etc.
        /// </summary>
        [ChoFixedLengthRecordField(3, 80)]
        public string PaymentRelatedInformation { get; set; }

        /// <summary>
        /// This number is consecutively assigned to each Addenda Record following an Entry Detail Record.
        /// The first number must be ‘0001’.
        /// </summary>
        [ChoFixedLengthRecordField(83, 4)]
        [Range(1, int.MaxValue, ErrorMessage = "Addenda Sequence Number must be > 0.")]
        public uint AddendaSequenceNumber { get; set; }

        /// <summary>
        /// The Bank will assign a trace number.
        /// </summary>
        [ChoFixedLengthRecordField(87, 7)]
        [Range(1, ulong.MaxValue, ErrorMessage = "Entry Detail Sequence Number must be > 0.")]
        public ulong EntryDetailSequenceNumber { get; set; }
    }
}

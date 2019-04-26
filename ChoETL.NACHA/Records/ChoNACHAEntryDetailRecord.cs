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
    [ChoRecordTypeCode(ChoRecordTypeCode.EntryDetail)]
    public class ChoNACHAEntryDetailRecord
    {
        /// <summary>
        /// The code identifying the File Header Record is 6.
        /// </summary>
        [DefaultValue(ChoRecordTypeCode.EntryDetail)]
        [ChoFixedLengthRecordField(0, 1)]
        public ChoRecordTypeCode RecordTypeCode { get; set; }

        /// <summary>
        /// Choose the appropriate Transaction
        /// Code from this list:
        ///     22 Live Checking Account Credit
        ///     23 Pre-note Checking Account Credit
        ///     27 Live Checking Account Debit
        ///     28 Pre-note Checking Account Debit
        ///     32 Live Savings Account Credit
        ///     33 Pre-note Savings Account Credit
        ///     37 Live Savings Account Debit
        ///     38 Pre-note Savings Account Debit
        /// </summary>
        [ChoFixedLengthRecordField(1, 2)]
        public int TransactionCode { get; set; }

        /// <summary>
        /// This is the first eight digits of the routing number of the bank at which your customer/employee banks.
        /// Beware of numbers that do not begin with a 0, 1 or 2. Those are NOT routing numbers.
        /// </summary>
        [ChoFixedLengthRecordField(3, 8)]
        public ulong ReceivingDFIID { get; set; }

        /// <summary>
        /// This is the last digit of the routing number.
        /// </summary>
        [ChoFixedLengthRecordField(11, 1)]
        //[ChoCustomCodeValidator("v => Char.IsDigit(v[0])", ErrorMessage = "CheckDigit must be number.", ParamType = typeof(string))]
        [ChoIsDigit(ErrorMessage = "CheckDigit must be number.")]
        public char CheckDigit { get; set; }

        /// <summary>
        /// Your employee’s or customer’s bank account number.
        /// </summary>
        [ChoFixedLengthRecordField(12, 17)]
        public string DFIAccountNumber { get; set; }

        /// <summary>
        /// This is the dollar amount you are crediting or debiting the Receiver.
        /// The decimal point is implied, not hard coded.
        /// </summary>
        [ChoFixedLengthRecordField(29, 10)]
        [ChoTypeConverter(typeof(ChoCustomExprConverter), Parameters = @"'v => Decimal.Parse(v)/100.00', 'v => v.ToString(""#.00"").Replace(""."", """")'")]
        public decimal Amount { get; set; }

        /// <summary>
        /// This is the identification number you assign to your employee or customer.
        /// </summary>
        [ChoFixedLengthRecordField(39, 15)]
        public string IndividualIDNumber { get; set; }

        /// <summary>
        /// Name of receiver.
        /// </summary>
        [ChoFixedLengthRecordField(54, 22)]
        public string IndividualName { get; set; }

        /// <summary>
        /// For your company’s internal use if desired.
        /// </summary>
        [ChoFixedLengthRecordField(76, 2)]
        public string DiscretionaryData { get; set; }

        /// <summary>
        /// 0 = Entry does not have an Addenda Record
        /// 1 = Entry has an Addenda Record
        /// </summary>
        [ChoFixedLengthRecordField(78, 1)]
        [ChoTypeConverter(typeof(ChoBooleanConverter), Parameters = "ZeroOrOne")]
        public bool AddendaRecordIndicator { get; set; }

        /// <summary>
        /// The Bank will assign a trace number.
        /// </summary>
        [ChoFixedLengthRecordField(79, 15)]
        public string TraceNumber { get; set; }
    }
}

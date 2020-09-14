using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtpaymentBillDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int PayId { get; set; }
        public string PblTrcode { get; set; }
        public string PblCode { get; set; }
        public double? PblSerialNo { get; set; }
        public string InvCode { get; set; }
        public double? PblBillAmount { get; set; }
        public double? PblPaidAmount { get; set; }
        public string PblSubRemarks { get; set; }
        public string PblSupCode { get; set; }
        public double? PblOwingAmount { get; set; }
        public DateTime? PblDate { get; set; }
        public double? PblBalanceAmount { get; set; }
        public int? SupId { get; set; }
        public int? ItmId { get; set; }
    }
}

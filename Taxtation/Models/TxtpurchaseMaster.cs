using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtpurchaseMaster
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int PurId { get; set; }
        public string PurPoref { get; set; }
        public string PurDcref { get; set; }
        public DateTime? PurDate { get; set; }
        public DateTime? PurSupDate { get; set; }
        public string PurType { get; set; }
        public int? SupId { get; set; }
        public string PurPayTerm { get; set; }
        public int? BnkId { get; set; }
        public string PurChqNo { get; set; }
        public DateTime? PurChqDate { get; set; }
        public string PurScope { get; set; }
        public string PurPrices { get; set; }
        public string PurRemarks { get; set; }
        public bool? PurActive { get; set; }
        public string PurPomapCode { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public int? CurId { get; set; }
        public int? StrId { get; set; }
        public double? PurExRate { get; set; }
        public string PurItmType { get; set; }
        public int? SitId { get; set; }
    }
}

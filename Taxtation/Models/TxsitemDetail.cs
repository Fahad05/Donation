using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxsitemDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int ItmId { get; set; }
        public int? ItmPid { get; set; }
        public string ItmName { get; set; }
        public string ItmType { get; set; }
        public string ItmBcode { get; set; }
        public string ItmUom { get; set; }
        public bool? ItmIsSale { get; set; }
        public bool? ItmIsPurchase { get; set; }
        public bool? ItmIsService { get; set; }
        public double? ItmMinLevel { get; set; }
        public double? ItmMaxLevel { get; set; }
        public double? ItmReLevel { get; set; }
        public double? ItmCp { get; set; }
        public double? ItmSp { get; set; }
        public double? ItmOpQty { get; set; }
        public int? ItmOpStore { get; set; }
        public bool? ItmActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
    }
}

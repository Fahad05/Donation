using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxtinventoryStockDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int Sno { get; set; }
        public int? ItmId { get; set; }
        public double? InsSerialNo { get; set; }
        public string InsCondition { get; set; }
        public string InsNo { get; set; }
        public DateTime? InsDate { get; set; }
        public string InsRefNo { get; set; }
        public string InsRemarks { get; set; }
        public string InsSubRemarks { get; set; }
        public double? InsQuantityIn { get; set; }
        public double? InsQuantityOut { get; set; }
        public double? InsWeightIn { get; set; }
        public double? InsWeightOut { get; set; }
        public int? StrId { get; set; }
        public string InsDocReference { get; set; }
        public int? TxsId { get; set; }
        public double? QprTaxPercent { get; set; }
        public double? QprAmount { get; set; }
        public int? CurId { get; set; }
        public double? QprExchangeRate { get; set; }
        public double? QprAmountConverted { get; set; }
        public int? ProId { get; set; }
        public string InsEntryType { get; set; }
        public string InsPostStatus { get; set; }
        public DateTime? InsPostDate { get; set; }
        public double? OpeningQuantity { get; set; }
        public double? ClosingQuantity { get; set; }
        public double? TransactionQuantity { get; set; }
        public double? RunningQuantity { get; set; }
        public DateTime? InsExpiryDate { get; set; }
        public string InsBatchNo { get; set; }
        public int? SitId { get; set; }
        public DateTime? Pstdate { get; set; }
        public string InsType { get; set; }
        public string InsBarcode { get; set; }
        public string InsBarcodeAuto { get; set; }
    }
}

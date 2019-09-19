using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class TxscustomerDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int CusId { get; set; }
        public string CusType { get; set; }
        public string CusName { get; set; }
        public string CusAbbr { get; set; }
        public string CusPerson { get; set; }
        public string CusAddress { get; set; }
        public string CusPhNo { get; set; }
        public string CusFaxNo { get; set; }
        public string CusEmail { get; set; }
        public string CusNtn { get; set; }
        public string CusStrn { get; set; }
        public string CusCity { get; set; }
        public string CusCountry { get; set; }
        public string CusDesc { get; set; }
        public bool? CusActive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public int? CusCrDays { get; set; }
    }
}

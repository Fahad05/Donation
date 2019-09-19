using System;
using System.Collections.Generic;

namespace Taxtation.Models
{
    public partial class Txsuomdetail
    {
        public int Uomid { get; set; }
        public string Uomname { get; set; }
        public string Uomabbr { get; set; }
        public string Uomdesc { get; set; }
        public bool? Uomactive { get; set; }
        public string EnterBy { get; set; }
        public DateTime? EnterDate { get; set; }
    }
}

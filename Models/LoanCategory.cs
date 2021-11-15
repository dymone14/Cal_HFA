using System;
using System.Collections.Generic;

#nullable disable

namespace Cal_HFA.Models
{
    public partial class LoanCategory
    {
        public LoanCategory()
        {
            LoanTypes = new HashSet<LoanType>();
        }

        public int LoanCategoryId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<LoanType> LoanTypes { get; set; }
    }
}

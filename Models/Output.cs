using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cal_HFA.Models
{
    public class Output
    {
        // int LoanID from Loan/ LoanCategoryID from LoanType/ StatusCode/ StatusDate from LoanStatus
        public int LoanID { get; set; }
        public int LoanCategoryID { get; set; }
        public int StatusCode { get; set; }
        public DateTime StatusDate { get; set; }
    }

}
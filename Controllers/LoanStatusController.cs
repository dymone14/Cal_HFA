using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cal_HFA.Models;


namespace Cal_HFA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanStatusController : ControllerBase
    {
        private readonly CalHFAContext _context;

        public LoanStatusController(CalHFAContext context)
        {
            _context = context;
        }

        // GET: api/LoanStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatus>>> GetLoanStatuses()
        {
            return await _context.LoanStatuses.ToListAsync();
        }

        // GET: api/LoanStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanStatus>> GetLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);

            if (loanStatus == null)
            {
                return NotFound();
            }

            return loanStatus;
        }

        // PUT: api/LoanStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoanStatus(int id, LoanStatus loanStatus)
        {
            if (id != loanStatus.LoanStatusId)
            {
                return BadRequest();
            }

            _context.Entry(loanStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //This get method will return the correct counts for each loan queue as stated in Slack
        //Going to need to figure out correct dates code to get dates
        [Route("count")]
        [HttpGet]
        public string GetLoanCount()
        {
        
            //each of these variables will find the correct loans by using the specfic paramaters(status codes and categoryId)
            var ComplianceLoansInLine = GetEachList(410, 1);
            var SuspenseLoansInLine = GetEachList(422, 1);
            var PostClosingLoansInLine = GetEachList(510, 2);
            var PostClosingSuspenseDate = GetEachList(522, 2);
   
            //using json to return data
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { Loans = "ComplianceLoansInLine:" + ComplianceLoansInLine.Count + " SuspenseLoansInLine:" + SuspenseLoansInLine.Count + " PostClosingLoansInLine:" + PostClosingLoansInLine.Count + " PostClosingSuspenseDate:" + PostClosingSuspenseDate.Count });
        
        }

        /// created a model to match the correct output categories as given in Slack
        /// will use the query jacob made and new model to find the right output and store it into the list
        /// placeholders {0} {1} will be used for each status code and categoryID
        /// will return the correct loans
        /// will need to figure out a way to display the counts instead of the list as shown sqlquery1
        private List<Output> GetEachList(int statusCode, int categoryID)
        {
            string SQLQuery = @"SELECT Loan.LoanID, LoanType.LoanCategoryID, StatusCode, LoanStatus.StatusDate FROM Loan INNER JOIN(SELECT LoanStatus.LoanID, LoanStatus.StatusCode, LoanStatus.StatusSequence, LoanStatus.StatusDate FROM LoanStatus INNER JOIN(SELECT LoanStatus.LoanID, MAX(LoanStatus.StatusSequence) AS StatusSequence FROM LoanStatus GROUP BY LoanID) MaxTable ON LoanStatus.LoanID = MaxTable.LoanID AND LoanStatus.StatusSequence = MaxTable.StatusSequence) LoanStatus ON Loan.LoanID = LoanStatus.LoanID INNER Join(SELECT LoanType.LoanCategoryID, LoanType.LoanTypeID FROM LoanType WHERE LoanType.LoanCategoryID = {0}) LoanType ON LoanType.LoanTypeID = Loan.LoanTypeID WHERE StatusCode = {1} ORDER BY Loan.LoanID";

            var correctLoans = _context.Output.FromSqlRaw(SQLQuery, categoryID, statusCode).ToList();

            return correctLoans;
        }


/* [Route("counts")]
 [HttpGet]
 public string GetLoanCounts()
 {

     var count = _context.SqlQuery<string>("SELECT Loan.LoanID, LoanType.LoanCategoryID, StatusCode, LoanStatus.StatusDate FROM Loan INNER JOIN(SELECT LoanStatus.LoanID, LoanStatus.StatusCode, LoanStatus.StatusSequence, LoanStatus.StatusDate FROM LoanStatus INNER JOIN(SELECT LoanStatus.LoanID, MAX(LoanStatus.StatusSequence) AS StatusSequence FROM LoanStatus GROUP BY LoanID) MaxTable ON LoanStatus.LoanID = MaxTable.LoanID AND LoanStatus.StatusSequence = MaxTable.StatusSequence) LoanStatus ON Loan.LoanID = LoanStatus.LoanID INNER Join(SELECT LoanType.LoanCategoryID, LoanType.LoanTypeID FROM LoanType WHERE LoanType.LoanCategoryID = 1) LoanType ON LoanType.LoanTypeID = Loan.LoanTypeID WHERE StatusCode = 410 ORDER BY Loan.LoanID;").ToList();
     return count;

 }*/

// POST: api/LoanStatus
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
 [HttpPost]
 public async Task<ActionResult<LoanStatus>> PostLoanStatus(LoanStatus loanStatus)
 {
     _context.LoanStatuses.Add(loanStatus);
     await _context.SaveChangesAsync();

     return CreatedAtAction("GetLoanStatus", new { id = loanStatus.LoanStatusId }, loanStatus);
 }

 // DELETE: api/LoanStatus/5
 [HttpDelete("{id}")]
 public async Task<IActionResult> DeleteLoanStatus(int id)
 {
     var loanStatus = await _context.LoanStatuses.FindAsync(id);
     if (loanStatus == null)
     {
         return NotFound();
     }

     _context.LoanStatuses.Remove(loanStatus);
     await _context.SaveChangesAsync();

     return NoContent();
 }


 private bool LoanStatusExists(int id)
 {
     return _context.LoanStatuses.Any(e => e.LoanStatusId == id);
 }
}
}
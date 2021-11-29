SELECT Loan.LoanID, LoanType.LoanCategoryID, StatusCode, LoanStatus.StatusDate
FROM Loan
INNER JOIN(
SELECT LoanStatus.LoanID, LoanStatus.StatusCode, LoanStatus.StatusSequence, LoanStatus.StatusDate
FROM LoanStatus
INNER JOIN (
SELECT LoanStatus.LoanID, MAX(LoanStatus.StatusSequence) AS StatusSequence
FROM LoanStatus
GROUP BY LoanID
) MaxTable ON LoanStatus.LoanID = MaxTable.LoanID AND LoanStatus.StatusSequence = MaxTable.StatusSequence
) LoanStatus ON Loan.LoanID = LoanStatus.LoanID
INNER Join(
SELECT LoanType.LoanCategoryID, LoanType.LoanTypeID
FROM LoanType
WHERE LoanType.LoanCategoryID = 1
) LoanType ON LoanType.LoanTypeID = Loan.LoanTypeID
WHERE StatusCode = 410
ORDER BY Loan.LoanID;

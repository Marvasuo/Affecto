using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affecto
{
    public interface ISpecification<in TEntity>
    {
        IEnumerable<string> ReasonsForDissatisfaction { get; }
        bool IsSatisfiedBy(TEntity entity);
    }

    public class BusinessIdSpecification : ISpecification<TEntity>
    {
        string businessId = "";
        public BusinessIdSpecification()
        {
            // Default constructor
        }
        public BusinessIdSpecification(string output)
        {
            // Assigning the given ID to the businessId-variable.
            businessId = output;
            
        }
        public IEnumerable<string> ReasonsForDissatisfaction
        {
            get
            {
                string invalidIDNumber = "ID number is in invalid format.";
                string detectionPartTooShort = "Detection part of the ID is too short. " + 
                                               "Detection part of the business ID needs to have 7 numbers.";
                string detectionPartTooLong = "Detection part of the ID is too long. " + 
                                              "Detection part of the business ID needs to have 7 numbers.";
                string tooLongCheckingNumber = "Checking number in Given ID is too long. Checking number consist 1-2 numbers.";;
                string invalidCheckingNumber = "Checking number does not add up with the detection part.";
                string validBusinessId = "Business ID is in correct format.";

                yield return invalidIDNumber;
                yield return detectionPartTooShort;
                yield return detectionPartTooLong;
                yield return tooLongCheckingNumber;
                yield return invalidCheckingNumber;
                yield return validBusinessId;      
                
            }
        }

        public bool IsSatisfiedBy(TEntity entity)
        {
            // Calling the validation function and assigning its results to the variable.
            bool validityOfID = entity.ValidatingBusinessId(businessId);
            return validityOfID;
        }
    }

    public class TEntity
    {
        BusinessIdSpecification idSpecification = new BusinessIdSpecification();
        public bool ValidatingBusinessId(string inputID)
        {
            // Proper indicates the validation status of the given ID.
            bool proper = true;
            int countOfHyphens = 0;
            int indexOfHyphen = 0;
            int multiplyingResult = 0;
            int modulusResult = 0;
            int[] multiplyingFactors = { 7, 9, 10, 5, 8, 4, 2 };
            List<int> idCharacters = new List<int>();

            //Assigning every character from inputID to the list
            foreach (var sign in inputID)
            {
                idCharacters.Add((int)char.GetNumericValue(sign));
            }

/*
    CHECKING IF THE GIVEN BUSINESS ID IS IN NNNNNNN-T(T) FORMAT. BUSINESS ID CONSIST OF 7 DETECTION NUMBERS,
    ONE HYPHEN AND 1-2 CHECKING NUMBERS IN THIS ORDER.
*/

            // Counting how many hyphens there are in given ID.
            foreach (var number in idCharacters)
            {
                if (number == -1)
                {
                    countOfHyphens++;
                }
            }

            // Getting the position of the first hyphen in given ID.
            indexOfHyphen = idCharacters.IndexOf(-1);

            // If there are more hyphens than 1, then the ID is not proper. Print the error message.
            if (countOfHyphens != 1)
            {
                proper = false;
                Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(0));
                return proper;
            }

            // If there are no digits before or after the hyphen, then the ID is not proper. Print the error message.
            if (indexOfHyphen == 0 || idCharacters.Count - 1 == indexOfHyphen)
            {
                proper = false;
                Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(0));
                return proper;
            }

            // Checking if the hyphen is in correct position. If there are not 7 digits before the hyphen or 1-2 digits
            // after the hyphen, check which condition is relevant, chance the status of proper-variable and print
            // correct message.
            if (indexOfHyphen != 7 || (idCharacters.Count - 1) - indexOfHyphen > 2)
            {
                //Less than 7 digits before the hyphen and 2 or less after.
                if (indexOfHyphen < 7 && (idCharacters.Count - 1) - indexOfHyphen <= 2)
                {
                    proper = false;
                    Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(1));
                    return proper;
                }

                // 7 digits before but over 2 after the hyphen.
                else if (indexOfHyphen == 7 && (idCharacters.Count - 1) - indexOfHyphen > 2)
                {
                    proper = false;
                    Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(3));
                    return proper;
                }

                // Less than 7 digits before and over 2 after the hyphen.
                else if (indexOfHyphen < 7 && (idCharacters.Count - 1) - indexOfHyphen > 2)
                {
                    proper = false;
                    Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(1) + " " + 
                        idSpecification.ReasonsForDissatisfaction.ElementAt(3));
                    return proper;
                }

                // Over 7 digits before and 2 or less after the hyphen.
                else if (indexOfHyphen > 7 && (idCharacters.Count - 1) - indexOfHyphen <= 2)
                {
                    proper = false;
                    Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(2));
                    return proper;
                }

                // If none of the previous conditions match, then there are over 7 digits before
                // and over 2 digits after the hyphen.
                else
                {
                    proper = false;
                    Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(2) + " " +
                        idSpecification.ReasonsForDissatisfaction.ElementAt(3));
                    return proper;
                }
            }

/*  
    COUNTING THE CORRECT CHECKING NUMBER ACCORDING TO THE GIVEN DETECTION NUMBERS
    AND COMPARING THE RESULT WITH THE GIVEN CHECKING NUMBER.
*/
            // After evaluating the ID number, if there are more than 9 characters in given ID,
            // the checking number in given ID consist 2 digits. In list digits are separately.
            // The last two characters are combined to together forming 2 digit number and replaced
            // with old ones.
            if (idCharacters.Count > 9)
            {
                string lastNumber = idCharacters[9].ToString();
                string secondToLastNumber = idCharacters[8].ToString();
                string newLastNumber = secondToLastNumber + lastNumber;
                idCharacters.RemoveRange(8, 2);
                idCharacters.Add(int.Parse(newLastNumber));
            }

            // Multiplying all the detection numbers with the corresponding multiplying number
            // from the array multiplyingFactors.
            for (int i = 0; i < 7; i++)
            {
                multiplyingResult += idCharacters[i] * multiplyingFactors[i];
            }

            //Counting the correct checking number.
            modulusResult = multiplyingResult % 11;

            // Comparing the result with the given checking number. If they match, then the ID is valid and the message
            //is printed. Otherwise changing the status and printing appropriate error.
            if (modulusResult != idCharacters[idCharacters.Count-1])
            {
                proper = false;
                Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(4));
                return proper;
            }
            Console.WriteLine(idSpecification.ReasonsForDissatisfaction.ElementAt(5));
            return proper;
        }   
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool testingResult = false; 

            // Looping as long as the ID is valid.
            do
            {
                Console.WriteLine("Give the business ID.");

                // Sending the given ID for processing.
                BusinessIdSpecification testingIdSpecification = new BusinessIdSpecification(Console.ReadLine());
                TEntity entityTest = new TEntity();

                // Checking if the given ID is valid and assigning its result to the boolean variable.
                testingResult = testingIdSpecification.IsSatisfiedBy(entityTest);

            } while (testingResult != true);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}

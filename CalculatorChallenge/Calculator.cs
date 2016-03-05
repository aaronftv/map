using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CalculatorChallenge
{
    /// <summary>
    /// Type of operators
    /// </summary>
    public enum Operator
    {
        Exp,
        Sqrt,
        Mult,
        Div,
        Add,
        Sub
    };
    
    /// <summary>
    /// Calculator class
    /// </summary>
    public class Calculator
    {
        /// <summary>
        /// Perform evaluation
        /// </summary>
        /// <param name="expression">The expression to calculate</param>
        /// <returns>The result of the evaluation</returns>
        public string CalculateExpression(string expression)
        {
            string result = string.Empty;
            List<SingleOperation> singleOperationList = new List<SingleOperation>();

            if (expression == string.Empty)
                return string.Empty;

            if (this.ValidateExpressionSyntax(expression))
            {
                if (this.ValidateOperatorsExistence(expression))
                {
                    if (this.ParseInput(expression, ref singleOperationList))
                    {
                        result = this.ExecuteOperators(singleOperationList);
                    }
                    else
                    {
                        result = "Inconsistent operations";
                    }
                }
                else 
                {
                    result = "No operators present";
                }
            }
            else 
            {
                result = "Invalid syntax";
            }
            
            return result;
        }

        /// <summary>
        /// Validate the correct syntax
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Whether or not syntax is valid</returns>
        private bool ValidateExpressionSyntax(string expression)
        {
            string validInputRegex = @"((?:\d*\.)?\d+)?[\^\*\/\+\-]?((?:\d*\.)?\d+)?(\bsqrt\((?:\d*\.)?\d+\))?";
            
            Regex isValidInputRegex = new Regex(validInputRegex);
            string concatExp = string.Empty;

            foreach (Match regexMatch in isValidInputRegex.Matches(expression)) 
            {
                concatExp = concatExp + regexMatch.Value;
            }

            if (expression.Length == concatExp.Length)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validate there is operators present
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Whether or not there are operatiors present</returns>
        private bool ValidateOperatorsExistence(string expression)
        {
            Match areOperatorsPresent;
            string lookForOperators1 = @"[\^\*\/\+\-]";
            string isThereOperators2 = @"(\bsqrt\((?:\d*\.)?\d+\))";
            areOperatorsPresent = Regex.Match(expression, lookForOperators1);

            if (areOperatorsPresent.Success)
            {
                return true;
            }
            else
            {
                areOperatorsPresent = Regex.Match(expression, isThereOperators2);
                if (areOperatorsPresent.Success)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Parse expression in a list of single expressions objects
        /// </summary>
        /// <param name="input">Expression</param>
        /// <param name="singleOperationList">List of independent single expressions</param>
        /// <returns>Whether or not data is consistent</returns>
        private bool ParseInput(string input, ref List<SingleOperation> singleOperationList)
        {
            string originalExpression = input;

            singleOperationList = new List<SingleOperation>();

            foreach (Operator oper in Enum.GetValues(typeof(Operator)))
            {
                singleOperationList.AddRange(this.ParseSingleOperations(oper, originalExpression));
            }

            singleOperationList = singleOperationList.OrderBy(o => o.OperationIndex).ToList();

            return this.ValidateDataConsistency(singleOperationList);
        }

        /// <summary>
        /// Parse single expressions of a given type of operator
        /// </summary>
        /// <param name="oper">Operator type</param>
        /// <param name="originalExpression">Expression</param>
        /// <returns>List of expressions with the given operator</returns>
        private List<SingleOperation> ParseSingleOperations(Operator oper, string originalExpression)
        {
            List<SingleOperation> parsedSingleOperations = new List<SingleOperation>();
            string regexExp = string.Empty;
            Regex singleOperationMatches;
            Match argumentMatch;

            SingleOperation currentOperation;
            string singleExpressionStr = string.Empty;
            double arg1;
            double arg2;

            switch(oper)
            {
                case Operator.Exp:
                    regexExp = @"((?:\d*\.)?\d+)?\^((?:\d*\.)?\d+)?";
                break;
                case Operator.Sqrt:
                    regexExp = @"\bsqrt\((?:\d*\.)?\d+\)";
                break;
                case Operator.Mult:
                    regexExp = @"((?:\d*\.)?\d+)?\*((?:\d*\.)?\d+)?";
                break;
                case Operator.Div:
                    regexExp = @"((?:\d*\.)?\d+)?\/((?:\d*\.)?\d+)?";
                break;
                case Operator.Add:
                    regexExp = @"((?:\d*\.)?\d+)?\+((?:\d*\.)?\d+)?";
                break;
                case Operator.Sub:
                    regexExp = @"((?:\d*\.)?\d+)?\-((?:\d*\.)?\d+)?";
                break;
            }

            singleOperationMatches = new Regex(regexExp);

            foreach (Match singleOperationMatch in singleOperationMatches.Matches(originalExpression)) 
            {
                singleExpressionStr = singleOperationMatch.Value;
                
                //Getting first argument
                if(oper != Operator.Sqrt)
                    argumentMatch = Regex.Match(singleExpressionStr, @"^(?:\d*\.)?\d+");
                else
                    argumentMatch = Regex.Match(singleExpressionStr, @"(?:\d*\.)?\d+");

                if (argumentMatch.Success)
                    arg1 = Convert.ToDouble(argumentMatch.Value);
                else
                    arg1 = double.NaN;

                //Getting second argument
                argumentMatch = Regex.Match(singleExpressionStr, @"(?:\d*\.)?\d+$");
                if (argumentMatch.Success)
                    arg2 = Convert.ToDouble(argumentMatch.Value);
                else
                    arg2 = double.NaN;

                currentOperation = new SingleOperation() 
                {
                    OperationIndex = singleOperationMatch.Index,
                    Operator = oper,
                    Arg1 = arg1,
                    Arg2 = arg2
                };

                parsedSingleOperations.Add(currentOperation);            
            }

            return parsedSingleOperations;
        }

        /// <summary>
        /// Validate that data is consistent
        /// </summary>
        /// <param name="singleOperationList">List of single expressions</param>
        /// <returns>Whether or not data is consistent</returns>
        private bool ValidateDataConsistency(List<SingleOperation> singleOperationList)
        {
            foreach (SingleOperation singleOperation in singleOperationList)
            {
                //Exponent operation must be sure to have both arguments with a value due to it's required to perform first
                if (singleOperation.Operator == Operator.Exp)
                {
                    if (double.IsNaN(singleOperation.Arg1) || double.IsNaN(singleOperation.Arg2))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Perform the independent operations from list until it gets final result
        /// </summary>
        /// <param name="singleOperationList">List of single operations</param>
        /// <returns></returns>
        private string ExecuteOperators(List<SingleOperation> singleOperationList)
        {
            List<SingleOperation> currentOperatorList = new List<SingleOperation>();
            double result = 0.0;

            foreach (Operator oper in Enum.GetValues(typeof(Operator)))
            {
                foreach (SingleOperation singleOperation in singleOperationList)
                {
                    if (singleOperation.Operator == oper) 
                    {
                        int elementIndex = singleOperationList.IndexOf(singleOperation);

                        if (double.IsNaN(singleOperation.Arg1) || (singleOperation.Operator != Operator.Sqrt && double.IsNaN(singleOperation.Arg2)))
                        {
                            return "Inconsistent operation";
                        }
                        else
                        {
                            result = PerformSingleOperation(singleOperation.Arg1, singleOperation.Arg2, singleOperation.Operator);

                            if (elementIndex > 0) 
                            {
                                singleOperationList[elementIndex - 1].Arg2 = result;
                            }
                            if (elementIndex + 1 < singleOperationList.Count) 
                            {
                                singleOperationList[elementIndex + 1].Arg1 = result;
                            }

                            currentOperatorList.Add(singleOperation);
                        }
                    }
                }

                foreach (SingleOperation singleOperation in currentOperatorList) 
                {
                    singleOperationList.Remove(singleOperation);
                }
                currentOperatorList.Clear();
            }

            return result.ToString();
        }

        /// <summary>
        /// Perform a single operation with the given operator
        /// </summary>
        /// <param name="number1">First number</param>
        /// <param name="number2">Second number</param>
        /// <param name="oper">Operation to perform</param>
        /// <returns>Single operation result</returns>
        private double PerformSingleOperation(double number1, double number2, Operator oper)
        {
            double result = 0.0;

            switch (oper)
            {
                case Operator.Exp:
                    result = Math.Pow(number1, number2);
                break;
                case Operator.Sqrt:
                    result = Math.Sqrt(number1);
                break;
                case Operator.Mult:
                    result = number1 * number2;
                break;
                case Operator.Div:
                    result = number1 / number2;
                break;
                case Operator.Add:
                    result = number1 + number2;
                break;
                case Operator.Sub:
                    result = number1 - number2;
                break;
            }

            return result;
        }
    }

    /// <summary>
    /// SingleOperation entity
    /// </summary>
    public class SingleOperation
    {
        /// <summary>
        /// Operation index from origial expression string
        /// </summary>
        public int OperationIndex { get; set; }
        /// <summary>
        /// First argument
        /// </summary>
        public double Arg1 { get; set; }
        /// <summary>
        /// Second argument
        /// </summary>
        public double Arg2 { get; set; }
        /// <summary>
        /// Operation to perform
        /// </summary>
        public Operator Operator { get; set; }
    }
}
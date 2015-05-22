//Completed by Jack Stafford for CS 3500, September 2014

// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private string normalizedFormula, originalFormula;
        private List<string> tokens;
        private HashSet<string> variables;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            if (formula.Length == 0) //There must be at least one token
                throw new FormulaFormatException("Formula must have at least one character.");

            foreach (char c in formula)     //Verify that the only tokens are (, ), +, -, *, /, variables, and floating-point numbers     
                if (!(char.IsLetterOrDigit(c) || c == ' ' || c == '_' || c == '*' || c == '/' || c == '-' || c == '+' || c == '(' || c == ')' || c == '.'))
                    throw new FormulaFormatException(c + " is not a valid character.");

            tokens = new List<string>();
            variables = new HashSet<string>();
            normalizedFormula = "";
            originalFormula = "";

            foreach (string s in GetTokens(formula))
            {
                double d;
                string n = normalize(s);

                // operator
                if (s == "*" || s == "/" || s == "+" || s == "-" || s == "(" || s == ")")
                {
                    normalizedFormula += s;
                    tokens.Add(s);
                }

                // double, e, E, int
                else if (double.TryParse(s, out d))
                {
                    if (d.ToString().Contains("."))     // trim trailing fractional zeros
                        normalizedFormula += d.ToString().TrimEnd('0').TrimEnd('.');
                    else
                        normalizedFormula += d + "";
                    tokens.Add(d + "");
                }

                // only variables or invalid statements should get here, so check variables    
                else if (!(char.IsLetter(n.ToCharArray()[0]) || n.ToCharArray()[0] == '_'))
                    throw new FormulaFormatException("Normalized variables must start with a letter or underscore");
                else
                {
                    foreach (char c in n)     //Verify that the only characters in a normalized variable are letters, digits, and underscores 
                        if (!(char.IsLetterOrDigit(c) || c == '_'))
                            throw new FormulaFormatException(c + " is not a valid variable character.");

                    if (isValid(normalize(s)))
                    {
                        normalizedFormula += n;
                        tokens.Add(n);
                        variables.Add(n);
                    }

                   // variable doesn't exist
                    else
                        throw new FormulaFormatException(s + " is not a defined variable.");
                }

                originalFormula += s;
            }
            checkFormulaSyntax();
        }

        /// <summary>
        /// Ensures formula is arithmetically valid, so there are no mishaps during evaluation
        /// 
        /// FormulaFormatException thrown if syntax error found
        /// </summary>
        private void checkFormulaSyntax()
        {
            int openParentheses = 0;
            int closeParentheses = 0;

            string s = tokens[0];
            if (s == "*" || s == "/" || s == "+" || s == "-" || s == ")") //The first token of an expression must be a number, a variable, or an opening parenthesis
                throw new FormulaFormatException("First entry must be a number, a variable, or an opening parenthesis.");
            if (s == "(")
                openParentheses++;

            string previous = s;
            for (int i = 1; i < tokens.Count; i++)
            {
                s = tokens[i];

                //When reading tokens from left to right, at no point should the number of closing parentheses seen so far be greater than the number of opening parentheses seen so far
                if (s == "(")
                    openParentheses++;
                else if (s == ")")
                {
                    if (openParentheses > closeParentheses)
                        closeParentheses++;
                    else
                        throw new FormulaFormatException("Parentheses must exist in matching pairs.");
                }

                //Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis
                char current = s.ToCharArray()[0];
                if ((previous == "(" || previous == "*" || previous == "/" || previous == "+" || previous == "-") &&
                    !(char.IsLetterOrDigit(current) || current == '('))
                    throw new FormulaFormatException(s + " may not immediately follow " + previous);

                //Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis
                char last = previous.ToCharArray()[0];
                if ((char.IsLetterOrDigit(last) || last == '_' || last == ')') &&
                    !(current == '*' || current == '/' || current == '+' || current == '-' || current == ')'))
                    throw new FormulaFormatException(s + " may not immediately follow " + previous);
                previous = s;
            }

            if (openParentheses != closeParentheses) //The total number of opening parentheses must equal the total number of closing parentheses
                throw new FormulaFormatException("Parentheses must exist in matching pairs.");

            char previousChar = previous.ToCharArray()[0]; //get first char of last token added
            if (!(char.IsLetterOrDigit(previousChar) || previousChar == '_' || previousChar == ')')) //The last token of an expression must be a number, a variable, or a closing parenthesis
                throw new FormulaFormatException("Last entry must be a number, a variable, or a closing parenthesis.");

        }



        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            try
            {
                foreach (string token in tokens)
                {
                    //double
                    double d;
                    if (double.TryParse(token, out d))
                    {
                        if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                            doOperation(d, values.Pop(), operators.Pop(), values);
                        else
                            values.Push(d);
                    }

                    //addition or subtraction
                    else if (token == "+" || token == "-")
                    {
                        if (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"))
                            doOperation(values.Pop(), values.Pop(), operators.Pop(), values);
                        operators.Push(token);
                    }

                    //multiplication or division
                    else if (token == "*" || token == "/")
                        operators.Push(token);

                    //open parenthesis
                    else if (token == "(")
                        operators.Push(token);

                    //close parenthesis
                    else if (token == ")")
                    {
                        if (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"))
                            doOperation(values.Pop(), values.Pop(), operators.Pop(), values);
                        if (operators.Count > 0 && operators.Peek() == "(")
                            operators.Pop();
                        if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                            doOperation(values.Pop(), values.Pop(), operators.Pop(), values);
                    }

                    //variable
                    else
                    {
                        try
                        {
                            d = lookup(token);
                        }
                        catch (ArgumentException)
                        {
                            return new FormulaError("Variable " + d + " does not exist.");
                        }
                        if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                            doOperation(d, values.Pop(), operators.Pop(), values);
                        else
                            values.Push(d);
                    }
                }


                if (operators.Count > 0)
                    doOperation(values.Pop(), values.Pop(), operators.Pop(), values);
            }
            catch (FormulaFormatException)
            {
                return new FormulaError("Cannot divide by zero.");
            }
            return values.Pop();
        }

        /// <summary>
        /// Performs the given arithmetic operation, and pushes the result onto the values stack.
        /// result = left op right
        /// e.g. doOperation(1, 2, +, values) would be  2 + 1 and 3 would be pushed onto values
        /// 
        /// If an invalid operation symbol is given, an ArgumentException will be thrown
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <param name="op"></param>
        private static void doOperation(double right, double left, string op, Stack<double> values)
        {
            switch (op)
            {
                case "+":
                    values.Push(left + right);
                    break;
                case "-":
                    values.Push(left - right);
                    break;
                case "*":
                    values.Push(left * right);
                    break;
                case "/":
                    if (right == 0)
                        throw new FormulaFormatException("");
                    values.Push(left / right);
                    break;
            }
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return normalizedFormula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Formula))
                return false;

            return normalizedFormula == (obj as Formula).normalizedFormula;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null) || ReferenceEquals(f2, null))
            {
                if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
                    return true;
                return false;
            }

            return f1.normalizedFormula == f2.normalizedFormula;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return normalizedFormula.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}


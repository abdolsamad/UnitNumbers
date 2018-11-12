using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitConversionNS
{
    public class UnitsCore
    {
        private Dictionary<string, Unit> _basicUnits;
        private Dictionary<string, Unit> _complexUnits;
        private Dictionary<char, int> operationPrecedence;
        private Regex unitNumberRegex;
        public UnitsCore()
        {
            unitNumberRegex = new Regex(@"^(?<number>[0-9.eE+-]+?)(?:\s*\[(?<unit>\s*.+?\s*)\]\s*)?$");
            
            _basicUnits = new Dictionary<string, Unit>();
            _complexUnits = new Dictionary<string, Unit>();
            operationPrecedence = new Dictionary<char, int>();
            operationPrecedence.Add('(', 0);
            operationPrecedence.Add('*', 1);
            operationPrecedence.Add('/', 1);
            operationPrecedence.Add('-', 2);
            operationPrecedence.Add('^', 2);
        }

        public void AddBasicUnit(Unit unit)
        {
            if (!unit.IsBasic)
                throw new Exception("Unit is not basic!");
            _basicUnits.Add(unit.ToString().ToLower(), unit);
        }

        public void AddComplexUnit(Unit unit)
        {
            _complexUnits.Add(unit.ToString().ToLower(), unit);
        }

        public bool Contain(string unit)
        {
            return _basicUnits.ContainsKey(unit.ToLower()) || _complexUnits.ContainsKey(unit.ToLower());
        }

        #region Parsing
        private enum TokenType
        {
            Unit = 0,
            Operation = 1,
            LeftBracket = 2,
            RightBracket = 3,
            Number = 4
        }

        private class Token
        {
            public TokenType Type;
            public object Value;
        }
        public Unit ParseUnit(string unit)
        {
            unit = unit.ToLower().Trim();
            if (!IsMixedUnit(unit))
            {
                if (_basicUnits.ContainsKey(unit))
                    return _basicUnits[unit];
                if (_complexUnits.ContainsKey(unit))
                    return _complexUnits[unit];
                else
                {
                    throw new Exception($"Unit ({unit}) not found!");
                }
            }

            var tokens = Seperate(unit);

            Stack<Token> valStack = new Stack<Token>();
            Stack<Token> opStack = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Unit:
                    case TokenType.Number:
                        valStack.Push(token);
                        break;
                    case TokenType.LeftBracket:
                        opStack.Push(token);
                        break;
                    case TokenType.RightBracket:
                        PopOperations(true, token, valStack, opStack);
                        break;
                    case TokenType.Operation:
                        Token operation1Token = token;
                        char operation1 = (char) operation1Token.Value;
                        while (opStack.Count > 0 && opStack.Peek().Type == TokenType.Operation)
                        {
                            Token operation2Token = opStack.Peek();

                            char operation2 = (char) operation2Token.Value;

                            if ((IsLeftAssociative(operation1) &&
                                 operationPrecedence[operation1] <= operationPrecedence[operation2]) ||
                                operationPrecedence[operation1] < operationPrecedence[operation2])
                            {
                                opStack.Pop();
                                valStack.Push(ConvertOperation(operation2, valStack, opStack));
                            }
                            else
                            {
                                break;
                            }
                        }

                        opStack.Push(operation1Token);
                        break;
                }
            }

            PopOperations(false, null, valStack, opStack);
            VerifyResultStack(valStack);
            return (Unit) valStack.Pop().Value;
        }

        public UnitNumber ParseNumber(string number)
        {
            var match = unitNumberRegex.Match(number);
            if(!match.Success)
                throw new Exception("Entered value is not a valid number.");
            Unit unit;
            if (match.Groups["unit"].Success)
                unit = ParseUnit(match.Groups["unit"].Value);
            else
            {
                unit = new Unit("",Dimensions.Empty,1);
            }

            var num = double.Parse(match.Groups["number"].Value);
            return new UnitNumber(num,unit);
        }
        private void VerifyResultStack(Stack<Token> resultStack)
        {
            if (resultStack.Count > 1)
            {
                throw new Exception("The syntax of the provided formula is not valid.");
            }
        }

        private Token ConvertOperation(char operation, Stack<Token> valStack, Stack<Token> opStack)
        {
            try
            {
                Token argument1, argument2;
                switch (operation)
                {
                    case '-':
                    case '*':
                        argument2 = valStack.Pop();
                        argument1 = valStack.Pop();
                        if (argument2.Type != TokenType.Unit || argument1.Type != TokenType.Unit)
                            throw new Exception("multiplication on invalid symbols");
                        return new Token
                        {
                            Type = TokenType.Unit,
                            Value = (Unit) argument1.Value * (Unit) argument2.Value
                        };

                    case '/':
                        argument2 = valStack.Pop();
                        argument1 = valStack.Pop();
                        if (argument2.Type != TokenType.Unit)
                            throw new Exception("Division on invalid symbols");
                        if (argument1.Type == TokenType.Unit)
                            return new Token
                            {
                                Type = TokenType.Unit,
                                Value = (Unit) argument1.Value / (Unit) argument2.Value
                            };
                        else if (argument1.Type == TokenType.Number && (int) argument1.Value == 1)
                            return new Token
                            {
                                Type = TokenType.Unit,
                                Value = ((Unit) argument2.Value).Pow(-1)
                            };
                        else
                            throw new Exception("Division on invalid symbols");

                    case '^':
                        argument2 = valStack.Pop();
                        argument1 = valStack.Pop();
                        if (argument2.Type != TokenType.Number || argument1.Type != TokenType.Unit)
                            throw new Exception("Power on invalid symbols");
                        return new Token
                        {
                            Type = TokenType.Unit,
                            Value = ((Unit) argument1.Value).Pow((short) argument2.Value)
                        };
                    default:
                        throw new ArgumentException(string.Format("Unknown operation \"{0}\".", operation),
                            "operation");
                }
            }
            catch (InvalidOperationException)
            {
                // If we encounter a Stack empty issue this means there is a syntax issue in 
                // the mathematical formula
                throw new Exception(string.Format("There is a syntax issue for the operation \"{0}\" " +
                                                  "The number of arguments does not match with what is expected.",
                    operation));
            }
        }

        private static bool IsLeftAssociative(char operation1)
        {
            return (operation1 == '-' || operation1 == '*' || operation1 == '/');
        }

        private void PopOperations(bool upToLeftBracket, Token currentToken, Stack<Token> valStack,
            Stack<Token> opStack)
        {
            if (upToLeftBracket && currentToken == null)
                throw new ArgumentNullException("currentToken",
                    "If the parameter \"untillLeftBracket\" is set to true, " +
                    "the parameter \"currentToken\" cannot be null.");

            while (opStack.Count > 0 && opStack.Peek().Type != TokenType.LeftBracket)
            {
                Token token = opStack.Pop();

                switch (token.Type)
                {
                    case TokenType.Operation:
                        valStack.Push(ConvertOperation((char)token.Value, valStack, opStack));
                        break;
                }
            }

            if (upToLeftBracket)
            {
                if (opStack.Count > 0 && opStack.Peek().Type == TokenType.LeftBracket)
                    opStack.Pop();
                else
                    throw new Exception(string.Format("No matching left bracket found for the right " +
                                                      "bracket at token {0}.", currentToken.Value));
            }
            else
            {
                if (opStack.Count > 0 && opStack.Peek().Type == TokenType.LeftBracket)
                    throw new Exception(string.Format("No matching right bracket found for the left bracket for {0}.",
                        opStack.Peek()));
            }
        }

        private List<Token> Seperate(string unit)
        {
            string part = String.Empty;
            List<Token> tokens = new List<Token>();
            Token lastToken = null;
            for (int i = 0; i < unit.Length; i++)
            {
                if (char.IsWhiteSpace(unit[i]))
                {
                    /*if (!string.IsNullOrEmpty(part))
                    {
                        tokens.Add(new Token(){Value= ParseUnit(part),Type = TokenType.Unit});
                        part = String.Empty;
                    }*/
                    continue;
                }

                if (unit[i] == '(')
                {
                    lastToken = new Token() {Value = null, Type = TokenType.LeftBracket};
                }
                else if (unit[i] == ')')
                {
                    lastToken = new Token() {Value = null, Type = TokenType.RightBracket};
                }
                else if (unit[i] == '*' || unit[i] == '/' || unit[i] == '-' || unit[i] == '^')
                {
                    lastToken = new Token() {Value = unit[i], Type = TokenType.Operation};
                }
                else if (Char.IsLetter(unit[i]))
                {
                    while (i < unit.Length && Char.IsLetterOrDigit(unit[i]))
                    {
                        part += unit[i];
                        i++;
                    }

                    i--;
                    lastToken = new Token() {Value = ParseUnit(part), Type = TokenType.Unit};
                    part = String.Empty;
                }
                else if (Char.IsDigit(unit[i]))
                {
                    while (i < unit.Length && Char.IsDigit(unit[i]))
                    {
                        part += unit[i];
                        i++;
                    }
                    i--;
                    short sign = 1;
                    if (lastToken != null && lastToken.Type == TokenType.Operation && (char) lastToken.Value == '-')
                    {
                        tokens.RemoveAt(tokens.Count - 1);
                        sign = -1;
                    }

                    lastToken = new Token() {Value = (short)(sign * Int16.Parse(part)), Type = TokenType.Number};
                    part=String.Empty;
                }
                else
                {
                    throw new Exception("Unknown symbol reached");
                }

                tokens.Add(lastToken);
            }

            return tokens;
        }

        private bool IsMixedUnit(string unit)
        {
            var ops = new List<char>(new[] {'(', ')', '*', '/', '-', '^'});
            for (int i = 0; i < unit.Length; i++)
            {
                if (ops.Contains(unit[i]))
                    return true;
            }

            return false;
        }
        #endregion
    }
}
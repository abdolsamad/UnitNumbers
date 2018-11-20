using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnitConversionNS.Exceptions;

namespace UnitConversionNS.ExpressionParsing.Tokenizer
{
    /// <summary>
    /// A token reader that converts the input string in a list of tokens.
    /// </summary>
    public class TokenReader
    {
        private readonly CultureInfo cultureInfo;
        private readonly char decimalSeparator;
        private readonly char argumentSeparator;

        public TokenReader()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public TokenReader(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            this.decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
            this.argumentSeparator = cultureInfo.TextInfo.ListSeparator[0];
        }

        /// <summary>
        /// Read in the provided formula and convert it into a list of takens that can be processed by the
        /// Abstract Syntax Tree Builder.
        /// </summary>
        /// <param name="formula">The formula that must be converted into a list of tokens.</param>
        /// <exception cref="ParseException"></exception>
        /// <returns>The list of tokens for the provided formula.</returns>
        public List<Token> Read(string formula)
        {
            if (string.IsNullOrEmpty(formula))
                throw new ArgumentNullException(nameof(formula));

            List<Token> tokens = new List<Token>();

            char[] characters = formula.ToCharArray();

            bool isFormulaSubPart = true;

            for (int i = 0; i < characters.Length; i++)
            {
                if (IsPartOfNumeric(characters[i], true, isFormulaSubPart))
                {
                    string buffer = ReadWholeNumber(ref formula, i);

                    double doubleValue;
                    if (double.TryParse(buffer, NumberStyles.Float | NumberStyles.AllowThousands,
                        cultureInfo, out doubleValue))
                    {
                        tokens.Add(new Token()
                        {
                            TokenType = TokenType.Number,
                            Value = doubleValue,
                            StartPosition = i,
                            Length = buffer.Length
                        });
                        isFormulaSubPart = false;
                    }
                    else if (buffer == "-")
                    {
                        // Verify if we have a unary minus, we use the token '_' for a unary minus in the AST builder
                        tokens.Add(new Token()
                        {
                            TokenType = TokenType.Operation,
                            Value = '_',
                            StartPosition = i,
                            Length = 1
                        });
                    }

                    i += buffer.Length;
                    if (i == characters.Length)
                    {
                        // Last character read
                        continue;
                    }
                }

                if (char.IsLetter(characters[i]))
                {
                    string buffer = "" + characters[i];
                    int startPosition = i;
                    bool allowDot = true;
                    while (++i < characters.Length)
                    {
                        if (characters[i] == '.')
                        {
                            if(!allowDot) break;
                            allowDot = false;
                        }
                        else if (!char.IsLetterOrDigit(characters[i]) && characters[i]!='_')
                        {
                            break;
                        }
                        else
                        {
                            allowDot = true;
                        }
                        buffer += characters[i];
                    }

                    tokens.Add(new Token()
                    {
                        TokenType = TokenType.Text,
                        Value = buffer,
                        StartPosition = startPosition,
                        Length = i - startPosition
                    });
                    isFormulaSubPart = false;

                    if (i == characters.Length)
                    {
                        // Last character read
                        continue;
                    }
                }

                if (characters[i] == '[')
                {
                    string buffer = "";
                    int startPosition = i;

                    while (++i < characters.Length)
                    {
                        if (characters[i] == ']') break;
                        buffer += characters[i];
                    }

                    tokens.Add(new Token()
                    {
                        TokenType = TokenType.Unit,
                        Value = buffer,
                        StartPosition = startPosition,
                        Length = i - startPosition
                    });
                    continue;
                }

                if (characters[i] == this.argumentSeparator)
                {
                    tokens.Add(new Token()
                    {
                        TokenType = TokenType.ArgumentSeparator,
                        Value = characters[i],
                        StartPosition = i,
                        Length = 1
                    });
                    isFormulaSubPart = false;
                }
                else
                {
                    switch (characters[i])
                    {
                        case ' ':
                            continue;
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                        case '^':
                            if (IsUnaryMinus(characters[i], tokens))
                            {
                                // We use the token '_' for a unary minus in the AST builder
                                tokens.Add(new Token()
                                {
                                    TokenType = TokenType.Operation,
                                    Value = '_',
                                    StartPosition = i,
                                    Length = 1
                                });
                            }
                            else
                            {
                                tokens.Add(new Token()
                                {
                                    TokenType = TokenType.Operation,
                                    Value = characters[i],
                                    StartPosition = i,
                                    Length = 1
                                });
                            }

                            isFormulaSubPart = true;
                            break;
                        case '(':
                            tokens.Add(new Token()
                            {
                                TokenType = TokenType.LeftBracket,
                                Value = characters[i],
                                StartPosition = i,
                                Length = 1
                            });
                            isFormulaSubPart = true;
                            break;
                        case ')':
                            tokens.Add(new Token()
                            {
                                TokenType = TokenType.RightBracket,
                                Value = characters[i],
                                StartPosition = i,
                                Length = 1
                            });
                            isFormulaSubPart = false;
                            break;
                        default:
                            throw new ParseException(string.Format("Invalid token \"{0}\" detected at position {1}.",
                                characters[i], i));
                    }
                }
            }

            return tokens;
        }

        private bool IsPartOfNumeric(char character, bool isFirstCharacter, bool isFormulaSubPart)
        {
            return character == decimalSeparator || char.IsDigit(character);
        }

        private string ReadWholeNumber(ref string expression, int start)
        {
            StringBuilder result = new StringBuilder();
            bool dot = false;
            bool e = false;
            for (int i = start; i < expression.Length; i++)
            {
                if (char.IsDigit(expression[i]))
                {
                    result.Append(expression[i]);
                }
                else if (expression[i] == '.' && !dot && !e)
                {
                    result.Append(expression[i]);
                    dot = true;
                }
                else if ((expression[i] == 'e' || expression[i] == 'E') && !e)
                {
                    result.Append(expression[i]);
                    e = true;
                }
                else if ((expression[i] == '+' || expression[i] == '-') && i != start &&
                         (expression[i - 1] == 'E' || expression[i - 1] == 'e'))
                {
                    result.Append(expression[i]);
                }
                else
                {
                    break;
                }
            }

            var ret = result.ToString();
            return ret;
        }

        private bool IsUnaryMinus(char currentToken, List<Token> tokens)
        {
            if (currentToken == '-')
            {
                if (tokens.Count == 0) return true;
                Token previousToken = tokens[tokens.Count - 1];

                return !(previousToken.TokenType == TokenType.Number ||
                         previousToken.TokenType == TokenType.Text ||
                         previousToken.TokenType == TokenType.Unit ||
                         previousToken.TokenType == TokenType.RightBracket);
            }
            else
                return false;
        }
    }
}
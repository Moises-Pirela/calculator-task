using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum OperationType
{
    Exponent = 0,
    Addition,
    Subtraction,
    Division,
    Multiplication,
    Modulo
}

public struct Operation
{
    public OperationType OperationType;
    public double[] Numbers;

    public double Calculate()
    {
        switch (OperationType)
        {
            case OperationType.Exponent:
                return Math.Pow(Numbers[0], Numbers[1]);
            case OperationType.Addition:
                return Numbers[0] + Numbers[1];
            case OperationType.Subtraction:
                return Numbers[0] - Numbers[1];
            case OperationType.Division:
                return Numbers[0] / Numbers[1];
            case OperationType.Multiplication:
                return Numbers[0] * Numbers[1];
            case OperationType.Modulo:
                return Numbers[0] % Numbers[1];
        }
        return 0;
    }
}

public class Calculator
{
    private static readonly Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int>()
    {
        {"+", 1},
        {"-", 1},
        {"*", 2},
        {"/", 2},
        {"%", 2},
        {"^", 3}
    };

    private static double LastResult;
    private static bool HasLastResult;

    public static void Main(string[] args)
    {
        Console.Title = "Simple Calculator";

        Console.WriteLine("Input an expression");

        while (true)
        {
            if (HasLastResult)
                Console.WriteLine($"Last result: {LastResult}");
            
            string userInput = Console.ReadLine();
            
            if (userInput == "clear")
            {
                HasLastResult = false;
                LastResult = 0;
            }
            else if (IsValidInput(userInput))
            {   
                double result = EvaluateExpression(userInput);
            
                Console.WriteLine($"{userInput} = {result}");   
            }
        }
    }

    private static bool IsValidInput(string userInput)
    {
        int errors = Regex.Matches(userInput,@"[a-zA-Z]").Count;
        
        if (errors > 0)
        {
            Console.WriteLine("Please input a valid expression using digits and operators only (+ - * / ^ %)");
        }

        return errors == 0                      &&
               !String.IsNullOrEmpty(userInput) && 
               !string.IsNullOrWhiteSpace(userInput);

    }

    public static double EvaluateExpression(string expression)
    {
        string[] tokens = ParseExpression(expression);
        Stack<double> numberStack = new Stack<double>();
        Stack<string> operatorStack = new Stack<string>();

        if (HasLastResult)
            numberStack.Push(LastResult);

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            if (double.TryParse(token, out double number))
            {
                numberStack.Push(number);
            }
            else if (IsOperator(token))
            {
                if (token == "-" && (i == 0 || IsOperator(tokens[i - 1]) || tokens[i - 1] == "("))
                {
                    // Handle negative number
                    if (double.TryParse(tokens[i + 1], out double negativeNumber))
                    {
                        numberStack.Push(-negativeNumber);
                        i++; // Skip the next token since it has been processed as part of the negative number
                    }
                    else
                    {
                        throw new ArgumentException("Invalid expression: expected number after '-' operator");
                    }
                }
                else
                {
                    while (operatorStack.Count > 0 &&
                           IsOperator(operatorStack.Peek()) &&
                           OperatorPrecedence[token] <= OperatorPrecedence[operatorStack.Peek()])
                    {
                        ApplyOperator(numberStack, operatorStack);
                    }

                    operatorStack.Push(token);
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            ApplyOperator(numberStack, operatorStack);
        }

        return numberStack.Pop();
    }

    private static string[] ParseExpression(string expression)
    {
        string[] tokens = Regex.Split(expression, @"(\+|\-|\*|\/|\%|\^)");
        List<string> sanitizedTokens = new List<string>();

        foreach (string token in tokens)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                sanitizedTokens.Add(token.Trim());
            }
        }

        return sanitizedTokens.ToArray();
    }

    private static void ApplyOperator(Stack<double> numberStack, Stack<string> operatorStack)
    {
        string op = operatorStack.Pop();
        double operand2 = numberStack.Pop();
        double operand1 = numberStack.Pop();
        double result = Calculate(operand1, operand2, op);
        LastResult = result;
        HasLastResult = true;
        numberStack.Push(result);
    }

    private static double Calculate(double operand1, double operand2, string op)
    {
        OperationType operationType;

        switch (op)
        {
            case "^":
                operationType = OperationType.Exponent;
                break;
            case "+":
                operationType = OperationType.Addition;
                break;
            case "-":
                operationType = OperationType.Subtraction;
                break;
            case "*":
                operationType = OperationType.Multiplication;
                break;
            case "/":
                operationType = OperationType.Division;
                break;
            case "%":
                operationType = OperationType.Modulo;
                break;
            default:
                throw new InvalidOperationException($"Invalid operator: {op}");
        }

        Operation operation = new Operation()
        {
            OperationType = operationType,
            Numbers = new double[] { operand1, operand2 }
        };

        return operation.Calculate();
    }

    private static bool IsOperator(string token)
    {
        return OperatorPrecedence.ContainsKey(token);
    }
}

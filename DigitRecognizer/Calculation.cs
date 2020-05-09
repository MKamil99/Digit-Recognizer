using System;
using System.Collections.Generic;

namespace DigitRecognizer
{
    class Calculation //Aby obliczyć RPN: string result = toRPN(dzialanie); Stack<string> temp = ConvertToStack(result); double wynik = evalRPN(temp);
    {
        public static string Calculate(string equation)
        {
            if (equation.EndsWith(" - ") || equation.EndsWith(" + ") || equation.EndsWith(" * ") || equation.EndsWith(" / "))
                return "BŁĘDNY ZAPIS!";
            string result = toRPN(equation); 
            Stack<string> temp = ConvertToStack(result); 
            return " = " + evalRPN(temp).ToString();
        }

        public static string toRPN(string token) //metoda zwracająca wyrażenie w RPN w stringu
        {
            Dictionary<string, int> precedence = new Dictionary<string, int>
            {
                { "+", 1 }, { "-", 1 }, { "/", 2 }, { "*", 2 }
            };

            Stack<string> stack = new Stack<string>();

            string result = "";
            string[] equation = token.Split(' ');

            foreach (string item in equation)
            {
                try
                {
                    double temp = Convert.ToDouble(item);
                    result += " " + item;
                }
                catch
                {
                    while (stack.Count != 0 && precedence[item] <= precedence[stack.Peek()])
                        result += " " + stack.Pop();

                    stack.Push(item);
                }
            }

            while (stack.Count != 0)
                result += " " + stack.Pop();

            result = result.Remove(0, 1);

            return result;
        }

        public static Stack<string> ConvertToStack(string tokens) //metoda konwertująca string na Stack
        {
            string[] result = tokens.Split();
            Stack<string> stack = new Stack<string>();

            foreach (string token in result)
                stack.Push(token);

            return stack;
        }

        public static double evalRPN(Stack<string> tokens) //metoda zwracająca wynik wyrażenia w RPN
        { 
            string token = tokens.Pop();
            double firstNumber, secondNumber;

            if (!Double.TryParse(token, out firstNumber))
            {
                secondNumber = evalRPN(tokens);
                firstNumber = evalRPN(tokens);

                if (token == "+")
                    firstNumber += secondNumber;
                else if (token == "-")
                    firstNumber -= secondNumber;
                else if (token == "*")
                    firstNumber *= secondNumber;
                else if (token == "/")
                    firstNumber /= secondNumber;
                else throw new Exception();
            }

            return firstNumber;
        }
    }
}

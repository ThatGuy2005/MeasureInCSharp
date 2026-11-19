using System;
using System.Diagnostics;

namespace PalindromeChecker
{
    internal class Program
    {
        private const int MaxSize = 1000;

        private readonly struct TestResult
        {
            public bool IsPalindrome { get; }
            public int Cycles { get; }
            public double DurationNs { get; }

            public TestResult(bool isPalindrome, int cycles, double durationNs)
            {
                IsPalindrome = isPalindrome;
                Cycles = cycles;
                DurationNs = durationNs;
            }
        }

        private class Node
        {
            public char Data;
            public Node? Next;

            public Node(char data, Node? next = null)
            {
                Data = data;
                Next = next;
            }
        }

        // --- A) STATIC STACK (Fixed-size array) ---
        private class StaticStack
        {
            private readonly char[] _arr = new char[MaxSize];
            private int _top = -1;

            public bool IsEmpty => _top == -1;

            public void Push(char c)
            {
                if (_top < MaxSize - 1)
                {
                    _arr[++_top] = c;
                }
            }

            public char Pop()
            {
                return !IsEmpty ? _arr[_top--] : '\0';
            }
        }

        // --- A) DYNAMIC QUEUE (Singly linked list) ---
        private class DynamicQueue
        {
            private Node? _front;
            private Node? _rear;

            public bool IsEmpty => _front == null;

            public void Enqueue(char c)
            {
                Node newNode = new Node(c);
                if (_rear == null)
                {
                    _front = _rear = newNode;
                    return;
                }
                _rear.Next = newNode;
                _rear = newNode;
            }

            public char Dequeue()
            {
                if (IsEmpty) return '\0';
                char c = _front!.Data;
                _front = _front.Next;
                if (_front == null) _rear = null;
                return c;
            }
        }

        // --- B) DYNAMIC STACK (Singly linked list) ---
        private class DynamicStack
        {
            private Node? _top;

            public bool IsEmpty => _top == null;

            public void Push(char c)
            {
                _top = new Node(c, _top);
            }

            public char Pop()
            {
                if (IsEmpty) return '\0';
                char c = _top!.Data;
                _top = _top.Next;
                return c;
            }
        }

        // --- B) STATIC QUEUE (Circular fixed-size array) ---
        private class StaticQueue
        {
            private readonly char[] _arr = new char[MaxSize];
            private int _front = 0;
            private int _rear = 0;
            private int _count = 0;

            public bool IsEmpty => _count == 0;

            public void Enqueue(char c)
            {
                if (_count < MaxSize)
                {
                    _arr[_rear] = c;
                    _rear = (_rear + 1) % MaxSize;
                    _count++;
                }
            }

            public char Dequeue()
            {
                if (IsEmpty) return '\0';
                char c = _arr[_front];
                _front = (_front + 1) % MaxSize;
                _count--;
                return c;
            }
        }

        private static TestResult CheckMethodA(string word)
        {
            long startTimestamp = Stopwatch.GetTimestamp();
            int cycles = 0;
            bool result = true;

            StaticStack s = new StaticStack();
            DynamicQueue q = new DynamicQueue();

            foreach (char c in word)
            {
                cycles++;
                char lowerC = char.ToLowerInvariant(c);
                s.Push(lowerC);
                q.Enqueue(lowerC);
            }

            while (!s.IsEmpty && !q.IsEmpty)
            {
                cycles++;
                if (s.Pop() != q.Dequeue())
                {
                    result = false;
                    break;
                }
            }

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            return new TestResult(result, cycles, elapsed.TotalNanoseconds);
        }

        private static TestResult CheckMethodB(string word)
        {
            long startTimestamp = Stopwatch.GetTimestamp();
            int cycles = 0;
            bool result = true;

            DynamicStack s = new DynamicStack();
            StaticQueue q = new StaticQueue();

            foreach (char c in word)
            {
                cycles++;
                char lowerC = char.ToLowerInvariant(c);
                s.Push(lowerC);
                q.Enqueue(lowerC);
            }

            while (!s.IsEmpty && !q.IsEmpty)
            {
                cycles++;
                if (s.Pop() != q.Dequeue())
                {
                    result = false;
                    break;
                }
            }

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            return new TestResult(result, cycles, elapsed.TotalNanoseconds);
        }

        private static void Main()
        {
            Console.Write("Enter a word: ");
            string word = Console.ReadLine()?.Trim() ?? string.Empty;

            TestResult resA = CheckMethodA(word);
            TestResult resB = CheckMethodB(word);

            Console.WriteLine("\n--- Option A (Static Stack + Dynamic Queue) ---");
            Console.WriteLine($"Result: {(resA.IsPalindrome ? "Palindrome" : "Not a palindrome")}");
            Console.WriteLine($"Cycles: {resA.Cycles}");
            Console.WriteLine($"Time:   {resA.DurationNs:F0} ns ({resA.DurationNs / 1000.0:F3} us)");

            Console.WriteLine("\n--- Option B (Dynamic Stack + Static Queue) ---");
            Console.WriteLine($"Result: {(resB.IsPalindrome ? "Palindrome" : "Not a palindrome")}");
            Console.WriteLine($"Cycles: {resB.Cycles}");
            Console.WriteLine($"Time:   {resB.DurationNs:F0} ns ({resB.DurationNs / 1000.0:F3} us)");
        }
    }
}
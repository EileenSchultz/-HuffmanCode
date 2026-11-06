using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuffmanCoding
{
    public class HuffmanCoder
    {
        // 1) Частоты символов
        public Dictionary<char, int> BuildFrequencyTable(string text)
        {
            var freq = new Dictionary<char, int>();
            foreach (var c in text)
                freq[c] = freq.TryGetValue(c, out var n) ? n + 1 : 1;
            return freq;
        }

        // 2) Построение дерева ДВУМЯ ОЧЕРЕДЯМИ (классический two-queue Huffman)
        public HuffmanNode BuildTree(string text, bool verbose = false)
        {
            var freq = BuildFrequencyTable(text);
            if (freq.Count == 0)
                throw new ArgumentException("Пустая строка", nameof(text));

            // Очередь 1: листья, отсортированные по частоте
            var leavesQueue = new Queue<HuffmanNode>(
                freq.OrderBy(kv => kv.Value)
                    .ThenBy(kv => kv.Key)
                    .Select(kv => new HuffmanNode(kv.Key, kv.Value)));

            // Очередь 2: внутренние узлы (построятся в нужном порядке сами)
            var internalQueue = new Queue<HuffmanNode>();

            if (verbose)
            {
                Console.WriteLine("Начальное состояние очередей:");
                PrintQueues(leavesQueue, internalQueue);
            }

            // Достаём минимум из фронтов двух очередей
            HuffmanNode TakeMin()
            {
                if (leavesQueue.Count == 0) return internalQueue.Dequeue();
                if (internalQueue.Count == 0) return leavesQueue.Dequeue();
                return leavesQueue.Peek().Frequency <= internalQueue.Peek().Frequency
                    ? leavesQueue.Dequeue()
                    : internalQueue.Dequeue();
            }

            int step = 0;
            while (leavesQueue.Count + internalQueue.Count > 1)
            {
                step++;
                var left  = TakeMin();
                var right = TakeMin();
                var parent = new HuffmanNode(null, left.Frequency + right.Frequency,
                                             left, right);
                internalQueue.Enqueue(parent);

                if (verbose)
                    Console.WriteLine($"Шаг {step}: слили " +
                        $"'{left.Symbol?.ToString() ?? "*"}':{left.Frequency} + " +
                        $"'{right.Symbol?.ToString() ?? "*"}':{right.Frequency} " +
                        $"-> {parent.Frequency}");
            }

            return TakeMin();
        }

        private static void PrintQueues(Queue<HuffmanNode> leaves, Queue<HuffmanNode> internalQ)
        {
            Console.WriteLine("  leaves:   [" +
                string.Join(", ", leaves.Select(n => $"{n.Symbol}:{n.Frequency}")) + "]");
            Console.WriteLine("  internal: [" +
                string.Join(", ", internalQ.Select(n => $"{n.Symbol}:{n.Frequency}")) + "]");
        }

        // 3) Обход дерева: 0 — налево, 1 — направо
        public Dictionary<char, string> BuildCodes(HuffmanNode root)
        {
            var codes = new Dictionary<char, string>();
            Traverse(root, "", codes);
            return codes;
        }

        private void Traverse(HuffmanNode? node, string code,
                              Dictionary<char, string> codes)
        {
            if (node == null) return;
            if (node.IsLeaf)
            {
                codes[node.Symbol!.Value] = code.Length == 0 ? "0" : code;
                return;
            }
            Traverse(node.Left,  code + "0", codes);
            Traverse(node.Right, code + "1", codes);
        }

        // 4) Кодирование
        public string Encode(string text, Dictionary<char, string> codes)
        {
            var sb = new StringBuilder(text.Length * 2);
            foreach (var c in text) sb.Append(codes[c]);
            return sb.ToString();
        }

        // 5) Декодирование: бит за битом спускаемся по дереву
        public string Decode(string bits, HuffmanNode root)
        {
            if (root.IsLeaf)
            {
                if (bits.Any(b => b != '0'))
                    throw new InvalidOperationException("Некорректная битовая строка");
                return new string(root.Symbol!.Value, bits.Length);
            }

            var sb = new StringBuilder();
            var current = root;

            foreach (var bit in bits)
            {
                current = (bit == '0' ? current.Left : current.Right)
                          ?? throw new InvalidOperationException($"Некорректный бит: {bit}");
                if (current.IsLeaf)
                {
                    sb.Append(current.Symbol!.Value);
                    current = root;
                }
            }

            if (current != root)
                throw new InvalidOperationException("Последовательность оборвалась посреди кода");
            return sb.ToString();
        }
    }
}
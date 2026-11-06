using System;

namespace HuffmanCoding
{
    /// <summary>
    /// Узел дерева Хаффмана.
    /// В листовых узлах хранится символ, во внутренних — Symbol = null.
    /// </summary>
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char? Symbol { get; }
        public int Frequency { get; }
        public HuffmanNode? Left { get; }
        public HuffmanNode? Right { get; }

        public HuffmanNode(char? symbol, int frequency,
            HuffmanNode? left = null, HuffmanNode? right = null)
        {
            Symbol = symbol;
            Frequency = frequency;
            Left = left;
            Right = right;
        }

        public bool IsLeaf => Left == null && Right == null;

        public int CompareTo(HuffmanNode? other)
            => other == null ? 1 : Frequency.CompareTo(other.Frequency);
    }
}
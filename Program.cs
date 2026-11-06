using System;
using System.Linq;

namespace HuffmanCoding
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Пример из главы 7 «Грокаем алгоритмы»
            var text = "paranoid android";
            var coder = new HuffmanCoder();

            var root = coder.BuildTree(text, verbose: true);
            var codes = coder.BuildCodes(root);
            var freq  = coder.BuildFrequencyTable(text);

            Console.WriteLine($"Исходный текст : \"{text}\"");
            Console.WriteLine($"Длина          : {text.Length} симв. = {text.Length * 8} бит (ISO-8859-1)\n");

            Console.WriteLine("Коды Хаффмана (частые буквы — короче):");
            foreach (var kv in codes.OrderBy(kv => kv.Value.Length).ThenBy(kv => kv.Key))
            {
                var shown = kv.Key == ' ' ? '␣' : kv.Key;
                Console.WriteLine($"  '{shown}' (частота {freq[kv.Key],2}) -> {kv.Value}");
            }

            var encoded = coder.Encode(text, codes);
            Console.WriteLine($"\nЗакодировано : {encoded}");
            Console.WriteLine($"Длина кода   : {encoded.Length} бит");
            Console.WriteLine($"Сжатие       : {(1 - (double)encoded.Length / (text.Length * 8)) * 100:F1} %\n");

            var decoded = coder.Decode(encoded, root);
            Console.WriteLine($"Декодировано : \"{decoded}\"");
            Console.WriteLine(decoded == text ? "✅ Совпадает с исходным" : "❌ Ошибка!");
        }
    }
}
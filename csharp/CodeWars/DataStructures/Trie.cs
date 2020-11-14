using System.Collections.Generic;

namespace CodeWars.DataStructures
{
    public class Trie
    {
        private readonly char? _letter;
        private readonly Dictionary<char, Trie> _children = new Dictionary<char, Trie>();


        public Trie(char? letter)
        {
            _letter = letter;
        }

        public int Count { get; set; }

        public Trie GetChild(char letter)
        {
            if (_children.TryGetValue(letter, out var value))
                return value;
            return null;
        }

        public Trie AddChild(char letter)
        {
            var child = new Trie(letter);
            _children.Add(letter, child);
            return child;
        }

        public void AddWord(string word)
        {
            var runner = this;
            foreach (var chr in word)
            {
                var next = runner.GetChild(chr)
                           ?? runner.AddChild(chr);
                next.Count++;
                runner = next;
            }
        }

        public int FindCount(string prefix)
        {
            var runner = this;
            foreach (var chr in prefix)
            {
                var next = runner.GetChild(chr);
                if (next == null)
                    return 0;
                runner = next;
            }
            return runner.Count;
        }
    }
}


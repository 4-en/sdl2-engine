using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Utils
{
    public class Highscores<N> : IEnumerable<Tuple<string, N>> where N : IComparable, IFormattable
    {

        private List<Tuple<string, N>> highscores;
        private int maxHighscores = 100;
        private string? path = null;

        public Highscores(int maxHighscores=100, string? path=null)
        {
            this.maxHighscores = maxHighscores;
            this.path = path;
            this.highscores = new();
        }

        public void AddHighscore(string name, N score)
        {
            bool remove_last = highscores.Count >= maxHighscores;
            int maxIndex = highscores.Count >= maxHighscores ? maxHighscores : highscores.Count;


            for (int i = 0; i < maxIndex; i++)
            {
                if (score.CompareTo(highscores[i].Item2) > 0)
                {
                    highscores.Insert(i, new Tuple<string, N>(name, score));
                    if (remove_last)
                    {
                        highscores.RemoveAt(maxHighscores);
                    }
                    return;
                }
            }

            if (highscores.Count < maxHighscores)
            {
                highscores.Add(new Tuple<string, N>(name, score));
            }
            
        }

        private void SortHighscores()
        {
            highscores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        }

        public List<Tuple<string, N>> GetHighscores(int n = 0)
        {
            if (n > 0)
            {
                return highscores.GetRange(0, n);
            }
            else
            {
                return highscores;
            }
        }

        public void ClearHighscores()
        {
            highscores.Clear();
        }

        private static string ConvertTupleToString(Tuple<string, N> tuple)
        {
            return tuple.Item1 + "=" + tuple.Item2.ToString();
        }

        private static Tuple<string, N> ConvertStringToTuple(string str)
        {
            int lastEquals = str.LastIndexOf("=");
            string[] parts = new string[2];
            parts[0] = str.Substring(0, lastEquals);
            parts[1] = str.Substring(lastEquals + 1);

            return new Tuple<string, N>(parts[0], (N)Convert.ChangeType(parts[1], typeof(N)));
        }

        public void Save(string? path=null)
        {
            path = path ?? this.path ?? "highscores.txt"; // default path is "highscores.txt

            string[] strings = new string[highscores.Count];
            for (int i = 0; i < highscores.Count; i++)
            {
                strings[i] = ConvertTupleToString(highscores[i]);
            }

            Serialization.SaveArray(strings, path);

        }

        public static Highscores<N> Load(string? path, int maxHighscores=100)
        {

            path = path ?? "highscores.txt"; // default path is "highscores.txt

            Highscores<N> highscores = new(maxHighscores);

            string[] strings = Serialization.LoadArray<string>((string s) => s, path);

            for (int i = 0; i < strings.Length; i++)
            {
                highscores.AddHighscore(ConvertStringToTuple(strings[i]).Item1, ConvertStringToTuple(strings[i]).Item2);
            }

            highscores.SortHighscores();

            return highscores;
        }

        public List<Tuple<string, string>> AsString()
        {
            List<Tuple<string, string>> result = new();
            for (int i = 0; i < highscores.Count; i++)
            {
                result.Add(new Tuple<string, string>(highscores[i].Item1, item2: highscores[i].Item2.ToString() ?? "null"));
            }

            return result;
        }

        public IEnumerator<Tuple<string, N>> GetEnumerator()
        {
            return highscores.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           return highscores.GetEnumerator();
        }
    }
}

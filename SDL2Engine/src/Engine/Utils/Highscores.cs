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

        public void AddHighscore(string name, N score, bool save=false)
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
                    if (save)
                    {
                        Save();
                    }

                    return;
                }
            }

            if (highscores.Count < maxHighscores)
            {
                highscores.Add(new Tuple<string, N>(name, score));
                if (save)
                {
                    Save();
                }
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

        public virtual void Save(string? path=null)
        {
            path = path ?? this.path ?? "highscores.txt"; // default path is "highscores.txt

            string[] strings = new string[highscores.Count];
            for (int i = 0; i < highscores.Count; i++)
            {
                strings[i] = ConvertTupleToString(highscores[i]);
            }

            Serialization.SaveArray(strings, path);

        }

        public static Highscores<N> LoadFile(string? path, int maxHighscores=100)
        {

            path = path ?? "highscores.txt"; // default path is "highscores.txt

            Highscores<N> highscores = new(maxHighscores);
            highscores.path = path;

            highscores.Update();

            return highscores;
        }

        public virtual void Update()
        {

            var hs_path = path ?? "highscores.txt";

            string[] strings = Serialization.LoadArray<string>((string s) => s, hs_path);

            for (int i = 0; i < strings.Length; i++)
            {
                AddHighscore(ConvertStringToTuple(strings[i]).Item1, ConvertStringToTuple(strings[i]).Item2);
            }

            SortHighscores();

        }

        public List<Tuple<string, string>> AsString(int max=0)
        {
            max = max > 0 ? max : highscores.Count;
            max = Math.Min(max, highscores.Count);
            List<Tuple<string, string>> result = new();
            for (int i = 0; i < max; i++)
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

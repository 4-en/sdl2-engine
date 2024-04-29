using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Pong.src
{
    public class ScoreManager
    {
        private int points = 0;
        private IntPtr font;
        private SDL_Color textColor = new SDL_Color { r = 255, g = 255, b = 255, a = 255 }; // Weiß

        // Methode zum Erhöhen der Punktzahl um 1
        public void IncreaseScore()
        {
            points++;
            Console.WriteLine("Points: " + points); // Ausgabe der Punkte zum Testen
        }

        // Methode zum Zurücksetzen der Punktzahl auf 0
        public void ResetScore()
        {
            points = 0;
            Console.WriteLine("Score reset to 0");
        }

        // Methode zum Abrufen der aktuellen Punktzahl
        public int GetScore()
        {
            return points;
        }
    }
}

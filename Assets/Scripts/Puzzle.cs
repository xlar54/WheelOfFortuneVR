using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class Puzzle
    {
        public string Category;
        public string[] AnswerLine = new string[4];
        public string Answer;

        public void GeneratePuzzle()
        {
            string[] puzzles = {
                "ONLY YOU CAN PREVENT FOREST FIRES",
                "THE EMPIRE STRIKES BACK",
                "KERMIT THE FROG",
                "A STITCH IN TIME SAVES NINE",
                "PRESCRIPTION MEDICATION",
                "ICE CREAM SANDWHICH",
                "GEORGE JEFFERSON",
                "ADDRESS BOOK",
                "BOOKSHELF",
                "BEANBAG CHAIR",
                "BOOKENDS",
                "CERAMIC BOWL",
                "ELECTRIC TOOTHBRUSH",
                "FISHING TACKLE BOX",
                "HOME OFFICE",
                "JEWEL BOX",
                "LAVA LAMP",
                "LINEN NAPKINS",
                "MOUSEPAD",
                "OAK CHINA CABINET",
                "POLISHING WAX",
                "PENS AND PENCILS",
                "PHOTO ALBUM",
                "PROFICIENTLY DISPLAYED AWARDS",
                "PRUNING SHEARS",
                "RECLINING LOUNGE CHAIR",
                "SHAMPOO AND CONDITIONER",
                "STURDY TABLE",
                "WINDOW SCREENS",
                "WOODEN TABLE"
            };

            string[] categories =
            {
                "Phrase",
                "Movie",
                "Person",
                "Phrase",
                "Thing",
                "Thing",
                "Person",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
                "Thing",
            };

            Random random = new Random();
            int randomNumber = random.Next(0, puzzles.Length);

            this.Answer = puzzles[randomNumber];
            this.Category = categories[randomNumber];

            string[] puzzleWordArray = this.Answer.Split(' ');

            int row = 0;

            AnswerLine[0] = "";
            AnswerLine[1] = "";
            AnswerLine[2] = "";
            AnswerLine[3] = "";

            foreach(string s in puzzleWordArray)
            {
                string txt = AnswerLine[row];
                txt += " " + s;

                if (txt.Length <= 12)
                {
                    AnswerLine[row] += " " + s;
                }
                else
                {
                    row++;
                    AnswerLine[row] += " " + s;
                }
            }

            if (AnswerLine[3].Trim() == "" && AnswerLine[2].Trim() == "")
            {
                AnswerLine[2] = AnswerLine[1];
                AnswerLine[1] = AnswerLine[0];
                AnswerLine[0] = "";
            }

            AnswerLine[0] = PadBoth(AnswerLine[0], 12);
            AnswerLine[1] = PadBoth(AnswerLine[1], 14);
            AnswerLine[2] = PadBoth(AnswerLine[2], 14);
            AnswerLine[3] = PadBoth(AnswerLine[3], 12);


        }

        public string PadBoth(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);

        }
    }

}

using System;
using System.Collections.Generic;

namespace Pages.Data
{
    public class BandInfo
    {
        public string Name { get; }
        public string Origin { get; }
        public string Bio { get; }
        public string Image { get; }
        public int Members { get; }
        public int Price { get; }
        public Dictionary<string, int> Occupancy { get; }

        public BandInfo(int number)
        {
            Random rndm = new Random();
            Name = getRanomName(rndm);
            Origin = getRandomOrigin(rndm);
            Bio = getLoremSubstr(rndm.Next(25, 150));
            Image = getImage(rndm);
            Members = rndm.Next(2, 10);
            Price = rndm.Next(20, 100);
            Occupancy = getOccupancy(rndm, 2);
        }

        /// <summary>
        /// Returns random occupancy of given band for given months into the future
        /// </summary>
        /// <param name="rndm">System.Random</param>
        /// <param name="months">Int</param>
        /// <returns> Dictionary</returns>
        private static Dictionary<string, int> getOccupancy(Random rndm, int months)
        {
            DateTime date = DateTime.Now;
            Dictionary<string, int> o = new Dictionary<string, int>();
            
            for (int i = 1; i < months + 1; ++i) {
                int y = date.Month / 12;
                int month = ((date.Month + i) % 13) + (y != 0 ? 1 : 0);
                generateOccupancy(rndm, o, date.Year + y, month - 1);
            }

            return o;
        }

        /// <summary>
        /// Fills given dictionary with random occupancy for given month and year
        /// </summary>
        /// <param name="rndm">System.Random</param>
        /// <param name="o">Dictionary</param>
        /// <param name="year">Int</param>
        /// <param name="month">Int</param>
        private static void generateOccupancy(Random rndm, Dictionary<string, int> o, int year, int month)
        {
            int occupiedCount = rndm.Next(2, 12);
            int day = 1;
            int maxDayInc = 31 / occupiedCount;

            for (int i = 0; i < occupiedCount; ++i) {
                o.Add(year + "-" + month.ToString("00") + "-" + rndm.Next(day, day + maxDayInc - 1), 1);
                day += maxDayInc;
            }
        }

        /// <summary>
        /// Returns random combination of predefined adjectives and names
        /// </summary>
        /// <param name="rndm">System.Random</param>
        /// <returns>string</returns>
        private static string getRanomName(Random rndm)
        {
            string[] adjecties = {
                "talented", "talentless", "heavy", "strong", "powerfull", "blue", "dark", "shiny", "golden",
                "crazy", "dull", "raw", "noisy", "blazy", "sparkling", "epic", "legendary", "friendly", "deadly",
                "iron", "black", "celtic", "blind", "limp", "saint", "diamond", "immortal", "night"
            };
            string[] names = {
                "dwarves", "skulls", "elves", "heads", "voices", "lords", "kings", "machines", "monks", "dragons",
                "wind", "stone", "fire", "pirates", "warriors", "storm", "lions", "priests", "blizard", "guns",
                "chain", "factory", "angels", "guardians", "woods"
            };

            return adjecties[rndm.Next(adjecties.Length)] + " " + names[rndm.Next(names.Length)];
        }

        /// <summary>
        /// Returns random place of origin from predefined ones
        /// </summary>
        /// <param name="rndm">Sysyem.Random</param>
        /// <returns>string</returns>
        private static string getRandomOrigin(Random rndm)
        {
            string[] origins = new string[]{
                "US", "France", "Sweden", "UK", "Austaralia", "Austria", "Germany", "Czech Rep.", "Norway"
            };

            return origins[rndm.Next(origins.Length)];
        }

        /// <summary>
        /// Returns substring of Lorem Ipsum of given length
        /// </summary>
        /// <param name="length">int</param>
        /// <returns>string</returns>
        private static string getLoremSubstr(int length)
        {
            const string loremIpsum = "" +
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut " +
                "labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris " +
                "nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit" +
                " esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt" +
                " in culpa qui officia deserunt mollit anim id est laborum.";

            if (length >= loremIpsum.Length)
                return loremIpsum;

            return loremIpsum.Substring(length) + "...";
        }

        /// <summary>
        /// Returns randomly one of predefined images
        /// </summary>
        /// <param name="rndm">System.Random</param>
        /// <returns>string</returns>
        private static string getImage(Random rndm)
        {
            string[] imgs = new string[] {
                "https://images.unsplash.com/photo-1524721696987-b9527df9e512?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1790&q=80",
                "https://images.unsplash.com/photo-1541701494587-cb58502866ab?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1488554378835-f7acf46e6c98?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1051&q=80",
                "https://images.unsplash.com/photo-1496715976403-7e36dc43f17b?ixlib=rb-1.2.1&auto=format&fit=crop&w=1950&q=80",
                "https://images.unsplash.com/photo-1529641484336-ef35148bab06?ixlib=rb-1.2.1&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1494253109108-2e30c049369b?ixlib=rb-1.2.1&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1550684848-86a5d8727436?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1518289646039-3e6c87a5aaf6?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1051&q=80",
                "https://images.unsplash.com/photo-1510906594845-bc082582c8cc?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1028&q=80",
                "https://images.unsplash.com/photo-1550684848-fac1c5b4e853?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1431576901776-e539bd916ba2?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1051&q=80",
                "https://images.unsplash.com/photo-1533693706533-57740e69765d?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1485254767195-60704c46702e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1051&q=80",
                "https://images.unsplash.com/photo-1523895665936-7bfe172b757d?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1500043204644-768d20653f32?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1492546643178-96d64f3fd824?ixlib=rb-1.2.1&auto=format&fit=crop&w=1051&q=80",
                "https://images.unsplash.com/photo-1462556791646-c201b8241a94?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1045&q=80",
                "https://images.unsplash.com/photo-1505934333218-8fe21ff87e69?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1488229297570-58520851e868?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1049&q=80",
                "https://images.unsplash.com/photo-1518155532395-6d065a269628?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                "https://images.unsplash.com/photo-1537237858032-3ad1b513cbcc?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80"
            };

            return imgs[rndm.Next(imgs.Length)];
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

public class RazorModel : PageModel
{
    public static BandInfo[] bands = getDummyData();

    private static BandInfo[] getDummyData()
    {
        int bandCount = 20;
        BandInfo[] b = new BandInfo[bandCount];

        for (int i = 0; i < bandCount; ++i) {
            b[i] = new BandInfo(i);
        }

        return b;
    }

    public class BandInfo
    {
        public string name { get; }
        public string origin { get; }
        public int members { get; }
        public int price { get; }
        public KeyValuePair<string, int> occupancy { get; }

        public BandInfo(int number) {
            Random rndm = new Random();
            name = getRanomName(rndm);
            origin = "Unknown";
            members = rndm.Next(2, 10);
            price = rndm.Next(20, 100);
            occupancy = getOccupancy(rndm);

        }

        private static KeyValuePair<string, int> getOccupancy(Random rndm)
        {
            int occupiedCount = rndm.Next(5, 20);
            DateTime date = DateTime.Now;
            Dictionary<string, int> o = new Dictionary<string, int>();
            int day = 1;
            int maxDayInc = 31 / occupiedCount;

            for (int i = 0; i < occupiedCount; ++i) {
                o.Add(date.Year + "-" + date.Month + "-" + rndm.Next(day, day + maxDayInc - 1), 1);
                day += maxDayInc;
            }
            return new KeyValuePair<string, int>();
            //return o.ToArray();
        }

        private static string getRanomName(Random rndm)
        {
            string[] adjecties = {
                "talented", "talentless", "heavy", "strong", "powerfull", "blue", "dark", "shiny", "golden",
                "crazy", "dull", "raw", "noisy", "blazy", "sparkling", "epic", "legendary"
            };
            string[] names = {
                "dwarves", "skulls", "elves", "heads", "voices", "lords", "kings", "machines", "monks", "dragons",
                "wind", "stone", "fire", "pirates", "warriors"
            };

            return adjecties[rndm.Next(adjecties.Length)] + " " + names[rndm.Next(names.Length)];
        }
    }
}
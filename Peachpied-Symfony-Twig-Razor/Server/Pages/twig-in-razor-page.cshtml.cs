using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

public class TwigRazorModel : PageModel
{
    public static KeyValuePair<string, int>[] data;

    public TwigRazorModel()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        dict.Add("2019-09-14", 1);
        dict.Add("2019-09-18", 1);
        dict.Add("2019-09-20", 1);
        dict.Add("2019-09-24", 1);

        data = dict.ToArray();
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using Pages.Data;

public class RazorModel : PageModel
{
    public static BandInfo[] bands = getDummyData(20);

    /// <summary>
    /// Creates dummy data of given number of bands
    /// </summary>
    /// <returns>BandInfo[]</returns>
    private static BandInfo[] getDummyData(int count)
    {
        BandInfo[] b = new BandInfo[count];

        for (int i = 0; i < count; ++i) {
            b[i] = new BandInfo(i);
        }

        return b;
    }
}
using Microsoft.AspNetCore.Builder;

namespace razor_plugin
{
    public class RazorCalendarsPlugin
    {
        public string Title { get; } = "Razor Calendars Plugin";
        public string Description { get; } = "Enables rendering razor templates within PHP code or separately";

        public RazorCalendarsPlugin() { }

        public void Configure(IApplicationBuilder app)
        {
           
        }
    }
}

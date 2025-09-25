using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RummikubApp.ViewModels
{
    internal class LogInPageVM
    {
        //אסור כאן הכל צריך להעיף.
        private string name { get; set; } = string.Empty;
        public string Name
        {
            get => Name;
            set
            {
                name = value;
                Preferences.Set("name", name);
            }
        }
        public LogInPageVM()
        {
            name = Preferences.Get("name", string.Empty);
        }
    }
}

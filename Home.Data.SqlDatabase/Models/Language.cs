using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smoehring.Home.Data.SqlDatabase.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string LanguageCultureName { get; set; }
        public string DisplayName { get; set; }

        public virtual ICollection<Currency> Currencies { get; set; }
    }
}

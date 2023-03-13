using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smoehring.Home.Data.SqlDatabase.Models
{
    public class MediaName
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Media Media { get; set; }
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smoehring.Home.Data.SqlDatabase.Models
{
    public class AssetState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Possession { get; set; }
        public bool Ownership { get; set; }
        public bool IsDefault { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
    }
}

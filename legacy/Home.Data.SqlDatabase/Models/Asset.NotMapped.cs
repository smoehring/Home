using System.ComponentModel.DataAnnotations.Schema;

namespace Smoehring.Home.Data.SqlDatabase.Models
{
    public partial class Asset
    {
        /// <summary>
        /// Not a Database Field.
        /// Used to mark this Object as Selected.
        /// </summary>
        [NotMapped] public bool IsSelected { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Models
{
    public class CurrencyRate : BaseEntity
    {
        public int Id { get; set; }
        public Currency Currency { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string? Notes { get; set; }
    }
}

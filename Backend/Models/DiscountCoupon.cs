using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopForHome.API.Models
{
    public class DiscountCoupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; } = string.Empty; // 'Percentage' or 'FixedAmount'

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();

        [NotMapped]
        public bool IsValid => IsActive && DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidTo;

        [NotMapped]
        public bool CanBeUsed => IsValid && (UsageLimit == null || UsedCount < UsageLimit);
    }
}
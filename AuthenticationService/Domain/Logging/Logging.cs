
namespace AuthenticationService.Domain.Logging
{
    // [Table("Log", Schema = "dbo")]
    public class LogDto
    {
        // [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // [Column("Id")]
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ZenegyCalendar.DAL
{
    public class GoogleAuthItem
    {
        [Key]
        [MaxLength(100)]
        public string Key { get; set; }

        [MaxLength(500)]
        public string Value { get; set; }
    }
}
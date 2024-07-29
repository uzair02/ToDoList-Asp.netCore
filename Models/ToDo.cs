using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class ToDo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
    }
}

namespace ToDoApp.Models
{
    public class ToDoViewModel
    {
        public IEnumerable<ToDo> ToDos { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string TitleFilter { get; set; }
    }
}

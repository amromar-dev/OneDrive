namespace OneDrive.Web.Models
{
    public class FileDateGroupedModel
    {
        public DateTime DateGroup { get; set; }

        public string DateGroupFormat => DateGroup.ToString("MMM yyyy");

        public List<FileModel> Files { get; set; }
    }
}

namespace OneDrive.Web.Models
{
    public class FileModel
    {
        public FileModel(string id, string name, string downloadUrl, DateTimeOffset createdAt)
        {
            Id = id;
            Name = name;
            DownloadUrl = downloadUrl;
            CreatedAt = createdAt;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string DownloadUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}

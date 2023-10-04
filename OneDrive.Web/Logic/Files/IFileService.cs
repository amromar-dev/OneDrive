using OneDrive.Web.Models;

namespace OneDrive.Web.Logic.Files
{
    public interface IFileService
    {
        Task<List<FileDateGroupedModel>> GetFilesGroupedByDate();
    }
}
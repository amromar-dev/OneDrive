using Microsoft.Graph;
using OneDrive.Web.Logic.Authentication;
using OneDrive.Web.Models;
using System.Net.Http.Headers;

namespace OneDrive.Web.Logic.Files
{
    public class FileService : IFileService
    {
        private readonly ITokenService tokenService;

        public FileService(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public async Task<List<FileDateGroupedModel>> GetFilesGroupedByDate()
        {
            var accessToken = await tokenService.GetTokenAsync();
            var authProvider = new DelegateAuthenticationProvider(async (request) =>
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                await Task.CompletedTask;
            });

            var graphClient = new GraphServiceClient(authProvider);

            var children = await graphClient.Me.Drive.Root.Children.Request().GetAsync();
            var files = await ScanInnerFiles(graphClient, children);

            return files
                .GroupBy(x => new DateTime(x.CreatedAt.Year, x.CreatedAt.Month, 1))
                .Select(x => new FileDateGroupedModel()
                {
                    DateGroup = x.Key,
                    Files = x.OrderBy(s => s.CreatedAt).ToList(),
                })
                .OrderBy(x => x.DateGroup).ToList();
        }

        #region Private Methods

        private async Task<List<FileModel>> ScanInnerFiles(GraphServiceClient graphClient, IDriveItemChildrenCollectionPage children)
        {
            var files = new List<FileModel>();
            foreach (var child in children)
            {
                if (child.File != null)
                {
                    var downloadUrl = child.AdditionalData["@microsoft.graph.downloadUrl"].ToString();
                    files.Add(new FileModel(child.Id, child.Name, downloadUrl, child.CreatedDateTime.Value));
                    continue;
                }

                if (child.Folder == null)
                    continue;

                if (child.Folder.ChildCount == 0)
                    continue;

                var subChildren = await graphClient.Drive.Items[child.Id].Children.Request().GetAsync();
                var childFiles = await ScanInnerFiles(graphClient, subChildren);
                files.AddRange(childFiles);
            }

            return files;
        }

        #endregion
    }
}

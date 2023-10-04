using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneDrive.Web.Logic.Files;

namespace OneDrive.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IFileService fileService;
        
        public HomeController(ILogger<HomeController> logger, IFileService fileService)
        {
            this.logger = logger;
            this.fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var files = await fileService.GetFilesGroupedByDate();
                return View(files);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());

                ViewData["Error"] = ex.Message.ToString();

                return View();
            }
        }
    }
}
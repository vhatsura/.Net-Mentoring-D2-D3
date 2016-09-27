using System.IO;
using System.Linq;
using System.Web.Mvc;
using ProfileSample.DAL;
using ProfileSample.Models;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ProfileSampleEntities())
            {
                var sources = context.ImgSources
                    .Take(20)
                    .Select(x => new ImageModel()
                    {
                        Name = x.Name,
                        Data = x.Data
                    }).ToList();
                return View(sources);
            }
        }


        public ActionResult Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

            using (var context = new ProfileSampleEntities())
            {
                foreach (var file in files)
                {
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        var entity = new ImgSource()
                        {
                            Name = Path.GetFileName(file),
                            Data = new byte[stream.Length],
                        };

                        stream.Read(entity.Data, 0, (int)stream.Length);

                        context.ImgSources.Add(entity);
                        context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
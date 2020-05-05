using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTurk.Controllers
{
    [ApiController, Route("dwn/{counter}")]
    public class DownloadController : ControllerBase
    {

        [HttpGet]
        public ActionResult Get(int counter)
        {

            var buffer = Encoding.UTF8.GetBytes("Hello! Content is here.");
            var stream = new MemoryStream(buffer);

            var result = new FileStreamResult(stream, "text/plain");
            result.FileDownloadName = $"test.csv";
            return result;
        }


    }
}

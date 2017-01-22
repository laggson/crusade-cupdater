using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Updater.Web.Controllers
{
    public class VersionsController : ApiController
    {
        private const string PATH = @"D:\Data\Update\Content\";

        [HttpGet]
        [Route("api/version/{name}")]
        public HttpResponseMessage GetNewestVersion(string name)
        {
            FileVersionInfo fileVer;
            try
            {
                fileVer = FileVersionInfo.GetVersionInfo(GetPath(name));

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(fileVer.FileVersion);
                return response;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("api/files/{name}")]
        public IEnumerable<string> GetFilesInFolder(string name)
        {
            var data = new List<string>();

            Directory.GetFiles(PATH + name).ToList().ForEach(x => data.Add(x.Substring(PATH.Length -1)));

            return data;
        }

        private string GetPath(string name)
        {
            return Directory.GetFiles(PATH + name).Single(x => x.EndsWith(".exe") && !x.EndsWith(".vshost.exe"));
        }
    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace zyUploadMVC.Controllers
{
    public class HomeController : Controller
    {
        string uploadpath = null;
        HttpPostedFileBase uploadFile = null;
        string oldName = null;
        string fileName = null;
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadPicture()
        {
            return upFile(this);
        }

        public JsonResult upFile(Controller cxt)
        {
            try
            {
                uploadpath = Server.MapPath("/Upload/");
                uploadFile = cxt.Request.Files[0];
                oldName = uploadFile.FileName;
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }


                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + uploadFile.FileName.Substring((uploadFile.FileName.LastIndexOf('.')), (uploadFile.FileName.Length- uploadFile.FileName.LastIndexOf('.')));
                uploadFile.SaveAs(uploadpath + fileName);
                return Json(new {Code=0,Message="上传成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { Code = 1001, Message = "上传失败！"+ex.Message });
            }
        }
    }

    public static class NameFormater
    {
        public static string Format(string format, string filename)
        {
            if (String.IsNullOrWhiteSpace(format))
            {
                format = "{filename}{rand:6}";
            }
            string ext = Path.GetExtension(filename);
            filename = Path.GetFileNameWithoutExtension(filename);
            format = format.Replace("{filename}", filename);
            format = new Regex(@"\{rand(\:?)(\d+)\}", RegexOptions.Compiled).Replace(format, new MatchEvaluator(delegate (Match match)
            {
                var digit = 6;
                if (match.Groups.Count > 2)
                {
                    digit = Convert.ToInt32(match.Groups[2].Value);
                }
                var rand = new Random();
                return rand.Next((int)Math.Pow(10, digit), (int)Math.Pow(10, digit + 1)).ToString();
            }));
            format = format.Replace("{time}", DateTime.Now.Ticks.ToString());
            format = format.Replace("{yyyy}", DateTime.Now.Year.ToString());
            format = format.Replace("{yy}", (DateTime.Now.Year % 100).ToString("D2"));
            format = format.Replace("{mm}", DateTime.Now.Month.ToString("D2"));
            format = format.Replace("{dd}", DateTime.Now.Day.ToString("D2"));
            format = format.Replace("{hh}", DateTime.Now.Hour.ToString("D2"));
            format = format.Replace("{ii}", DateTime.Now.Minute.ToString("D2"));
            format = format.Replace("{ss}", DateTime.Now.Second.ToString("D2"));
            var invalidPattern = new Regex(@"[\\\/\:\*\?\042\<\>\|]");
            format = invalidPattern.Replace(format, "");
            return format + ext;
        }
    }
}
using System;
using System.IO;
using System.Web.Mvc;
namespace SCMS.Actions
{
    public class ExcelResult : ActionResult
    {
        private readonly String _excelStream;
        private readonly String _fileName;
        public ExcelResult(string excel, String fileName)
        {
            _excelStream = excel;
            _fileName = fileName;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("We need a context!");
            }
            var response = context.HttpContext.Response;
            response.ClearContent();
            response.AddHeader("content-disposition", "attachment; filename=" + _fileName);
            response.AddHeader("Content-Type", "application/vnd.ms-excel");
            response.Output.Write(_excelStream);

            response.End();
        }
    }
}
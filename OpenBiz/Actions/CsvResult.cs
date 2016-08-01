using SCMS.DataExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SCMS.Actions
{
    public class CsvResult<T> : ActionResult
    {
        private readonly IQueryable<T> _records;
        private readonly string _filename;
        public CsvResult(IQueryable<T> records, string filename)
        {
            _records = records;
            _filename = filename;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var sb = new StringBuilder();
            sb.Append(_records.ToCsv<T>(","));
            var response = context.HttpContext.Response;
            response.ContentType = "text/csv";
            response.AddHeader("content-disposition",
                String.Format("attachment; filename={0}", _filename));

            response.Write(sb.ToString());

            response.Flush();
            response.End();
        }
    }
}

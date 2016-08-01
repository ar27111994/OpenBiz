using System.Web;
using System.Web.Optimization;

namespace SCMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/FrontEnd/jquery/dist/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/FrontEnd/jquery-validation/dist/jquery.validate.js",
                        "~/Content/FrontEnd/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
                        "~/Content/FrontEnd/jquery-ajax-unobtrusive/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/patternfly").Include(
            "~/Content/FrontEnd/c3/c3.js",
            "~/Content/FrontEnd/d3/d3.js",
            "~/Content/FrontEnd/datatables/media/js/jquery.dataTables.js",
            "~/Content/FrontEnd/datatables-colreorder/js/dataTables.colReorder.js",
            "~/Content/FrontEnd/datatables-colvis/js/dataTables.colVis.js",
            "~/Content/FrontEnd/patternfly/dist/js/patternfly.js",
            "~/Content/FrontEnd/bootstrap-combobox/js/bootstrap-combobox.js",
            "~/Content/FrontEnd/bootstrap-datepicker/dist/js/bootstrap-datepicker.js",
            "~/Content/FrontEnd/bootstrap-select/js/bootstrap-select.js",
            "~/Content/FrontEnd/bootstrap-switch/dist/js/bootstrap-switch.js",
            "~/Content/FrontEnd/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.js",
            "~/Content/FrontEnd/bootstrap-treeview/dist/bootstrap-treeview.min.js",
            "~/Content/FrontEnd/matchHeight/jquery.matchHeight.js",
            "~/Content/FrontEnd/jquery-easing/jquery.easing.js",
            "~/Content/FrontEnd/jsrender/jsrender.js",
            "~/Content/FrontEnd/syncfusion-javascript/Scripts/ej/web/ej.web.all.min.js",
            "~/Content/FrontEnd/moment/moment.js",
            "~/Content/FrontEnd/eonasdan-bootstrap-datetimepicker/build/js/bootstrap-datetimepicker.min.js",
            "~/Content/FrontEnd/ckeditor/ckeditor.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/FrontEnd/modernizr/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/FrontEnd/bootstrap/dist/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/dtExport").Include(
                      "~/Content/FrontEnd/datatables-buttons/js/dataTables.buttons.js",
                      "~/Content/FrontEnd/datatables-buttons/js/buttons.flash.js",
                      "~/Content/FrontEnd/jszip/dist/jszip.js",
                      "~/Content/FrontEnd/jszip/dist/jszip.js",
                      "~/Content/FrontEnd/pdfmake/build/pdfmake.js",
                      "~/Content/FrontEnd/pdfmake/build/vfs_fonts.js",
                      "~/Content/FrontEnd/datatables-buttons/js/buttons.html5.js",
                      "~/Content/FrontEnd/datatables-buttons/js/buttons.print.js"));

            bundles.Add(new StyleBundle("~/Styles/bootstrap").Include(
                      "~/Content/FrontEnd/bootstrap/dist/css/bootstrap.css",
                      "~/Content/FrontEnd/font-awesome/css/font-awesome.css",
                      "~/Content/FrontEnd/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker-standalone.css",
                      "~/Content/FrontEnd/patternfly/dist/css/patternfly.css",
                      "~/Content/FrontEnd/patternfly/dist/css/patternfly-additions.css",
            "~/Content/FrontEnd/syncfusion-javascript/Content/ej/web/flat-azure-dark/ej.web.all.min.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                "~/Content/FrontEnd/custom-scripts/Barcode.js",
                "~/Content/FrontEnd/custom-scripts/crud-modal.js"));

            bundles.Add(new StyleBundle("~/bundles/styles-custom").Include(
                "~/Content/FrontEnd/custom-styles/grid.css"));
        }
    }
}

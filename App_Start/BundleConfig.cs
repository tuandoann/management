using System.Web;
using System.Web.Optimization;

namespace QUANLYTIEC
{
    public class BundleConfig
    {
        /// <summary>
        /// include javasrcipt and style for web
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region script
            #region index home
            bundles.Add(new ScriptBundle("~/Scripts/loadHeaderIndex").Include(
                        "~/Extension/jquery/dist/jquery.min.js",
                        "~/Extension/bootstrap/dist/js/bootstrap.min.js",
                        "~/Extension/moment/moment.min.js",
                        "~/Extension/moment/locale/vi.js",
                        "~/Extension/jquery-cookie/jquery.cookie.js",
                        "~/Extension/validator/validator.js"));
            bundles.Add(new ScriptBundle("~/Scripts/loadfooterIndex").Include(
                        "~/Extension/fastclick/lib/fastclick.js",
                        "~/Extension/nprogress/nprogress.js",
                        "~/Extension/iCheck/icheck.min.js",
                        "~/Extension/toastr-master/toastr.min.js",
                        "~/Scripts/global/globalFunction.js",
                        "~/Scripts/global/globalParameter.js",
                        "~/Scripts/custom.min.js"));
            #endregion
            // datatable.net
            bundles.Add(new ScriptBundle("~/Scripts/datatable").Include(
                        "~/Extension/datatables.net/js/jquery.dataTables.min.js",
                        "~/Extension/datatables.net-bs/js/dataTables.bootstrap.min.js",
                        "~/Extension/datatables.net-buttons/js/dataTables.buttons.min.js",
                        "~/Extension/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
                        "~/Extension/datatables.net-buttons/js/buttons.flash.min.js",
                        "~/Extension/datatables.net-buttons/js/buttons.html5.min.js",
                        "~/Extension/datatables.net-buttons/js/buttons.print.min.js",
                        "~/Extension/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                        "~/Extension/datatables.net-keytable/js/dataTables.keyTable.min.js",
                        "~/Extension/datatables.net-responsive/js/dataTables.responsive.min.js",
                        "~/Extension/datatables.net-responsive-bs/js/responsive.bootstrap.js",
                        "~/Extension/datatables.net-scroller/js/dataTables.scroller.min.js",
                        "~/Extension/datatables.net-select/js/dataTables.select.js",
                        "~/Extension/jszip/dist/jszip.min.js",
                        "~/Extension/iCheck/icheck.min.js",
                        "~/Extension/pdfmake/build/pdfmake.min.js",
                        "~/Extension/pdfmake/build/vfs_fonts.js"));
            // input mask
            bundles.Add(new ScriptBundle("~/Scripts/inputmask").Include(
                        "~/Extension/jquery.inputmask/dist/min/inputmask/inputmask.min.js",
                        "~/Extension/jquery.inputmask/dist/min/inputmask/inputmask.extensions.min.js",
                        "~/Extension/jquery.inputmask/dist/min/inputmask/inputmask.numeric.extensions.min.js",
                        "~/Extension/jquery.inputmask/dist/min/jquery.inputmask.bundle.min.js"));
            //login
            bundles.Add(new ScriptBundle("~/Scripts/login").Include(
                        "~/Extension/jquery/dist/jquery.min.js",
                        "~/Extension/bootstrap/dist/js/bootstrap.min.js",
                        "~/Extension/fastclick/lib/fastclick.js",
                        "~/Extension/nprogress/nprogress.js",
                        "~/Extension/validator/validator.js",
                        "~/Scripts/custom.min.js"));
            // full canlendar
            bundles.Add(new ScriptBundle("~/Scripts/fullCalendar").Include(
                        "~/Extension/fullcalendar/fullcalendar.min.js",
                        "~/Extension/fullcalendar/vi.js"));
            #endregion

            #region style
            //home
            bundles.Add(new StyleBundle("~/Style/index").Include(
                "~/Extension/bootstrap/dist/css/bootstrap.css",
                "~/Extension/datatables.net-bs/css/dataTables.bootstrap.min.css",
                "~/Extension/font-awesome/css/font-awesome.min.css",
                 "~/Extension/nprogress/nprogress.css",
                 "~/Extension/iCheck/skins/flat/green.css",
                  "~/Extension/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css",
                  "~/Extension/jqvmap/dist/jqvmap.min.css",
                  "~/Extension/bootstrap-daterangepicker/daterangepicker.css",
                  "~/Extension/toastr-master/toastr.min.css",
                  "~/Content/custom.min.css"));
            //datatable
            bundles.Add(new StyleBundle("~/Style/datatable").Include(
                "~/Extension/datatables.net/css/jquery.dataTables.min.css",
                "~/Extension/datatables.net-bs/css/dataTables.bootstrap.min.css",
                "~/Extension/datatables.net-buttons-bs/css/buttons.bootstrap.min.css",
                "~/Extension/datatables.net-select/css/select.dataTables.css",
                 "~/Extension/datatables.net-fixedheader-bs/css/fixedHeader.bootstrap.min.css",
                 "~/Extension/datatables.net-responsive-bs/css/responsive.bootstrap.min.css",
                 "~/Extension/iCheck/skins/flat/green.css",
                 "~/Extension/datatables.net-scroller-bs/css/scroller.bootstrap.min.css"));
            //login
            bundles.Add(new StyleBundle("~/Style/login").Include(
                "~/Extension/bootstrap/dist/css/bootstrap.min.css",
                "~/Extension/font-awesome/css/font-awesome.min.css",
                 "~/Extension/nprogress/nprogress.css",
                 "~/Extension/animate.css/animate.min.css",
                  "~/Content/custom.min.css"));
            //login
            bundles.Add(new StyleBundle("~/Style/fullCalendar").Include(
                "~/Extension/fullcalendar/fullcalendar.min.css"));
            #endregion

            BundleTable.EnableOptimizations = false;
        }
    }
}

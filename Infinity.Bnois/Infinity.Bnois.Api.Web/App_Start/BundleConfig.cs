﻿using System.Web;
using System.Web.Optimization;

namespace Infinity.Bnois.Api.Web
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BundleConfig'
    public class BundleConfig
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BundleConfig'
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BundleConfig.RegisterBundles(BundleCollection)'
        public static void RegisterBundles(BundleCollection bundles)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BundleConfig.RegisterBundles(BundleCollection)'
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}

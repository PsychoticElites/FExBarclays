using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "webViewRunner" , Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class webViewRunner : Activity
    {
        WebView web_view;
        string urlToOpen;

        public class HelloWebViewClient : WebViewClient
        {
#pragma warning disable CS0672 // Member overrides obsolete member
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
#pragma warning restore CS0672 // Member overrides obsolete member
            {
                view.LoadUrl(url);
                return true;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.myWebView);
            // Create your application here
            Console.WriteLine("Working!");
            urlToOpen = Intent.GetStringExtra("url") ?? "http://www.fiaformulae.com/en";
            //urlToOpen = Intent.GetStringExtra("url") ?? "http://xonshiz.heliohost.org/unitedhcl/webviews/videoPlayer.html";
            web_view = FindViewById<WebView>(Resource.Id.webViewMain);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.LoadUrl(urlToOpen);
            web_view.SetWebViewClient(new HelloWebViewClient());
        }
    }
}
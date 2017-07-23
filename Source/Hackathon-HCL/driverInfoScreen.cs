using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "driverInfoScreen", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class driverInfoScreen : AppCompatActivity
    {
        
        public string sample1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.driverInfoListView);

        }
    }
}
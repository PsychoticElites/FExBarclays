using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "Hackathon - HCL", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class OpenScreenActivity : Activity
    {

        private ImageButton OpenFE;
        private ImageButton OpenBarclays;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.OpeningScreen);
            var FEButton = FindViewById<ImageButton>(Resource.Id.buttonFE);
            var BarclaysButton= FindViewById<ImageButton>(Resource.Id.buttonBarclays);

            FEButton.Click += FEButton_Click;
            BarclaysButton.Click += BarclaysButton_Click;
        }
        private void BarclaysButton_Click(object sender, EventArgs e)
        {
            var IntentBarclaysButton = new Intent(this, typeof(BarclayMain));
            StartActivity(IntentBarclaysButton);
        }

        private void FEButton_Click(object sender, EventArgs e)
        {
            var IntentFEButton = new Intent(this, typeof(LoginScreen));
            StartActivity(IntentFEButton);
        }

        public override void OnBackPressed()
        { 
            Toast.MakeText(this, "This function is not allowed.", ToastLength.Long).Show(); //Showing Bad Connection Error
        }
    }
}
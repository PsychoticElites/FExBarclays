using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Content;

namespace Hackathon_HCL
{
    [Activity(Label = "Sample Data", Theme = "@style/Theme.DesignDemo", Icon = "@drawable/icon")]
    public class SampleExp : AppCompatActivity
    {
        
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MainActivitySample);
            string extraData = Intent.GetStringExtra("value");
            var dataField = FindViewById<EditText>(Resource.Id.sampleTextField);
            dataField.Text = extraData;

        }
    }
}
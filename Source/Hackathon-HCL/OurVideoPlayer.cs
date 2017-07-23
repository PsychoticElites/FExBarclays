using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Hackathon_HCL
{
    [Activity(Label = "OurVideoPlayer", MainLauncher = false, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar.Fullscreen", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class OurVideoPlayer : Activity
    {
        private VideoView VideoPlayer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.OurVideoPlayer);

            string videoUrl = Intent.GetStringExtra("videoUrl") ?? "Data not available"; ;
            VideoPlayer = FindViewById<VideoView>(Resource.Id.LiveVideoMain);

            var mediaController = new MediaController(this);
            VideoPlayer.SetVideoURI(Android.Net.Uri.Parse(videoUrl));
            mediaController.SetAnchorView(VideoPlayer);
            VideoPlayer.SetMediaController(mediaController);
            VideoPlayer.RequestFocus();
            VideoPlayer.Start();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            VideoPlayer.StopPlayback();
            VideoPlayer.Suspend();
        }
    }
}
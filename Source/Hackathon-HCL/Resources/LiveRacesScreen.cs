using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Content;
using Android.Graphics;

namespace Hackathon_HCL
{
    [Activity(Label = "Livestream Dashboard", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LiveRacesScreen : Activity
    {
        private string overHeadCamFeed, tvCamFeed, carCamFeed, trackCamFeed;
        private Button ButtonCarCamera, ButtonTrackCamera, ButtonOverheadCamera, ButtonTVFeed,goVR;
        private VideoView previewVideoPlayer;
        

       
        
        public override void OnBackPressed()
        {
            
            previewVideoPlayer.Suspend();
            base.OnBackPressed();
        }

        protected override void OnResume()
        {
            base.OnResume();
            previewVideoPlayer.Resume();
        }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LiveRaces);
            FindViews();
            HandleEvents();
            Console.WriteLine("Starting the method");
            VideoPlayerMain();
            Console.WriteLine("Done!");
            await Task.Run(() => StreamFetcher());
        }

        private void FindViews()
        {
            
            ButtonCarCamera = FindViewById<Button>(Resource.Id.ButtonCarCamera);
            ButtonTrackCamera = FindViewById<Button>(Resource.Id.ButtonTrackCamera);
            ButtonOverheadCamera = FindViewById<Button>(Resource.Id.ButtonOverheadCamera);
            ButtonTVFeed = FindViewById<Button>(Resource.Id.ButtonTVFeed);
            goVR = FindViewById<Button>(Resource.Id.GoVR);
            previewVideoPlayer = FindViewById<VideoView>(Resource.Id.videoView1);
            Typeface tf = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            ButtonCarCamera.SetTypeface(tf, TypefaceStyle.Bold);
            ButtonTrackCamera.SetTypeface(tf, TypefaceStyle.Bold);
            ButtonOverheadCamera.SetTypeface(tf, TypefaceStyle.Bold);
            ButtonTVFeed.SetTypeface(tf, TypefaceStyle.Bold);
            goVR.SetTypeface(tf, TypefaceStyle.Bold);
            
        }


        private void VideoPlayerMain()
        {


            var mediaController = new MediaController(this);
            previewVideoPlayer.SetVideoURI(Android.Net.Uri.Parse("http://xonshiz.heliohost.org/unitedhcl/streams/preview_video.mp4"));
            mediaController.SetAnchorView(previewVideoPlayer);
            previewVideoPlayer.SetMediaController(mediaController);
            previewVideoPlayer.RequestFocus();
            Console.WriteLine("Got the focus");
            
            Console.WriteLine("Starting the video");
            previewVideoPlayer.Start();
            Console.WriteLine("Done!");
        }

        private void HandleEvents()
        {

            goVR.Click += GoVR_Click;
            // Just send the Stream URL to the VideoPlayerMain() method and it'll check the rest.
            ButtonCarCamera.Click += delegate
            {
                //VideoPlayerMain(carCamFeed);
                if (String.IsNullOrEmpty(carCamFeed))
                {
                    Toast.MakeText(this, "Wait till I Fetch The Stream List", ToastLength.Long).Show();
                    Task.Run(() => StreamFetcher());
                }
                else
                {
                    var playerActivity = new Intent(this, typeof(OurVideoPlayer));
                    playerActivity.PutExtra("videoUrl", carCamFeed);
                    StartActivity(playerActivity);
                }
            };
            
            ButtonTrackCamera.Click += delegate
            {
                //VideoPlayerMain(trackCamFeed);
                if (String.IsNullOrEmpty(trackCamFeed))
                {
                    Toast.MakeText(this, "Wait till I Fetch The Stream List", ToastLength.Long).Show();
                    Task.Run(() => StreamFetcher());
                }
                else
                {
                    var playerActivity = new Intent(this, typeof(OurVideoPlayer));
                    playerActivity.PutExtra("videoUrl", trackCamFeed);
                    StartActivity(playerActivity);
                }
            };

            ButtonOverheadCamera.Click += delegate
            {
                //VideoPlayerMain(overHeadCamFeed);
                if (String.IsNullOrEmpty(overHeadCamFeed))
                {
                    Toast.MakeText(this, "Wait till I Fetch The Stream List", ToastLength.Long).Show();
                    Task.Run(() => StreamFetcher());
                }
                else
                {
                    var playerActivity = new Intent(this, typeof(OurVideoPlayer));
                    playerActivity.PutExtra("videoUrl", overHeadCamFeed);
                    StartActivity(playerActivity);
                }
            };

            ButtonTVFeed.Click += delegate
            {
                //VideoPlayerMain(tvCamFeed);
                if (String.IsNullOrEmpty(tvCamFeed))
                {
                    Toast.MakeText(this, "Wait till I Fetch The Stream List", ToastLength.Long).Show();
                    Task.Run(() => StreamFetcher());
                }
                else
                {
                    var playerActivity = new Intent(this, typeof(OurVideoPlayer));
                    playerActivity.PutExtra("videoUrl", tvCamFeed);
                    StartActivity(playerActivity);
                }
            };
        }

        private void GoVR_Click(object sender, EventArgs e)
        {
            
            Toast.MakeText(this, "This feature will be available in future updates", ToastLength.Short).Show();
        }

        private async Task StreamFetcher()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://xonshiz.heliohost.org");

                var result = await client.GetAsync("/unitedhcl/api/streams.php");
                string resultContent = await result.Content.ReadAsStringAsync();
                Console.WriteLine("Streams Information : " + resultContent);

                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    this.overHeadCamFeed = obj2.over_head_cam ;
                    this.tvCamFeed = obj2.tv_cam ;
                    this.carCamFeed = obj2.car_cam ;
                    this.trackCamFeed = obj2.track_cam ;
                }
                catch (Exception)
                {

                    throw;
                }

            }

        }
    }
}
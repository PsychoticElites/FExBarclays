using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net.Http;
using Android.Graphics;
using System.Net;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "driverProfile", ScreenOrientation = ScreenOrientation.Portrait)]
    public class driverProfileScreen : Activity
    {
        EditText tf1, tf2, tf3, tf4, tf5, tf6, tf7, tf8, tf9, tf10, tf11;
        ImageView myImage;
        private string driverDriverImage;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.driverProfile);
            string id_got = Intent.GetStringExtra("id") ?? "0";
            FindViews();
            getRaceSchedule(id_got);
        }

        private void FindViews()
        {
            tf1 = FindViewById<EditText>(Resource.Id.sampleTextField1);
            tf2 = FindViewById<EditText>(Resource.Id.sampleTextField2);
            tf3 = FindViewById<EditText>(Resource.Id.sampleTextField3);
            tf4 = FindViewById<EditText>(Resource.Id.sampleTextField4);
            tf5 = FindViewById<EditText>(Resource.Id.sampleTextField5);
            tf6 = FindViewById<EditText>(Resource.Id.sampleTextField6);
            tf7 = FindViewById<EditText>(Resource.Id.sampleTextField7);
            tf8 = FindViewById<EditText>(Resource.Id.sampleTextField8);
            tf9 = FindViewById<EditText>(Resource.Id.sampleTextField9);
            tf10 = FindViewById<EditText>(Resource.Id.sampleTextField10);
            tf11 = FindViewById<EditText>(Resource.Id.sampleTextField11);
            myImage = FindViewById<ImageView>(Resource.Id.imageView1);
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        public async void getRaceSchedule(string driverId)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            handler.Proxy = null;

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("https://xonshiz.heliohost.org");

                var content = new FormUrlEncodedContent(new[]
                {

                new KeyValuePair<string, string>("id", driverId)
            });
                var result = await client.PostAsync("/unitedhcl/jsonData/drivers_info.php", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    Console.WriteLine("obj2 : " + obj2);
                    this.tf1.Text = obj2.driver_name;
                    this.tf2.Text = obj2.age;
                    this.tf3.Text = obj2.dob;
                    this.tf4.Text = obj2.debut_season;
                    this.tf5.Text = obj2.current_team;
                    this.tf6.Text = obj2.starts;
                    this.tf7.Text = obj2.championships;
                    this.tf8.Text = obj2.wins;
                    this.tf9.Text = obj2.poles;
                    this.tf10.Text = obj2.fastest_laps;
                    this.tf11.Text = obj2.best_finish;
                    this.driverDriverImage = obj2.driver_image;
                    var imageBitmap = GetImageBitmapFromUrl(this.driverDriverImage);
                    myImage.SetImageBitmap(imageBitmap);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
    }
}
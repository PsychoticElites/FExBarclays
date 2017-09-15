using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Square.Picasso;
using Android.Support.V7.App;
using UniversalImageLoader.Core;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Android.Graphics;
using System.Net;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "driverInfoScreen", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class driverInfoScreen : AppCompatActivity
    {
        private List<driverInfo> mItem;
        private ListView mListView;
        public string sample1;
        //private string driverNameApi, driverAge, driverDob, driverDebutSeason, driverCurrentSeason, driverStarts, driverChampionships, driverWins, driverPoles, driverFastestLaps, driverBestFinish, driverDriverImage;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.driverInfoListView);

            //mListView = FindViewById<ListView>(Resource.Id.driverListView);

            //mItem = new List<driverInfo>();
            //ImageLoader imageLoader = ImageLoader.Instance;

            //try
            //{
            //    HttpClientHandler handler = new HttpClientHandler()
            //    {
            //        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            //    };

            //    handler.Proxy = null;

            //    using (var client = new HttpClient(handler))
            //    {
            //        client.BaseAddress = new Uri("https://xonshiz.heliohost.org");

            //        var result = await client.GetAsync("/unitedhcl/jsonData/drivers_information.json");
            //        string resultContent = await result.Content.ReadAsStringAsync();
            //        Console.WriteLine("Result Content : " + resultContent);
            //        JArray v = JArray.Parse(resultContent);
            //        for (int i = 0; i < v.Count; i++)
            //        {
            //            var imageBitmap = GetImageBitmapFromUrl(v[i]["driver_image"].ToString());
            //            mItem.Add(new driverInfo()
            //            {
            //                driverName = v[i]["driver_name"].ToString(),
            //                driverTeamName = v[i]["current_team"].ToString(),
            //                //v[i]["driver_image"].ToString() <-- Driver's Image URL.
            //                // imageBitmap has the converted image. You need to check line number 104 and 105 of DriverProfileScreen.

            //            });
            //        }
            //    }
            //}
            //catch (Exception ConnectorError)
            //{
            //    Console.WriteLine("ConnectorError : " + ConnectorError);
            //    throw;
            //}


                //mItem.Add(new driverInfo()
                //{
                //    driverName = "Sebastien Buemi",
                //    driverTeamName = "Renault e.Dams"
                //    driverPhoto = driverImage[0]

                //});
                //mItem.Add(new driverInfo()
                //{
                //    driverName = "Lucas di Grassi",
                //    driverTeamName = "Audi Sports Team Joest"
                //    //driverPhoto = driverImage[1]
                //});
                //mItem.Add(new driverInfo()
                //{
                //    driverName = "Felix Rosenqvist",
                //    driverTeamName = "Mahindra Racing"
                //    //driverPhoto = driverImage[2]
                //});
                //mItem.Add(new driverInfo()
                //{
                //    driverName = "Nicolas Jean Prost",
                //    driverTeamName = "Rebellion Racing"
                //    //driverPhoto = driverImage[3]
                //});
                //mItem.Add(new driverInfo()
                //{
                //    driverName = "Sam Bird",
                //    driverTeamName = "DS Virgin Racing"
                //});

            //CustomAdapter adapter = new CustomAdapter(this, mItem);

            //mListView.Adapter = adapter;

            //mListView.ItemClick += MListView_ItemClick;

            // Create your application here
        }

        //private Bitmap GetImageBitmapFromUrl(string url)
        //{
        //    Bitmap imageBitmap = null;

        //    using (var webClient = new WebClient())
        //    {
        //        var imageBytes = webClient.DownloadData(url);
        //        if (imageBytes != null && imageBytes.Length > 0)
        //        {
        //            imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
        //        }
        //    }

        //    return imageBitmap;
        //}

        //private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    var driverCaseSelected = e.Position;
        //    driverSelected(driverCaseSelected);
        //}

        //public async void getRaceSchedule(int driverId)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://xonshiz.heliohost.org");

        //        var content = new FormUrlEncodedContent(new[]
        //        {

        //        new KeyValuePair<string, string>("id", driverId.ToString())
        //    });
        //        var result = await client.PostAsync("/unitedhcl/jsonData/drivers_info.php", content);
        //        string resultContent = await result.Content.ReadAsStringAsync();

        //        try
        //        {
        //            dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
        //            // private string driverName, driverAge, driverDob, driverDebutSeason, driverCurrentSeason, driverStarts,, driverChampionships, driverWins, driverPoles, driverFastestLaps, driverBestFinish, driverDriverImage;
        //            driverNameApi = obj2.driver_name;
        //            driverAge = obj2.age;
        //            driverDob = obj2.dob;
        //            driverDebutSeason = obj2.debut_season;
        //            driverCurrentSeason = obj2.current_team;
        //            driverStarts = obj2.starts;
        //            driverChampionships = obj2.championships;
        //            driverWins = obj2.wins;
        //            driverPoles = obj2.poles;
        //            driverFastestLaps = obj2.fastest_laps;
        //            driverBestFinish = obj2.best_finish;
        //            driverDriverImage = obj2.driver_image;
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //            //Toast.MakeText(this, loginException.ToString(), ToastLength.Long).Show(); //Showing Bad Connection Error
        //        }

        //    }
        //}

        //public void driverSelected(int driverSelected)
        //{
        //    Intent driverProfileIntent;
        //    driverProfileIntent = new Intent(this, typeof(driverProfileScreen)); //When referencing to Driver Profile , change  SampleExp => driverProfileScreen
        //    driverProfileIntent.PutExtra("id", driverSelected.ToString());
        //    Console.WriteLine("Driver ki ID : " + driverSelected);
        //    StartActivity(driverProfileIntent);
        //    //var textfield = FindViewById<EditText>(Resource.Id.sampleTextField); //Textfield from SampleActivity.cs for testing purpose


        //    //switch (driverSelected)
        //    //{
        //    //    case 0:
        //    //        sample1 = "Ankkit Passi";
        //    //        driverProfileIntent.PutExtra("value", sample1);
        //    //        StartActivity(driverProfileIntent);
        //    //        break;
        //    //    case 1:
        //    //        sample1 = "Dhruv";
        //    //        driverProfileIntent.PutExtra("value", sample1);
        //    //        StartActivity(driverProfileIntent);
        //    //        break;
        //    //    case 2:
        //    //        sample1 = "Not Dhruv";
        //    //        driverProfileIntent.PutExtra("value", sample1);
        //    //        StartActivity(driverProfileIntent);
        //    //        break;
        //    //    case 3:
        //    //        sample1 = "Devesh";
        //    //        driverProfileIntent.PutExtra("value", sample1);
        //    //        StartActivity(driverProfileIntent);
        //    //        break;
        //    //    case 4:
        //    //        sample1 = "Shubham";
        //    //        driverProfileIntent.PutExtra("value", sample1);
        //    //        StartActivity(driverProfileIntent);
        //    //        break;
        //    //}
        //}
    }
}
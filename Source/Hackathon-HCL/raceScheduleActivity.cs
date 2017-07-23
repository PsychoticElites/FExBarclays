using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using FR.Ganfra.Materialspinner;
using System.Net.Http;
using System.Net;
using Android.Content.PM;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Views;

namespace Hackathon_HCL
{
    [Activity(Label = "raceScheduleActivity",Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class raceScheduleActivity : AppCompatActivity
    {
        MaterialSpinner spinner;
        List<string> listItems = new List<string>();
        ArrayAdapter<string> adapter;
       
        TextView cktName, raceRound, raceDay, raceDate, eRace, raceQualifying, racePractice1, racePractice2;
        public string countryName, circuitName, dateOfRace, dayOfRace, roundOfRace, entryStart, practiceOne, practiceTwo, qualifying, finalRace, cktImage;
        ImageView circuitImage;

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

        public override bool OnOptionsItemSelected(IMenuItem item)
        { 
            if (item.ItemId == Android.Resource.Id.Home)
                this.OnBackPressed();

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.raceSchedule);
            circuitImage = FindViewById<ImageView>(Resource.Id.cktImage);
            cktName = FindViewById<TextView>(Resource.Id.textViewCktName);
            raceRound = FindViewById<TextView>(Resource.Id.textViewRound);
            raceDay = FindViewById<TextView>(Resource.Id.textViewDay);
            raceDate = FindViewById<TextView>(Resource.Id.textViewDate);
            eRace = FindViewById<TextView>(Resource.Id.textViewErace);
            raceQualifying = FindViewById<TextView>(Resource.Id.textViewQualifying);
            racePractice1 = FindViewById<TextView>(Resource.Id.textViewPractice1);
            racePractice2 = FindViewById<TextView>(Resource.Id.textViewPractice2);
            Android.Graphics.Typeface tf1 = Android.Graphics.Typeface.CreateFromAsset(Assets, "Khula-Regular.ttf");
            Android.Graphics.Typeface tf3 = Android.Graphics.Typeface.CreateFromAsset(Assets, "Khula-SemiBold.ttf");
            Android.Graphics.Typeface tf = Android.Graphics.Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            Android.Graphics.Typeface tf2 = Android.Graphics.Typeface.CreateFromAsset(Assets, "Khula-Light.ttf");
            cktName.SetTypeface(tf, Android.Graphics.TypefaceStyle.Bold);
            raceRound.SetTypeface(tf, Android.Graphics.TypefaceStyle.Bold);
            raceDay.SetTypeface(tf1, Android.Graphics.TypefaceStyle.Bold);
            raceDate.SetTypeface(tf2, Android.Graphics.TypefaceStyle.Normal);
            eRace.SetTypeface(tf3, Android.Graphics.TypefaceStyle.Normal);
            raceQualifying.SetTypeface(tf3, Android.Graphics.TypefaceStyle.Normal);
            racePractice1.SetTypeface(tf3, Android.Graphics.TypefaceStyle.Normal);
            racePractice2.SetTypeface(tf3, Android.Graphics.TypefaceStyle.Normal);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Race Schedule";
            SetSupportActionBar(toolbar);
            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }

            InitItems();
            spinner = FindViewById<MaterialSpinner>(Resource.Id.spinnerCountrySelect);
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, listItems);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleDropDownItem1Line);
            spinner.Adapter = adapter;
            spinner.ItemSelected += async (s, e) =>
            {
                if (e.Position != -1)
                {
                    long selected = spinner.GetItemIdAtPosition(e.Position);
                    switch (selected)
                    {
                        case 0:
                            Console.WriteLine("Cases {0}", selected);
                            await getRaceSchedule(selected);
                            break;

                        case 1:
                            Console.WriteLine("Cases {0}", selected);
                            await getRaceSchedule(selected);
                            break;

                        case 2:
                            Console.WriteLine("Cases {0}", selected);
                            await getRaceSchedule(selected);
                            break;

                        case 3:
                            Console.WriteLine("Cases {0}", selected);
                            await getRaceSchedule(selected);
                            break;
                    }
                }
            };
        }

        private void InitItems()
        {
          
            listItems.Add("Berlin");
            listItems.Add("Hong Kong");
            listItems.Add("Monaco");
            listItems.Add("New York City");
        }

        public async Task getRaceSchedule(long countryId)
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

                new KeyValuePair<string, string>("id", countryId.ToString())
            });
                var result = await client.PostAsync("/unitedhcl/jsonData/schedule.php", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    this.countryName = obj2.country;
                    this.cktName.Text = obj2.circuit_name;
                    this.raceDay.Text = obj2.day;
                    this.raceDate.Text = obj2.date;
                    this.raceRound.Text = obj2.round;
                    this.entryStart = obj2.entry_start;
                    this.racePractice1.Text = obj2.practice_1;
                    this.racePractice2.Text = obj2.practice_2;
                    this.raceQualifying.Text = obj2.qualifying;
                    this.eRace.Text = obj2.e_race;
                    this.cktImage = obj2.ckt_image;
                    var imageBitmap = GetImageBitmapFromUrl(this.cktImage);
                    circuitImage.SetImageBitmap(imageBitmap);
                }
                catch (Exception MainExcheption)
                {
                    Console.WriteLine("MainExcheption : " + MainExcheption);
                    throw;
                }

            }
        }
    }
}
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Content;
using System;
using Android.Content.PM;
using Android.Graphics;
using System.Threading.Tasks;
using RadialProgress;
using System.Globalization;

namespace Hackathon_HCL
{
    [Activity(Label = "Formula E Dashboard", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]

    public class MainActivity : AppCompatActivity
    {
        private Button LiveRaces;
        private Button ViewMap;
        
        private RadialProgressView day, hour, minute;
        
        private LinearLayout layoutDate, layoutHours, layoutMinutes;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        enum Stroke
        {
            REGULAR = 3,
            THIN = 2,
            EXTRA_THIN = 1
        };

        public override void OnBackPressed()
        {
            Toast.MakeText(this, "You will now be logged out and redirected to main menu.", ToastLength.Long).Show();
            var IntentFEButton = new Intent(this, typeof(OpenScreenActivity));
            StartActivity(IntentFEButton);
        }

        protected override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LayoutMain);
            FindViews();
            HandleEvents();
            RadialProgressMaker();
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
#pragma warning disable 
            drawerLayout.SetDrawerListener(drawerToggle);
#pragma warning restore CS0618 
            drawerToggle.SyncState();
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            Task.Run(() => SetupDrawerContent(navigationView));

        }

        private async Task SetupDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_raceSchedule:
                        var raceIntent = new Intent(this, typeof(raceScheduleActivity));
                        StartActivity(raceIntent);
                        break;

                    case Resource.Id.nav_driverInfo:
                        var driverInformation = new Intent(this, typeof(webViewRunner));
                        driverInformation.PutExtra("url", "http://xonshiz.heliohost.org/unitedhcl/jsonData/driverList.php");
                        StartActivity(driverInformation);
                        break;
                   
                    case Resource.Id.nav_aboutUs:
                        var webAboutUs = new Intent(this, typeof(webViewRunner));
                        webAboutUs.PutExtra("url", "http://xonshiz.heliohost.org/unitedhcl/webviews/Team.html");
                        StartActivity(webAboutUs);
                        break;
                    case Resource.Id.nav_aboutFE:
                        var webAboutFe = new Intent(this, typeof(webViewRunner));
                        webAboutFe.PutExtra("url", "http://www.fiaformulae.com/en");
                        StartActivity(webAboutFe);
                        break;
                    case Resource.Id.nav_buyTicket:
                        var webBuyTickets = new Intent(this, typeof(webViewRunner));
                        webBuyTickets.PutExtra("url", "http://info.fiaformulae.com/");
                        StartActivity(webBuyTickets);
                        break;
                    case Resource.Id.nav_visitUs:
                        var webVisitUs = new Intent(this, typeof(webViewRunner));
                        webVisitUs.PutExtra("url", "http://www.fiaformulae.com/en");
                        StartActivity(webVisitUs);
                        break;
                    case Resource.Id.nav_protoApp:
                        var webProtoApp = new Intent(this, typeof(webViewRunner));
                        webProtoApp.PutExtra("url", "http://xonshiz.heliohost.org/unitedhcl/webviews/PrototypeVersion.html");
                        StartActivity(webProtoApp);
                        //http://xonshiz.heliohost.org/unitedhcl/webviews/PrototypeVersion.html
                        break;
                }

                e.MenuItem.SetChecked(true);
                drawerLayout.CloseDrawers();
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            navigationView.InflateMenu(Resource.Menu.nav_menu); //Navigation Drawer Layout Menu Creation  
            return true;
        }

        private void FindViews()
        {
            Typeface tf = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            Typeface tf1 = Typeface.CreateFromAsset(Assets, "Khula-SemiBold.ttf");

            var objTwitter = FindViewById<Android.Support.V7.Widget.AppCompatImageView>(Resource.Id.connectTwitter);
            var objFB = FindViewById<Android.Support.V7.Widget.AppCompatImageView>(Resource.Id.connectFB);
            var objInsta = FindViewById<Android.Support.V7.Widget.AppCompatImageView>(Resource.Id.connectInsta);
            var objYoutube = FindViewById<Android.Support.V7.Widget.AppCompatImageView>(Resource.Id.connectUtube);
            objTwitter.Click += Obj_Click;
            objFB.Click += ObjFB_Click;
            objInsta.Click += ObjInsta_Click;
            objYoutube.Click += ObjYoutube_Click;

            LiveRaces = FindViewById<Button>(Resource.Id.MainActivityRace);
            ViewMap = FindViewById<Button>(Resource.Id.MainActivityMap);
            LiveRaces.SetTypeface(tf, TypefaceStyle.Bold);
            ViewMap.SetTypeface(tf, TypefaceStyle.Normal);
            var timerLabel = FindViewById<TextView>(Resource.Id.textCountdown);
            timerLabel.SetTypeface(tf, TypefaceStyle.Bold);
            day = FindViewById<RadialProgressView>(Resource.Id.dayProgress);
            hour = FindViewById<RadialProgressView>(Resource.Id.hourProgress);
            minute = FindViewById<RadialProgressView>(Resource.Id.minuteProgress);
            var dayLabel = FindViewById<TextView>(Resource.Id.textViewDayLabel);
            var hourLabel = FindViewById<TextView>(Resource.Id.textViewHourLabel);
            var minutesLabel = FindViewById<TextView>(Resource.Id.textViewMinutesLabel);
            layoutDate = FindViewById<LinearLayout>(Resource.Id.linearLayoutDays);
            layoutHours = FindViewById<LinearLayout>(Resource.Id.linearLayoutHours);
            layoutMinutes = FindViewById<LinearLayout>(Resource.Id.linearLayoutMinutes);
            dayLabel.SetTypeface(tf1, TypefaceStyle.Normal);
            hourLabel.SetTypeface(tf1, TypefaceStyle.Normal);
            minutesLabel.SetTypeface(tf1, TypefaceStyle.Normal);
        }

        private void ObjYoutube_Click(object sender, EventArgs e)
        {
            var webProtoApp = new Intent(this, typeof(webViewRunner));
            webProtoApp.PutExtra("url", "https://www.youtube.com/channel/UC-DuRqsBQOEk_5o1q4Ze-Fg");
            StartActivity(webProtoApp);
        }

        private void ObjInsta_Click(object sender, EventArgs e)
        {
            var webProtoApp = new Intent(this, typeof(webViewRunner));
            webProtoApp.PutExtra("url", "https://www.instagram.com/fiaformulae/");
            StartActivity(webProtoApp);
        }

        private void ObjFB_Click(object sender, EventArgs e)
        {
            var webProtoApp = new Intent(this, typeof(webViewRunner));
            webProtoApp.PutExtra("url", "https://www.facebook.com/fiaformulae/");
            StartActivity(webProtoApp);
        }




        //Event Handling of Activities
        private void HandleEvents()
        {
            LiveRaces.Click += LiveRaces_Click;
            ViewMap.Click += ViewMap_Click;

        }

        private void Obj_Click(object sender, EventArgs e)
        {
            var webProtoApp = new Intent(this, typeof(webViewRunner));
            webProtoApp.PutExtra("url", "https://twitter.com/FIAformulaE");
            StartActivity(webProtoApp);
        }

        private void Barclays_Click(object sender, EventArgs e)
        {
            var IntentBarclays = new Intent(this, typeof(BarclayMain));
            StartActivity(IntentBarclays);
        }

        private void LiveRaces_Click(object sender, EventArgs e)
        {
            var IntentLiveRaces = new Intent(this, typeof(LiveRacesScreen));
            StartActivity(IntentLiveRaces);
        }

        private void ViewMap_Click(object sender, EventArgs e)
        {
            var IntentViewMap = new Intent(this, typeof(Maps));
            StartActivity(IntentViewMap);

        }

        private void RadialProgressMaker()
        {

            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");
            DateTime race_date = DateTime.Parse("07/30/2017", cultureinfo);
            Console.WriteLine("Race Date : " + race_date);

            DateTime dt = DateTime.Now.ToLocalTime();
            DateTime currentDate = DateTime.Parse(DateTime.Now.ToLongDateString(), cultureinfo);
            Console.WriteLine("currentDate : " + currentDate);

            int totalHoursLeft = Convert.ToInt32(Math.Round((race_date - DateTime.Now).TotalHours));
            int totalMinutesLeft = Convert.ToInt32(Math.Round((race_date - DateTime.Now).TotalMinutes));
            int totalDaysLeft = Convert.ToInt32(Math.Round((race_date - DateTime.Now).TotalDays));

            day.MaxValue = 100;
            day.MinValue = 0;
            day.Value = totalDaysLeft;

            if (totalDaysLeft > 4)
            {
                layoutHours.Visibility = ViewStates.Gone;
                layoutMinutes.Visibility = ViewStates.Gone;
            }
            else
            {
                hour.Visibility = ViewStates.Visible;

                hour.MaxValue = 100;
                hour.MinValue = 0;
                hour.Value = totalHoursLeft;

                if (totalMinutesLeft > 100)
                {
                    layoutMinutes.Visibility = ViewStates.Visible;
                }
                else
                { 
                    layoutMinutes.Visibility = ViewStates.Visible;
                    minute.MaxValue = 100;
                    minute.MinValue = 0;
                    minute.Value = totalMinutesLeft;
                }
            }

        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }
    }
}
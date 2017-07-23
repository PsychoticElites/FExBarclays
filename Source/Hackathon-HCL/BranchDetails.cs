using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Views;
using System.Threading.Tasks;
using System.Net.Http;
using System.Device.Location;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Util;
using Android.Gms.Common;
using Android.Locations;
using Plugin.TextToSpeech;
using Android.Speech;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Graphics;

namespace Hackathon_HCL
{
    [Activity(Label = "Nearest Bank Locator", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class BranchDetails : AppCompatActivity, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        // Declare all the global variables here.

        private EditText branchTelephone, branchLocation, isOpenNow;// branchOpeningTime, branchClosingTime;
        private EditText userLocation, branchDistance;
        private ProgressBar spinner;
        private string atmAddress1, telephoneNumber, openingTime, closingTime, userLati, userLongi, branchDistanceValue;
        private Dictionary<double, List<string>> listOfATMs = new Dictionary<double, List<string>>();
        List<string> tempList = new List<string>();
        List<string> branchList = new List<string>();

        bool _isGooglePlayServicesInstalled;
        GoogleApiClient apiClient;
        LocationRequest locRequest;
        string coordinates, isTheBranchOpen;
        private bool locateBranch = true;

        private TimeSpan timeRightNow;

        private readonly int VOICE = 10;

        // Declaration of variables Ends Here.

        private string userLatitude, userLongitude;


        protected async override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BranchDetailScreen);
            Typeface tf = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            Android.Support.V7.Widget.Toolbar toolbar1 = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar11);

            toolbar1.SetPadding(10, 0, 0, 0);
            toolbar1.SetTitleTextColor(Android.Graphics.Color.White);
            SetSupportActionBar(toolbar1);
            FindViews();
            HandleEvents();
            
            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();
            
            if (_isGooglePlayServicesInstalled)
            {
                // pass in the Context, ConnectionListener and ConnectionFailedListener
                apiClient = new GoogleApiClient.Builder(this, this, this)
                    .AddApi(LocationServices.API).Build();
                
                // generate a location request that we will pass into a call for location updates
                locRequest = new LocationRequest();
                
            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                await SpeakerAsync("Google Play Services is not installed.");
                Finish();
            }

            apiClient.Connect();

        }

        private async System.Threading.Tasks.Task SpeakerAsync(string speakingText)
        {
            await CrossTextToSpeech.Current.Speak(speakingText,
                    pitch: (float)1.0,
                    speakRate: (float)0.9,
                    volume: (float)2.0);
        }

        private void FindViews()
        {
            branchTelephone = FindViewById<EditText>(Resource.Id.BranchTelephone);
            branchLocation = FindViewById<EditText>(Resource.Id.BranchLocation);
            branchDistance = FindViewById<EditText>(Resource.Id.BranchDistanceLocation);
            Typeface tf = Typeface.CreateFromAsset(Assets, "NotoSans-Regular.ttf");
            isOpenNow = FindViewById<EditText>(Resource.Id.isOpenView);
            userLocation = FindViewById<EditText>(Resource.Id.userLocation);
            branchTelephone.SetTypeface(tf, TypefaceStyle.Normal);
            branchLocation.SetTypeface(tf, TypefaceStyle.Normal);
            branchDistance.SetTypeface(tf, TypefaceStyle.Normal);
            isOpenNow.SetTypeface(tf, TypefaceStyle.Normal);
            userLocation.SetTypeface(tf, TypefaceStyle.Normal);
            spinner = FindViewById<ProgressBar>(Resource.Id.progressBarbranch);
        }

        private void HandleEvents()
        {
       
        }

        // Method to get the nearest Branch.
        private async void ValuesGetter()
        {
            Toast.MakeText(this, "Please wait while I fetch the records...", ToastLength.Long).Show();
            await SpeakerAsync("Please wait while I fetch the records.");
            spinner.Visibility = ViewStates.Visible;
            await Task.Run(() => JsonFetcher(userLati, userLongi));
            spinner.Visibility = ViewStates.Gone;

            // Populate the fields with the returned data.
            branchTelephone.Text = this.telephoneNumber;
            branchLocation.Text = this.atmAddress1;
            isOpenNow.Text = isTheBranchOpen;
            branchDistance.Text = Math.Round(Convert.ToDouble(branchDistanceValue), 2).ToString() + "meters";


            if (branchLocation.Text == "No Bank Found Within 5000 Meters...")
            {
                this.locateBranch = false;
                
                Toast.MakeText(this, "No Bank Found in your Vicinity", ToastLength.Long).Show(); //Showing Bad Connection Error
                await SpeakerAsync("Sorry, It seems like there is No Barclays Bank Within 5000 Meters.");
                await Task.Delay(5000);
                await SpeakerAsync("Taking You Back To Main Menu.");
                
               
                base.OnBackPressed();
            }
            else
            {
                this.locateBranch = false;
                await SpeakerAsync("The Nearest Barclays Branch To You Right Now is at " + branchLocation.Text.ToString() + ". You can reach them at " + branchTelephone.Text.ToString().Aggregate(string.Empty, (c, i) => c + i + "  "));
                await SpeakerAsync("It is approximately " + branchDistance.Text + " away from your current location");
                
                if (isOpenNow.Text == "Open Now")
                {
                    await SpeakerAsync("This Branch Is Currently Opened.");
                    await SpeakerAsync("Opening the branch's location in Maps.");

                    string geoLocationGenerator = string.Format("geo:{0},{1}?q={2}", userLatitude, userLongitude, branchLocation.Text);
                    base.OnBackPressed();
                    Console.WriteLine("geoLocationGenerator : " + geoLocationGenerator);
                    var geoUri = Android.Net.Uri.Parse(geoLocationGenerator);
                    var mapIntent = new Intent(Intent.ActionView, geoUri);
                    StartActivity(mapIntent);
                }
                else
                {
                    await SpeakerAsync("Seems like this branch is closed right now..");
                    await SpeakerAsync("Taking You Back To Main Menu.");
                    await Task.Delay(5000);
                    base.OnBackPressed();
                }

            }
        }

        private string AddressGetter(string eCoord)
        {
            Console.WriteLine("eCord : " + eCoord);
            XmlDocument xDoc = new XmlDocument();

            string myUrl = "https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + Convert.ToString(eCoord).Replace(" ", "");
            Console.WriteLine("myURL : " + myUrl);
            xDoc.Load(myUrl + "&sensor=false");

            XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
            XmlNode xNode = xNodelst.Item(0);
            return xNode.SelectSingleNode("formatted_address").InnerText.ToString();
        }

        private async Task JsonFetcher(string desiredLat, string desiredLong)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://atlas.api.barclays:443");
                var result = await client.GetAsync("/open-banking/v1.3/branches");
                string resultContent = await result.Content.ReadAsStringAsync();

                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    for (int x = 0; x < obj2.data.Count; x++)
                    {
                        var y = obj2.data[x].GeographicLocation.Latitude;

                        var z = obj2.data[x].GeographicLocation.Longitude;

                        var sCoord = new GeoCoordinate(Convert.ToDouble(desiredLat), Convert.ToDouble(desiredLong));

                        var eCoord = new GeoCoordinate(Convert.ToDouble(y.Value), Convert.ToDouble(z.Value));

                        var distance = sCoord.GetDistanceTo(eCoord); 


                        if (distance < 5000)
                        {
                            try
                            {
                                DateTime dt = DateTime.Now.ToLocalTime();
                                string currentDate = DateTime.Now.ToString("MM.dd.yyyy");
                                TimeSpan currentTime = dt.TimeOfDay;
                                string currentDay = dt.DayOfWeek.ToString();

                                this.timeRightNow = currentTime;

                                this.telephoneNumber = obj2.data[x].TelephoneNumber.ToString();
                                int indexNumber = IndexGetter(currentDay);

                                this.openingTime = obj2.data[x].OpeningTimes[indexNumber].OpeningTime.ToString();
                                this.closingTime = obj2.data[x].OpeningTimes[indexNumber].ClosingTime.ToString();

                                XmlDocument xDoc = new XmlDocument();
                                xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + Convert.ToString(eCoord).Replace(" ", "") + "&sensor=false");

                                XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
                                XmlNode xNode = xNodelst.Item(0);
                                this.atmAddress1 = Convert.ToString(xNode.SelectSingleNode("formatted_address").InnerText);
                            }
                            catch (Exception)
                            {

                                this.telephoneNumber = "Couldn't Fetch The Data...";
                                this.atmAddress1 = "Couldn't Fetch The Data...";
                                this.openingTime = "Couldn't Fetch The Data...";
                                this.closingTime = "Couldn't Fetch The Data...";
                            }

                            finally
                            {

                                if (this.telephoneNumber == "Couldn't Fetch The Data...")
                                {
                                    this.telephoneNumber = "Couldn't Fetch The Data...";
                                    this.atmAddress1 = "Couldn't Fetch The Data...";
                                    this.openingTime = "Couldn't Fetch The Data...";
                                    this.closingTime = "Couldn't Fetch The Data...";
                                }
                                else
                                {
                                    
                                    tempList.Add(this.telephoneNumber);
                                    tempList.Add(this.openingTime);
                                    tempList.Add(this.closingTime);
                                    tempList.Add(this.atmAddress1);
                                    listOfATMs.Add(distance, tempList);
                                }
                            }
                        }

                    }
                    
                    if (listOfATMs.Count == 0)
                    {
                        this.telephoneNumber = "N/A";
                        this.atmAddress1 = "No Bank Found Within 5000 Meters...";
                        this.openingTime = "N/A";
                        this.closingTime = "N/A";
                        this.branchDistanceValue = "0";
                    }
                    
                    else
                    {
                        var atmDist = listOfATMs.Keys.ToList();
                        atmDist.Sort();
                        Console.WriteLine("atmDist : " + atmDist);

                        // Loop through keys and assign the data accordingly.
                        foreach (var key in atmDist)
                        {
                            branchList.Add(Convert.ToString(listOfATMs[key][0]));
                            branchList.Add(Convert.ToString(listOfATMs[key][1]));
                            branchList.Add(Convert.ToString(listOfATMs[key][2]));
                            branchList.Add(Convert.ToString(listOfATMs[key][3]));
                        }

                        
                        this.atmAddress1 = Convert.ToString(branchList.First());

                        this.telephoneNumber = Convert.ToString(branchList[0]);
                        this.atmAddress1 = Convert.ToString(branchList[3]);
                        this.branchDistanceValue = atmDist[0].ToString();
                        TimeSpan openingTime2 = TimeSpan.Parse(branchList[1]);
                        TimeSpan closingTime2 = TimeSpan.Parse(branchList[2]);
                        
                        bool openNow = timeChecker(openingTime2, closingTime2, timeRightNow);

                        isTheBranchOpen = "Open Now";
                        Console.WriteLine("Is Open : " + openNow);

                    }
                }
                catch (Exception GettingError)
                {

                    Console.WriteLine("Exception hui gwa : " + GettingError);
                    throw;
                }

            }

        }

        private bool timeChecker(TimeSpan start, TimeSpan end, TimeSpan now)
        {
            Console.WriteLine("Inside this time checker");
            if (start == TimeSpan.Parse("00:00:00") || end == TimeSpan.Parse("00:00:00"))
            {
                return false;
            }
            else if (start < end)
            {
                return start <= now && now <= end;
            }
           
            else
            {
                return !(end < now && now < start);
            }
        }

        private int IndexGetter(string currentDay)
        {
            int index = 0;
            if (currentDay == "Monday")
            {
                index = 0;
            }

            else if (currentDay == "Tuesday")
            {
                index = 1;
            }

            else if (currentDay == "Wednesday")
            {
                index = 2;
            }

            else if (currentDay == "Thursday")
            {
                index = 3;
            }

            else if (currentDay == "Friday")
            {
                index = 4;
            }

            else if (currentDay == "Saturday")
            {
                index = 5;
            }

            else if (currentDay == "Sunday")
            {
                index = 6;
            }

            return index;
        }

        bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("ManActivity", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);

            }
            return false;
        }

        
        protected async override void OnResume()
        {
            base.OnResume();
            
            await SpeakerAsync("Trying To Locate You.");
            if (locateBranch == true)
            {
                apiClient.Connect();
                if (apiClient.IsConnected)
                {

                    Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
                    if (location != null)
                    {
                        coordinates = "Latitude: " + location.Latitude.ToString() + " , Longitude: " + location.Longitude.ToString();
                        
                        userLati = location.Latitude.ToString();
                        userLongi = location.Longitude.ToString();
                        userLatitude = userLati;
                        userLongitude = userLongi;
                        try
                        {
                            userLocation.Text = AddressGetter(userLati + "," + userLongi);
                        }
                        catch (Exception LOL)
                        {
                            Console.WriteLine("LOL : " + LOL);

                            throw;
                        }

                        // As soon as the location of the user is fetched, look for the nearest branch.
                        ValuesGetter();
                    }
                    else
                    {
                        Toast.MakeText(this, "Could not find your location. Make sure GPS is on and try again later...", ToastLength.Long).Show(); // GPS isn't On.
                        await SpeakerAsync("Could not find your location. Make sure GPS is on and try again later.");
                        await Task.Delay(5000);
                        base.Recreate();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Could not connect to API...", ToastLength.Long).Show(); //Showing Bad Connection Error
                    await SpeakerAsync("Could not connect to API.");
                }
            }
        }

        protected override async void OnPause()
        {
            base.OnPause();
            Log.Debug("OnPause", "OnPause called, stopping location updates");

            if (apiClient.IsConnected)
            {
                // stop location updates, passing in the LocationListener
                apiClient.Disconnect();
            }
        }

        public void OnConnected(Bundle bundle)
        {

            Log.Info("LocationClient", "Now connected to client");
        }

        public void OnDisconnected()
        {

            Log.Info("LocationClient", "Now disconnected from client");
        }

        public void OnConnectionFailed(ConnectionResult bundle)
        {

            Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        }

        public void OnLocationChanged(Location location)
        {
            Log.Debug("LocationClient", "Location updated");

        }

        public void OnConnectionSuspended(int i)
        {

        }

        public void VoiceIntentStarter()
        {
            // Initialize the Voice Intent.
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 20000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 20000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 25000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            StartActivityForResult(voiceIntent, VOICE);
        }

        protected async override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            try
            {
                if (requestCode == VOICE)
                {
                    if (resultVal == Result.Ok)
                    {
                        var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);

                        if (matches.Count != 0)
                        {
                            Console.WriteLine("what I SPOKE : " + matches[0].Substring(0, 5).ToLower());
                            /*
                             * Here I'm using IF ELSE, because I wanted to accept the long sentences as well.
                             * SWITCH statements had only one phrase. So, if a person said "ATM Location" or "ATM",
                             * they will be 2 separate cases. 
                             * But, in this case, the Speaker can say anything along with the word "ATM".
                             * So, as long as we hear "ATM", we want to show the user the "ATM" location.
                             * This will save us from generating various scenarios and phrases.
                             */
                            foreach (string item in matches)
                            {
                                Console.WriteLine("My Word : " + item);
                                if (item.ToLower().Contains("yes"))
                                {
                                    await SpeakerAsync("Going Back To The Main Menu.");
                                    base.OnBackPressed();
                                    break;
                                }
                                else if (item.ToLower().Contains("no"))
                                {
                                    await SpeakerAsync("Okay, I'll be staying here.");
                                    break;
                                }
                                else if (item.ToLower().Contains("exit"))
                                {
                                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                                }
                                else
                                {
                                    await SpeakerAsync("I could not understand what you said. Please Speak Again.");
                                    VoiceIntentStarter();
                                }
                            }

                        }
                        else
                        {
                            await SpeakerAsync("I could not understand what you said. Please Speak Again.");
                            VoiceIntentStarter();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("My Errur : " + ex);
                await SpeakerAsync("I could not understand what you said. Please Speak Again.");
                VoiceIntentStarter();
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }
    }
}
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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Net;

namespace Hackathon_HCL
{
    [Activity(Label = "Nearest Bank Locator", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class BranchDetails : AppCompatActivity, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener, IOnMapReadyCallback
    {
        // Declare all the global variables here.

        private EditText branchTelephone, branchLocation, isOpenNow;// branchOpeningTime, branchClosingTime;
        private EditText userLocation, branchDistance;
        private ProgressBar spinner;
        private ScrollView BranchMainLayout;
        private LinearLayout BranchProgressBar;
        private string atmAddress1, telephoneNumber, openingTime, closingTime, userLati, userLongi, branchDistanceValue, userAddress = "";
        private Dictionary<double, List<string>> listOfATMs = new Dictionary<double, List<string>>();
        List<string> tempList = new List<string>();
        List<string> branchList = new List<string>();

        bool _isGooglePlayServicesInstalled;
        GoogleApiClient apiClient;
        LocationRequest locRequest;
        LatLng CurrentLocation;
        string toastcoords;
        private GoogleMap mMap;
        string coordinates, isTheBranchOpen;
        public string branchLatitude, branchLongitude, branchAddress, branchTelephoneNumber, branchStatus, branchDistanceReceived;
        private bool locateBranch = true;

        private TimeSpan timeRightNow;

        private readonly int VOICE = 10;

        // Declaration of variables Ends Here.

        //private string userLatitude, userLongitude, branchLatitude, branchLongitude;
        private string userLatitude, userLongitude;

        public override void OnBackPressed()
        {
            //base.OnStop();
            try
            {
                base.OnBackPressed();
                //CrossTextToSpeech.Dispose();
                //base.OnDestroy();
            }
            catch (Exception BackError2)
            {
                Console.WriteLine("BackError2 : " + BackError2);
                throw;
            }
        }

        protected async override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.BranchDetailScreen);
            SetUpMap();
            Typeface tf = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            Android.Support.V7.Widget.Toolbar toolbar1 = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar11);
            //toolbar1.SetLogo(Resource.Drawable.barlcayslogo);
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
            //await SpeakerAsync("Please Click On The Locate Me Button.");

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;

        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.Map).GetMapAsync(this);
            }
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
            BranchMainLayout = FindViewById<ScrollView>(Resource.Id.Branch_MainLayout);
            BranchProgressBar = FindViewById<LinearLayout>(Resource.Id.BranchlinearLayout_ProgressBar);
            //spinner = FindViewById<ProgressBar>(Resource.Id.progressBarbranch);

            //Default Values :
            branchTelephone.Text = "Searching...";
            branchLocation.Text = "Searching...";
            branchDistance.Text = "Calculating...";
            isOpenNow.Text = "-";
            userLocation.Text = "Searching...";
        }

        private void HandleEvents()
        {
            // Button Click for Fetching the Location.
            //buttonBranch.Click += async (object sender, EventArgs e) =>
            //{
            //    await SpeakerAsync("Trying To Locate You.");
            //    await Locatebutton_ClickAsync(sender, e);
            //};

        }

        // Method to get the nearest Branch.
        private async void ValuesGetter()
        {
            Toast.MakeText(this, "Please wait while I fetch the records...", ToastLength.Long).Show();
            await SpeakerAsync("Please wait while I fetch the records.");
            BranchMainLayout.Visibility = ViewStates.Gone;
            BranchProgressBar.Visibility = ViewStates.Visible;
            await Task.Run(() => JsonFetcher(userLati, userLongi));
            BranchMainLayout.Visibility = ViewStates.Visible;
            BranchProgressBar.Visibility = ViewStates.Gone;

            // Populate the fields with the returned data.
            branchTelephone.Text = this.telephoneNumber;
            branchLocation.Text = this.atmAddress1;
            isOpenNow.Text = isTheBranchOpen;
            branchDistance.Text = branchDistanceReceived;
            userLocation.Text = userAddress;


            if (branchLocation.Text == "No Bank Found Within 5000 Meters...")
            {
                this.locateBranch = false;
                branchDistance.Text = "-";
                branchTelephone.Text = "-";
                Toast.MakeText(this, "No Bank Found in your Vicinity", ToastLength.Long).Show(); //Showing Bad Connection Error
                await SpeakerAsync("Sorry, It seems like there is No Barclays Bank Within 5000 Meters.");
                await Task.Delay(5000);
                await SpeakerAsync("Taking Your Back To Main Menu.");

                // Don't ask the user whether they want to go back or not. Just take them back.
                try
                {
                    base.OnBackPressed();
                }
                catch (Exception BackError)
                {
                    Console.WriteLine("BackError : " + BackError);
                    throw;
                }
            }
            else
            {
                // Need to Break the Phon Number digit by digit. Otherwise, AI will speak it as an Integer (9 billion 7 million and so on).
                this.locateBranch = false;
                await SpeakerAsync("The Nearest Barclays Branch To You Right Now is at " + branchLocation.Text.ToString() + ". You can reach them at " + branchTelephone.Text.ToString().Aggregate(string.Empty, (c, i) => c + i + "  "));
                await SpeakerAsync("It is approximately " + branchDistance.Text + "meters away from your current location");

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

        private async Task JsonFetcher(string desiredLat, string desiredLong)
        {
            Console.WriteLine("desiredLat : " + desiredLat);
            Console.WriteLine("desiredLong : " + desiredLong);

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
                //new KeyValuePair<string, string>("", "login")
                new KeyValuePair<string, string>("user_lattitude", desiredLat),
                new KeyValuePair<string, string>("user_longitude", desiredLong)
            });
                var result = await client.PostAsync("/unitedhcl/api/barclays/branchLocationGetter.php", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    Console.WriteLine("OBJ2 : " + obj2);

                    this.branchLatitude = obj2.latitude;
                    this.branchLongitude = obj2.longitude;
                    this.atmAddress1 = Convert.ToString(obj2.buildingNumberOrName) + ", " + Convert.ToString(obj2.streetName) + ", " + Convert.ToString(obj2.townName) + ", " + Convert.ToString(obj2.postCode);
                    this.telephoneNumber = obj2.telephoneNumber;
                    this.branchStatus = obj2.status;
                    var sCoord = new GeoCoordinate(Convert.ToDouble(desiredLat), Convert.ToDouble(desiredLong));

                    var eCoord = new GeoCoordinate(Convert.ToDouble(this.branchLatitude), Convert.ToDouble(this.branchLongitude));

                    var distance = sCoord.GetDistanceTo(eCoord); // Distance is in Meters.
                    
                    this.branchDistanceReceived = Math.Round(distance, 2).ToString();
                    Console.WriteLine("branchDistanceReceived : " + branchDistanceReceived);
                    Console.WriteLine("Lattidtude : " + branchLatitude);

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + Convert.ToString(sCoord).Replace(" ", "") + "&sensor=false");

                    XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
                    XmlNode xNode = xNodelst.Item(0);
                    this.userAddress = Convert.ToString(xNode.SelectSingleNode("formatted_address").InnerText); //NullException Here in 2nd round

                    if (!string.IsNullOrEmpty(this.branchLatitude))
                    {
                        if (string.IsNullOrEmpty(this.atmAddress1) || this.atmAddress1 == "")
                        {
                            this.atmAddress1 = "No Bank Found Within 5000 Meters...";
                        }
                        else
                        {
                            if (this.branchStatus == "open")
                            {
                                isTheBranchOpen = "Open Now";
                            }
                            else
                            {
                                isTheBranchOpen = "Closed Now";
                            }
                        }
                    }
                    else
                    {
                        this.atmAddress1 = "No Bank Found Within 5000 Meters...";
                    }
                }
                catch (Exception ExNew)
                {
                    Console.WriteLine("ExNew : " + ExNew);
                    throw;
                    //Toast.MakeText(this, loginException.ToString(), ToastLength.Long).Show();
                }
            }

        }

        //private async Task JsonFetcher(string desiredLat, string desiredLong)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://atlas.api.barclays:443");
        //        var result = await client.GetAsync("/open-banking/v1.3/branches");
        //        string resultContent = await result.Content.ReadAsStringAsync();

        //        try
        //        {
        //            dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
        //            for (int x = 0; x < obj2.data.Count; x++)
        //            {
        //                var y = obj2.data[x].GeographicLocation.Latitude;

        //                var z = obj2.data[x].GeographicLocation.Longitude;

        //                var sCoord = new GeoCoordinate(Convert.ToDouble(desiredLat), Convert.ToDouble(desiredLong));

        //                var eCoord = new GeoCoordinate(Convert.ToDouble(y.Value), Convert.ToDouble(z.Value));

        //                var distance = sCoord.GetDistanceTo(eCoord); // Distance is in Meters.


        //                if (distance < 5000)
        //                {
        //                    try
        //                    {
        //                        DateTime dt = DateTime.Now.ToLocalTime();
        //                        string currentDate = DateTime.Now.ToString("MM.dd.yyyy");
        //                        TimeSpan currentTime = dt.TimeOfDay;
        //                        string currentDay = dt.DayOfWeek.ToString();

        //                        this.timeRightNow = currentTime;

        //                        this.telephoneNumber = obj2.data[x].TelephoneNumber.ToString();
        //                        int indexNumber = IndexGetter(currentDay);

        //                        this.openingTime = obj2.data[x].OpeningTimes[indexNumber].OpeningTime.ToString();
        //                        this.closingTime = obj2.data[x].OpeningTimes[indexNumber].ClosingTime.ToString();

        //                        XmlDocument xDoc = new XmlDocument();
        //                        // https://stackoverflow.com/questions/33613828/get-city-name-from-reverse-geocoding-with-latitude-and-longitude <-- Source
        //                        xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + Convert.ToString(eCoord).Replace(" ", "") + "&sensor=false");

        //                        XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
        //                        XmlNode xNode = xNodelst.Item(0);
        //                        this.atmAddress1 = Convert.ToString(xNode.SelectSingleNode("formatted_address").InnerText);
        //                    }
        //                    catch (Exception)
        //                    {
        //                        // Exception happened. Let's send back "data not found", instead of crashing the application.
        //                        this.telephoneNumber = "Couldn't Fetch The Data...";
        //                        this.atmAddress1 = "Couldn't Fetch The Data...";
        //                        this.openingTime = "Couldn't Fetch The Data...";
        //                        this.closingTime = "Couldn't Fetch The Data...";
        //                    }

        //                    finally
        //                    {
        //                        // If exception occured, let's call it quits here...
        //                        if (this.telephoneNumber == "Couldn't Fetch The Data...")
        //                        {
        //                            this.telephoneNumber = "Couldn't Fetch The Data...";
        //                            this.atmAddress1 = "Couldn't Fetch The Data...";
        //                            this.openingTime = "Couldn't Fetch The Data...";
        //                            this.closingTime = "Couldn't Fetch The Data...";
        //                        }
        //                        else
        //                        {
        //                            // Got correct data? Let's move further...
        //                            tempList.Add(this.telephoneNumber);
        //                            tempList.Add(this.openingTime);
        //                            tempList.Add(this.closingTime);
        //                            tempList.Add(this.atmAddress1);
        //                            listOfATMs.Add(distance, tempList);
        //                        }
        //                    }
        //                }

        //            }
        //            // If the List of ATM/Branches is 0, we don't have anything.
        //            if (listOfATMs.Count == 0)
        //            {
        //                this.telephoneNumber = "N/A";
        //                this.atmAddress1 = "No Bank Found Within 5000 Meters...";
        //                this.openingTime = "N/A";
        //                this.closingTime = "N/A";
        //                this.branchDistanceValue = "N/A";
        //            }
        //            // Else, let's have some fun.
        //            else
        //            {
        //                var atmDist = listOfATMs.Keys.ToList();
        //                atmDist.Sort();
        //                Console.WriteLine("atmDist : " + atmDist);

        //                // This listofATMs is a DICTIONARY (double, list). So, 'double' is distance and 'list' is the data associated with that particular branch.
        //                // Loop through keys and assign the data accordingly.
        //                foreach (var key in atmDist)
        //                {
        //                    //Console.WriteLine("Data --> {0}: {1} : {2} : {3} : {4}", key, listOfATMs[key][0], listOfATMs[key][1], listOfATMs[key][2], listOfATMs[key][3]);
        //                    branchList.Add(Convert.ToString(listOfATMs[key][0]));
        //                    branchList.Add(Convert.ToString(listOfATMs[key][1]));
        //                    branchList.Add(Convert.ToString(listOfATMs[key][2]));
        //                    branchList.Add(Convert.ToString(listOfATMs[key][3]));
        //                }

        //                //Console.WriteLine("listOfATMs : " + listOfATMs);
        //                this.atmAddress1 = Convert.ToString(branchList.First());

        //                this.telephoneNumber = Convert.ToString(branchList[0]);
        //                this.atmAddress1 = Convert.ToString(branchList[3]);
        //                this.branchDistanceValue = atmDist[0].ToString();
        //                TimeSpan openingTime2 = TimeSpan.Parse(branchList[1]);
        //                TimeSpan closingTime2 = TimeSpan.Parse(branchList[2]);
        //                Console.WriteLine("OP Time : " + openingTime);
        //                Console.WriteLine("ED Time : " + closingTime);
        //                Console.WriteLine("NOW Time : " + timeRightNow);
        //                bool openNow = timeChecker(openingTime2, closingTime2, timeRightNow);

        //                if (openNow == true)
        //                {
        //                    isTheBranchOpen = "Open Now";
        //                }
        //                else
        //                {
        //                    isTheBranchOpen = "Closed Now";
        //                }
        //                Console.WriteLine("Is Open : " + openNow);

        //            }
        //        }
        //        catch (Exception GettingError)
        //        {

        //            Console.WriteLine("Exception hui gwa : " + GettingError);
        //            throw;
        //        }

        //    }

        //}

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
            // start is after end, so do the inverse comparison
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

        // Let's check whether the user has Google Play Services Installed or not. Because FuseLocator needs it.
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

                // Show error dialog to let user debug google play services
            }
            return false;
        }

        //DONT REMOVE METHODS BELOW
        protected async override void OnResume()
        {
            base.OnResume();
            // We need to re-locate user on resume and re-fetch the closes location of branch.
            //buttonBranch.PerformClick();
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
                        //userLocation.Text = coordinates;
                        //UserLocation is TextView that is updated with coords
                        userLati = location.Latitude.ToString();
                        userLongi = location.Longitude.ToString();
                        userLatitude = userLati;
                        userLongitude = userLongi;

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
            Location location1 = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
            if (location1 != null)
            {
                toastcoords = "Latitude: " + location1.Latitude.ToString() + " Longitude: " + location1.Longitude.ToString() + " Provider: " + location1.Provider.ToString();
                //Toast.MakeText(this, toastcoords, ToastLength.Long).Show();
                CurrentLocation = new LatLng(location1.Latitude, location1.Longitude);
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(CurrentLocation, 14);
                mMap.AnimateCamera(cameraUpdate, 4000, null);
                mMap.AddMarker(new MarkerOptions().SetPosition(CurrentLocation).SetTitle("UserLocation"));
            }
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
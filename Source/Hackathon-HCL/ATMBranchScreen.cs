using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using System.Device.Location;
using System.Xml;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Util;
using Android.Gms.Common;
using Android.Locations;
using Plugin.TextToSpeech;
using Android.Speech;
using Android.Content;
using Android.Support.V7.App;
using Android.Graphics;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Net;

namespace Hackathon_HCL
{
    //[Activity(Label = " Nearest Barclays ATM ")]
    [Activity(Label = " Nearest Barclays ATM ", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ATMBranchScreen : AppCompatActivity, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener, IOnMapReadyCallback
    {
        // Declare all the global variables here.

        GoogleApiClient apiClient;
        LocationRequest locRequest;
        EditText atmLocation, atmDist;
        EditText UserLocation;
        string coordinates;
        private string branchLatitude, branchLongitude;
        private string userLati, userLongi, userAddress = "", atmAddress /*= "Nothing is near to you"*/;
        private double atmDistance;
        Dictionary<double, string> listOfATMs = new Dictionary<double, string>();
        List<string> atmList = new List<string>();
        private bool locateChecker = true;
        LatLng CurrentLocation;
        string toastcoords;
        private GoogleMap mMap;
        private ScrollView ATM_MainLayout;
        private LinearLayout ATM_ProgressLayout;

        private ProgressBar spinner;
        bool _isGooglePlayServicesInstalled;

        private readonly int VOICE = 10;

        // Declaration of variables Ends Here.

        protected async override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LayoutATMBranch);
            SetUpMap();

            Typeface tf = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            Android.Support.V7.Widget.Toolbar toolbar1 = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //toolbar1.SetLogo(Resource.Drawable.barlcayslogo);
            toolbar1.SetPadding(10, 0, 0, 0);
            toolbar1.SetTitleTextColor(Android.Graphics.Color.White);
            SetSupportActionBar(toolbar1);
            FindViews();

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
            //HandleEvents();

           
        }


        public override void OnBackPressed()
        {
            //base.OnStop();
            //base.OnDestroy();
            //CrossTextToSpeech.Dispose();
            base.OnBackPressed();
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.toolbar, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.contactUs)
            {
                Toast.MakeText(this, "Contact Us", ToastLength.Long);
            }

            return base.OnOptionsItemSelected(item);
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
            UserLocation = FindViewById<EditText>(Resource.Id.UserLocation);
            atmLocation = FindViewById<EditText>(Resource.Id.ATMLocation);
            Typeface tf = Typeface.CreateFromAsset(Assets, "NotoSans-Regular.ttf");
            atmLocation.SetTypeface(tf, TypefaceStyle.Normal);
            UserLocation.SetTypeface(tf, TypefaceStyle.Normal);
            atmDist = FindViewById<EditText>(Resource.Id.BranchLocation);
            ATM_MainLayout = FindViewById<ScrollView>(Resource.Id.ATM_MainLayout);
            ATM_ProgressLayout = FindViewById<LinearLayout>(Resource.Id.ATMlinearLayout_ProgressBar);
            //spinner = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            // Default Text
            UserLocation.Text = "Searching...";
            atmLocation.Text = "Searching...";
            atmDist.Text = "Calculating...";
        }

        private async void AtmFetcher()
        {
            //Toast.MakeText(this, "Please wait while I fetch the records...", ToastLength.Long).Show(); //Showing Bad Connection Error
            await SpeakerAsync("Please wait while I fetch the records.");
            //spinner.Visibility = ViewStates.Visible;
            ATM_MainLayout.Visibility = ViewStates.Gone;
            ATM_ProgressLayout.Visibility = ViewStates.Visible;
            await Task.Run(() => JsonFetcher(userLati, userLongi));
            
            if (this.atmAddress == "No ATM Found within 5000 meters...")
            {
                Toast.MakeText(this, "No ATM Found within 5000 meters.", ToastLength.Long).Show(); //Showing Bad Connection Error
                atmLocation.Text = "No ATM Found within 5000 meters.";
                atmDist.Text = Convert.ToString("-");
                UserLocation.Text = this.userAddress;
                ATM_MainLayout.Visibility = ViewStates.Visible;
                ATM_ProgressLayout.Visibility = ViewStates.Gone;
                //spinner.Visibility = ViewStates.Gone;
                locateChecker = false;
                await SpeakerAsync("Sorry, It seems like there is No Barclays ATM Within 5000 Meters.");

                await SpeakerAsync("Taking You Back To Main Menu.");
                await Task.Delay(5000);

                // Don't ask the user whether they want to go back or not. Just take them back.
                base.OnBackPressed();
            }
            else
            {
                atmLocation.Text = this.atmAddress;
                atmDist.Text = Convert.ToString(this.atmDistance);
                UserLocation.Text = this.userAddress;
                ATM_MainLayout.Visibility = ViewStates.Visible;
                ATM_ProgressLayout.Visibility = ViewStates.Gone;
                //spinner.Visibility = ViewStates.Gone;
                await SpeakerAsync("The Nearest ATM To You Right Now is " + atmLocation.Text);
                await SpeakerAsync("It is approximately " + atmDist.Text + "meters away from your current location");
                locateChecker = false;
                await SpeakerAsync("Opening the ATM's location in Maps.");
                string geoLocationGenerator = string.Format("geo:0,0?q={0}", atmLocation.Text);
                base.OnBackPressed();
                Console.WriteLine("geoLocationGenerator : " + geoLocationGenerator);
                var geoUri = Android.Net.Uri.Parse(geoLocationGenerator);
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                StartActivity(mapIntent);                
            }
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

                // Show error dialog to let user debug google play services
            }
            return false;
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
                var result = await client.PostAsync("/unitedhcl/api/barclays/atmLocationGetter.php", content);
                string resultContent = await result.Content.ReadAsStringAsync();

                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    Console.WriteLine("OBJ2 : " + obj2);
                    

                    this.branchLatitude = obj2.latitude;
                    this.branchLongitude = obj2.longitude;
                    this.atmAddress = Convert.ToString(obj2.buildingNumberOrName) + ", " + Convert.ToString(obj2.streetName) + ", " + Convert.ToString(obj2.townName) + ", " + Convert.ToString(obj2.postCode);
                    var sCoord = new GeoCoordinate(Convert.ToDouble(desiredLat), Convert.ToDouble(desiredLong));

                    var eCoord = new GeoCoordinate(Convert.ToDouble(this.branchLatitude), Convert.ToDouble(this.branchLongitude));

                    var distance = sCoord.GetDistanceTo(eCoord); // Distance is in Meters.

                    this.atmDistance = Math.Round(distance, 2);
                    Console.WriteLine("branchDistanceReceived : " + atmDistance);
                    Console.WriteLine("Lattidtude : " + branchLatitude);
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + Convert.ToString(sCoord).Replace(" ", "") + "&sensor=false");

                    XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
                    XmlNode xNode = xNodelst.Item(0);
                    this.userAddress = Convert.ToString(xNode.SelectSingleNode("formatted_address").InnerText); //NullException Here in 2nd round

                    if (!string.IsNullOrEmpty(this.branchLatitude))
                    {
                        if (string.IsNullOrEmpty(this.atmAddress) || this.atmAddress == "")
                        {
                            this.atmAddress = "No ATM Found within 5000 meters...";
                            this.atmDistance = 0.00;
                        }
                    }
                    else
                    {
                        this.atmAddress = "No ATM Found within 5000 meters...";
                        this.atmDistance = 0.00;
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
        //        var result = await client.GetAsync("/open-banking/v1.3/atms");
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
        //                        this.atmAddress = Convert.ToString(obj2.data[x].Address.BuildingNumberOrName.ToString() + ", " + obj2.data[x].Address.StreetName.ToString() + ", " + obj2.data[x].Address.TownName.ToString() + ", " + obj2.data[x].Address.CountrySubDivision.ToString() + ", " + obj2.data[x].Address.Country.ToString() + ", " + obj2.data[x].Address.PostCode.ToString() + ", United Kingdom");
        //                    }
        //                    catch (Exception)
        //                    {
        //                        XmlDocument xDoc = new XmlDocument();
        //                        // https://stackoverflow.com/questions/33613828/get-city-name-from-reverse-geocoding-with-latitude-and-longitude <-- Source
        //                        xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + Convert.ToString(eCoord).Replace(" ", "") + "&sensor=false");

        //                        XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
        //                        XmlNode xNode = xNodelst.Item(0);
        //                        this.atmAddress = Convert.ToString(xNode.SelectSingleNode("formatted_address").InnerText); //NullException Here in 2nd round
        //                    }
        //                    finally
        //                    {
        //                        listOfATMs.Add(distance, Convert.ToString(this.atmAddress));
        //                    }
        //                }

        //            }
        //            if (listOfATMs.Count == 0)
        //            {
        //                this.atmAddress = "No ATM Found within 5000 meters...";
        //                this.atmDistance = 0.00;
        //            }
        //            else
        //            {
        //                var atmDist = listOfATMs.Keys.ToList();
        //                atmDist.Sort();

        //                // Loop through keys.
        //                foreach (var key in atmDist)
        //                {
        //                    atmList.Add(Convert.ToString(listOfATMs[key]));
        //                }

        //                this.atmAddress = Convert.ToString(atmList.First());
        //                this.atmDistance = Math.Round(Convert.ToDouble(atmDist.First()), 2);
        //            }
        //        }
        //        catch (Exception GettingError)
        //        {

        //            Console.WriteLine("Exception hui gwa : " + GettingError);
        //            await SpeakerAsync("An Exception Has Occurred stating that " + GettingError.ToString());
        //            throw;
        //        }

        //    }

        //}

        //DONT REMOVE METHODS BELOW

        protected async override void OnResume()
        {
            base.OnResume();

            // We need to re-locate user on resume and re-fetch the closes location of ATM.
            if (locateChecker == true)
            {
                await SpeakerAsync("Trying To Locate You.");
                apiClient.Connect();
                if (apiClient.IsConnected)
                {

                    Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
                    if (location != null)
                    {
                        coordinates = "Latitude: " + location.Latitude.ToString() + " , Longitude: " + location.Longitude.ToString();
                        //UserLocation.Text = coordinates;
                        UserLocation.Text = "Searching...";
                        //UserLocation is TextView that is updated with coords
                        userLati = location.Latitude.ToString();
                        userLongi = location.Longitude.ToString();

                        // As soon as the location of the user is fetched, look for the nearest branch.
                        await SpeakerAsync("Searching For The Nearest ATM Now.");
                        AtmFetcher();
                    }
                    else
                    {
                        Toast.MakeText(this, "Could not find your location. Make sure GPS is on and try again later...", ToastLength.Long).Show(); //Showing Bad Connection Error
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
                Toast.MakeText(this, toastcoords, ToastLength.Long).Show();
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
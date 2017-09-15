using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Runtime;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace Hackathon_HCL
{
    [Activity(Label = "Formula E Trackside Wayfinding")]
    public class Maps : Activity, IOnMapReadyCallback, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener, GoogleMap.IInfoWindowAdapter
    {
        GoogleApiClient apiClient;
        LocationRequest locRequest;
        Spinner spinner;
        bool isGooglePlayServicesInstalled;
        public int ServiceFlag;
        string toastcoords;
        private GoogleMap mMap;
        private ImageButton buttonRestRoom, buttonCafe, buttonGrandStand, buttonFEHQ, buttonLocation;
        LatLng UserLocation;
        List<string> listStrLineElements;
        List<double> DOBresult;
        string tempLocationString;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapScreen);

            FindViews();
            SetUpMap();
            ClickEvents();

            isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();
            if (isGooglePlayServicesInstalled)
            {
                apiClient = new GoogleApiClient.Builder(this, this, this).AddApi(LocationServices.API).Build();
                locRequest = new LocationRequest();
            }
            else
            {
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }

            apiClient.Connect();    //Connect to Google Services API Client
        }

        private async Task CordiFetcher(string type, string country)
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
                new KeyValuePair<string, string>("location_type", type),
                new KeyValuePair<string, string>("country_name", country)
            });
                var result = await client.PostAsync("/unitedhcl/api/maps/mapMarkers.php", content);
                tempLocationString = await result.Content.ReadAsStringAsync();
            }


        }

        private void ClickEvents()
        {
            buttonRestRoom.Click += buttonRestRoom_Click;
            buttonCafe.Click += buttonCafe_Click;
            buttonGrandStand.Click += buttonGrandStand_ClickAsync;
            buttonFEHQ.Click += buttonFEHQ_Click;
            spinner.ItemSelected += Spinner_ItemSelected;
            buttonLocation.Click += buttonLocation_Click;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            //Create Google Maps Object
            mMap = googleMap;
            mMap.SetInfoWindowAdapter(this);
        }

        private void FindViews()
        {
            spinner = FindViewById<Spinner>(Resource.Id.spinner);
            buttonRestRoom = FindViewById<ImageButton>(Resource.Id.buttonRestRoom);
            buttonCafe = FindViewById<ImageButton>(Resource.Id.buttonCafe);
            buttonGrandStand = FindViewById<ImageButton>(Resource.Id.buttonGrandStand);
            buttonFEHQ = FindViewById<ImageButton>(Resource.Id.buttonFEHQ);
            buttonLocation = FindViewById<ImageButton>(Resource.Id.buttonLocation);
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0: //Normal MapType
                    mMap.MapType = GoogleMap.MapTypeNormal;
                    break;
                case 1: //Hybrid MapType
                    mMap.MapType = GoogleMap.MapTypeHybrid;
                    break;
                case 2: //Sattelite MapType
                    mMap.MapType = GoogleMap.MapTypeSatellite;
                    break;
                case 3: //Terrain MapType
                    mMap.MapType = GoogleMap.MapTypeTerrain;
                    break;
                default:
                    mMap.MapType = GoogleMap.MapTypeHybrid;
                    break;
            }
        }
        bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);

            if (queryResult == ConnectionResult.Success)
            {
                //Toast.MakeText(this, "Google API Connection Successful!", ToastLength.Short).Show();
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Toast.MakeText(this, "There is a problem with Google Play Services on this device: " + queryResult + errorString, ToastLength.Long).Show();
            }

            return false;
        }
        private void APItoMarkerMapUpdater()
        {
            mMap.Clear();
            Toast.MakeText(this, "Coordinates Recieved from the Server.", ToastLength.Short).Show();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(52.830168, -1.3770267), 14);
            mMap.AnimateCamera(cameraUpdate, 4000, null);

            listStrLineElements = tempLocationString.Split(',').ToList();
            listStrLineElements.Remove(listStrLineElements.Last());
            DOBresult = listStrLineElements.Select(x => double.Parse(x)).ToList();

            switch (ServiceFlag)
            {
                case 1:     //Formula E HQ Marker
                    for (int i = 0; i < DOBresult.Count; i = i + 2)
                    {
                        LatLng Coordinates = new LatLng(DOBresult[i], DOBresult[i + 1]);
                        mMap.AddMarker(new MarkerOptions().SetPosition(Coordinates).SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueMagenta)));
                    }
                    break;
                case 2:     //GrandStands Marker
                    for (int i = 0; i < DOBresult.Count; i = i + 2)
                    {
                        LatLng Coordinates = new LatLng(DOBresult[i], DOBresult[i + 1]);
                        mMap.AddMarker(new MarkerOptions().SetPosition(Coordinates).SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow)));
                    }
                    break;
                case 3:     //Cafes Marker
                    for (int i = 0; i < DOBresult.Count; i = i + 2)
                    {
                        LatLng Coordinates = new LatLng(DOBresult[i], DOBresult[i + 1]);
                        mMap.AddMarker(new MarkerOptions().SetPosition(Coordinates).SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueViolet)));
                    }
                    break;
                case 4:     //RestRooms Marker
                    for (int i = 0; i < DOBresult.Count; i = i + 2)
                    {
                        LatLng Coordinates = new LatLng(DOBresult[i], DOBresult[i + 1]);
                        mMap.AddMarker(new MarkerOptions().SetPosition(Coordinates).SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
                    }
                    break;
            }

        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            mMap.Clear();
            //Fetch GPS Location using Fused Location Provider and Google Location API
            Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
            if (location != null)
            {
                toastcoords = "Latitude: " + location.Latitude.ToString() + " Longitude: " + location.Longitude.ToString() + " Provider: " + location.Provider.ToString();
                UserLocation = new LatLng(location.Latitude, location.Longitude);
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(UserLocation, 14);
                mMap.AnimateCamera(cameraUpdate, 4000, null);
                mMap.AddMarker(new MarkerOptions().SetPosition(UserLocation).SetTitle("UserLocation").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed)));
            }
            ServiceFlag = 0;
        }

        private async void buttonFEHQ_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Fetching Coordinates for FEHQ...", ToastLength.Short).Show();
            await Task.Run(() => CordiFetcher("FEHQ", "England"));
            ServiceFlag = 1;
            APItoMarkerMapUpdater();
        }

        private async void buttonGrandStand_ClickAsync(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Fetching Coordinates for GrandStands...", ToastLength.Short).Show();
            await Task.Run(() => CordiFetcher("GrandStand", "England"));
            ServiceFlag = 2;
            APItoMarkerMapUpdater();
        }

        private async void buttonCafe_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Fetching Coordinates for Cafes...", ToastLength.Short).Show();
            await Task.Run(() => CordiFetcher("Cafe", "England"));
            ServiceFlag = 3;
            APItoMarkerMapUpdater();
        }

        private async void buttonRestRoom_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Fetching Coordinates for RestRooms...", ToastLength.Short).Show();
            await Task.Run(() => CordiFetcher("Restroom", "England"));
            ServiceFlag = 4;
            APItoMarkerMapUpdater();
        }

        //Initialize Maps and Views
        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.Map).GetMapAsync(this);
            }
        }

        //Display Info Window on Marker Click 
        public View GetInfoWindow(Marker marker)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.MakerInfoWindow, null, false);
            switch (ServiceFlag)
            {
                case 0:     //Location Marker
                    view.FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.ic_location);
                    view.FindViewById<TextView>(Resource.Id.HeadingtextView).Text = "You Are Here!";
                    view.FindViewById<TextView>(Resource.Id.InfotextView).Text = "Your Current Location";
                    break;
                case 1:     //Formula E HQ Marker
                    view.FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.HQIcon);
                    view.FindViewById<TextView>(Resource.Id.HeadingtextView).Text = "Formula E HQ";
                    view.FindViewById<TextView>(Resource.Id.InfotextView).Text = "Formula E, officially the FIA Formula E Championship, is a class of auto racing that uses only electric-powered cars.";
                    break;
                case 2:     //GrandStands Marker
                    view.FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.Grandstand);
                    view.FindViewById<TextView>(Resource.Id.HeadingtextView).Text = "GrandStand";
                    view.FindViewById<TextView>(Resource.Id.InfotextView).Text = "The Stands where people can sit down and watch FIA Formula E Races.";
                    break;
                case 3:     //Cafes Marker
                    view.FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.cafe);
                    view.FindViewById<TextView>(Resource.Id.HeadingtextView).Text = "Cafe";
                    view.FindViewById<TextView>(Resource.Id.InfotextView).Text = "Donningtin Park Circuit has two Multi-Cuisine Cafes that will satisfy every hunger of the Race-goers";
                    break;
                case 4:     //RestRooms Marker
                    view.FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.restrooms);
                    view.FindViewById<TextView>(Resource.Id.HeadingtextView).Text = "Restroom";
                    view.FindViewById<TextView>(Resource.Id.InfotextView).Text = "Public Restrooms.";
                    break;
            }
            return view;
        }
        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }

        public void OnConnected(Bundle bundle)
        {
        }

        public void OnDisconnected()
        {
        }

        public void OnConnectionFailed(ConnectionResult bundle)
        {
        }

        public void OnLocationChanged(Location location)
        {
        }

        public void OnConnectionSuspended(int i)
        {
        }
    }
}


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
        public int InfoWindowFlag;
        string toastcoords;
        private GoogleMap mMap;
        private Button buttonRestRoom, buttonCafe, buttonGrandStand, buttonFEHQ, buttonLocation;
        LatLng UserLocation, Stadium, GrandStand1, GrandStand2, Restroom1, Restroom2, Restroom3, Restroom4, Restroom5, Restroom6, Restroom7, Restroom8, Cafe1, Cafe2, FEHQ;
        //Coordinates are Hardcoded as no resources were provided for location of Basic necessities around the tracks. It is done just for Prototype phase and PoC.

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapScreen);

            FindViews();
            CoordsAssign();
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

        private void ClickEvents()
        {
            buttonRestRoom.Click += buttonRestRoom_Click;
            buttonCafe.Click += buttonCafe_Click;
            buttonGrandStand.Click += buttonGrandStand_Click;
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
            buttonRestRoom = FindViewById<Button>(Resource.Id.buttonRestRoom);
            buttonCafe = FindViewById<Button>(Resource.Id.buttonCafe);
            buttonGrandStand = FindViewById<Button>(Resource.Id.buttonGrandStand);
            buttonFEHQ = FindViewById<Button>(Resource.Id.buttonFEHQ);
            buttonLocation = FindViewById<Button>(Resource.Id.buttonLocation);
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0: //Normal MapType
                    mMap.MapType = GoogleMap.MapTypeHybrid;
                    break;
                case 1: //Hybrid MapType
                    mMap.MapType = GoogleMap.MapTypeNormal;
                    
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
                Toast.MakeText(this, "Google API Connection Successful!", ToastLength.Short).Show();
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Toast.MakeText(this, "There is a problem with Google Play Services on this device: " + queryResult + errorString, ToastLength.Long).Show();
            }

            return false;
        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            mMap.Clear(); 
            //Fetch GPS Location using Fused Location Provider and Google Location API
            Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
            if (location != null)
            {
                toastcoords = "Latitude: " + location.Latitude.ToString() + " Longitude: " + location.Longitude.ToString() + " Provider: " + location.Provider.ToString();
                Toast.MakeText(this, toastcoords, ToastLength.Long).Show();
                UserLocation = new LatLng(location.Latitude, location.Longitude);
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(UserLocation, 14);
                mMap.AnimateCamera(cameraUpdate, 4000, null);
                mMap.AddMarker(new MarkerOptions().SetPosition(UserLocation).SetTitle("UserLocation").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed)));
            }
            InfoWindowFlag = 0;
        }

        //Add Google Markers on Button Click
        //Animate Camera to Marker Locations
        private void buttonFEHQ_Click(object sender, System.EventArgs e)
        {
            mMap.Clear();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(Stadium, 14);
            mMap.AnimateCamera(cameraUpdate, 4000, null);
            MarkerOptions FEHQmarker = new MarkerOptions();
            FEHQmarker.SetPosition(FEHQ).SetTitle("FEHQ").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueViolet));
            mMap.AddMarker(FEHQmarker);
            InfoWindowFlag = 1;
        }

        private void buttonGrandStand_Click(object sender, System.EventArgs e)
        {
            mMap.Clear();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(Stadium, 14);
            mMap.AnimateCamera(cameraUpdate, 4000, null);
            MarkerOptions GrandStand = new MarkerOptions();
            GrandStand.SetPosition(GrandStand1).SetTitle("GrandStand1").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));
            mMap.AddMarker(GrandStand);
            mMap.AddMarker(new MarkerOptions().SetPosition(GrandStand2).SetTitle("GrandStand2").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow)));
            InfoWindowFlag = 2;
        }

        private void buttonCafe_Click(object sender, System.EventArgs e)
        {
            mMap.Clear();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(Stadium, 14);
            mMap.AnimateCamera(cameraUpdate, 4000, null);
            MarkerOptions Cafe = new MarkerOptions();
            Cafe.SetPosition(Cafe1).SetTitle("Cafe1").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
            mMap.AddMarker(Cafe);
            mMap.AddMarker(new MarkerOptions().SetPosition(Cafe2).SetTitle("Cafe2").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen)));
            InfoWindowFlag = 3;
        }

        private void buttonRestRoom_Click(object sender, System.EventArgs e)
        {
            mMap.Clear();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(Stadium, 14);
            mMap.AnimateCamera(cameraUpdate, 4000, null);
            MarkerOptions RestRoom = new MarkerOptions();
            RestRoom.SetPosition(Restroom1).SetTitle("RestRoom1").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue));
            mMap.AddMarker(RestRoom);
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom2).SetTitle("RestRoom2").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom3).SetTitle("RestRoom3").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom4).SetTitle("RestRoom4").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom5).SetTitle("RestRoom5").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom6).SetTitle("RestRoom6").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom7).SetTitle("RestRoom7").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            mMap.AddMarker(new MarkerOptions().SetPosition(Restroom8).SetTitle("RestRoom8").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));
            InfoWindowFlag = 4;
        }
        protected void CoordsAssign()
        {
            //Assign Latitude, Longitude to display basic necessities on the map for PoC.
            Stadium = new LatLng(52.830013, -1.374737);
            GrandStand1 = new LatLng(52.830136, -1.379141);
            GrandStand2 = new LatLng(52.829419, -1.378130);
            Restroom1 = new LatLng(52.828499, -1.364191);
            Restroom2 = new LatLng(52.832253, -1.367951);
            Restroom3 = new LatLng(52.833201, -1.376405);
            Restroom4 = new LatLng(52.832603, -1.382452);
            Restroom5 = new LatLng(52.830085, -1.383991);
            Restroom6 = new LatLng(52.829926, -1.382816);
            Restroom7 = new LatLng(52.828714, -1.383107);
            Restroom8 = new LatLng(52.828281, -1.374109);
            Cafe1 = new LatLng(52.828800, -1.3824590);
            Cafe2 = new LatLng(52.827159, -1.364998);
            FEHQ = new LatLng(52.827941, -1.384954);
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
            switch (InfoWindowFlag)
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


using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "Hackathon_HCL", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SignupScreen : AppCompatActivity
    {
        private TextView SignupLoginProceed, signupName, signupPassword, signupEmail, signupPhone;
        private Button SignupProceed;
        private string name, userName, email, phoneNumber, apiSecret, message, countryCodeSelected, finalPhoneNumber;
        private Boolean signupStatus;
        private ProgressBar spinner;
        private Spinner SpinnerCountryCode;

        protected override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SignupScreen);

            FindViews();
            HandleEvents();
        }

        private void FindViews()
        {
            Typeface tf = Typeface.CreateFromAsset(Assets, "Khula-Regular.ttf");
            Typeface tf1 = Typeface.CreateFromAsset(Assets, "Khula-Light.ttf");
            Typeface tf2 = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            SignupLoginProceed = FindViewById<TextView>(Resource.Id.SignupLogin);
            SignupLoginProceed.SetTypeface(tf1, TypefaceStyle.Bold);
            SignupProceed = FindViewById<Button>(Resource.Id.SignupButton);
            SignupProceed.SetTypeface(tf2, TypefaceStyle.Bold);
            signupName = FindViewById<TextView>(Resource.Id.SignupName);
            signupName.SetTypeface(tf, TypefaceStyle.Normal);
            signupPassword = FindViewById<TextView>(Resource.Id.SignupPassword);
            signupPassword.SetTypeface(tf, TypefaceStyle.Normal);
            signupEmail = FindViewById<TextView>(Resource.Id.SignupEmail);
            signupEmail.SetTypeface(tf, TypefaceStyle.Normal);
            signupPhone = FindViewById<TextView>(Resource.Id.SignupPhone);
            signupPhone.SetTypeface(tf, TypefaceStyle.Normal);
            spinner = FindViewById<ProgressBar>(Resource.Id.progressBar2);
            var SignupLogin1 = FindViewById<TextView>(Resource.Id.SignupLogin);
            SignupLogin1.SetTypeface(tf, TypefaceStyle.Bold);
            SpinnerCountryCode = FindViewById<Spinner>(Resource.Id.SignupSpinnerCountry);

            //Code for Drop Down list of Country Code
            SpinnerCountryCode.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Country_Code, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            SpinnerCountryCode.Adapter = adapter;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {

            Spinner spinner = (Spinner)sender;

            string toast = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
            countryCodeSelected = toast.Substring(0, 3); //Selects only first 3 letters of Country Code : '+91 - India' will change to '+91'
        
        }
        private void HandleEvents()
        {
            SignupLoginProceed.Click += SignupLoginProceed_Click;
            SignupProceed.Click += async (sender, e) =>
            {
                finalPhoneNumber = countryCodeSelected + "-" + signupPhone.Text;

                if (string.IsNullOrEmpty(signupName.Text))
                {
                    Toast.MakeText(this, "Please Enter Your Name.", ToastLength.Long).Show();
                }
                else if (string.IsNullOrEmpty(finalPhoneNumber))
                {
                    Toast.MakeText(this, "Please Enter Your Phone Number.", ToastLength.Long).Show();
                }
                else if (string.IsNullOrEmpty(signupEmail.Text))
                {
                    Toast.MakeText(this, "Please Enter Your Email ID.", ToastLength.Long).Show();
                }
                else if (string.IsNullOrEmpty(signupPassword.Text))
                {
                    Toast.MakeText(this, "Please Enter A Password.", ToastLength.Long).Show();
                }
                else
                {
                    if (signupPhone.Text.Count() < 10)
                    {
                        Toast.MakeText(this, "Phone Number Cannot be smaller than 10 digits.", ToastLength.Long).Show();
                    }
                    else
                    {
                        if (Android.Util.Patterns.EmailAddress.Matcher(signupEmail.Text).Matches())
                        {
                            spinner.Visibility = ViewStates.Visible;

                            await Task.Run(() => SignUpPoster(signupName.Text, signupEmail.Text, finalPhoneNumber, signupPassword.Text));


                            if (this.signupStatus == true)
                            {
                                var IntentLoginProceed = new Intent(this, typeof(MainActivity));
                                StartActivity(IntentLoginProceed);
                                Finish();
                            }
                            else
                            {
                                Toast.MakeText(this, this.message, ToastLength.Long).Show(); //Showing Bad Connection Error
                            }

                            spinner.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            Toast.MakeText(this, "Please Enter A Valid Email Address.", ToastLength.Long).Show();
                        }
                    }

                }

            };
        }


        private void SignupLoginProceed_Click(object sender, EventArgs e)
        {
            var IntentSignupLoginProceed = new Intent(this, typeof(LoginScreen));
            StartActivity(IntentSignupLoginProceed);
            Finish();
        }

        private async Task SignUpPoster(string name, string email, string phoneNumber, string password)
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
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("phoneNumber", phoneNumber),
                new KeyValuePair<string, string>("password", password)
            });
                var result = await client.PostAsync("/unitedhcl/api/users/sign_up.php", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                
                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);

                    if (obj2.error_code == 1)
                    {
                        Console.WriteLine(obj2.message);
                        this.message = obj2.message;
                        this.signupStatus = false;
                    }
                    else
                    {
                        this.name = obj2.name;
                        this.userName = obj2.username;
                        this.email = obj2.email;
                        this.phoneNumber = obj2.phone;
                        this.apiSecret = obj2.api_secret;
                        this.signupStatus = true; // Flag to tell the other methods about Sign up status
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
    }
}
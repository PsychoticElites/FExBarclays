using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using Android.Graphics;
using Android.Support.V7.App;
using System.Net;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "Hackathon HCL", MainLauncher = false, Theme ="@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginScreen : AppCompatActivity
    {
        private Button LoginProceed;
        private TextView LoginSignupProceed, userEmailId, userPassword;
        private string name, userName, email, phoneNumber, apiSecret, message;
        private Boolean loginStatus;
        private ProgressBar spinner;
        
        protected override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
 
            SetContentView(Resource.Layout.LoginScreen);

            FindViews();
            HandleEvents();
        }

        private void FindViews()
        {
            Typeface tf = Typeface.CreateFromAsset(Assets, "Khula-Regular.ttf");
            Typeface tf2 = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            LoginProceed = FindViewById<Button>(Resource.Id.ButtonLogin);
            LoginProceed.SetTypeface(tf2, TypefaceStyle.Normal);
            LoginSignupProceed = FindViewById<TextView>(Resource.Id.LoginSignup);
            Typeface tf1 = Typeface.CreateFromAsset(Assets, "Khula-Light.ttf");
            LoginSignupProceed.SetTypeface(tf, TypefaceStyle.Bold);
            userEmailId = FindViewById<TextView>(Resource.Id.LoginEmail);
            userEmailId.InputType = Android.Text.InputTypes.TextVariationEmailAddress;
            userEmailId.SetTypeface(tf, TypefaceStyle.Normal);
            userPassword = FindViewById<TextView>(Resource.Id.LoginPassword);
            userPassword.SetTypeface(tf, TypefaceStyle.Normal);
            spinner = FindViewById<ProgressBar>(Resource.Id.progressBar1);

        }

        private void HandleEvents()
        {

            LoginProceed.Click += async (sender, e) =>
            {

                if (String.IsNullOrEmpty(userEmailId.Text))
                {

                    Toast.MakeText(this, "Email ID Field cannot be blank", ToastLength.Long).Show(); //Showing Bad Connection Error
                }
                else if (String.IsNullOrEmpty(userPassword.Text))
                {
                    Toast.MakeText(this, "Password Field cannot be blank", ToastLength.Long).Show(); //Showing Bad Connection Error
                }
                else
                {
                    if (Android.Util.Patterns.EmailAddress.Matcher(userEmailId.Text).Matches())
                    {
                        spinner.Visibility = ViewStates.Visible;
                        //LoginProceed_Click;
                        await Task.Run(() => LoginPoster(userEmailId.Text, userPassword.Text));
                        spinner.Visibility = ViewStates.Gone;

                        if (this.loginStatus == true)
                        {
                            var IntentLoginProceed = new Intent(this, typeof(MainActivity));
                            StartActivity(IntentLoginProceed);
                            Finish();
                        }
                        else
                        {
                            // Reset the fields on un-successful Login.
                            userEmailId.Text = "";
                            userPassword.Text = "";
                            Toast.MakeText(this, this.message, ToastLength.Long).Show(); //Showing Bad Connection Error
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Please Enter A Valid Email Address.", ToastLength.Long).Show(); //Showing Bad Connection Error
                    }
                }
            };
            LoginSignupProceed.Click += LoginSignupProceed_Click;
        }

        private void LoginSignupProceed_Click(object sender, EventArgs e)
        {
            var IntentSignupProceed = new Intent(this, typeof(SignupScreen));
            StartActivity(IntentSignupProceed);
            Finish();
        }



        
        private async Task LoginPoster(string userEmail, string userPassword)
        {
            Console.WriteLine("Email Id : " + userEmail);
            Console.WriteLine("Password : " + userPassword);

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
                
                new KeyValuePair<string, string>("email", userEmail),
                new KeyValuePair<string, string>("password", userPassword)
            });
                var result = await client.PostAsync("/unitedhcl/api/users/sign_in.php", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                Console.WriteLine("resultContent : " + resultContent);
                
                try
                {
                    dynamic obj2 = Newtonsoft.Json.Linq.JObject.Parse(resultContent);
                    Console.WriteLine("OBJ2 : " + obj2);
                    Console.WriteLine("error_code : " + obj2.error_code);
                    if (obj2.error_code == "1")
                    {
                        Console.WriteLine(obj2.message);

                        this.message = obj2.message;
                        this.loginStatus = false; 
                    }
                    else
                    {
                        this.name = obj2.name;
                        this.userName = obj2.username;
                        this.email = obj2.email;
                        this.phoneNumber = obj2.phone;
                        this.apiSecret = obj2.api_secret;
                        this.loginStatus = true; 
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
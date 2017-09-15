using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Views;
using Android.Content;
using Plugin.TextToSpeech;
using Android.Speech;
using Android.Graphics;
using Android.Content.PM;

namespace Hackathon_HCL
{
    [Activity(Label = "Barclays Bank", ScreenOrientation = ScreenOrientation.Portrait)]
    public class BarclayMain : Activity
    {
        private Button ATMLocation;
        private Button BranchLocation;
        private bool started = true;
        private readonly int VOICE = 10;

        public override void OnBackPressed()
        {
            //base.OnStop();
            base.OnDestroy();
            CrossTextToSpeech.Dispose();
            base.OnBackPressed();
        }
        protected async override void OnCreate(Bundle bundle)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Typeface tf = Typeface.CreateFromAsset(Assets, "PoppinsMedium.ttf");
            var ATM = FindViewById<Button>(Resource.Id.ATMBranch);
            ATM.SetTypeface(tf, TypefaceStyle.Normal);

            //Typeface tf1 = Typeface.CreateFromAsset(Assets, "SourceSansPro-Regular.ttf");
            var Branch = FindViewById<Button>(Resource.Id.BranchDetailsScreen);
            Branch.SetTypeface(tf, TypefaceStyle.Normal);

            FindViews();

            started = false;
            
            // Speak the Menu...
            await SpeakerAsync("Welcome To Barclays Application.");
            await SpeakerAsync("What would you like to do?.");
            await SpeakerAsync("Find nearest ATM Location,or ");
            await SpeakerAsync("Find nearest Barclays Bank Branch");
            await SpeakerAsync("Or do you want to go back to main menu?");

            // Let the Gal speak first, then we handle events...
            HandleEvents();

            // Start the voice intent.
            VoiceIntentStarter();
        }

        protected async override void OnRestart()
        {
            base.OnRestart();
            try
            {
                if (started == false)
                {
                    // Speak the Menu...
                    await SpeakerAsync("Welcome To Barclays Application.");
                    await SpeakerAsync("What would you like to do?.");
                    await SpeakerAsync("Find nearest ATM Location,or ");
                    await SpeakerAsync("Find nearest Barclays Bank Branch");
                    await SpeakerAsync("Or do you want to go back to main menu?");

                    // Start the voice intent.
                    VoiceIntentStarter();
                }
            }
            catch (Exception ResumeException)
            {

                Console.WriteLine("ResumeException" + ResumeException);
                throw;
            }
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

        private void HandleEvents()
        {
            ATMLocation.Click += ATMLocation_Click;
            BranchLocation.Click += BranchLocation_Click;
        }

        private void BranchLocation_Click(object sender, EventArgs e)
        {
            var IntentBranchLocation = new Intent(this, typeof(BranchDetails));
            StartActivity(IntentBranchLocation);
        }

        private void ATMLocation_Click(object sender, EventArgs e)
        {
            var IntentATMLocation = new Intent(this, typeof(ATMBranchScreen));
            StartActivity(IntentATMLocation);
        }

        private void FindViews()
        {
            ATMLocation = FindViewById<Button>(Resource.Id.ATMBranch);
            BranchLocation = FindViewById<Button>(Resource.Id.BranchDetailsScreen);
        }

        private async System.Threading.Tasks.Task SpeakerAsync(string speakingText)
        {
            await CrossTextToSpeech.Current.Speak(speakingText,
                    pitch: (float)1.0,
                    speakRate: (float)0.9,
                    volume: (float)2.0);
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
                                if (item.Replace(" ", "").ToLower().Contains("atm"))
                                {
                                    await SpeakerAsync("Selecting ATM Location Finder.");
                                    started = false;
                                    ATMLocation.PerformClick();
                                    break;
                                }
                                else if (item.Replace(" ", "").ToLower().Contains("adm"))
                                {
                                    await SpeakerAsync("Selecting ATM Location Finder.");
                                    started = false;
                                    ATMLocation.PerformClick();
                                    break;
                                }
                                else if (item.Replace(" ", "").ToLower().Contains("adam"))
                                {
                                    await SpeakerAsync("Selecting ATM Location Finder.");
                                    started = false;
                                    ATMLocation.PerformClick();
                                    break;
                                }
                                else if (item.Replace(" ", "").ToLower().Contains("branch"))
                                {
                                    await SpeakerAsync("Selecting Branch Location Finder.");
                                    started = false;
                                    BranchLocation.PerformClick();
                                    break;
                                }
                                else if (item.Replace(" ", "").ToLower().Contains("yes"))
                                {
                                    await SpeakerAsync("Taking you back to main menu.");
                                    started = false;
                                    base.OnBackPressed();
                                    break;
                                }
                                else if (item.Replace(" ", "").ToLower().Contains("back"))
                                {
                                    await SpeakerAsync("Taking you back to main menu.");
                                    started = false;
                                    base.OnBackPressed();
                                    break;
                                }
                                else if (item.ToLower().Contains("bank branch"))
                                {
                                    await SpeakerAsync("Selecting Branch Location Finder.");
                                    started = false;
                                    BranchLocation.PerformClick();
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
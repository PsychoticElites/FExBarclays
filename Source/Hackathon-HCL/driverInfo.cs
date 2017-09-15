using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Hackathon_HCL
{
    public class driverInfo
    {
        public int driverID { get; set; }
        public string driverName { get; set; }
        public string driverTeamName { get; set; }
        public ImageView driverPhoto { get; set; }
    }
}
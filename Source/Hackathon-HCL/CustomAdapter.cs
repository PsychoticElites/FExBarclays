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
using Java.Lang;

namespace Hackathon_HCL
{
    public class CustomAdapter : BaseAdapter<driverInfo>
    {
        public List<driverInfo> mItem;
        private Context mContext;

        public CustomAdapter(Context context, List<driverInfo> items)
        {
            mItem = items;
            mContext = context;
        }
        public override int Count
        {
            get { return mItem.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override driverInfo this[int position]
        {
            get { return mItem[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.driverInfo_row, null, false);
            }

            TextView driverName1 = row.FindViewById<TextView>(Resource.Id.driverName);
            driverName1.Text = mItem[position].driverName;

            TextView driverTeamName1 = row.FindViewById<TextView>(Resource.Id.driverTeamName);
            driverTeamName1.Text = mItem[position].driverTeamName;

            Console.WriteLine("This is a Data : ",mItem[position].driverPhoto);
            return row;
        }
    }
}
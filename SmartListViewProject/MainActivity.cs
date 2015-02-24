using Android.App;
using Android.OS;

namespace SmartListViewProject
{
    [Activity(Label = "SmartListViewProject", MainLauncher = true, Icon = "@drawable/icon" , ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            if (savedInstanceState == null)
            {
                FragmentManager.BeginTransaction().Add(Resource.Id.SmartListFrame, SmartListViewFragment.NewInstance()).Commit();
            }
        }
    }
}



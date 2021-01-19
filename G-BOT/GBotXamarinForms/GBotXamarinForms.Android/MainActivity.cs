using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Plugin.NFC;
using Android.Hardware.Usb;
using GBotXamarinForms.Droid.Sat;

[assembly: Xamarin.Forms.Dependency(typeof(GBotXamarinForms.Droid.MainActivity))]
namespace GBotXamarinForms.Droid
{
    [Activity(Label = "GertecOne XamarinForms G-BOT", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize ),
         MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter"),
        IntentFilter(new[] { "android.hardware.usb.action.USB_DEVICE_ATTACHED", "android.intent.action.MAIN" })
]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IKioskMode
    {
        public static MainActivity mContext { get; private set; }

        public static SatFunctions satFunctions;
        public TEF.Tef tef;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // Plugin NFC: Initialization
            CrossNFC.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            mContext = this;
            tef = new TEF.Tef();
            satFunctions = new SatFunctions(Application.Context);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {   // quando detecta movimento o sensor emite a tecla F4
            if (e.KeyCode == Keycode.F4 && e.Action == KeyEventActions.Down)
            {                
                Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Sensor_Notification");                
            }
            //necessario retornar o evento para a base para que as teclas sejam processadas normalmente
            return base.DispatchKeyEvent(e);
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Plugin NFC: Restart NFC listening on resume (needed for Android 10+) 
            CrossNFC.OnResume();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            // Plugin NFC: Tag Discovery Interception
            CrossNFC.OnNewIntent(intent);
        }

        public void StartKioskMode()
        {   
            // ativa o Lock da Tela na aplicacao
            mContext.StartLockTask();
            SetVisibleMode(true);


        }

        public void StopKioskMode()
        {
            // Desativa o modo lock da tela
            SetVisibleMode(false);
            mContext.StopLockTask();
        }

        public bool StatusKioskMode()
        {
            // obtem o status do lock da tela
            ActivityManager activityManager = (ActivityManager)Application.Context.GetSystemService(Context.ActivityService);
            return activityManager.LockTaskModeState != LockTaskMode.None;
        }

        private void SetVisibleMode(bool isFullscreen)
        {
            RunOnUiThread(() =>
            {
                if (isFullscreen)
                {
                    //altera a interface para fullscreen
                    mContext.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                        SystemUiFlags.Fullscreen
                        | SystemUiFlags.HideNavigation
                        | SystemUiFlags.Immersive
                        | SystemUiFlags.ImmersiveSticky
                        | SystemUiFlags.LowProfile
                        | SystemUiFlags.LayoutStable
                        | SystemUiFlags.LayoutHideNavigation
                        | SystemUiFlags.LayoutFullscreen
                    );
                }
                else
                {
                    mContext.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                        SystemUiFlags.LayoutStable
                    );
                }
            });
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            tef.ResultadoTef(requestCode, resultCode, data);
        }

    }
}
using Android.Runtime;
using System;

namespace GBotXamarinForms
{
    class LedUtil
    {
        public static void SetBlueLed()
        {
            SetLed("0x0200");
        }

        public static void SetBluePlusWhiteLed()
        {
            SetLed("0x0220");
        }

        public static void SetLightBlueLed()
        {
            SetLed("0x0270");
        }

        public static void SetPinkLed()
        {
            SetLed("0x0280");
        }

        public static void SetLightPinkLed()
        {
            SetLed("0x0380");
        }

        public static void SetCyanLed()
        {
            SetLed("0x0300");
        }

        public static void SetGreenLed()
        {
            SetLed("0x0100");
        }

        public static void SetLightGreen()
        {
            SetLed("0x0119");
        }

        public static void SetLightGreenPlusWhite()
        {
            SetLed("0x0140");
        }

        public static void SetRedLed()
        {
            SetLed("0x0080");
        }

        public static void SetOrangeLed()
        {
            SetLed("0x0180");
        }

        public static void SetOnLed()
        {
            SetLed("0xffff");
        }

        public static void SetOffLed()
        {
            SetLed("0x0000");
        }
        public static void SetLed(string cmd)
        {
            try
            {
                // JNI localiza a classe WriteSysFileManager
                IntPtr class_ref = JNIEnv.FindClass("android/os/WriteSysFileManager");

                // JNI para chamar o construtor
                IntPtr id_ctor_I = JNIEnv.GetMethodID(class_ref, "<init>", "()V");

                // JNI Criar uma instancia da classe
                IntPtr lrefInstance = JNIEnv.NewObject(class_ref, id_ctor_I);

                // JNI para localizar o metodo writeSysFile
                IntPtr id_write = JNIEnv.GetStaticMethodID(class_ref, "writeSysFile", "(Ljava/lang/String;Ljava/lang/String;)Z");

                // JNI para chamar o metodo writeSysFile e alterar os parametros do Led via sys
                _ = JNIEnv.CallStaticBooleanMethod(class_ref, id_write, new JValue[2] { new JValue(new Java.Lang.String("/sys/class/leds/aw9109_led/onoff_by_reg")), new JValue(new Java.Lang.String(cmd)) });
            
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}

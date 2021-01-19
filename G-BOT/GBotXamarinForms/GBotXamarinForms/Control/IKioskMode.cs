using System;
using System.Collections.Generic;
using System.Text;

namespace GBotXamarinForms
{
    public interface IKioskMode
    {
        void StartKioskMode();
        void StopKioskMode();
        Boolean StatusKioskMode();
    }
}

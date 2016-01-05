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
using Android.Views.Animations;

namespace XamarinViewPager
{
    public class DefaultInterpolator : Java.Lang.Object, IInterpolator
    {
        public float GetInterpolation(float input)
        {
            input -= 1.0f;
            return input * input * input + 1.0f;
        }
    }
}
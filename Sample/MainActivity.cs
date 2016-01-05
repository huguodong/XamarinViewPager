using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamarinViewPager;
using Android.Support.V4.View;
using Android.Graphics;

namespace Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private XamarinViewPager.XamarinViewPager mXvp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            SetupXamarinViewPager(TransitionEffect.Tablet);
        }

        private void SetupXamarinViewPager(TransitionEffect effect)
        {
            mXvp = FindViewById<XamarinViewPager.XamarinViewPager>(Resource.Id.jazzy_pager);
            mXvp.TransitionEffect = effect;
            mXvp.Adapter = new MainAdapter(this, mXvp);
            mXvp.PageMargin = 30;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            int i = 0;
            menu.Add(0, -1, 0, "Toggle Fade");
            String[] effects = Resources.GetStringArray(Resource.Array.Xamarin_effects);
            foreach (string effect in effects)
            {
                menu.Add(0, i++, 0, effect);
            }
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case -1:
                    {
                        mXvp.FadeEnable = !mXvp.FadeEnable;
                    }
                    break;
                default:
                    {
                        SetupXamarinViewPager((TransitionEffect)item.ItemId);
                    }
                    break;
            }
            return true;
        }

        private class MainAdapter : PagerAdapter
        {
            private Context mainContext;
            private XamarinViewPager.XamarinViewPager mViewPager;

            public MainAdapter(Context context,XamarinViewPager.XamarinViewPager viewPager)
            {
                mainContext = context;
                mViewPager = viewPager;
            }

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                TextView text = new TextView(mainContext);
                text.Gravity = GravityFlags.Center;
                text.TextSize = 30;
                text.SetTextColor(Color.White);
                text.Text = "Page " + position;
                text.SetPadding(30, 30, 30, 30);
                Random r = new Random();
                Color bg = Color.Rgb((int)Math.Floor(r.NextDouble() * 128) + 64,
                    (int)Math.Floor(r.NextDouble() * 128) + 64,
                    (int)Math.Floor(r.NextDouble() * 128) + 64);
                text.SetBackgroundColor(bg);
                container.AddView(text, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                mViewPager.SetObjectForPosition(text, position);
                return text;
            }

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
            {
                container.RemoveView(mViewPager.FindViewFromObject(position));
            }

            public override int Count
            {
                get { return 10; }
            }

            public override bool IsViewFromObject(View view, Java.Lang.Object @object)
            {
                if (view is OutlineContainer)
                {
                    return ((OutlineContainer)view).GetChildAt(0) == @object;
                }
                else
                {
                    return view == @object;
                }
            }
        }
    }
}


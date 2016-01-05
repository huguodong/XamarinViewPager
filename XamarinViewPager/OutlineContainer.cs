using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinViewPager
{
    public class OutlineContainer : FrameLayout, IAnimatable
    {
        private Paint mOutlinePaint;

        private bool mIsRunning = false;
        private long mStartTime;
        private float mAlpha = 1f;
        private const long ANIMATION_DURATION = 500;
        private const long FRAME_DURATION = 1000 / 60;
        private IInterpolator mInterpolator = new DefaultInterpolator();
        private Action mUpdater;

        public OutlineContainer(Context context)
            : this(context, null) { }

        public OutlineContainer(Context context, IAttributeSet attrs)
            : this(context, attrs, 0) { }

        public OutlineContainer(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init();
        }

        private int Dp2Px(int dp)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
        }

        private void Init()
        {
            mOutlinePaint = new Paint();
            mOutlinePaint.AntiAlias = true;
            mOutlinePaint.StrokeWidth = Dp2Px(10);
            mOutlinePaint.SetStyle(Paint.Style.Stroke);
            mUpdater = () =>
            {
                long now = AnimationUtils.CurrentAnimationTimeMillis();
                long duration = now - mStartTime;
                if (duration >= ANIMATION_DURATION)
                {
                    mAlpha = 0f;
                    Invalidate();
                    Stop();
                    return;
                }
                else
                {
                    mAlpha = mInterpolator.GetInterpolation(1 - duration / (float)ANIMATION_DURATION);
                    Invalidate();
                }
                PostDelayed(mUpdater, FRAME_DURATION);
            };

            int padding = Dp2Px(10);
            SetPadding(padding, padding, padding, padding);
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);
            int offset = Dp2Px(5);
            if (mOutlinePaint.Color != XamarinViewPager.OutlineColor)
            {
                mOutlinePaint.Color = XamarinViewPager.OutlineColor;
            }
            mOutlinePaint.Alpha = (int)mAlpha * 255;
            Rect rect = new Rect(offset, offset, MeasuredWidth - offset, MeasuredHeight - offset);
            canvas.DrawRect(rect, mOutlinePaint);
        }

        public void SetOutlineAlpha(float alpha)
        {
            mAlpha = alpha;
        }

        #region IAnimatable

        public bool IsRunning
        {
            get
            {
                return mIsRunning;
            }
        }

        public void Start()
        {
            if (mIsRunning)
                return;
            mIsRunning = true;
            mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
            Post(mUpdater);
        }

        public void Stop()
        {
            if (!IsRunning)
                return;
            mIsRunning = false;
        }

        #endregion
    }
}

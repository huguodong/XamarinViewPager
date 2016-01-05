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
using Android.Support.V4.View;
using Android.Graphics;
using Android.Util;
using Android.Content.Res;

namespace XamarinViewPager
{
    public class XamarinViewPager : ViewPager
    {
        public static String TAG = "XamarinViewPager";

        private Dictionary<int, Java.Lang.Object> mObjs = new Dictionary<int, Java.Lang.Object>();

        private static float SCALE_MAX = 0.5f;
        private static float ZOOM_MAX = 0.5f;
        private static float ROT_MAX = 15.0f;

        private bool mOutlineEnabled = false;
        public bool OutlineEnable
        {
            get
            {
                return mOutlineEnabled;
            }
            set
            {
                mOutlineEnabled = value;
                WrapWithOutlines();
            }
        }
        public static Color OutlineColor { get; set; }
        public bool FadeEnable { get; set; }
        public bool PagingEnabled { get; set; }
        public TransitionEffect TransitionEffect { get; set; }

        static XamarinViewPager()
        {
            OutlineColor = Color.Wheat;
        }
            

        public XamarinViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            SetClipChildren(false);

            PagingEnabled = true;

            TypedArray ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.XamarinViewPager);
            int effect = ta.GetInt(Resource.Styleable.XamarinViewPager_style, 0);
            String[] transitions = Resources.GetStringArray(Resource.Array.Xamarin_effects);
            TransitionEffect = (global::XamarinViewPager.TransitionEffect)effect;
            FadeEnable = ta.GetBoolean(Resource.Styleable.XamarinViewPager_fadeEnabled, false);

        }

        private void WrapWithOutlines()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                View v = GetChildAt(i);
                if (!(v is OutlineContainer))
                {
                    RemoveView(v);
                    base.AddView(WrapChild(v), i);
                }
            }
        }

        private View WrapChild(View v)
        {
            if(!OutlineEnable|| v is OutlineContainer)
            {
                return v;
            }
            OutlineContainer oc = new OutlineContainer(Context);
            oc.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
            oc.AddView(v);
            return oc;
        }

        public override void AddView(View child)
        {
            base.AddView(WrapChild(child));
        }

        public override void AddView(View child, int index)
        {
            base.AddView(WrapChild(child), index);
        }

        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
        {
            base.AddView(WrapChild(child), index, @params);
        }

        public override void AddView(View child, int width, int height)
        {
            base.AddView(WrapChild(child), width, height);
        }

        public override void AddView(View child, ViewGroup.LayoutParams @params)
        {
            base.AddView(WrapChild(child), @params);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return Enabled ? base.OnInterceptTouchEvent(ev) : false;
        }

        private enum State
        {
            IDLE,
            GOING_LEFT,
            GOING_RIGHT
        }

        private State mState;
        private int oldPage;
        private View mLeft;
        private View mRight;
        private float mRot;
        private float mTrans;
        private float mScale;

        protected virtual void AnimateScrol(int position,float positionOffset)
        {
            if(mState != State.IDLE)
            {
                mRot = (float)(1 - Math.Cos(2 * Math.PI * positionOffset)) / 2 * 30f;
                this.RotationY = mState == State.GOING_RIGHT ? mRot : -mRot;
                this.PivotX = MeasuredWidth * 0.5f;
                this.PivotY = MeasuredHeight * 0.5f;
            }
        }

        protected virtual void AnimateTablet(View left,View right,float positionOffset)
        {
            if(mState != State.IDLE)
            {
                if(left != null)
                {
                    ManageLayer(left, true);
                    mRot = 30f * positionOffset;
                    mTrans = GetOffsetXForRotation(mRot, left.MeasuredWidth,
                        left.MeasuredHeight);
                    left.PivotX = left.MeasuredWidth / 2;
                    left.PivotY = left.MeasuredHeight / 2;
                    left.TranslationX = mTrans;
                    left.RotationY = mRot;
                }
                if(right != null)
                {
                    ManageLayer(right, true);
                    mRot = -30f * (1 - positionOffset);
                    mTrans = GetOffsetXForRotation(mRot, right.MeasuredWidth,
                        MeasuredHeight);
                    right.PivotX = right.MeasuredWidth * 0.5f;
                    right.PivotY = right.MeasuredHeight * 0.5f;
                    right.TranslationX = mTrans;
                    right.RotationY = mRot;
                }
            }
        }

        protected virtual void AnimateCube(View left,View right,float positionOffset,bool sn)
        {
            if(mState != State.IDLE)
            {
                if(left != null)
                {
                    ManageLayer(left, true);
                    mRot = (sn ? 90f : -90f) * positionOffset;
                    left.PivotX = left.MeasuredWidth;
                    left.PivotY = left.MeasuredHeight * 0.5f;
                    left.RotationY = mRot;
                }
                if(right != null)
                {
                    ManageLayer(right, true);
                    mRot = -(sn ? 90f : -90f) * (1 - positionOffset);
                    right.PivotX = 0;
                    right.PivotY = right.MeasuredHeight * 0.5f;
                    right.RotationY = mRot;
                }
            }
        }

        protected virtual void AnimateAccordion(View left,View right,float positionOffset)
        {
            if(mState != State.IDLE)
            {
                if (left != null)
                {
                    ManageLayer(left, true);
                    left.PivotX = left.MeasuredWidth;
                    left.PivotY = 0;
                    left.ScaleX = 1 - positionOffset;
                }
                if (right != null)
                {
                    ManageLayer(right, true);
                    right.PivotX = 0;
                    right.PivotY = 0;
                    right.ScaleX = positionOffset;
                }
            }
        }

        protected virtual void AnimateZoom(View left,View right,float positionOffset,bool sn)
        {
            if (mState != State.IDLE)
            {
                if (left != null)
                {
                    ManageLayer(left, true);
                    mScale = sn ? ZOOM_MAX + (1 - ZOOM_MAX) * (1 + positionOffset) :
                        1 + ZOOM_MAX - ZOOM_MAX * (1 - positionOffset);
                    left.PivotX = left.MeasuredWidth * 0.5f;
                    left.PivotY = left.MeasuredHeight * 0.5f;
                    left.ScaleX = mScale;
                    left.ScaleY = mScale;
                }
                if(right != null)
                {
                    ManageLayer(right, true);
                    mScale = sn ? ZOOM_MAX + (1 - ZOOM_MAX) * positionOffset : 1 + ZOOM_MAX - ZOOM_MAX * positionOffset;
                    right.PivotX = right.MeasuredWidth * 0.5f;
                    right.PivotY = right.MeasuredHeight * 0.5f;
                    right.ScaleX = mScale;
                    right.ScaleY = mScale;
                }
            }
        }

        protected virtual void AnimateRotate(View left,View right,float positionOffset,bool up)
        {
            if (mState != State.IDLE)
            {
                if (left != null)
                {
                    ManageLayer(left, true);
                    mRot = (up ? 1 : -1) * (ROT_MAX * positionOffset);
                    mTrans = (up ? -1 : 1) * (float)(MeasuredHeight - MeasuredHeight * Math.Cos(mRot * Math.PI / 180f));
                    left.PivotX = left.MeasuredWidth * 0.5f;
                    left.PivotY = up ? 0 : left.MeasuredHeight;
                    left.TranslationY = mTrans;
                    left.Rotation = mRot;
                }
                if(right != null)
                {
                    ManageLayer(right, true);
                    mRot = (up ? 1 : -1) * (-ROT_MAX + ROT_MAX * positionOffset);
                    mTrans = (up ? -1 : 1) * (float)(MeasuredHeight - MeasuredHeight * Math.Cos(mRot * Math.PI / 180f));
                    right.PivotX = right.MeasuredWidth * 0.5f;
                    right.PivotY = up ? 0 : right.MeasuredHeight;
                    right.TranslationY = mTrans;
                    right.Rotation = mRot;
                }
            }
        }

        protected virtual void AnimateFlipHorizontal(View left,View right,float positionOffset,int positionOffsetPixels)
        {
            if (mState != State.IDLE)
            {
                if (left != null)
                {
                    ManageLayer(left, true);
                    mRot = 180f * positionOffset;
                    if (mRot > 90f)
                    {
                        left.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        if(left.Visibility == ViewStates.Invisible)
                        {
                            left.Visibility = ViewStates.Visible;
                        }
                        mTrans = positionOffsetPixels;
                        left.PivotX = left.MeasuredWidth * 0.5f;
                        left.PivotY = left.MeasuredHeight * 0.5f;
                        left.TranslationX = mTrans;
                        left.RotationY = mRot;
                    }
                }
                if(right != null)
                {
                    ManageLayer(right, true);
                    mRot = -180f * (1 - positionOffset);
                    if (mRot < -90f)
                    {
                        right.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        if (right.Visibility == ViewStates.Invisible)
                        {
                            right.Visibility = ViewStates.Visible;
                        }
                        mTrans = -Width - PageMargin + positionOffsetPixels;
                        right.PivotX = MeasuredWidth * 0.5f;
                        right.PivotY = MeasuredHeight * 0.5f;
                        right.TranslationX = mTrans;
                        right.RotationY = mRot;
                    }
                }
            }
        }

        protected virtual void AnimateFlipVertical(View left,View right,float positionOffset,int positionOffsetPixels)
        {
            if (mState != State.IDLE)
            {
                if (left != null)
                {
                    ManageLayer(left, true);
                    mRot = 180f * positionOffset;
                    if(mRot > 90f)
                    {
                        left.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        if (left.Visibility == ViewStates.Invisible)
                        {
                            left.Visibility = ViewStates.Visible;
                        }
                        mTrans = positionOffsetPixels;
                        left.PivotX = left.MeasuredWidth * 0.5f;
                        left.PivotY = left.MeasuredHeight * 0.5f;
                        left.TranslationX = mTrans;
                        left.RotationX = mRot;
                    }
                }
                if (right != null)
                {
                    ManageLayer(right, true);
                    mRot = -180f * (1 - positionOffset);
                    if (mRot < -90f)
                    {
                        right.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        if (right.Visibility == ViewStates.Invisible)
                        {
                            right.Visibility = ViewStates.Visible;
                        }
                        mTrans = -Width - PageMargin + positionOffsetPixels;
                        right.PivotX = right.MeasuredWidth * 0.5f;
                        right.PivotY = right.MeasuredHeight * 0.5f;
                        right.TranslationX = mTrans;
                        right.RotationX = mRot;
                    }
                }
            }
        }

        protected virtual void AnimateStack(View left,View right,float positionOffset,int positionOffsetPixels)
        {
            if(mState != State.IDLE)
            {
                if (right != null)
                {
                    ManageLayer(right, true);
                    mScale = (1 - SCALE_MAX) * positionOffset + SCALE_MAX;
                    mTrans = -Width - PageMargin + positionOffsetPixels;
                    right.ScaleX = mScale;
                    right.ScaleY = mScale;
                    right.TranslationX = mTrans;
                }
                if (left != null)
                {
                    left.BringToFront();
                }
            }
        }

        private void ManageLayer(View left, bool p)
        {
            LayerType layerType = p ? LayerType.Hardware : LayerType.None;
            if (layerType != left.LayerType)
            {
                left.SetLayerType(layerType, null);
            }
        }

        private void DisableHardwareLayer()
        {
            View v = null;
            for (int i = 0; i < ChildCount; i++)
            {
                v = GetChildAt(i);
                if (v.LayerType != Android.Views.LayerType.None)
                {
                    v.SetLayerType(Android.Views.LayerType.None, null);
                }
            }
        }

        private Matrix mMatrix = new Matrix();
        private Camera mCamera = new Camera();
        private float[] mTempFloat2 = new float[2];

        protected virtual float GetOffsetXForRotation(float degress,int width,int height)
        {
            mMatrix.Reset();
            mCamera.Save();
            mCamera.RotateY(Math.Abs(degress));
            mCamera.GetMatrix(mMatrix);
            mCamera.Restore();

            mMatrix.PreTranslate(-width * 0.5f, -height * 0.5f);
            mMatrix.PostTranslate(width * 0.5f, height * 0.5f);
            mTempFloat2[0] = width;
            mTempFloat2[1] = height;
            mMatrix.MapPoints(mTempFloat2);
            return (width - mTempFloat2[0]) * (degress > 0f ? 1f : -1f);
        }

        protected virtual void AnimateFade(View left, View right, float positionOffset)
        {
            if (left != null)
            {
                left.Alpha = 1 - positionOffset;
            }
            if (right != null)
            {
                right.Alpha = positionOffset;
            }
        }

        protected virtual void AnimateOutline(View left, View right)
        {
            if (!(left is OutlineContainer))
                return;
            if (mState != State.IDLE)
            {
                if (left != null)
                {
                    ManageLayer(left, true);
                    ((OutlineContainer)left).SetOutlineAlpha(1f);
                }
                if (right != null)
                {
                    ManageLayer(right, true);
                    ((OutlineContainer)right).SetOutlineAlpha(1f);
                }
            }
            else
            {
                if (left != null)
                {
                    ((OutlineContainer)left).Start();
                }
                if (right != null)
                {
                    ((OutlineContainer)right).Start();
                }
            }
        }

        protected override void OnPageScrolled(int position, float offset, int offsetPixels)
        {
            if (mState == State.IDLE && offset > 0)
            {
                oldPage = CurrentItem;
                mState = position == oldPage ? State.GOING_RIGHT : State.GOING_LEFT;
            }
            bool goingRight = position == oldPage;
            if (mState == State.GOING_RIGHT && !goingRight)
            {
                mState = State.GOING_LEFT;
            }
            else if (mState == State.GOING_LEFT && goingRight)
            {
                mState = State.GOING_RIGHT;
            }

            float effectOffset = IsSmall(offset) ? 0 : offset;

            mLeft = FindViewFromObject(position);
            mRight = FindViewFromObject(position + 1);

            if (FadeEnable)
            {
                AnimateFade(mLeft, mRight, effectOffset);
            }
            if (OutlineEnable)
            {
                AnimateOutline(mLeft, mRight);
            }

            switch (TransitionEffect)
            {
                case global::XamarinViewPager.TransitionEffect.Standard:
                    break;
                case global::XamarinViewPager.TransitionEffect.Tablet:
                    {
                        AnimateTablet(mLeft, mRight, effectOffset);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.CubeIn:
                    {
                        AnimateCube(mLeft, mRight, effectOffset, true);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.CubeOut:
                    {
                        AnimateCube(mLeft, mRight, effectOffset, false);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.FlipVertical:
                    {
                        AnimateFlipVertical(mLeft, mRight, offset, offsetPixels);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.FlipHouizontal:
                    {
                        AnimateFlipHorizontal(mLeft, mRight, effectOffset, offsetPixels);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.Stack:
                    {
                        AnimateStack(mLeft, mRight, effectOffset, offsetPixels);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.ZoomIn:
                    {
                        AnimateZoom(mLeft, mRight, effectOffset, true);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.ZoomOut:
                    {
                        AnimateZoom(mLeft, mRight, effectOffset, false);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.RotateUp:
                    {
                        AnimateRotate(mLeft, mRight, effectOffset, true);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.RotateDown:
                    {
                        AnimateRotate(mLeft, mRight, effectOffset, false);
                    }
                    break;
                case global::XamarinViewPager.TransitionEffect.Accordion:
                    {
                        AnimateAccordion(mLeft, mRight, effectOffset);
                    }
                    break;
            }

            base.OnPageScrolled(position, offset, offsetPixels);

            if (effectOffset == 0)
            {
                DisableHardwareLayer();
                mState = State.IDLE;
            }
        }

        public View FindViewFromObject(int position)
        {
            Java.Lang.Object o = null;
            mObjs.TryGetValue(position, out o);
            if (o == null)
                return null;

            PagerAdapter a = Adapter;
            View v = null;
            for (int i = 0; i < ChildCount; i++)
            {
                v = GetChildAt(i);
                if (a.IsViewFromObject(v, o))
                {
                    return v;
                }
            }
            return null;
        }

        private bool IsSmall(float offset)
        {
            return Math.Abs(offset) < 0.0001;
        }

        public void SetObjectForPosition(Java.Lang.Object obj, int position)
        {
            if (mObjs.ContainsKey(position))
            {
                mObjs[position] = obj;
            }
            else
            {
                mObjs.Add(position, obj);
            }
        }
    }
}
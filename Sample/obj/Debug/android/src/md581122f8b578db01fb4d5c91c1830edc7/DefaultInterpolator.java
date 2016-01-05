package md581122f8b578db01fb4d5c91c1830edc7;


public class DefaultInterpolator
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.animation.Interpolator,
		android.animation.TimeInterpolator
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_getInterpolation:(F)F:GetGetInterpolation_FHandler:Android.Views.Animations.IInterpolatorInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("XamarinViewPager.DefaultInterpolator, XamarinViewPager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DefaultInterpolator.class, __md_methods);
	}


	public DefaultInterpolator () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DefaultInterpolator.class)
			mono.android.TypeManager.Activate ("XamarinViewPager.DefaultInterpolator, XamarinViewPager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public float getInterpolation (float p0)
	{
		return n_getInterpolation (p0);
	}

	private native float n_getInterpolation (float p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

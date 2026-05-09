using System.Windows.Media;

namespace GoldenAnvil.Utility.Windows
{
	public static class DrawingContextUtility
	{
		public static Scope ScopedTransform(this DrawingContext context, Transform transform)
		{
			context.PushTransform(transform);
			return Scope.Create(context.Pop);
		}

		public static Scope ScopedClip(this DrawingContext context, Geometry clip)
		{
			context.PushClip(clip);
			return Scope.Create(context.Pop);
		}

		public static Scope ScopedGuidelineSet(this DrawingContext context, GuidelineSet guidelineSet)
		{
			context.PushGuidelineSet(guidelineSet);
			return Scope.Create(context.Pop);
		}

		public static Scope ScopedOpacity(this DrawingContext context, double opacity)
		{
			context.PushOpacity(opacity);
			return Scope.Create(context.Pop);
		}
	}
}

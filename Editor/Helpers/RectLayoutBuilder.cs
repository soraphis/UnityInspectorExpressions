using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Helpers
{
	public interface IRectLayoutBuilder
	{
		public void ConditionalFlex(bool isFlex, int flex, float minWidth)
		{
			if (isFlex)
			{
				Flex(flex, minWidth);
			}
			else
			{
				Container(minWidth);
			}
		}
		public void Flex(int flex, float minWidth = 0);
		public void Container(float width);
	}

	public ref struct RectLayout
	{
		private readonly RectLayoutBuild _layoutBuilder;
		private readonly Span<Rect>      _output;

		public RectLayout(RectLayoutBuild layoutBuilder, Span<Rect> output)
		{
			_layoutBuilder = layoutBuilder;
			_output = output;
		}

		public void Flex(int flex, float minWidth = 0)
		{
			_layoutBuilder.Flex(flex, minWidth);
		}

		public void Container(float width)
		{
			_layoutBuilder.Container(width);
		}

		public void Dispose()
		{
			_layoutBuilder.GetRects(_output);
		}
	}

	public class RectLayoutBuild : IRectLayoutBuilder
	{
		private struct Def
		{
			public float width;
			public int flexWeight;
		}

		Rect total;
		List<Def> definitions = new List<Def>();

		public void Flex(int flex, float minWidth = 0)
		{
			definitions.Add(new Def { width = minWidth, flexWeight = flex });
		}

		public void Container(float width)
		{
			definitions.Add(new Def { width = width, flexWeight = 0 });
		}

		internal void Clear(Rect self)
		{
			total = new Rect(self);
			definitions.Clear();
		}
		internal void GetRects(Span<Rect> output)
		{
			float afterContainerWidth = total.width;
			int totalFlexWeight = 0;
			foreach (var l in definitions)
			{
				totalFlexWeight += l.flexWeight;
				afterContainerWidth -= l.width;
			}

			float lastX = 0;
			for (int i = 0; i < output.Length && i < definitions.Count; ++i)
			{
				var def = definitions[i];
				if (def.flexWeight == 0)
				{
					output[i] = new Rect(total) { x = total.x + lastX, width = def.width };
					lastX += def.width;
					continue;
				}

				var flex = (float)def.flexWeight/totalFlexWeight;
				var width = afterContainerWidth * flex + + def.width;
				width = Mathf.Max(width, def.width);

				output[i] = new Rect(total) { x = total.x + lastX, width = width };
				lastX += width;
			}

		}
	}
}
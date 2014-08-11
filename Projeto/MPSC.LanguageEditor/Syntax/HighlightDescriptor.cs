using System;
using System.Drawing;

namespace MPSC.LanguageEditor.Syntax
{
	public class HighlightDescriptor
	{
		public readonly Color Color;
		public readonly Font Font;
		public readonly String Token;
		public readonly String CloseToken;
		public readonly DescriptorType DescriptorType;
		public readonly DescriptorRecognition DescriptorRecognition;
		public readonly Boolean UseForAutoComplete;

		public HighlightDescriptor(String token, Color color, Font font, DescriptorType descriptorType, DescriptorRecognition descriptorRecognition, Boolean useForAutoComplete)
			: this(token, null, color, font, descriptorType, descriptorRecognition, useForAutoComplete)
		{
			if (descriptorType == DescriptorType.ToCloseToken)
				throw new ArgumentException("You may not choose ToCloseToken DescriptorType without specifing an end token.");
		}

		public HighlightDescriptor(String token, String closeToken, Color color, Font font, DescriptorType descriptorType, DescriptorRecognition descriptorRecognition, Boolean useForAutoComplete)
		{
			Token = token;
			CloseToken = closeToken;
			Color = color;
			Font = font;
			DescriptorType = descriptorType;
			DescriptorRecognition = descriptorRecognition;
			UseForAutoComplete = useForAutoComplete;
		}
	}

	public enum DescriptorType
	{
		/// <summary>
		/// Causes the highlighting of a single word
		/// </summary>
		Word,
		/// <summary>
		/// Causes the entire line from this point on the be highlighted, regardless of other tokens
		/// </summary>
		ToEOL,
		/// <summary>
		/// Highlights all text until the end token;
		/// </summary>
		ToCloseToken
	}

	public enum DescriptorRecognition
	{
		/// <summary>
		/// Only if the whole token is equal to the word
		/// </summary>
		WholeWord,
		/// <summary>
		/// If the word starts with the token
		/// </summary>
		StartsWith,
		/// <summary>
		/// If the word contains the Token
		/// </summary>
		Contains
	}
}
﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;
using SqlEditor.SearchAndReplace.Engine.TextIterator;
using Utilities.Forms.Dialogs;
using Application = System.Windows.Forms.Application;

namespace SqlEditor.SearchAndReplace.Engine.SearchStrategy
{
	public class RegExSearchStrategy : ISearchStrategy
	{
		Regex regex = null;
		
		public bool CompilePattern()
		{
			RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.Multiline;
			if (!SearchOptions.MatchCase) {
				regexOptions |= RegexOptions.IgnoreCase;
			}
			try {
				regex = new Regex(SearchOptions.FindPattern, regexOptions);
				return true;
			} catch (ArgumentException ex)
			{
			    Dialog.ShowErrorDialog(Application.ProductName, "Invalid regular expression.", ex.Message, ex.StackTrace);
				return false;
			}
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator)
		{
			string document = textIterator.TextBuffer.GetText(0, textIterator.TextBuffer.Length);
			
			while (textIterator.MoveAhead(1)) {
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || !m.Success) {
					while (textIterator.Position < document.Length - 1) {
						if (!textIterator.MoveAhead(1))
							return null;
					}
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						return new RegexSearchResult(m);
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator, int offset, int length)
		{
			string document = textIterator.TextBuffer.GetText(0, textIterator.TextBuffer.Length);
			
			while (textIterator.MoveAhead(1) && TextSelection.IsInsideRange(textIterator.Position, offset, length)) {
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || !m.Success) {
					while (textIterator.Position < document.Length - 1) {
						if (!textIterator.MoveAhead(1))
							return null;
					}
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						if (TextSelection.IsInsideRange(m.Index + m.Length - 1, offset, length)) {
							return new RegexSearchResult(m);
						} else {
							return null;
						}
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
		
		private sealed class RegexSearchResult : SearchResultMatch
		{
			Match m;
			
			internal RegexSearchResult(Match m) : base(m.Index, m.Length)
			{
				this.m = m;
			}
			
			public override string TransformReplacePattern(string pattern)
			{
				return m.Result(pattern);
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using System.Diagnostics;
using System.IO;
using SqlEditor.Properties;
using Utilities.Forms.Dialogs;
using Utilities.Text;
using log4net;

namespace SqlEditor
{
    public partial class FrmFindReplaceSimple : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<TextEditorControl, HighlightGroup> _highlightGroups = new Dictionary<TextEditorControl, HighlightGroup>();
        private bool _lastSearchWasBackward;
        private bool _lastSearchLoopedAround;

        public FrmFindReplaceSimple()
        {
            InitializeComponent();
            _search = new TextEditorSearcher();

            LoadSettings();
        }

        private void LoadSettings()
        {
            _log.Debug("Loading for geometry ...");
            if (!Settings.Default.FrmFindReplace_Geometry.IsNullEmptyOrWhitespace())
            {
                try
                {
                    Utilities.Forms.RestoreFormPosition.GeometryFromString(Settings.Default.FrmFindReplace_Geometry, this);
                }
                catch (Exception ex)
                {
                    _log.Error("Error loading form geometry.");
                    _log.Error(ex.Message, ex);
                    throw;
                }
            }
            else
            {
                _log.Warn("Form geometry is not set.");
            }
        }

        readonly TextEditorSearcher _search;
        TextEditorControl _editor;
        private TextEditorControl Editor
        {
            set
            {
                _editor = value;
                _search.Document = _editor.Document;
                UpdateTitleBar();
            }
        }

        private void UpdateTitleBar()
        {
            var text = ReplaceMode ? "Find & Replace" : "Find";
            if (_editor != null && _editor.FileName != null)
                text += " - " + Path.GetFileName(_editor.FileName);
            if (_search.HasScanRegion)
                text += " (selection only)";
            this.Text = text;
        }

        public void ShowFor(TextEditorControl editor, bool replaceMode)
        {
            Editor = editor;

            _search.ClearScanRegion();
            var sm = editor.ActiveTextAreaControl.SelectionManager;
            if (sm.HasSomethingSelected && sm.SelectionCollection.Count == 1)
            {
                var sel = sm.SelectionCollection[0];
                if (sel.StartPosition.Line == sel.EndPosition.Line)
                    _tbSearchTerm.Text = sm.SelectedText;
                else
                    _search.SetScanRegion(sel);
            }
            else
            {
                // Get the current word that the caret is on
                Caret caret = editor.ActiveTextAreaControl.Caret;
                int start = TextUtilities.FindWordStart(editor.Document, caret.Offset);
                int endAt = TextUtilities.FindWordEnd(editor.Document, caret.Offset);
                _tbSearchTerm.Text = editor.Document.GetText(start, endAt - start);
            }

            ReplaceMode = replaceMode;

            this.Owner = (Form)editor.TopLevelControl;
            this.Show();

            _tbSearchTerm.SelectAll();
            _tbSearchTerm.Focus();
        }

        public bool ReplaceMode
        {
            get { return _tbReplacementText.Visible; }
            set
            {
                btnReplace.Visible = btnReplaceAll.Visible = value;
                lblReplaceWith.Visible = _tbReplacementText.Visible = value;
                btnHighlightAll.Visible = !value;
                this.AcceptButton = value ? btnReplace : btnFindNext;
                UpdateTitleBar();
            }
        }

        private void BtnFindPrevious_Click(object sender, EventArgs e)
        {
            FindNext(false, true, "Text not found");
        }
        private void BtnFindNext_Click(object sender, EventArgs e)
        {
            FindNext(false, false, "Text not found");
        }



        public TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
        {
            _lblStatus.Text = string.Empty;
            _lblStatus.ForeColor = Color.Blue;

            RemoveHighlighting();
            if (string.IsNullOrEmpty(_tbSearchTerm.Text))
            {
                _lblStatus.ForeColor = Color.Red;
                _lblStatus.Text = "Nothing specified to look for.";
                return null;
            }

            _lastSearchWasBackward = searchBackward;
            _search.LookFor = _tbSearchTerm.Text;
            _search.MatchCase = chkMatchCase.Checked;
            _search.MatchWholeWordOnly = chkMatchWholeWord.Checked;

            _editor.Focus();
            var caret = _editor.ActiveTextAreaControl.Caret;
            if (viaF3 && _search.HasScanRegion && !caret.Offset.
                IsInRange(_search.BeginOffset, _search.EndOffset))
            {
                // user moved outside of the originally selected region
                _search.ClearScanRegion();
                UpdateTitleBar();
            }

            var startFrom = caret.Offset - (searchBackward ? 1 : 0);
            var range = _search.FindNext(startFrom, searchBackward, out _lastSearchLoopedAround);
            if (range != null)
                SelectResult(range);
            else if (messageIfNotFound != null)
            {
                //Dialog.ShowDialog(Application.ProductName, messageIfNotFound, string.Empty);
                _lblStatus.ForeColor = Color.Red;
                _lblStatus.Text = messageIfNotFound;
            }
            Focus();
            return range;
        }

        private void SelectResult(TextRange range)
        {
            var p1 = _editor.Document.OffsetToPosition(range.Offset);
            var p2 = _editor.Document.OffsetToPosition(range.Offset + range.Length);
            _editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
            _editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
            // Also move the caret to the end of the selection, because when the user 
            // presses F3, the caret is where we start searching next time.
            _editor.ActiveTextAreaControl.Caret.Position =
                _editor.Document.OffsetToPosition(range.Offset + range.Length);
        }



        private void BtnHighlightAll_Click(object sender, EventArgs e)
        {
            if (!_highlightGroups.ContainsKey(_editor))
                _highlightGroups[_editor] = new HighlightGroup(_editor);
            var group = _highlightGroups[_editor];

            if (string.IsNullOrEmpty(LookFor))
                // Clear highlights
                group.ClearMarkers();
            else
            {
                _search.LookFor = _tbSearchTerm.Text;
                _search.MatchCase = chkMatchCase.Checked;
                _search.MatchWholeWordOnly = chkMatchWholeWord.Checked;

                int offset = 0, count = 0;
                for (; ; )
                {
                    bool looped;
                    TextRange range = _search.FindNext(offset, false, out looped);
                    if (range == null || looped)
                        break;
                    offset = range.Offset + range.Length;
                    count++;

                    var m = new TextMarker(range.Offset, range.Length,
                            TextMarkerType.SolidBlock, Color.Yellow, Color.Black);
                    group.AddMarker(m);
                }
                if (count == 0)
                {
                    Dialog.ShowDialog(Application.ProductName, "Search text not found.", string.Empty);
                }
                else
                {
                    _editor.Refresh();
                }
            }
        }

        private void RemoveHighlighting()
        {
            if (!_highlightGroups.ContainsKey(_editor))
                _highlightGroups[_editor] = new HighlightGroup(_editor);
            HighlightGroup group = _highlightGroups[_editor];
            group.ClearMarkers();
        }

        private void FindAndReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {	// Prevent dispose, as this form can be re-used
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                if (this.Owner != null)
                    this.Owner.Select(); // prevent another app from being activated instead

                e.Cancel = true;
                Hide();

                // Discard search region
                _search.ClearScanRegion();
                _editor.Refresh(); // must repaint manually
            }
            else
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                Settings.Default.FrmFindReplace_Geometry = Utilities.Forms.RestoreFormPosition.GeometryToString(this);
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                _log.Error("Error saving for geometry.");
                _log.Error(ex.Message, ex);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            var sm = _editor.ActiveTextAreaControl.SelectionManager;
            if (string.Equals(sm.SelectedText, _tbSearchTerm.Text, StringComparison.OrdinalIgnoreCase))
                InsertText(_tbReplacementText.Text);
            FindNext(false, _lastSearchWasBackward, "Text not found.");
        }

        private void BtnReplaceAll_Click(object sender, EventArgs e)
        {
            var count = 0;
            // BUG FIX: if the replacement string contains the original search string
            // (e.g. replace "red" with "very red") we must avoid looping around and
            // replacing forever! To fix, start replacing at beginning of region (by 
            // moving the caret) and stop as soon as we loop around.
            _editor.Focus();
            _editor.ActiveTextAreaControl.Caret.Position =
                _editor.Document.OffsetToPosition(_search.BeginOffset);

            _editor.Document.UndoStack.StartUndoGroup();
            try
            {
                while (FindNext(false, false, null) != null)
                {
                    if (_lastSearchLoopedAround)
                        break;

                    // Replace
                    count++;
                    InsertText(_tbReplacementText.Text);
                }
            }
            finally
            {
                _editor.Document.UndoStack.EndUndoGroup();
                Focus();
            }
            if (count == 0)
            {
                _lblStatus.ForeColor = Color.Red;
                _lblStatus.Text = "No occurrences found.";
                //Dialog.ShowDialog(Application.ProductName, "No occurrences found.", string.Empty);
            }
            else
            {
                _lblStatus.ForeColor = Color.Blue;
                _lblStatus.Text = string.Format("Replaced {0} occurrence{1}.", count, count > 1 ? "s" : string.Empty);
                //Dialog.ShowDialog(Application.ProductName, string.Format("Replaced {0} occurrence{1}.", count, count > 1 ? "s" : string.Empty), string.Empty);
                //Close();
            }
            Focus();
        }

        private void InsertText(string text)
        {
            var textArea = _editor.ActiveTextAreaControl.TextArea;
            textArea.Document.UndoStack.StartUndoGroup();
            try
            {
                if (textArea.SelectionManager.HasSomethingSelected)
                {
                    textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
                    textArea.SelectionManager.RemoveSelectedText();
                }
                textArea.InsertString(text);
            }
            finally
            {
                textArea.Document.UndoStack.EndUndoGroup();
            }
        }

        public string LookFor { get { return _tbSearchTerm.Text; } }

        private void TxtReplaceWith_Enter(object sender, EventArgs e)
        {
            _tbReplacementText.SelectAll();
        }

    }

    public class TextRange : AbstractSegment
    {
        public TextRange(int offset, int length)
        {
            this.offset = offset;
            this.length = length;
        }
    }

    /// <summary>This class finds occurrences of a search string in a text 
    /// editor's IDocument... it's like Find box without a GUI.</summary>
    public class TextEditorSearcher : IDisposable
    {
        IDocument _document;
        public IDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    ClearScanRegion();
                    _document = value;
                }
            }
        }

        // I would have used the TextAnchor class to represent the beginning and 
        // end of the region to scan while automatically adjusting to changes in 
        // the document--but for some reason it is sealed and its constructor is 
        // internal. Instead I use a TextMarker, which is perhaps even better as 
        // it gives me the opportunity to highlight the region. Note that all the 
        // markers and coloring information is associated with the text document, 
        // not the editor control, so TextEditorSearcher doesn't need a reference 
        // to the TextEditorControl. After adding the marker to the document, we
        // must remember to remove it when it is no longer needed.
        TextMarker _region;
        /// <summary>Sets the region to search. The region is updated 
        /// automatically as the document changes.</summary>
        public void SetScanRegion(ISelection sel)
        {
            SetScanRegion(sel.Offset, sel.Length);
        }
        /// <summary>Sets the region to search. The region is updated 
        /// automatically as the document changes.</summary>
        public void SetScanRegion(int offset, int length)
        {
            var bkgColor = _document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
            _region = new TextMarker(offset, length, TextMarkerType.SolidBlock,
                bkgColor.HalfMix(Color.FromArgb(160, 160, 160)));
            _document.MarkerStrategy.AddMarker(_region);
        }
        public bool HasScanRegion
        {
            get { return _region != null; }
        }
        public void ClearScanRegion()
        {
            if (_region != null)
            {
                _document.MarkerStrategy.RemoveMarker(_region);
                _region = null;
            }
        }
        public void Dispose() { ClearScanRegion(); GC.SuppressFinalize(this); }
        ~TextEditorSearcher() { Dispose(); }

        /// <summary>Begins the start offset for searching</summary>
        public int BeginOffset
        {
            get
            {
                if (_region != null)
                    return _region.Offset;
                else
                    return 0;
            }
        }
        /// <summary>Begins the end offset for searching</summary>
        public int EndOffset
        {
            get
            {
                if (_region != null)
                    return _region.EndOffset;
                else
                    return _document.TextLength;
            }
        }

        public bool MatchCase;

        public bool MatchWholeWordOnly;

        string _lookFor;
        string _lookFor2; // uppercase in case-insensitive mode
        public string LookFor
        {
            get { return _lookFor; }
            set { _lookFor = value; }
        }

        /// <summary>Finds next instance of LookFor, according to the search rules 
        /// (MatchCase, MatchWholeWordOnly).</summary>
        /// <param name="beginAtOffset">Offset in Document at which to begin the search</param>
        /// <param name="searchBackward">Whether to search backwards.</param>
        /// <param name="loopedAround">Whether to loop around.</param>
        /// <remarks>If there is a match at beginAtOffset precisely, it will be returned.</remarks>
        /// <returns>Region of document that matches the search string</returns>
        public TextRange FindNext(int beginAtOffset, bool searchBackward, out bool loopedAround)
        {
            Debug.Assert(!string.IsNullOrEmpty(_lookFor));
            loopedAround = false;

            int startAt = BeginOffset, endAt = EndOffset;
            int curOffs = beginAtOffset.InRange(startAt, endAt);

            _lookFor2 = MatchCase ? _lookFor : _lookFor.ToUpperInvariant();

            TextRange result;
            if (searchBackward)
            {
                result = FindNextIn(startAt, curOffs, true);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(curOffs, endAt, true);
                }
            }
            else
            {
                result = FindNextIn(curOffs, endAt, false);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(startAt, curOffs, false);
                }
            }
            return result;
        }

        private TextRange FindNextIn(int offset1, int offset2, bool searchBackward)
        {
            Debug.Assert(offset2 >= offset1);
            offset2 -= _lookFor.Length;

            // Make behavior decisions before starting search loop
            Func<char, char, bool> matchFirstCh;
            Func<int, bool> matchWord;
            if (MatchCase)
                matchFirstCh = (lookFor, c) => (lookFor == c);
            else
                matchFirstCh = (lookFor, c) => (lookFor == Char.ToUpperInvariant(c));
            if (MatchWholeWordOnly)
                matchWord = IsWholeWordMatch;
            else
                matchWord = IsPartWordMatch;

            // Search
            var lookForCh = _lookFor2[0];
            if (searchBackward)
            {
                for (var offset = offset2; offset >= offset1; offset--)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset))
                        && matchWord(offset))
                        return new TextRange(offset, _lookFor.Length);
                }
            }
            else
            {
                for (var offset = offset1; offset <= offset2; offset++)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset))
                        && matchWord(offset))
                        return new TextRange(offset, _lookFor.Length);
                }
            }
            return null;
        }
        private bool IsWholeWordMatch(int offset)
        {
            if (IsWordBoundary(offset) && IsWordBoundary(offset + _lookFor.Length))
                return IsPartWordMatch(offset);
            else
                return false;
        }
        private bool IsWordBoundary(int offset)
        {
            return offset <= 0 || offset >= _document.TextLength ||
                !IsAlphaNumeric(offset - 1) || !IsAlphaNumeric(offset);
        }
        private bool IsAlphaNumeric(int offset)
        {
            char c = _document.GetCharAt(offset);
            return Char.IsLetterOrDigit(c) || c == '_';
        }
        private bool IsPartWordMatch(int offset)
        {
            string substr = _document.GetText(offset, _lookFor.Length);
            if (!MatchCase)
                substr = substr.ToUpperInvariant();
            return substr == _lookFor2;
        }
    }

    /// <summary>Bundles a group of markers together so that they can be cleared 
    /// together.</summary>
    public class HighlightGroup : IDisposable
    {
        readonly List<TextMarker> _markers = new List<TextMarker>();
        readonly TextEditorControl _editor;
        readonly IDocument _document;
        public HighlightGroup(TextEditorControl editor)
        {
            _editor = editor;
            _document = editor.Document;
        }
        public void AddMarker(TextMarker marker)
        {
            _markers.Add(marker);
            _document.MarkerStrategy.AddMarker(marker);
        }
        public void ClearMarkers()
        {
            foreach (TextMarker m in _markers)
                _document.MarkerStrategy.RemoveMarker(m);
            _markers.Clear();
            _editor.Refresh();
        }
        public void Dispose() { ClearMarkers(); GC.SuppressFinalize(this); }
        ~HighlightGroup() { Dispose(); }

        public IList<TextMarker> Markers { get { return _markers.AsReadOnly(); } }
    }
}

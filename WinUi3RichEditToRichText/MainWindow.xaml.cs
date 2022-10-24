using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3RichEditToRichText
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private bool _IsEditingToolsEnabled = true;
        public bool IsEditingToolsEnabled
        {
            get => _IsEditingToolsEnabled;
            set
            {
                if (_IsEditingToolsEnabled != value)
                {
                    _IsEditingToolsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void richedit_TextChanged(object sender, RoutedEventArgs e)
        {
            //if (sender is RichEditBox editBox)
            //{
            //    ConvertToHtml(editBox);
            //}
        }

        private void AppBarItem_Bold_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void AppBarItem_Italic_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void AppBarItem_Underline_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.Underline == UnderlineType.None)
                {
                    charFormatting.Underline = UnderlineType.Single;
                }
                else
                {
                    charFormatting.Underline = UnderlineType.None;
                }
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void AppBarItem_Strikeout_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Strikethrough = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void AppBarItem_LeftAlign_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                if (selectedText.ParagraphFormat.Alignment != ParagraphAlignment.Left)
                    selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            }
        }

        private void AppBarItem_CenterAlign_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                if (selectedText.ParagraphFormat.Alignment != ParagraphAlignment.Center)
                    selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
        }

        private void AppBarItem_RightAlign_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                if (selectedText.ParagraphFormat.Alignment != ParagraphAlignment.Right)
                    selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            }
        }

        private void AppBarItem_JustifyAlign_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                if (selectedText.ParagraphFormat.Alignment != ParagraphAlignment.Justify)
                    selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            }
        }

        private void AppBarItem_Preview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarToggleButton appBarToggleButton)
            {
                if (richedit.Visibility == Visibility.Visible)
                {
                    ConvertToHtml(richedit);
                    richTextBlock.Visibility = Visibility.Visible;
                    richedit.Visibility = Visibility.Collapsed;
                    appBarToggleButton.IsChecked = true;
                    IsEditingToolsEnabled = false;
                }
                else if (richTextBlock.Visibility == Visibility.Visible)
                {
                    richedit.Visibility = Visibility.Visible;
                    richTextBlock.Visibility = Visibility.Collapsed;
                    appBarToggleButton.IsChecked = false;
                    IsEditingToolsEnabled = true;
                }
            }

        }

        public string ConvertToHtml(RichEditBox richEditBox)
        {
            //richTextBlock.Blocks.Clear();
            string text, strColour, strFntName, strHTML;
            richEditBox.Document.GetText(TextGetOptions.None, out text);
            ITextRange txtRange = richEditBox.Document.GetRange(0, text.Length);
            strHTML = "<html>";
            if (richTextBlock.Blocks.Count == 0)
                richTextBlock.Blocks.Add(new Paragraph());

            Paragraph paragraph = richTextBlock.Blocks.FirstOrDefault() as Paragraph;
            if (paragraph == null)
                return null;

            if (paragraph.Inlines.Count == 0)
                paragraph.Inlines.Add(new Span());

            Span span = paragraph.Inlines.FirstOrDefault() as Span ;
            if (span == null)
                return null;
            span.Inlines.Clear();

            int lngOriginalStart = txtRange.StartPosition;
            int lngOriginalLength = txtRange.EndPosition;
            
            //font
            float fontSize = 11;
            string fontName;

            // txtRange.SetRange(txtRange.StartPosition, txtRange.EndPosition);
            bool bOpened = false, liOpened = false, numbLiOpened = false, iOpened = false, uOpened = false, bulletOpened = false, numberingOpened = false;
            for (int i = 0; i < text.Length; i++)
            {
                //a l'index 0 impossible de trouver son précédent
                bool isSamefromPrevious = i > 0;
                Run run = new ();
                Run previousRun = i > 0 ? span.Inlines[i - 1] as Run: null;

                txtRange.SetRange(i, i + 1);

                fontSize = txtRange.CharacterFormat.Size;
                if (isSamefromPrevious == false || previousRun == null || fontSize != previousRun.FontSize)
                {
                    isSamefromPrevious = false;
                    run.FontSize = fontSize;
                }

                fontName = txtRange.CharacterFormat.Name;
                if (isSamefromPrevious == false || previousRun == null || fontName != previousRun.FontFamily.Source)
                {
                    isSamefromPrevious = false;
                    run.FontFamily = new FontFamily(fontName);
                }

                //if (i == 0)
                //{
                //    strColour = txtRange.CharacterFormat.ForegroundColor.ToString();
                    
                //}
                //else
                //{

                //}

                //if (txtRange.CharacterFormat.Size != shtSize)
                //{
                //    shtSize = txtRange.CharacterFormat.Size;
                //    strHTML += "</span><span style=\"font-family: " + txtRange.CharacterFormat.Name + "; font-size: " + txtRange.CharacterFormat.Size + "pt; color: #" + txtRange.CharacterFormat.ForegroundColor.ToString().Substring(3) + "\">";
                //}

                if (txtRange.Character == Convert.ToChar(13))
                {
                    paragraph.Inlines.Add(new LineBreak());
                }

                #region bullet
                if (txtRange.ParagraphFormat.ListType == MarkerType.Bullet)
                {
                    if (!bulletOpened)
                    {
                        strHTML += "<ul>";
                        bulletOpened = true;
                    }

                    if (!liOpened)
                    {
                        strHTML += "<li>";
                        liOpened = true;
                    }

                    if (txtRange.Character == Convert.ToChar(13))
                    {
                        strHTML += "</li>";
                        liOpened = false;
                    }
                }
                else
                {
                    if (bulletOpened)
                    {
                        strHTML += "</ul>";
                        bulletOpened = false;
                    }
                }

                #endregion

                #region numbering
                if (txtRange.ParagraphFormat.ListType == MarkerType.LowercaseRoman)
                {
                    if (!numberingOpened)
                    {
                        strHTML += "<ol type=\"i\">";
                        numberingOpened = true;
                    }

                    if (!numbLiOpened)
                    {
                        strHTML += "<li>";
                        numbLiOpened = true;
                    }

                    if (txtRange.Character == Convert.ToChar(13))
                    {
                        strHTML += "</li>";
                        numbLiOpened = false;
                    }
                }
                else
                {
                    if (numberingOpened)
                    {
                        strHTML += "</ol>";
                        numberingOpened = false;
                    }
                }

                #endregion

                #region bold
                if (txtRange.CharacterFormat.Bold == FormatEffect.On)
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.FontWeight != FontWeights.Bold)
                    {
                        isSamefromPrevious = false;
                        if (run.FontWeight != FontWeights.Bold)
                            run.FontWeight = FontWeights.Bold;
                    }
                }
                else
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.FontWeight != FontWeights.Normal)
                    {
                        isSamefromPrevious = false;
                        if (run.FontWeight != FontWeights.Normal)
                            run.FontWeight = FontWeights.Normal;
                    }
                }
                #endregion

                #region italic
                if (txtRange.CharacterFormat.Italic == FormatEffect.On)
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.FontStyle != Windows.UI.Text.FontStyle.Italic)
                    {
                        isSamefromPrevious = false;
                        if (run.FontStyle != Windows.UI.Text.FontStyle.Italic)
                            run.FontStyle = Windows.UI.Text.FontStyle.Italic;
                    }
                }
                else
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.FontStyle != Windows.UI.Text.FontStyle.Normal)
                    {
                        isSamefromPrevious = false;
                        if (run.FontStyle == Windows.UI.Text.FontStyle.Italic)
                            run.FontStyle = Windows.UI.Text.FontStyle.Normal;
                    }
                }
                #endregion

                //#region underline
                //if (txtRange.CharacterFormat.Underline == UnderlineType.Single)
                //{
                //    if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                //}
                //else
                //{
                //    if (run.TextDecorations == Windows.UI.Text.TextDecorations.Underline)
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.None;
                //}
                //#endregion

                //#region strikeout
                //if (txtRange.CharacterFormat.Strikethrough == FormatEffect.On)
                //{
                //    if (run.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough)
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                //}
                //else
                //{
                //    if (run.TextDecorations == Windows.UI.Text.TextDecorations.Strikethrough)
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.None;
                //}
                //#endregion

                if (txtRange.CharacterFormat.Underline == UnderlineType.Single && txtRange.CharacterFormat.Strikethrough == FormatEffect.On)
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough && previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                    {
                        isSamefromPrevious = false;
                        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough | Windows.UI.Text.TextDecorations.Underline;
                    }
                    //if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                    //    run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                }
                else if (txtRange.CharacterFormat.Underline == UnderlineType.None && txtRange.CharacterFormat.Strikethrough == FormatEffect.On)
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough)
                    {
                        isSamefromPrevious = false;
                        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                    }
                    //if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                    //    run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                }
                else if (txtRange.CharacterFormat.Underline == UnderlineType.Single && txtRange.CharacterFormat.Strikethrough == FormatEffect.Off)
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                    {
                        isSamefromPrevious = false;
                        run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                    }
                    //if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                    //    run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                }
                else
                {
                    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.None)
                    {
                        isSamefromPrevious = false;
                        run.TextDecorations = Windows.UI.Text.TextDecorations.None;
                    }
                }

                Debug.WriteLine(run.TextDecorations.ToString());
                strHTML += txtRange.Character;

                if (isSamefromPrevious && previousRun != null)
                {
                    previousRun.Text += txtRange.Character.ToString();
                }
                else
                {
                    run.Text = txtRange.Character.ToString();
                    span.Inlines.Add(run);
                }
            }


            strHTML += "</span></html>";
            //richTextBlock.Blocks.Add(paragraph);
            return strHTML;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

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

        public void ConvertToHtml(RichEditBox richEditBox)
        {
            richTextBlock.Blocks.Clear();
            string text, strColour, strFntName, strHTML;
            richEditBox.Document.GetText(TextGetOptions.None, out text);
            ITextRange txtRange = richEditBox.Document.GetRange(0, text.Length);
            strHTML = "<html>";
            //if (richTextBlock.Blocks.Count == 0)
            //    richTextBlock.Blocks.Add(new Paragraph());

            //Paragraph paragraph = richTextBlock.Blocks.FirstOrDefault() as Paragraph;
            //if (paragraph == null)
            //    return;

            //if (paragraph.Inlines.Count == 0)
            //    paragraph.Inlines.Add(new Span());

            //Span span = paragraph.Inlines.FirstOrDefault() as Span ;
            //if (span == null)
            //    return;
            //span.Inlines.Clear();

            int lngOriginalStart = txtRange.StartPosition;
            int lngOriginalLength = txtRange.EndPosition;
            

            // txtRange.SetRange(txtRange.StartPosition, txtRange.EndPosition);
            bool liOpened = false, numbLiOpened = false, bulletOpened = false, numberingOpened = false;
            for (int i = 0; i < text.Length; i++)
            {
                txtRange.SetRange(i, i + 1);

                if (richTextBlock.Blocks.Count == 0)
                    richTextBlock.Blocks.Add(new Paragraph());

                Paragraph paragraph = richTextBlock.Blocks.LastOrDefault() as Paragraph;
                Paragraph nParagraph = null;
                if (paragraph == null)
                    return;

                if (i == 0)
                {
                    paragraph.TextAlignment = txtRange.ParagraphFormat.Alignment switch
                    {
                        ParagraphAlignment.Undefined => TextAlignment.Left,
                        ParagraphAlignment.Left => TextAlignment.Left,
                        ParagraphAlignment.Center => TextAlignment.Center,
                        ParagraphAlignment.Right => TextAlignment.Right,
                        ParagraphAlignment.Justify => TextAlignment.Justify,
                        _ => TextAlignment.Left,
                    };
                }
                else
                {
                    switch (txtRange.ParagraphFormat.Alignment)
                    {
                        case ParagraphAlignment.Undefined:
                            break;
                        case ParagraphAlignment.Left:
                            if (paragraph.TextAlignment != TextAlignment.Left)
                            {
                                if (nParagraph == null)
                                {
                                    nParagraph = new();
                                    richTextBlock.Blocks.Add(nParagraph);
                                }
                                nParagraph.TextAlignment = TextAlignment.Left;
                            }
                            break;
                        case ParagraphAlignment.Center:
                            if (paragraph.TextAlignment != TextAlignment.Center)
                            {
                                if (nParagraph == null)
                                {
                                    nParagraph = new();
                                    richTextBlock.Blocks.Add(nParagraph);
                                }
                                nParagraph.TextAlignment = TextAlignment.Center;
                            }
                            break;
                        case ParagraphAlignment.Right:
                            if (paragraph.TextAlignment != TextAlignment.Right)
                            {
                                if (nParagraph == null)
                                {
                                    nParagraph = new();
                                    richTextBlock.Blocks.Add(nParagraph);
                                }
                                nParagraph.TextAlignment = TextAlignment.Right;
                            }
                            break;
                        case ParagraphAlignment.Justify:
                            if (paragraph.TextAlignment != TextAlignment.Justify)
                            {
                                if (nParagraph == null) {
                                    nParagraph = new();
                                    richTextBlock.Blocks.Add(nParagraph);
                                }
                                nParagraph.TextAlignment = TextAlignment.Justify;
                            }
                            break;
                        default:
                            break;
                    }

                }

                Paragraph currentParagraph = nParagraph ?? paragraph;
                if (currentParagraph == null)
                    continue;

                //Run run = null;
                bool isFirstChar = currentParagraph.Inlines.Count == 0;
                //if (!isFirstChar)
                //    run = currentParagraph.Inlines.LastOrDefault() as Run ?? new Run();
                //else
                //    run = new Run();
                Run run = !isFirstChar ? currentParagraph.Inlines.LastOrDefault() as Run ?? new() : new();
                Run nRun = null;

                float fontSize = txtRange.CharacterFormat.Size;
                if (isFirstChar || currentParagraph.Inlines.LastOrDefault() is not Run)
                {
                    run.FontSize = fontSize;
                }
                else
                {
                    if (fontSize != Convert.ToSingle(run.FontSize))
                    {
                        if (nRun == null) 
                        { 
                            nRun = new();
                            currentParagraph.Inlines.Add(nRun);
                        }
                        nRun.FontSize = fontSize;
                    }
                }
                
                string fontName = txtRange.CharacterFormat.Name;
                if (isFirstChar || currentParagraph.Inlines.LastOrDefault() is not Run)
                {
                    run.FontFamily = new FontFamily(fontName);
                }
                else
                {
                    if (fontSize != Convert.ToSingle(run.FontSize))
                    {
                        if (nRun == null)
                        {
                            nRun = new();
                            currentParagraph.Inlines.Add(nRun);
                        }
                        nRun.FontFamily = new FontFamily(fontName);
                    }
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
                    currentParagraph.Inlines.Add(new LineBreak());
                    continue;
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
                bool isBold = txtRange.CharacterFormat.Bold == FormatEffect.On;
                if (isFirstChar || currentParagraph.Inlines.LastOrDefault() is not Run)
                {
                    run.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
                }
                else
                {
                    if (fontSize != Convert.ToSingle(run.FontSize))
                    {
                        if (nRun == null)
                        {
                            nRun = new();
                            paragraph.Inlines.Add(nRun);
                        }

                        nRun.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
                    }
                }
                #endregion

                #region italic
                bool isItalic = txtRange.CharacterFormat.Italic == FormatEffect.On;
                if (isFirstChar || currentParagraph.Inlines.LastOrDefault() is not Run)
                {
                    run.FontStyle = isItalic ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                }
                else
                {
                    if (fontSize != Convert.ToSingle(run.FontSize))
                    {
                        if (nRun == null)
                        {
                            nRun = new();
                            currentParagraph.Inlines.Add(nRun);
                        }

                        nRun.FontStyle = isItalic ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
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

                #region Text decoration
                bool isUnderline = txtRange.CharacterFormat.Underline == UnderlineType.Single;
                bool isStrikethrough = txtRange.CharacterFormat.Strikethrough == FormatEffect.On;
                if (isFirstChar || currentParagraph.Inlines.LastOrDefault() is not Run)
                {
                    if (isUnderline && isStrikethrough)
                        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough | Windows.UI.Text.TextDecorations.Underline;
                    else if (!isUnderline && isStrikethrough)
                        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                    else if (isUnderline && !isStrikethrough)
                        run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                    else
                        run.TextDecorations = Windows.UI.Text.TextDecorations.None;
                }
                else
                {
                    if (isUnderline && isStrikethrough)
                    {
                        if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline || run.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough)
                        {
                            if (nRun == null)
                            {
                                nRun = new();
                                currentParagraph.Inlines.Add(nRun);
                            }
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.Underline | Windows.UI.Text.TextDecorations.Strikethrough;
                        }
                    }
                    else if (!isUnderline && isStrikethrough)
                    {
                        if (run.TextDecorations == Windows.UI.Text.TextDecorations.Underline || run.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough)
                        {
                            if (nRun == null)
                            {
                                nRun = new();
                                currentParagraph.Inlines.Add(nRun);
                            }
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                        }
                    }
                    else if (isUnderline && !isStrikethrough)
                    {
                        if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline || run.TextDecorations == Windows.UI.Text.TextDecorations.Strikethrough)
                        {
                            if (nRun == null)
                            {
                                nRun = new();
                                currentParagraph.Inlines.Add(nRun);
                            }
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                        }
                    }
                    else
                    {
                        if (run.TextDecorations != Windows.UI.Text.TextDecorations.None)
                        {
                            if (nRun == null)
                            {
                                nRun = new();
                                currentParagraph.Inlines.Add(nRun);
                            }
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.None;
                        }
                    }
                }

                //if (txtRange.CharacterFormat.Underline == UnderlineType.Single && txtRange.CharacterFormat.Strikethrough == FormatEffect.On)
                //{
                //    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough && previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                //    {
                //        isSamefromPrevious = false;
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough | Windows.UI.Text.TextDecorations.Underline;
                //    }
                //    //if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                //    //    run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                //}
                //else if (txtRange.CharacterFormat.Underline == UnderlineType.None && txtRange.CharacterFormat.Strikethrough == FormatEffect.On)
                //{
                //    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough)
                //    {
                //        isSamefromPrevious = false;
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                //    }
                //    //if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                //    //    run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                //}
                //else if (txtRange.CharacterFormat.Underline == UnderlineType.Single && txtRange.CharacterFormat.Strikethrough == FormatEffect.Off)
                //{
                //    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                //    {
                //        isSamefromPrevious = false;
                //    }
                //    //if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline)
                //    //    run.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                //}
                //else
                //{
                //    if (isSamefromPrevious == false || previousRun == null || previousRun.TextDecorations != Windows.UI.Text.TextDecorations.None)
                //    {
                //        isSamefromPrevious = false;
                //        run.TextDecorations = Windows.UI.Text.TextDecorations.None;
                //    }
                //} 
                #endregion

                Run currentRun = nRun ?? run;
                if (currentRun == null)
                    continue;
                if (!currentParagraph.Inlines.Contains(currentRun))
                {
                    currentParagraph.Inlines.Add(currentRun);
                }
                currentRun.Text += txtRange.Character.ToString();
                //Debug.WriteLine(run.TextDecorations.ToString());
                //strHTML += txtRange.Character;

                //if (isSamefromPrevious && previousRun != null)
                //{
                //    previousRun.Text += txtRange.Character.ToString();
                //}
                //else
                //{
                //    run.Text = txtRange.Character.ToString();
                //    span.Inlines.Add(run);
                //}
            }


            strHTML += "</span></html>";
            //richTextBlock.Blocks.Add(paragraph);
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

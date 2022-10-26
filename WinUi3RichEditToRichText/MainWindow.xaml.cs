using ABI.Windows.Foundation;
using Microsoft.UI;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

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

        private ObservableCollection<ColorName> _ColorsNames = new();
        public ObservableCollection<ColorName> ColorsNames
        {
            get => _ColorsNames;
            set
            {
                if (_ColorsNames != value)
                {
                    _ColorsNames = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ColorName> _RecentColors = new();
        public ObservableCollection<ColorName> RecentColors
        {
            get => _RecentColors;
            set
            {
                if (_RecentColors != value)
                {
                    _RecentColors = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            IEnumerable<ColorName> colors = GetColorNames();
            foreach (ColorName color in colors)
                this.ColorsNames.Add(color);
            this.InitializeComponent();
        }

        private void GridView_ForeColor_ItemClick(object sender, ItemClickEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (e.ClickedItem is ColorName colorName)
                {
                    if (charFormatting.ForegroundColor != colorName.Color)
                    {
                        charFormatting.ForegroundColor = colorName.Color;
                        this.rectCurrentColor.Background = colorName.Brush;
                        AddRecentColor(colorName.Color);
                    }

                }
                //selectedText.CharacterFormat = charFormatting;
            }
        }

        private void Btn_SelectedColor_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                Color color = ((SolidColorBrush)rectCurrentColor.Background).Color;
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.ForegroundColor != color)
                {
                    charFormatting.ForegroundColor = color;
                    AddRecentColor(color);
                }
            }
        }

        private void Btn_MoreForeColor_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void Btn_ColorPickerApply_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.ForegroundColor != colorPicker.Color)
                {
                    charFormatting.ForegroundColor = colorPicker.Color;
                    this.rectCurrentColor.Background = new SolidColorBrush(colorPicker.Color);
                    AddRecentColor(colorPicker.Color);
                }
            }
        }

        private void Btn_ColorPickerCancel_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = false;
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

        private void AppBarItem_FontDecrease_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                var size = selectedText.CharacterFormat.Size;
                if (size <= 9)
                    selectedText.CharacterFormat.Size = 10.5f;
                else if (size - 0.5 > 9)
                    selectedText.CharacterFormat.Size -= 0.5f;
            }
        }

        private void AppBarItem_FontIncrease_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = richedit.Document.Selection;
            if (selectedText != null)
            {
                var size = selectedText.CharacterFormat.Size;
                if (size <= 9)
                    selectedText.CharacterFormat.Size = 10.5f;
                else if (size + 0.5 < 200)
                    selectedText.CharacterFormat.Size += 0.5f;
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
                //Run nRun = null;

                float fontSize = txtRange.CharacterFormat.Size;
                if (isFirstChar)
                {
                    run.FontSize = fontSize;
                }
                else
                {
                    if (fontSize != Convert.ToSingle(run.FontSize))
                    {
                        Run nRun = CreateCopy(run);
                        nRun.FontSize = fontSize;
                        currentParagraph.Inlines.Add(nRun);
                        run = nRun;
                    }
                }

                #region font Name
                string fontName = txtRange.CharacterFormat.Name;
                if (isFirstChar)
                {
                    run.FontFamily = new FontFamily(fontName);
                }
                else
                {
                    if (run.FontFamily.Source != fontName)
                    {
                        Run nRun = CreateCopy(run);
                        nRun.FontFamily = new FontFamily(fontName);
                        currentParagraph.Inlines.Add(nRun);
                        run = nRun;
                    }
                }
                #endregion

                #region font color
                Color fontColor = txtRange.CharacterFormat.ForegroundColor;
                if (isFirstChar)
                {
                    run.Foreground = new SolidColorBrush(fontColor);
                }
                else
                {
                    if (run.Foreground != new SolidColorBrush(fontColor))
                    {
                        Run nRun = CreateCopy(run);
                        nRun.Foreground = new SolidColorBrush(fontColor);
                        currentParagraph.Inlines.Add(nRun);
                        run = nRun;
                    }
                }
                #endregion
                
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
                if (isFirstChar)
                {
                    run.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
                }
                else
                {
                    if (isBold && run.FontWeight != FontWeights.Bold || !isBold && run.FontWeight == FontWeights.Bold)
                    {
                        Run nRun = CreateCopy(run);
                        nRun.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
                        currentParagraph.Inlines.Add(nRun);
                        run = nRun;
                    }
                }
                #endregion

                #region italic
                bool isItalic = txtRange.CharacterFormat.Italic == FormatEffect.On;
                if (isFirstChar)
                {
                    run.FontStyle = isItalic ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                }
                else
                {
                    if (isItalic && run.FontStyle != Windows.UI.Text.FontStyle.Italic || !isItalic && run.FontStyle == Windows.UI.Text.FontStyle.Italic)
                    {
                        Run nRun = CreateCopy(run);
                        nRun.FontStyle = isItalic ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                        currentParagraph.Inlines.Add(nRun);
                        run = nRun;
                    }
                }
                #endregion

                #region Text decoration
                bool isUnderline = txtRange.CharacterFormat.Underline == UnderlineType.Single;
                bool isStrikethrough = txtRange.CharacterFormat.Strikethrough == FormatEffect.On;
                if (isFirstChar)
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
                            Run nRun = CreateCopy(run);
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.Underline | Windows.UI.Text.TextDecorations.Strikethrough;
                            currentParagraph.Inlines.Add(nRun);
                            run = nRun;
                        }
                    }
                    else if (!isUnderline && isStrikethrough)
                    {
                        if (run.TextDecorations == Windows.UI.Text.TextDecorations.Underline || run.TextDecorations != Windows.UI.Text.TextDecorations.Strikethrough)
                        {
                            Run nRun = CreateCopy(run);
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                            currentParagraph.Inlines.Add(nRun);
                            run = nRun;
                        }
                    }
                    else if (isUnderline && !isStrikethrough)
                    {
                        if (run.TextDecorations != Windows.UI.Text.TextDecorations.Underline || run.TextDecorations == Windows.UI.Text.TextDecorations.Strikethrough)
                        {
                            Run nRun = CreateCopy(run);
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
                            currentParagraph.Inlines.Add(nRun);
                            run = nRun;
                        }
                    }
                    else
                    {
                        if (run.TextDecorations != Windows.UI.Text.TextDecorations.None)
                        {
                            Run nRun = CreateCopy(run);
                            nRun.TextDecorations = Windows.UI.Text.TextDecorations.None;
                            currentParagraph.Inlines.Add(nRun);
                            run = nRun;
                        }
                    }
                }
                #endregion
               
                if (!currentParagraph.Inlines.Contains(run))
                {
                    currentParagraph.Inlines.Add(run);
                }
                run.Text += txtRange.Character.ToString();
            }
        }

        private static Run CreateCopy(Run source)
        {
            if (source == null)
                return null;

            return new Run()
            {
                //FlowDirection = source.FlowDirection,
                //AccessKey = source.AccessKey,
                //AccessKeyScopeOwner = source.AccessKeyScopeOwner,
                //AllowFocusOnInteraction = source.AllowFocusOnInteraction,
                //ExitDisplayModeOnAccessKeyInvoked = source.ExitDisplayModeOnAccessKeyInvoked,
                //IsAccessKeyScope = source.IsAccessKeyScope,
                CharacterSpacing = source.CharacterSpacing,
                FontFamily = source.FontFamily,
                FontSize = source.FontSize,
                FontStretch = source.FontStretch,
                FontStyle = source.FontStyle,
                FontWeight = source.FontWeight,
                Foreground = source.Foreground,
                IsTextScaleFactorEnabled = source.IsTextScaleFactorEnabled,
                TextDecorations = source.TextDecorations,

            };
        }

        private void AddRecentColor(Color color)
        {
            string colorName = "couleur inconnue";
            ColorName item = RecentColors.FirstOrDefault(a => a.Color == color);
            if (item.Equals(default(ColorName)))
            {
                foreach (var colorvalue in typeof(Colors).GetRuntimeProperties())
                {
                    if (colorvalue.PropertyType != typeof(Color))
                        continue;

                    if ((Color)colorvalue.GetValue(null) == color)
                    {
                        colorName = colorvalue.Name;
                        break;
                    }
                }

                if (RecentColors.Count < 5)
                {
                    RecentColors.Add(new ColorName(colorName, color));
                }
                else
                {
                    RecentColors.Remove(RecentColors.LastOrDefault());
                    RecentColors.Insert(0, new ColorName(colorName, color));
                }
            }
            else
            {
                RecentColors.Remove(item);
                RecentColors.Insert(0, item);
            }
        }

        public IEnumerable<ColorName> GetColorNames()
        {
            foreach (var color in typeof(Colors).GetRuntimeProperties())
            {
                if (color.PropertyType == typeof(Color))
                    yield return new ColorName(color.Name, (Color)color.GetValue(null));
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
    }

    public struct ColorName
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public Brush Brush { get; set; }

        public ColorName(string name, Color color)
        {
            Name = name;
            Color = color;
            Brush = new SolidColorBrush(Color);
        }
    }
}

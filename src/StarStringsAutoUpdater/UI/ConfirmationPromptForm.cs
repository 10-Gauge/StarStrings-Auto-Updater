using System.Drawing;
using System.Windows.Forms;

namespace StarStringsAutoUpdater.UI;

/// <summary>Resizable Yes/No prompt that sizes itself to fit the heading, subheading, and
/// notes text, clamped to the screen's working area. Shared by the StarStrings update
/// prompt and the app's self-update prompt.</summary>
public sealed class ConfirmationPromptForm : Form
{
    private const int Margin = 16;
    private const int ContentWidth = 480;
    private const int ButtonWidth = 100;
    private const int ButtonHeight = 30;
    private const int ButtonSpacing = 10;
    private const int MinNotesHeight = 90;
    private const int MaxNotesHeight = 420;

    private readonly Font _headingFont;

    public ConfirmationPromptForm(
        string title,
        string heading,
        string subheading,
        string notesLabelText,
        string notesText,
        string primaryButtonText,
        string secondaryButtonText)
    {
        Text = title;
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = true;
        MinimizeBox = false;
        ShowInTaskbar = true;
        AutoScaleMode = AutoScaleMode.Dpi;
        MinimumSize = new Size(420, 320);

        _headingFont = new Font(Font.FontFamily, 11f, FontStyle.Bold);

        var headingLabel = new Label
        {
            Text = heading,
            Font = _headingFont,
            AutoSize = false,
            Location = new Point(Margin, Margin),
            Size = new Size(ContentWidth, MeasureTextHeight(heading, _headingFont, ContentWidth)),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
        };

        var subheadingLabel = new Label
        {
            Text = subheading,
            AutoSize = false,
            Location = new Point(Margin, headingLabel.Bottom + 10),
            Size = new Size(ContentWidth, MeasureTextHeight(subheading, Font, ContentWidth)),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
        };

        var notesLabel = new Label
        {
            Text = notesLabelText,
            AutoSize = false,
            Location = new Point(Margin, subheadingLabel.Bottom + 12),
            Size = new Size(ContentWidth, 18),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
        };

        // +20 covers the TextBox's padding/border so the measured text doesn't scroll.
        var neededNotesHeight = MeasureTextHeight(notesText, Font, ContentWidth - 12) + 20;
        var notesHeight = Math.Clamp(neededNotesHeight, MinNotesHeight, MaxNotesHeight);

        var notesBox = new TextBox
        {
            Text = notesText,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Location = new Point(Margin, notesLabel.Bottom + 4),
            Size = new Size(ContentWidth, notesHeight),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
        };

        // Shrink the notes box if the dialog would end up taller than the screen.
        var workingArea = Screen.FromPoint(Cursor.Position).WorkingArea;
        var maxClientHeight = (int)(workingArea.Height * 0.85);
        var naturalClientHeight = notesBox.Bottom + Margin + ButtonHeight + Margin;
        if (naturalClientHeight > maxClientHeight)
        {
            var overflow = naturalClientHeight - maxClientHeight;
            notesBox.Height = Math.Max(MinNotesHeight, notesBox.Height - overflow);
        }

        var buttonsTop = notesBox.Bottom + Margin;
        var primaryButton = new Button
        {
            Text = primaryButtonText,
            DialogResult = DialogResult.Yes,
            Size = new Size(ButtonWidth, ButtonHeight),
            Location = new Point(Margin + ContentWidth - ButtonWidth - ButtonSpacing - (ButtonWidth - 10), buttonsTop),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
        };

        var secondaryButton = new Button
        {
            Text = secondaryButtonText,
            DialogResult = DialogResult.No,
            Size = new Size(ButtonWidth - 10, ButtonHeight),
            Location = new Point(primaryButton.Right + ButtonSpacing, buttonsTop),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
        };

        // ClientSize has to be set before anchored controls are added, or they end up
        // positioned relative to the form's small default size instead of the real one.
        ClientSize = new Size(Margin * 2 + ContentWidth, buttonsTop + ButtonHeight + Margin);

        Controls.AddRange([headingLabel, subheadingLabel, notesLabel, notesBox, primaryButton, secondaryButton]);
        AcceptButton = primaryButton;
        CancelButton = secondaryButton;
    }

    private static int MeasureTextHeight(string text, Font font, int width) =>
        TextRenderer.MeasureText(text, font, new Size(width, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _headingFont.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Shows the prompt and returns true if the user chose the primary action.</summary>
    public static bool Ask(
        string title,
        string heading,
        string subheading,
        string notesLabelText,
        string notesText,
        string primaryButtonText,
        string secondaryButtonText)
    {
        using var form = new ConfirmationPromptForm(title, heading, subheading, notesLabelText, notesText, primaryButtonText, secondaryButtonText);
        return form.ShowDialog() == DialogResult.Yes;
    }
}

using System.ComponentModel;

namespace Nalu.Maui.TaskFlow;

public partial class AppShellTabBar
{
    private int _pendingIndex = -1;

    private static readonly Color DarkBg = Color.FromArgb("#252547");
    private static readonly Color DarkIcon = Color.FromArgb("#BBB");

    public AppShellTabBar()
    {
        InitializeComponent();

        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeChanged += (_, _) => ApplyTheme();
            // Apply initial theme in case the user previously set dark mode
            Dispatcher.Dispatch(ApplyTheme);
        }
    }

    private void ApplyTheme()
    {
        var isDark = Application.Current?.UserAppTheme == AppTheme.Dark;
        var bg = isDark ? DarkBg : Colors.White;
        var iconColor = isDark ? DarkIcon : Color.FromArgb("#888");
        var selectedGlyphColor = isDark ? Colors.White : Colors.Black;

        TabBarBorder.BackgroundColor = bg;
        FloatingBorder.BackgroundColor = bg;
        ((FontImageSource)SelectedButton.Source).Color = selectedGlyphColor;

        foreach (var child in Buttons.Children)
        {
            if (child is ImageButton btn && btn.Source is FontImageSource fis)
                fis.Color = iconColor;
        }
    }

    protected override void OnBindingContextChanged()
    {
        ShellItem?.PropertyChanged -= OnCurrentItemChanged;

        if (BindingContext is ShellItem item)
        {
            ShellItem = item;
            item.PropertyChanged += OnCurrentItemChanged;
            UpdateCurrentItem(item.CurrentItem);
        }
    }

    public ShellItem? ShellItem { get; set; }

    private void OnCurrentItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == Microsoft.Maui.Controls.ShellItem.CurrentItemProperty.PropertyName)
        {
            UpdateCurrentItem(ShellItem!.CurrentItem);
        }
    }

    private void UpdateCurrentItem(ShellSection currentItem)
    {
        var selectedIndex = Math.Min(3, ShellItem?.Items.IndexOf(currentItem) ?? 0);
        var parent = (View)SelectedShape.Parent;

        if (parent.Width <= 0)
        {
            // Layout not ready yet — defer until measured
            _pendingIndex = selectedIndex;
            parent.SizeChanged -= OnParentSizeChanged;
            parent.SizeChanged += OnParentSizeChanged;
            return;
        }

        AnimateToIndex(selectedIndex);
    }

    private void OnParentSizeChanged(object? sender, EventArgs e)
    {
        var parent = (View)sender!;
        if (parent.Width <= 0) return;
        parent.SizeChanged -= OnParentSizeChanged;

        if (_pendingIndex >= 0)
        {
            SnapToIndex(_pendingIndex);
            _pendingIndex = -1;
        }
    }

    private void SnapToIndex(int selectedIndex)
    {
        var endPosition = (1.0f / 3.0f) * selectedIndex;
        var availableWidth = ((View)SelectedShape.Parent).Width;
        var endTranslationX = (availableWidth - TabBarShape.InsetWidth) * (1.0 / 3.0) * selectedIndex + 36;

        TabBarShape.InsetPosition = (float)endPosition;
        SelectedShape.TranslationX = endTranslationX;
        SelectedShape.TranslationY = 0;
        SelectedButton.Opacity = 1;
        SelectedShape.ZIndex = 2;
        ((FontImageSource)SelectedButton.Source).Glyph = ((FontImageSource)((ImageButton)Buttons[selectedIndex]!).Source).Glyph;
    }

    private void AnimateToIndex(int selectedIndex)
    {
        this.CancelAnimations();
        var startPosition = TabBarShape.InsetPosition;
        var endPosition = (1.0f / 3.0f) * selectedIndex;
        var startTranslationX = SelectedShape.TranslationX;
        var availableTranslationWidth = ((View)SelectedShape.Parent).Width;
        var endTranslationX = (availableTranslationWidth - TabBarShape.InsetWidth) * (1.0 / 3.0) * selectedIndex + 36;
        var startTranslationY = SelectedShape.TranslationY;
        var middleTranslationY = 50;
        var startOpacity = SelectedButton.Opacity;
        var middleOpacity = 0f;
        var endTranslationY = 0;
        var endOpacity = 1f;

        SelectedShape.ZIndex = 0;

        this.Animate(
            "ButtonFadeOut",
            v =>
            {
                SelectedShape.TranslationY = startTranslationY + (middleTranslationY - startTranslationY) * v;
                SelectedButton.Opacity = startOpacity + (middleOpacity - startOpacity) * v;
            },
            length: 125,
            finished: (_, canceled) =>
            {
                if (canceled) return;

                ((FontImageSource)SelectedButton.Source).Glyph = ((FontImageSource)((ImageButton)Buttons[selectedIndex]!).Source).Glyph;

                this.Animate(
                    "ButtonFadeIn",
                    v =>
                    {
                        SelectedShape.TranslationY = middleTranslationY + (endTranslationY - middleTranslationY) * v;
                        SelectedButton.Opacity = middleOpacity + (endOpacity - middleOpacity) * v;
                    },
                    finished: (_, canceled2) =>
                    {
                        if (canceled2) return;
                        SelectedShape.ZIndex = 2;
                    }
                );
            }
        );

        this.Animate("CurrentItem",
            v =>
            {
                TabBarShape.InsetPosition = (float)(startPosition + (endPosition - startPosition) * v);
                SelectedShape.TranslationX = startTranslationX + (endTranslationX - startTranslationX) * v;
            },
            length: 250);
    }

    private void IconClicked(object? sender, EventArgs e)
    {
        var icon = (ImageButton)sender!;
        var parent = (Layout)icon.Parent!;
        var index = parent.IndexOf(icon);

        NaluTabBar.GoTo(ShellItem!.Items[index]);
    }

    private void SelectedButtonClicked(object? sender, EventArgs e)
    {
        // Re-tap current tab - could scroll to top in future
    }
}

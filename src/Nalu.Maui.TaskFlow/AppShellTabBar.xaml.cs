using System.ComponentModel;

namespace Nalu.Maui.TaskFlow;

public partial class AppShellTabBar
{
    private ShellItem Item => BindingContext as ShellItem ??
                              throw new InvalidOperationException(
                                  "AppShellTabBar must have a ShellItem as its BindingContext");

    public AppShellTabBar()
    {
        InitializeComponent();
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
        if (e.PropertyName == ShellItem.CurrentItemProperty.PropertyName)
        {
            UpdateCurrentItem(ShellItem!.CurrentItem);
        }
    }

    private void UpdateCurrentItem(ShellSection currentItem)
    {
        this.CancelAnimations();
        var numButtons = (double)Buttons.Count;
        var selectedIndex = ShellItem?.Items.IndexOf(currentItem) ?? 0;
        var startPosition = TabBarShape.InsetPosition;
        var buttonFractionalOffset = 1.0 / (numButtons - 1);
        var endPosition = buttonFractionalOffset * selectedIndex;
        var startTranslationX = SelectedShape.TranslationX;
        var availableTranslationWidth = ((View)SelectedShape.Parent.Parent).Width;
        var endTranslationX = (availableTranslationWidth - TabBarShape.InsetWidth) * buttonFractionalOffset * selectedIndex + 32;

        AnimateSelectedShapeJump(selectedIndex);

        SelectedShapeContainer.TranslationX = startTranslationX - 0.001;
        this.Animate("CurrentItem",
            v =>
            {
                TabBarShape.InsetPosition = (float)(startPosition + (endPosition - startPosition) * v);
                SelectedShapeContainer.TranslationX = startTranslationX + (endTranslationX - startTranslationX) * v;
            },
            length: 250);
    }

    private const string SelectedJumpOut = nameof(SelectedJumpOut);
    private const string SelectedJumpIn = nameof(SelectedJumpIn);

    private void AnimateSelectedShapeJump(int selectedIndex, double deltaY = 50)
    {
        SelectedShapeContainer.ZIndex = 0;
        var startTranslationY = SelectedShape.TranslationY;
        var middleTranslationY = deltaY;
        var startOpacity = SelectedButton.Opacity;
        var middleOpacity = 0f;
        var endTranslationY = 0;
        var endOpacity = 1f;
        this.Animate(
            SelectedJumpOut,
            v =>
            {
                SelectedShape.TranslationY = startTranslationY + (middleTranslationY - startTranslationY) * v;
                SelectedButton.Opacity = startOpacity + (middleOpacity - startOpacity) * v;
            },
            length: 125,
            finished: (_, canceled) =>
            {
                if (canceled)
                {
                    return;
                }

                ((FontImageSource)SelectedButton.Source).Glyph =
                    ((FontImageSource)((ImageButton)Buttons[selectedIndex]!).Source).Glyph;

                this.Animate(
                    SelectedJumpIn,
                    v =>
                    {
                        SelectedShape.TranslationY = middleTranslationY + (endTranslationY - middleTranslationY) * v;
                        SelectedButton.Opacity = middleOpacity + (endOpacity - middleOpacity) * v;
                    },
                    finished: (_, canceled2) =>
                    {
                        if (canceled2)
                        {
                            return;
                        }

                        SelectedShapeContainer.ZIndex = 2;
                    }
                );
            }
        );
    }

    private async void IconClicked(object? sender, EventArgs e)
    {
        var icon = (ImageButton)sender!;
        var parent = (Layout)icon.Parent!;
        var index = parent.IndexOf(icon);

        var targetSection = Item.Items[index];
        if (targetSection == Item.CurrentItem)
        {
            return;
        }

        await Shell.Current.GoToAsync($"//{targetSection.CurrentItem.Route}");
    }

    private void SelectedButtonClicked(object? sender, EventArgs e)
    {
        this.AbortAnimation(SelectedJumpIn);
        this.AbortAnimation(SelectedJumpOut);
        var index = Item.Items.IndexOf(Item.CurrentItem);
        AnimateSelectedShapeJump(index, 25);

        var descendants = Shell.Current.CurrentPage.GetVisualTreeDescendants();

        if (descendants.OfType<VirtualScroll>().FirstOrDefault() is { } virtualScroll)
        {
            virtualScroll.ScrollTo(0, 0, ScrollToPosition.Start);
        }
    }
}
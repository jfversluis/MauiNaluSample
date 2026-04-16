using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui;
using Nalu.Maui.TaskFlow.PageModels;
using Nalu.Maui.TaskFlow.Pages;
using Nalu.Maui.TaskFlow.PopupModels;
using Nalu.Maui.TaskFlow.Popups;
using Nalu.Maui.TaskFlow.Services;

namespace Nalu.Maui.TaskFlow;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseNaluNavigation<App>(nav => nav
                // Pages
                .AddPage<DashboardPageModel, DashboardPage>()
                .AddPage<TaskListPageModel, TaskListPage>()
                .AddPage<TaskEditorPageModel, TaskEditorPage>()
                .AddPage<TimeLogPageModel, TimeLogPage>()
                .AddPage<SettingsPageModel, SettingsPage>()
                // Popups
                .AddPage<DiscardChangesPopupModel, DiscardChangesPopup>())
            .UseNaluLayouts()
            .UseNaluControls()
            .UseNaluVirtualScroll()
            .UseNaluTabBar()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Filled.ttf", "MaterialFilled");
                fonts.AddFont("MaterialIcons-Round.otf", "MaterialRound");
                fonts.AddFont("MaterialIcons-Outlined.otf", "MaterialOutlined");
            });

        builder.Services.AddSingleton<TaskService>();

        return builder.Build();
    }
}

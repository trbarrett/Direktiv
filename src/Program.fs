namespace Direktiv.Program

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Media
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Threading

open Amazon.Runtime.CredentialManagement

open Direktiv

type MainState =
    { AwsProfile : CredentialProfile
      Region : AwsRegion
      LambdaName : string
      Request : string
      Response : string }

module MainState =
    let initial (profile : CredentialProfile) =
        { AwsProfile = profile
          Region = AwsRegion.fromRegionEndpoint profile.Region
          LambdaName = ""
          Request = ""
          Response = "" }

module Main =

    let view () =
        Component(fun ctx ->
            let profileOptions = AWS.loadProfiles ()
            let state = ctx.useState (MainState.initial (List.head profileOptions))

            DockPanel.create [
                DockPanel.children [
                    DockPanel.create [
                        DockPanel.dock Dock.Top
                        DockPanel.background "#212121"
                        DockPanel.children [
                            TextBlock.create [
                                TextBlock.margin (30, 10, 10, 0)
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.textAlignment TextAlignment.Right
                                TextBlock.dock Dock.Left
                                TextBlock.width 50
                                TextBlock.text "Profile:"
                            ]
                            ComboBox.create [
                                ComboBox.margin (10, 10, 10, 0)
                                ComboBox.verticalAlignment VerticalAlignment.Center
                                ComboBox.dock Dock.Left
                                ComboBox.width 250
                                ComboBox.dataItems profileOptions
                                ComboBox.selectedItem state.Current.AwsProfile
                                ComboBox.onSelectedItemChanged (function
                                    | :? CredentialProfile as profile ->
                                        state.Set(
                                            { state.Current with
                                                AwsProfile = profile
                                                Region = AwsRegion.fromRegionEndpoint profile.Region })
                                    | unknown -> eprintf $"Can't cast %A{unknown} to CredentialProfile")
                                ComboBox.itemTemplate (
                                    DataTemplateView.create<_, _>(fun (profile: CredentialProfile) ->
                                        DockPanel.create [
                                            DockPanel.children [
                                                TextBlock.create [
                                                    TextBlock.dock Dock.Left
                                                    TextBlock.text $"{profile.Name}"
                                                ]
                                                TextBlock.create [
                                                    TextBlock.dock Dock.Right
                                                    TextBlock.textAlignment TextAlignment.Right
                                                    TextBlock.foreground "#999999"
                                                    TextBlock.fontStyle FontStyle.Italic
                                                    TextBlock.text $"{profile.CredentialDescription}"
                                                ]
                                            ]
                                        ]
                                    ))
                            ]
                        ]
                    ]
                    DockPanel.create [
                        DockPanel.dock Dock.Top
                        DockPanel.background "#212121"
                        DockPanel.children [
                            TextBlock.create [
                                TextBlock.margin (30, 10, 10, 0)
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.textAlignment TextAlignment.Right
                                TextBlock.dock Dock.Left
                                TextBlock.width 50
                                TextBlock.text "Region:"
                            ]
                            ComboBox.create [
                                ComboBox.margin (10, 10, 10, 0)
                                ComboBox.verticalAlignment VerticalAlignment.Center
                                ComboBox.dock Dock.Left
                                ComboBox.width 250
                                ComboBox.dataItems AwsRegion.all
                                ComboBox.selectedItem state.Current.Region
                                ComboBox.onSelectedItemChanged (function
                                    | :? AwsRegion as region ->
                                        state.Set({ state.Current with Region = region})
                                    | unknown -> eprintf $"Can't cast %A{unknown} to AwsRegion")
                                ComboBox.itemTemplate (
                                    DataTemplateView.create<_, _>(fun (region: AwsRegion) ->
                                        DockPanel.create [
                                            DockPanel.children [
                                                TextBlock.create [
                                                    TextBlock.dock Dock.Left
                                                    TextBlock.text $"{AwsRegion.description region}"
                                                ]
                                                TextBlock.create [
                                                    TextBlock.dock Dock.Right
                                                    TextBlock.textAlignment TextAlignment.Right
                                                    TextBlock.foreground "#999999"
                                                    TextBlock.fontStyle FontStyle.Italic
                                                    TextBlock.text $"{AwsRegion.systemName region}"
                                                ]
                                            ]
                                        ]
                                    ))
                            ]
                            TextBlock.create [
                                TextBlock.margin (10, 10, 10, 0)
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.textAlignment TextAlignment.Right
                                TextBlock.dock Dock.Left
                                TextBlock.width 70
                                TextBlock.text "Lambda:"
                            ]
                            Button.create [
                                Button.margin (10, 10, 10, 0)
                                Button.verticalAlignment VerticalAlignment.Center
                                Button.dock Dock.Right
                                Button.width 60
                                Button.fontWeight FontWeight.Bold
                                Button.content "Send"
                                Button.onClick (fun _ ->
                                    let x = state.Current
                                    async {
                                        let! response =
                                            AWS.invoke
                                                x.AwsProfile.Name
                                                x.Region
                                                x.LambdaName
                                                x.Request
                                        state.Set({ state.Current with Response = response})
                                    } |> Async.Start
                                    ())
                            ]
                            TextBox.create [
                                TextBox.name "LambdaInput"
                                TextBox.margin (10 ,10 ,10 ,0)
                                TextBox.verticalAlignment VerticalAlignment.Center
                                TextBox.dock Dock.Right
                                TextBox.onTextChanged (fun text ->
                                    state.Set({ state.Current with LambdaName = text }))
                            ]
                        ]
                    ]
                    Grid.create [
                        Grid.dock Dock.Bottom
                        Grid.rowDefinitions (RowDefinitions "*,1,*")
                        Grid.children [
                            DockPanel.create [
                                DockPanel.row 0
                                DockPanel.minHeight 100
                                DockPanel.background "#212121"
                                DockPanel.children [
                                    TextBlock.create [
                                        TextBlock.margin (Thickness(10))
                                        TextBlock.verticalAlignment VerticalAlignment.Top
                                        TextBlock.textAlignment TextAlignment.Right
                                        TextBlock.dock Dock.Left
                                        TextBlock.width 70
                                        TextBlock.text "Request:"
                                    ]
                                    TextBox.create [
                                        TextBox.name "RegionInput"
                                        TextBox.margin (Thickness(10))
                                        TextBox.text state.Current.Request
                                        TextBox.onTextChanged (fun text ->
                                            state.Set({ state.Current with Request = text }))
                                    ]
                                ]
                            ]
                            GridSplitter.create [
                                GridSplitter.row 1
                                GridSplitter.padding 0
                                GridSplitter.maxHeight 1
                                GridSplitter.background "#272727"
                            ]
                            DockPanel.create [
                                DockPanel.row 2
                                DockPanel.minHeight 100
                                DockPanel.children [
                                    TextBlock.create [
                                        TextBlock.margin (Thickness(10))
                                        TextBlock.verticalAlignment VerticalAlignment.Top
                                        TextBlock.textAlignment TextAlignment.Right
                                        TextBlock.dock Dock.Left
                                        TextBlock.width 70
                                        TextBlock.text "Response:"
                                    ]
                                    TextBox.create [
                                        TextBox.margin (Thickness(10))
                                        TextBox.text state.Current.Response
                                        TextBox.onTextChanged (fun text ->
                                            state.Set({ state.Current with Response = text }))
                                    ]
                                ]
                            ]
                        ]
                    ]
                    //DockPanel.create [
                    //    DockPanel.dock Dock.Bottom
                    //    DockPanel.children [
                    //        Button.create [
                    //            Button.dock Dock.Bottom
                    //            Button.onClick (fun _ -> state.Set(state.Current - 1))
                    //            Button.content "-"
                    //            Button.horizontalAlignment HorizontalAlignment.Stretch
                    //            Button.horizontalContentAlignment HorizontalAlignment.Center
                    //        ]
                    //        Button.create [
                    //            Button.dock Dock.Bottom
                    //            Button.onClick (fun _ -> state.Set(state.Current + 1))
                    //            Button.content "+"
                    //            Button.horizontalAlignment HorizontalAlignment.Stretch
                    //            Button.horizontalContentAlignment HorizontalAlignment.Center
                    //        ]
                    //        TextBlock.create [
                    //            TextBlock.dock Dock.Top
                    //            TextBlock.fontSize 48.0
                    //            TextBlock.verticalAlignment VerticalAlignment.Center
                    //            TextBlock.horizontalAlignment HorizontalAlignment.Center
                    //            TextBlock.text (string state.Current)
                    //        ]
                    //    ]
                    //]
                ]
            ]
        )

type MainWindow() =
    inherit HostWindow()
    do
        base.Title <- "Direktiv"
        base.ClientSize <- Size(800, 420)
        base.Content <- Main.view ()
        //base.ExtendClientAreaToDecorationsHint <- true

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)

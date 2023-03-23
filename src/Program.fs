namespace Avaloniaui.Demo.Program

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Media
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout

type AwsRegion =
    | EuWest1
    | EuWest2

module AwsRegion =
    let regionDescription = function
        | EuWest1 -> "Europe (Ireland)"
        | EuWest2 -> "Europe (London)"

    let regionCode = function
        | EuWest1 -> "eu-west-1"
        | EuWest2 -> "eu-west-2"

    let all = [ EuWest1; EuWest2 ]

type MainState =
    { Region : AwsRegion
      LambdaName : string
      Request : string
      Response : string }

module MainState =
    let initial =
        { Region = EuWest1
          LambdaName = ""
          Request = ""
          Response = "" }

module Main =

    let view () =
        Component(fun ctx ->
            let state = ctx.useState MainState.initial

            DockPanel.create [
                DockPanel.children [
                    DockPanel.create [
                        DockPanel.dock Dock.Top
                        DockPanel.children [
                            TextBlock.create [
                                TextBlock.margin (30, 10, 10, 10)
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.textAlignment TextAlignment.Right
                                TextBlock.dock Dock.Left
                                TextBlock.width 50
                                TextBlock.text "Region:"
                            ]
                            ComboBox.create [
                                ComboBox.margin (Thickness(10))
                                ComboBox.verticalAlignment VerticalAlignment.Center
                                ComboBox.dock Dock.Left
                                ComboBox.width 250
                                ComboBox.dataItems AwsRegion.all
                                ComboBox.selectedItem AwsRegion.EuWest1
                                ComboBox.onSelectedItemChanged (function
                                    | :? AwsRegion as region -> state.Set({ state.Current with Region = region})
                                    | unknown -> eprintf $"Can't cast %A{unknown} to AwsRegion")
                                ComboBox.itemTemplate (
                                    DataTemplateView.create<_, _>(fun (region: AwsRegion) ->
                                        DockPanel.create [
                                            DockPanel.children [
                                                TextBlock.create [
                                                    TextBlock.dock Dock.Left
                                                    TextBlock.text $"{AwsRegion.regionDescription region}"
                                                ]
                                                TextBlock.create [
                                                    TextBlock.dock Dock.Right
                                                    TextBlock.textAlignment TextAlignment.Right
                                                    TextBlock.foreground "#999999"
                                                    TextBlock.text $"{AwsRegion.regionCode region}"
                                                ]
                                            ]
                                        ]
                                    ))
                            ]
                            TextBlock.create [
                                TextBlock.margin (Thickness(10))
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.textAlignment TextAlignment.Right
                                TextBlock.dock Dock.Left
                                TextBlock.width 70
                                TextBlock.text "Lambda:"
                            ]
                            Button.create [
                                Button.margin (Thickness(10))
                                Button.verticalAlignment VerticalAlignment.Center
                                Button.dock Dock.Right
                                Button.width 60
                                Button.content "Send"
                            ]
                            TextBox.create [
                                TextBox.name "LambdaInput"
                                TextBox.margin (Thickness(10))
                                TextBox.verticalAlignment VerticalAlignment.Center
                                TextBox.dock Dock.Right
                                TextBox.onTextChanged (fun text ->
                                    state.Set({ state.Current with LambdaName = text }))
                            ]
                        ]
                    ]
                    Grid.create [
                        Grid.dock Dock.Bottom
                        Grid.rowDefinitions (RowDefinitions "*,4,*")
                        Grid.children [
                            DockPanel.create [
                                DockPanel.row 0
                                DockPanel.minHeight 100
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
                                GridSplitter.background "black"
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

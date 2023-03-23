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

module Main =

    let view () =
        Component(fun ctx ->
            let state = ctx.useState 0

            DockPanel.create [
                DockPanel.children [
                    DockPanel.create [
                        DockPanel.dock Dock.Top
                        DockPanel.children [
                            TextBlock.create [
                                TextBlock.margin (Thickness(10))
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
                                ComboBox.selectedIndex 0
                                ComboBox.dataItems [
                                    "Europe (Ireland) eu-west-1"
                                    "Europe (London) eu-west-2"
                                ]
                            ]
                            TextBlock.create [
                                TextBlock.margin (Thickness(10))
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.textAlignment TextAlignment.Right
                                TextBlock.dock Dock.Left
                                TextBlock.width 70
                                TextBlock.text "Lambda:"
                            ]
                            TextBox.create [
                                TextBox.margin (Thickness(10))
                                TextBox.verticalAlignment VerticalAlignment.Center
                                TextBox.dock Dock.Right
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
                                        TextBlock.margin (Thickness(10))
                                        TextBox.text ""
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
                                        TextBlock.margin (Thickness(10))
                                        TextBox.text ""
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
        this.RequestedThemeVariant <- Styling.ThemeVariant.Default

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

# WPF UI Look  & Feel Demo (with Theming)

## Purpose

To test the viability of integrating a UI design framework like MahApps and add-ons such as Dragablz to see 
if and how these cooperate with CEFSharp browser controls and a (light?) MVVM framework.



---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.



---

## Update

While those packages looked promising at the time, I find WPF/XAML development quite tedious and not very useful as I don't intend to continue using the platform for other tools.

Moving to Web-based technologies, `electron`-like. I'm looking at you, **Chromely**! As said elsewhere, if Chromely doesn't fly, I'll take CEF/WebView2 for a spin instead, so that we get a 95% portable UI, rather than maybe a 100% portable one with Chromely, by using a minimal platform-specific core and the Chrome/MS-Edge embedded browser on top for the real UI, while C# is kept for the "business layer logic" underneath.

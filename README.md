# Task Timer & Tracker (WPF)

A lightweight Windows WPF application for measuring task completion efficiency. Can be run in manual or auto mode

---

## Use Case

This tool is designed for users who want to:
- Track how long individual runs take and compare them later
- Can detect scene changes automatically or be triggered by keys on the num pad
- You can drag the summary window where you want it to be

---



##  Setup & Usage

### 1. Run the Application

Download & unzip or build yourself

### 2. Select Trigger Mode

** Automatic **
This will detect scene changes if you give it a reference. Select File/TrackRegion and choose an area on the screen which is unique for that scene. Then lock in. 
Every time you go back to this screen, the timer will reset automatically.

** Manual **
Press the + key on the numpad to start listening.
Press the Enter key to log a run as completed.
Press the - key on the numpad to stop listening and pring a summary.


### 3. Export Task Data

In the main window, use the menu bar to export task durations as a `.json` file. Filenames include the task label and timestamp.

---

##  Requirements

- Windows 11
- .NET 6.0 or newer

---

## Repository Notes

- `MainWindow.xaml` — primary control and logging interface
- `SummaryWindow.xaml` — compact overlay with live statistics
- `RegionSelectorWindow.xaml` — screen region capture for auto-detection


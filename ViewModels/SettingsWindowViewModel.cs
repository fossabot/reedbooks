﻿using ReedBooks.Core;
using ReedBooks.Core.Version;
using ReedBooks.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Version = ReedBooks.Core.Version.Version;

namespace ReedBooks.ViewModels
{
    public class SettingsWindowViewModel : ObservableObject
    {
        private ObservableCollection<SettingsParameterViewModel> _languages;
        public ObservableCollection<SettingsParameterViewModel> Languages
        {
            get
            {
                if (_languages == null)
                {
                    var items = new ObservableCollection<SettingsParameterViewModel>();
                    foreach (var lang in Localizator.AppLanguages)
                    {
                        var item = new SettingsParameterViewModel
                        {
                            DisplayName = char.ToUpper(lang.NativeName[0]) + lang.NativeName.Substring(1),
                            Tag = lang.Name
                        };

                        items.Add(item);
                    }

                    _languages = items;
                }

                return _languages;
            }
        }

        private SettingsParameterViewModel _selectedLanguage;
        public SettingsParameterViewModel SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        private ObservableCollection<SettingsParameterViewModel> _themes;
        public ObservableCollection<SettingsParameterViewModel> Themes
        {
            get
            {
                if (_themes == null)
                {
                    var themes = new ObservableCollection<SettingsParameterViewModel>();

                    var lightTheme = new SettingsParameterViewModel
                    {
                        DisplayName = Application.Current.Resources[App.LIGHT_THEME_NAME].ToString(),
                        Tag = App.LIGHT_THEME_NAME
                    };
                    themes.Add(lightTheme);

                    var darkTheme = new SettingsParameterViewModel
                    {
                        DisplayName = Application.Current.Resources[App.DARK_THEME_NAME].ToString(),
                        Tag = App.DARK_THEME_NAME
                    };
                    themes.Add(darkTheme);

                    _themes = themes;
                }
                
                return _themes;
            }
        }

        private SettingsParameterViewModel _selectedTheme;
        public SettingsParameterViewModel SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));
            }
        }

        private ushort _selectedCurrentCountedDays;
        public ushort SelectedCurrentCountedDays
        {
            get => _selectedCurrentCountedDays;
            set
            {
                _selectedCurrentCountedDays = value;
                OnPropertyChanged(nameof(SelectedCurrentCountedDays));
            }
        }

        private byte _recentBooksNumberDisplaying;
        public byte RecentBookNumberDisplaying
        {
            get => _recentBooksNumberDisplaying;
            set
            {
                _recentBooksNumberDisplaying = value;
                OnPropertyChanged(nameof(RecentBookNumberDisplaying));
            }
        }

        private ObservableCollection<SettingsParameterViewModel> _tabs;
        public ObservableCollection<SettingsParameterViewModel> Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    _tabs = new ObservableCollection<SettingsParameterViewModel>
                    {
                        new SettingsParameterViewModel
                        {
                            DisplayName = Application.Current.Resources["m_reading_now"].ToString(),
                            Tag = "0"
                        },
                        new SettingsParameterViewModel
                        {
                            DisplayName = Application.Current.Resources["m_all_books"].ToString(),
                            Tag = "1"
                        },
                        new SettingsParameterViewModel
                        {
                            DisplayName = Application.Current.Resources["m_search"].ToString(),
                            Tag = "2"
                        }
                    };
                }

                return _tabs;
            }
            set
            {
                _tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }

        private SettingsParameterViewModel _selectedTab;
        public SettingsParameterViewModel SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public Version InstalledVersion
        {
            get => Version.Local;
        }

        public string DeleteUnusedFilesHint
        {
            get => $"{Application.Current.Resources["s_delete_unused_files_hint"]} {Math.Round(App.StorageManager.UnusedFilesSize, 1)} {Application.Current.Resources["megabytes"]}";
        }

        public string InstalledVersionLabel
        {
            get => $"{Application.Current.Resources["s_installed_version"]} {InstalledVersion}";
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteUnusedFilesCommand { get; }
        public ICommand DeleteAllBooksCommand { get; }

        public SettingsWindowViewModel()
        {
            SaveCommand = new RelayCommand(obj => Save());
            DeleteUnusedFilesCommand = new RelayCommand(obj => DeleteUnusedFiles());
            DeleteAllBooksCommand = new RelayCommand(obj => DeleteAllBooks());

            SelectedLanguage = Languages.Where(l => l.Tag == Properties.Settings.Default.Language.Name).First();
            SelectedTheme = Themes.Where(t => t.Tag == Properties.Settings.Default.Theme).First();
            SelectedCurrentCountedDays = Properties.Settings.Default.CurrentCountedDays;
            RecentBookNumberDisplaying = Properties.Settings.Default.RecentBooksNumberDisplaying;
            SelectedTab = Tabs.Where(t => t.Tag == Convert.ToString(Properties.Settings.Default.DefaultTab)).First();
        }

        public void Save()
        {
            if (_selectedLanguage != null) Properties.Settings.Default.Language = new CultureInfo(_selectedLanguage.Tag);
            if (_selectedTheme != null) Properties.Settings.Default.Theme = _selectedTheme.Tag;
            if (_selectedCurrentCountedDays != 0) Properties.Settings.Default.CurrentCountedDays = _selectedCurrentCountedDays;
            if (_recentBooksNumberDisplaying != 0) Properties.Settings.Default.RecentBooksNumberDisplaying = _recentBooksNumberDisplaying;
            if (_selectedTab != null) Properties.Settings.Default.DefaultTab = Convert.ToByte(_selectedTab.Tag);

            Properties.Settings.Default.Save();

            var dW = new DialogWindow(Application.Current.Resources["dialog_settings_title"].ToString(),
                Application.Current.Resources["dialog_settings_content"].ToString());

            if (dW.ShowDialog() == true) App.Restart();
        }

        public void DeleteUnusedFiles()
        {
            App.StorageManager.DeleteUnusedFiles();
            OnPropertyChanged(nameof(DeleteUnusedFilesHint));
        }

        public void DeleteAllBooks()
        {
            var dW = new DialogWindow(Application.Current.Resources["dialog_delete_all_books_title"].ToString(),
                Application.Current.Resources["dialog_delete_all_books_content"].ToString());

            if (dW.ShowDialog() == true)
            {
                App.StorageManager.DeleteAllBooks();
                App.Restart();
            }
        }
    }
}

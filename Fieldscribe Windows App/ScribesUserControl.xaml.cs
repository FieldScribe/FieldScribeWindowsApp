using Fieldscribe_Windows_App.Controllers;
using Fieldscribe_Windows_App.Infrastructure;
using Fieldscribe_Windows_App.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fieldscribe_Windows_App
{
    /// <summary>
    /// Interaction logic for ScribesUserControl.xaml
    /// </summary>
    public partial class ScribesUserControl : UserControl
    {
        private AppDataModel _appDataModel;
        private ScribesPanelDataModel _dataModel;
        private TokenManager _tokenManager;
        private bool _assignScribeSuccess;
        private (bool, string) _registerScribeSuccess;
        private (bool, string) _updateScribeSuccess;
        private bool _removeScribeSuccess;

        public ScribesUserControl()
        {
            InitializeComponent();

            _appDataModel = AppDataModel.Instance;
            _dataModel = ScribesPanelDataModel.Instance;
            this.DataContext = ScribesPanelDataModel.Instance;
            _tokenManager = TokenManager.Instance;

            try
            {
                _dataModel.Scribes = GetAllScribes(new string[] { });
                RefreshScribesList();
            }
            catch (Exception e)
            {
                // Will throw exception if not connected to internet
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                CollectionView view = (CollectionView)CollectionViewSource
                    .GetDefaultView(ScribesList.ItemsSource);

                if (view != null)
                    view.Filter = ScribesFilter;

                ScribesTextFilter.Text = "";
            }
        }


        private bool ScribesFilter(object item)
        {
            if (String.IsNullOrEmpty(ScribesTextFilter.Text))
            {
                return true;
            }
            else
            {
                return ((item as User).FirstName.IndexOf(ScribesTextFilter.Text,
                    StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as User).LastName.IndexOf(ScribesTextFilter.Text,
                    StringComparison.OrdinalIgnoreCase) >= 0
                    || (item as User).Email.IndexOf(ScribesTextFilter.Text,
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ScribesList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(ScribesList.ItemsSource)
                    .Refresh();
        }

        private void RegisterScribeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Clear Fields
            _dataModel.Email = null;
            _dataModel.FirstName = null;
            _dataModel.LastName = null;
            PasswordBox.Password = null;
            PasswordConfirmBox.Password = null;

            RegisterMessage.Visibility = Visibility.Hidden;
        }

        private void RegisterScribeCreateBtn_Click(object sender, RoutedEventArgs e)
        {
            // Input already validated
            _dataModel.Password = PasswordBox.Password;

            RegisterMessage.Visibility = Visibility.Hidden;
            RegisterProgressBar.Visibility = Visibility.Visible;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_RegisterScribe;
            worker.RunWorkerCompleted += worker_RegisterScribeComplete;
            worker.RunWorkerAsync();
        }

        private void worker_RegisterScribe(object sender, DoWorkEventArgs e)
        {
            UsersController uc = new UsersController();

            Task t = Task.Run(() => {
                if (_tokenManager.Token != "")

                    _registerScribeSuccess = uc.RegisterScribe(
                        new RegisterForm
                        {
                            Email = _dataModel.Email,
                            Password = _dataModel.Password,
                            FirstName = _dataModel.FirstName,
                            LastName = _dataModel.LastName
                        },
                        _tokenManager.Token);

                else
                    _registerScribeSuccess = (false, "Registration failed. Try again.");

            });

            if(! t.Wait(10000))
                _registerScribeSuccess = (false, "Registration failed. Try again.");

        }

        private void worker_RegisterScribeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            (bool success, string message) = _registerScribeSuccess;

            RegisterProgressBar.Visibility = Visibility.Hidden;
            RegisterMessage.Visibility = Visibility.Visible;

            if (success)
            {
                RegisterMessage.Foreground = Brushes.Green;
                RegisterMessage.Text = message;
                ScribesDialogHost.IsOpen = false;
            }
            else
            {
                RegisterMessage.Foreground = Brushes.Red;
                RegisterMessage.Text = message;
            }
        }


        private void RegisterScribeCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterMessage.Visibility = Visibility.Hidden;
        }


        private void DeleteScribeBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditScribeBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataModel.EditScribeFirstName = _dataModel.SelectedScribe.FirstName;
            _dataModel.EditScribeLastName = _dataModel.SelectedScribe.LastName;
        }

        private void EditScribeSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateMessage.Visibility = Visibility.Hidden;
            UpdateProgressBar.Visibility = Visibility.Visible;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += woker_UpdateScribe;
            worker.RunWorkerCompleted += worker_UpdateScribeComplete;
            worker.RunWorkerAsync();
        }


        private void woker_UpdateScribe(object sender, DoWorkEventArgs e)
        {
            UsersController uc = new UsersController();

            if (_tokenManager.Token != "")

                _updateScribeSuccess = uc.UpdateScribe(
                    new EditUserForm
                    {
                        UserId = _dataModel.SelectedScribe.Id,
                        Email = _dataModel.Email,
                        FirstName = _dataModel.EditScribeFirstName,
                        LastName = _dataModel.EditScribeLastName
                    },
                    _tokenManager.Token);

            else
                _registerScribeSuccess = (false, "Update failed. Try again.");

            //Task t = Task.Run(() => {
  

            //});

            //if (!t.Wait(10000))
            //    _registerScribeSuccess = (false, "Update failed. Try again.");
        }


        private void worker_UpdateScribeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            (bool success, string message) = _updateScribeSuccess;

            UpdateProgressBar.Visibility = Visibility.Hidden;
            

            if(success)
            {
                ScribesDialogHost.IsOpen = false;
                _dataModel.Scribes[ScribesList.SelectedIndex].FirstName =
                    EditFirstNameTextBox.Text;
                _dataModel.Scribes[ScribesList.SelectedIndex].LastName =
                    EditLastNameTextBox.Text;
            }
            else
            {
                UpdateMessage.Foreground = Brushes.Red;
                UpdateMessage.Text = message;
                UpdateMessage.Visibility = Visibility.Visible;               
            }
        }


        private void ResetPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            // Clear Fields
            ResetPasswordBox.Password = null;
            ConfirmResetPasswordBox.Password = null;
        }


        private void ResetPasswordSubmitBtn_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ScribesListBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (ScribesList.SelectedIndex >= 0)
            {
                AssignedScribesList.SelectedIndex = -1;

                AddScribeBtn.IsEnabled = _appDataModel.MeetSelected;

                _dataModel.SelectedScribe = (User)ScribesList.SelectedItem;
            }
            else
            {
                AddScribeBtn.IsEnabled = false;
            }
        }

        private void AssignedScribesListBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (AssignedScribesList.SelectedIndex >= 0)
            {
                ScribesList.SelectedIndex = -1;

                RemoveScribeBtn.IsEnabled = true;

                _dataModel.SelectedScribe = (User)AssignedScribesList.SelectedItem;
            }
            else
            {
                RemoveScribeBtn.IsEnabled = false;
                _dataModel.SelectedScribe = null;
            }
        }


        private void AddScribeBtn_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_AssignScribe;
            worker.RunWorkerCompleted += worker_AssignScribeComplete;
            worker.RunWorkerAsync();
        }

        private void worker_AssignScribe(object sender, DoWorkEventArgs e)
        {
            UsersController uc = new UsersController();

            if (_tokenManager.Token != "")
                _assignScribeSuccess = uc.AssignScribe(
                    _appDataModel.SelectedMeet.MeetId,
                    _dataModel.SelectedScribe.Id, _tokenManager.Token);

            else
                _assignScribeSuccess = false;
        }

        private void worker_AssignScribeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_assignScribeSuccess)
            {
                _dataModel.Scribes.RemoveAt(ScribesList.SelectedIndex);
                _dataModel.AssignedScribes.Add(_dataModel.SelectedScribe);

                RefreshAssignedScribesList();
                RefreshScribesList();

                _dataModel.SelectedScribe = null;
            }
        }

        private void RemoveScribeBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataModel.SelectedScribe = (User)AssignedScribesList.SelectedItem;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_RemoveScribe;
            worker.RunWorkerCompleted += worker_RemoveScribeComplete;
            worker.RunWorkerAsync();
        }

        private void worker_RemoveScribe(object sender, DoWorkEventArgs e)
        {
            UsersController uc = new UsersController();

            if (_tokenManager.Token != "")
                _removeScribeSuccess = uc.RemoveScribe(
                    _appDataModel.SelectedMeet.MeetId,
                    _dataModel.SelectedScribe.Id, _tokenManager.Token);

            else
                _removeScribeSuccess = false;
        }

        private void worker_RemoveScribeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_removeScribeSuccess)
            {
                _dataModel.AssignedScribes.RemoveAt(
                    AssignedScribesList.SelectedIndex);
                _dataModel.Scribes.Add(_dataModel.SelectedScribe);

                RefreshScribesList();
                RefreshAssignedScribesList();

                _dataModel.SelectedScribe = null;
            }
        }


        private void RefreshScribesList()
        {
            _dataModel.Scribes = _dataModel.Scribes
                .OrderBy(scribe => scribe.LastName).ToList();

            ScribesList.ItemsSource = _dataModel.Scribes
                .Select(scribe => new User
                {
                    Id = scribe.Id,
                    FirstName = scribe.FirstName,
                    LastName = scribe.LastName,
                    Email = scribe.Email,
                    CreatedAt = scribe.CreatedAt,
                    Roles = scribe.Roles
                });
        }

        private void RefreshAssignedScribesList()
        {
            _dataModel.AssignedScribes = _dataModel.AssignedScribes
                .OrderBy(scribe => scribe.LastName).ToList();

            AssignedScribesList.ItemsSource = _dataModel.AssignedScribes
                .Select(scribe => new User
                {
                    Id = scribe.Id,
                    FirstName = scribe.FirstName,
                    LastName = scribe.LastName,
                    Email = scribe.Email,
                    CreatedAt = scribe.CreatedAt,
                    Roles = scribe.Roles
                });
        }

        IList<User> GetAllScribes(string[] searchTerms)
        {
            UsersController uc = new UsersController();

            if (TokenManager.Instance.Token != "")
            {
                (bool success, IList<User> users) =
                    uc.GetScribes(searchTerms, TokenManager.Instance.Token);

                return users;
            }
            return null;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Length >= 8)
            {
                PasswordCheck.Visibility = Visibility.Visible;

                if (PasswordConfirmBox.Password == PasswordBox.Password)
                {
                    _dataModel.PasswordValid = true;
                    PasswordConfirmCheck.Visibility = Visibility.Visible;
                }
                else
                {
                    _dataModel.PasswordValid = false;
                    PasswordConfirmCheck.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                _dataModel.PasswordValid = false;
                PasswordCheck.Visibility = Visibility.Hidden;
                PasswordConfirmCheck.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordConfirmBox.Password.Length >= 8 &&
                PasswordConfirmBox.Password == PasswordBox.Password)
            {
                _dataModel.PasswordValid = true;
                PasswordConfirmCheck.Visibility = Visibility.Visible;
            }
            else
            {
                _dataModel.PasswordValid = false;
                PasswordConfirmCheck.Visibility = Visibility.Hidden;
            }
        }


        private void ResetPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ResetPasswordBox.Password.Length >= 8)
            {
                ResetPasswordCheck.Visibility = Visibility.Visible;

                if (ConfirmResetPasswordBox.Password == ResetPasswordBox.Password)
                {
                    _dataModel.ResetPasswordValid = true;
                    ConfirmResetPasswordCheck.Visibility = Visibility.Visible;
                }

                else
                {
                    _dataModel.ResetPasswordValid = false;
                    ConfirmResetPasswordCheck.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                _dataModel.ResetPasswordValid = false;
                ResetPasswordCheck.Visibility = Visibility.Hidden;
                ConfirmResetPasswordCheck.Visibility = Visibility.Hidden;
            }
        }

        private void ConfirmResetPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ConfirmResetPasswordBox.Password.Length >= 8 &&
                ConfirmResetPasswordBox.Password == ResetPasswordBox.Password)
            {
                _dataModel.ResetPasswordValid = true;
                ConfirmResetPasswordCheck.Visibility = Visibility.Visible;
            }
            else
            {
                _dataModel.ResetPasswordValid = false;
                ConfirmResetPasswordCheck.Visibility = Visibility.Hidden;
            }
        }


        private void ScribesDialogHost_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            EditFirstNameTextBox.CaretIndex = EditFirstNameTextBox.Text.Length;
            EditLastNameTextBox.CaretIndex = EditLastNameTextBox.Text.Length;
        }
    }
}

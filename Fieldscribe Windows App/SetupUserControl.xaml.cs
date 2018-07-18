using Fieldscribe_Windows_App.Controllers;
using Fieldscribe_Windows_App.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
using System.IO;
using System.Windows.Threading;
using System.ComponentModel;

namespace Fieldscribe_Windows_App
{
    /// <summary>
    /// Interaction logic for SetupUserControl.xaml
    /// </summary>
    public partial class SetupUserControl : UserControl
    {
        private SetupPanelDataModel _dataModel;
        private string path = string.Empty;
        private enum DialogStatus { Edit, Create };
        private (bool, string) _deleteMeetSuccess;
        private (bool, string) _editMeetSuccess;

        public SetupUserControl()
        {
            InitializeComponent();

            _dataModel = SetupPanelDataModel.Instance;
            this.DataContext = SetupPanelDataModel.Instance;
           
            CreateMeetMeasurementPicker.ItemsSource = new string[] { "English", "Metric" };
            EditMeetMeasurementPicker.ItemsSource = new string[] { "English", "Metric" };
        }


        // Event Handlers

        #region Event Handlers

        private void SelectFolderBtn_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog folderDialog =
                new System.Windows.Forms.FolderBrowserDialog();

            folderDialog.ShowDialog();

            path = folderDialog.SelectedPath;

            if (path != null && path != "")
            {
                SelectFolderText.Text = path;
                SelectFolderBtn.Background = Brushes.Green;
                SelectFolderBtn.BorderBrush = Brushes.Green;
                RaiseEvent(new RoutedEventArgs(FolderSelectionChanged));
            }
        }

        private void EditMeetBtn_Click(object sender, RoutedEventArgs e)
        {
            EditMeetProgressBar.Visibility = Visibility.Hidden;
            EditMeetMessage.Visibility = Visibility.Hidden;

            _dataModel.MeetName = _dataModel.SelectedMeet.MeetName;
            _dataModel.MeetLocation = _dataModel.SelectedMeet.MeetLocation;
            _dataModel.MeetDate = _dataModel.SelectedMeet.MeetDate;
            _dataModel.MeasurementType = _dataModel.SelectedMeet.MeasurementType;
        }

        private void DeleteMeetBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteMeetProgressBar.Visibility = Visibility.Hidden;
            DeleteMeetMessage.Visibility = Visibility.Hidden;
        }

        private void CreateMeetBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateMeetProgressBar.Visibility = Visibility.Hidden;
            CreateMeetMessage.Visibility = Visibility.Hidden;

            ClearCreateMeetFields();
        }
        
        private void CreateMeetCreateBtnClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CreateMeetBtnClicked));   
        }

        private void MeetPicker_DropDownClosed(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(MeetSelectionChanged));
        }

        private void MeetPicker_DropDownOpen(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(MeetPickerOpen));
        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));
        }

        private void StartStopBtn_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(StartStopBtnClicked));
        }

        private void EditMeetSaveBtnClick(object sender, RoutedEventArgs e)
        {
            EditMeetProgressBar.Visibility = Visibility.Visible;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_EditMeet;
            worker.RunWorkerCompleted += worker_EditMeetComplete;
            worker.RunWorkerAsync();
        }

        private void worker_EditMeet(object sender, DoWorkEventArgs e)
        {
            MeetsController mc = new MeetsController();

            _editMeetSuccess = mc.EditMeet(new Meet
                {
                    MeetId = _dataModel.SelectedMeet.MeetId,
                    MeetName = _dataModel.MeetName,
                    MeetLocation = _dataModel.MeetLocation,
                    MeetDate = (DateTime)_dataModel.MeetDate,
                    MeasurementType = _dataModel.MeasurementType
                });
        }

        private void worker_EditMeetComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            (bool success, string message) = _editMeetSuccess;

            if(success)
            {
                RaiseEvent(new RoutedEventArgs(MeetSaved));
                RaiseEvent(new RoutedEventArgs(CloseDialog));
            }
            else
            {
                EditMeetMessage.Text = message;
                EditMeetMessage.Visibility = Visibility.Visible;
            }
        }

        private void DeleteMeetYesBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteMeetProgressBar.Visibility = Visibility.Visible;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DeleteMeet;
            worker.RunWorkerCompleted += worker_DeleteMeetComplete;
            worker.RunWorkerAsync();
        }

        private void worker_DeleteMeet(object sender, DoWorkEventArgs e)
        {
            MeetsController mc = new MeetsController();

            _deleteMeetSuccess = mc.DeleteMeet(
                _dataModel.SelectedMeet.MeetId);
        }

        private void worker_DeleteMeetComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            (bool success, string message) = _deleteMeetSuccess;

            DeleteMeetProgressBar.Visibility = Visibility.Hidden;

            if (success)
            {
                RaiseEvent(new RoutedEventArgs(MeetDeleted));
                RaiseEvent(new RoutedEventArgs(CloseDialog));
            }
            else
            {
                DeleteMeetMessage.Text = message;
                DeleteMeetMessage.Visibility = Visibility.Visible;
            }
        }

        #endregion


        // Helper Functions

        #region Helper Functions


        private void ClearCreateMeetFields()
        {
            _dataModel.MeetName = null;
            _dataModel.MeetLocation = null;
            _dataModel.MeetDate = null;
            CreateMeetMeasurementPicker.SelectedIndex = -1;
            CreateMeetMeasurementPicker.SelectedValue = null;
        }

        #endregion


        // Routed Event Handlers

        #region Routed Event Handlers

        public static readonly RoutedEvent MeetSelectionChanged =
            EventManager.RegisterRoutedEvent("MeetPicker_SelectionChanged",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler MeetSelection_Changed
        {
            add { AddHandler(MeetSelectionChanged, value); }
            remove { RemoveHandler(MeetSelectionChanged, value); }
        }

        public static readonly RoutedEvent MeetPickerOpen =
            EventManager.RegisterRoutedEvent("MeetPicker_Open",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler MeetPicker_Open
        {
            add { AddHandler(MeetPickerOpen, value); }
            remove { RemoveHandler(MeetPickerOpen, value); }
        }

        public static readonly RoutedEvent FolderSelectionChanged =
            EventManager.RegisterRoutedEvent("SelectFolderBtn_Click",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler FolderSelection_Changed
        {
            add { AddHandler(FolderSelectionChanged, value); }
            remove { RemoveHandler(FolderSelectionChanged, value); }
        }

        public static readonly RoutedEvent StartStopBtnClicked =
            EventManager.RegisterRoutedEvent("StartStopBtn_Click",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler StartStopBtn_Clicked
        {
            add { AddHandler(StartStopBtnClicked, value); }
            remove { RemoveHandler(StartStopBtnClicked, value); }
        }

        public static readonly RoutedEvent MeetDeleted =
            EventManager.RegisterRoutedEvent("Meet_Deleted",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler Meet_Deleted
        {
            add { AddHandler(MeetDeleted, value); }
            remove { RemoveHandler(MeetDeleted, value); }
        }

        public static readonly RoutedEvent EditBtnClicked =
            EventManager.RegisterRoutedEvent("EditBtn_Click",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler EditBtn_Clicked
        {
            add { AddHandler(EditBtnClicked, value); }
            remove { RemoveHandler(EditBtnClicked, value); }
        }

        public static readonly RoutedEvent CreateMeetBtnClicked =
            EventManager.RegisterRoutedEvent("CreateMeetBtn_Click",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler CreateMeetBtn_Clicked
        {
            add { AddHandler(CreateMeetBtnClicked, value); }
            remove { RemoveHandler(CreateMeetBtnClicked, value); }
        }

        public static readonly RoutedEvent MeetSaved =
            EventManager.RegisterRoutedEvent("Meet_Saved",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler Meet_Saved
        {
            add { AddHandler(MeetSaved, value); }
            remove { RemoveHandler(MeetSaved, value); }
        }

        public static readonly RoutedEvent CloseDialog =
            EventManager.RegisterRoutedEvent("Close_Dialog",
                RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SetupUserControl));

        public event RoutedEventHandler Close_Dialog
        {
            add { AddHandler(CloseDialog, value); }
            remove { AddHandler(CloseDialog, value); }
        }

        #endregion
    }
}

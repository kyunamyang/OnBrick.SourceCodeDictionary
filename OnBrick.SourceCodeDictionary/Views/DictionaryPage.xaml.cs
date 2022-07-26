using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using OnBrick.SourceCodeDictionary.Library;
using OnBrick.SourceCodeDictionary.Library.Models;
using OnBrick.SourceCodeDictionary.Library.Walker;
using OnBrick.SourceCodeDictionary.Models;
using OnBrick.SourceCodeDictionary.Services;
using OnBrick.SourceCodeDictionary.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Services.Store;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace OnBrick.SourceCodeDictionary.Views
{
    public sealed partial class DictionaryPage : Page
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public DictionaryViewModel ViewModel { get; } = new DictionaryViewModel();

        private DispatcherTimer timer;
        ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView();
        private string _targetFileExtension = ".cs";

        public DictionaryPage()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += ttSavedTimer_Tick;
        }

        private void ShowNotice(string message, bool isAutoClose, int interval = 2)
        {
            ttNotice.Subtitle = message;
            ttNotice.IsOpen = true;

            if (isAutoClose)
            {
                timer.Interval = TimeSpan.FromSeconds(interval);
                timer.Start();
            }
        }

        // 들어올때
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.SizeChanged += frame_SizeChanged;

            SetGenerateButtonStatus();
        }

        // 나갈때
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Frame.SizeChanged -= frame_SizeChanged;
        }

        private void SetGenerateButtonStatus()
        {
            if (ViewModel.GridItems.Count > 0)
            {
                btnExport.IsEnabled = true;
            }
            else
            {
                btnExport.IsEnabled = false;
            }
        }

        #region button event

        private async void btnSourceFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
                filePicker.FileTypeFilter.Add(_targetFileExtension);

                List<DocumentModel> documents = new List<DocumentModel>();
                IReadOnlyList<StorageFile> files = await filePicker.PickMultipleFilesAsync();

                if (files != null)
                {
                    gridMain.ItemsSource = null;

                    DocumentWalker w = new DocumentWalker();
                    foreach (var file in files)
                    {
                        string filePath = file.Path;
                        string text = await FileIO.ReadTextAsync(file);

                        documents.Add(w.GetDocumentModel(filePath, text));
                    }

                    await ViewModel.Load(documents);
                }

                gridMain.ItemsSource = ViewModel.GridItems;
                gridMain.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                gridMain.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                SetGenerateButtonStatus();
            }
            catch(Exception ex)
            {
                ShowNotice(ex.Message, true, 3);
            }
            finally
            {

            }
        }

        private async void btnSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                var folder = await folderPicker.PickSingleFolderAsync();
                Dictionary<string, StorageFile> filteredList = new Dictionary<string, StorageFile>();

                if (folder != null)
                {
                    await dispatcherQueue.EnqueueAsync(() => ViewModel.IsLoading = true);
                    await Task.Delay(10);

                    var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName); // 하위 폴더 포함
                    ////if (files.Count > 1000)
                    ////{
                    ////    ContentDialog dialog = new ContentDialog();

                    ////    // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                    ////    dialog.XamlRoot = this.XamlRoot;
                    ////    //dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    ////    dialog.Title = _resourceLoader.GetString("txt_confirm"); // [WORD] Confirm
                    ////    dialog.Content = _resourceLoader.GetString("txt_message_1") + Environment.NewLine + _resourceLoader.GetString("txt_continue"); // [WORD] "지정한 폴더와 하위 폴더에 1000개가 넘는 파일이 있습니다.\r\n계속 하시겠습니까?"
                    ////    dialog.PrimaryButtonText = "Yes";
                    ////    dialog.CloseButtonText = "No";
                    ////    dialog.DefaultButton = ContentDialogButton.Primary;

                    ////    var result = await dialog.ShowAsync();
                    ////    if (result != ContentDialogResult.Primary)
                    ////    {
                    ////        return;
                    ////    }
                    ////}

                    foreach (var file in files)
                    {
                        if (file.Path.Contains("\\bin\\"))
                            continue;
                        if (file.Path.Contains("\\obj\\"))
                            continue;
                        if (Path.GetExtension(file.Path).ToLower() == _targetFileExtension)
                        {
                            filteredList.Add(file.Path, file);
                        }
                    }

                    if (filteredList.Count > 0)
                    {
                        if (filteredList.Count > 1000)
                        {
                            ContentDialog dialog = new ContentDialog();

                            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                            dialog.XamlRoot = this.XamlRoot;
                            //dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                            dialog.Title = _resourceLoader.GetString("txt_confirm"); // [WORD] Confirm
                            dialog.Content = _resourceLoader.GetString("txt_message_2") + Environment.NewLine + _resourceLoader.GetString("txt_continue");  // [WORD] "대상 파일이 1000개가 넘어 처리에 시간이 걸릴 수 있습니다.\r\n계속 하시겠습니까?"; 
                            dialog.PrimaryButtonText = "Yes";
                            dialog.CloseButtonText = "No";
                            dialog.DefaultButton = ContentDialogButton.Primary;

                            var result = await dialog.ShowAsync();
                            if (result != ContentDialogResult.Primary)
                            {
                                return;
                            }
                        }

                        List<DocumentModel> documents = new List<DocumentModel>();
                        gridMain.ItemsSource = null;

                        DocumentWalker w = new DocumentWalker();
                        foreach (var el in filteredList)
                        {
                            string filePath = el.Key;
                            string text = await FileIO.ReadTextAsync(el.Value);

                            documents.Add(w.GetDocumentModel(filePath, text));
                        }

                        await ViewModel.Load(documents);
                    }

                    gridMain.ItemsSource = ViewModel.GridItems;
                    gridMain.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    gridMain.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                    SetGenerateButtonStatus();
                }
            }
            catch (Exception ex)
            {
                // show messageBox
                ShowNotice(ex.Message, true, 3);
            }
            finally
            {
                await dispatcherQueue.EnqueueAsync(() => ViewModel.IsLoading = false);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            gridMain.ItemsSource = null;
            ViewModel.Clear();

            SetGenerateButtonStatus();
        }

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.GridItems == null || ViewModel.GridItems.Count == 0)
                    return;

                var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                    FileService fs = new FileService();
                    await fs.SaveExcelFile(folder, ViewModel.GridItems);

                    var resourceLoader = ResourceLoader.GetForCurrentView();
                    ShowNotice(resourceLoader.GetString("txt_saved"), true, 2);
                }
            }
            catch (Exception ex)
            {
                // show messageBox
                ShowNotice(ex.Message, true, 3);
            }
            finally
            {
            }
        }

        #endregion

        #region grid event

        private void gridMain_Loaded(object sender, RoutedEventArgs e)
        {
            var h = Frame.ActualHeight - cbLeft.ActualOffset.Y - cbLeft.ActualHeight - 100;
            gridMain.Height = h;
            pbGrid.Width = gridMain.ActualWidth;

            gridMain.Width = rpMain.ActualWidth - 20;
        }

        private void gridMain_LayoutUpdated(object sender, object e)
        {
            var h = Frame.ActualHeight - cbLeft.ActualOffset.Y - cbLeft.ActualHeight - 100;
            gridMain.Height = h;
            pbGrid.Width = gridMain.ActualWidth;
        }

        private async void gridMain_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            await dispatcherQueue.EnqueueAsync(() => ViewModel.IsLoading = true);

            await Task.Delay(10);

            try
            {
                var tag = e.Column.Tag.ToString();
                Func<GridItemModel, object> orderByFunc = null;
                if (tag == "Seq")
                {
                    orderByFunc = el => el.Sequence;
                }
                else if (tag == "File")
                {
                    orderByFunc = el => el.File;
                }
                else if (tag == "Namespace")
                {

                    orderByFunc = el => el.Namespace;
                }
                else if (tag == "FQDN")
                {
                    orderByFunc = el => el.FQDN;
                }
                else if (tag == "MemberType")
                {
                    orderByFunc = el => el.MemberType;

                }
                else if (tag == "Public")
                {
                    orderByFunc = el => el.Public;

                }
                else if (tag == "Protected")
                {
                    orderByFunc = el => el.Protected;

                }
                else if (tag == "Private")
                {
                    orderByFunc = el => el.Private;

                }
                else if (tag == "Internal")
                {
                    orderByFunc = el => el.Internal;

                }
                else if (tag == "Partial")
                {
                    orderByFunc = el => el.Partial;

                }
                else if (tag == "Static")
                {
                    orderByFunc = el => el.Static;

                }
                else if (tag == "New")
                {
                    orderByFunc = el => el.New;

                }
                else if (tag == "Abstract")
                {
                    orderByFunc = el => el.Abstract;

                }
                else if (tag == "Override")
                {
                    orderByFunc = el => el.Override;

                }
                else if (tag == "Type")
                {
                    orderByFunc = el => el.Type;

                }
                else if (tag == "ReturnType")
                {
                    orderByFunc = el => el.ReturnType;

                }
                else //if (tag == "Comment")
                {
                    orderByFunc = el => el.Identifier;
                }
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    gridMain.ItemsSource = ViewModel.GridItems.OrderBy(el => orderByFunc(el));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    gridMain.ItemsSource = ViewModel.GridItems.OrderByDescending(el => orderByFunc(el));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }

                // Remove sorting indicators from other columns
                foreach (var dgColumn in gridMain.Columns)
                {
                    if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                    {
                        dgColumn.SortDirection = null;
                    }
                }
            }
            catch
            {

            }
            finally
            {
                await dispatcherQueue.EnqueueAsync(() => ViewModel.IsLoading = false);
            }
        }

        #endregion

        #region etc event

        private void ttSavedTimer_Tick(object sender, object e)
        {
            (sender as DispatcherTimer).Stop();

            if (ttNotice == null)
                return;

            ttNotice.IsOpen = false;
        }

        private void frame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridMain.Width = rpMain.ActualWidth - 20;
        }

        #endregion

    }
}
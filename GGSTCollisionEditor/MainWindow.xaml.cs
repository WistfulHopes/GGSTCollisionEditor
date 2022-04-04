using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Foundation;
using Microsoft.UI;
using System.Text;
using Windows.Storage.FileProperties;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GGSTCollisionEditor
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        PACFile col;
        int oldListIndex = -1;
        OverlaidImage overlaidImage;
        StorageFile file;
        StorageFile tempFile;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Guilty Gear -Strive- Collision Editor";
        }

        private async void openFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            // Use file picker like normal!
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".pac");

            StorageFile newFile = await openPicker.PickSingleFileAsync();
            if (newFile != null)
            {
                file = newFile;
                Stream stream = (await file.OpenReadAsync()).AsStreamForRead();
                col = new PACFile(stream);
                SpriteList.Items.Clear();
                foreach (var entry in col.pacentries)
                {
                    SpriteList.Items.Add(entry);
                }
                stream.Close();
                StorageFolder Folder1 = ApplicationData.Current.TemporaryFolder;
                tempFile = await Folder1.CreateFileAsync("tempPAC\\" + file.Name, CreationCollisionOption.ReplaceExisting);
                canvas.Invalidate();
            }
        }

        private void SpriteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpriteList.Items.Count > 0)
            {
                PACFile.PACEntry colentry = (PACFile.PACEntry)SpriteList.Items[SpriteList.SelectedIndex];
                var colOffset = col.getOffsetByName(colentry.name);
                oldListIndex = SpriteList.SelectedIndex;
                overlaidImage = new OverlaidImage(file, colOffset);

                BoxList.Items.Clear();
                foreach (var box in overlaidImage.hurtboxes)
                {
                    BoxList.Items.Add(box);
                }
                foreach (var box in overlaidImage.hitboxes)
                {
                    BoxList.Items.Add(box);
                }
                if (BoxList.Items.Count != 0)
                {
                    BoxList.SelectedIndex = 0;
                }
                canvas.Invalidate();
            }
        }

        private void BoxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoxList.Items.Count > 0)
            {
                JonbinBox currbox = (JonbinBox)BoxList.Items[BoxList.SelectedIndex];
                xPos.TextChanged -= xPos_TextChanged;
                yPos.TextChanged -= yPos_TextChanged;
                xScl.TextChanged -= xScl_TextChanged;
                yScl.TextChanged -= yScl_TextChanged;
                xPos.Text = currbox.x.ToString();
                yPos.Text = currbox.y.ToString();
                xScl.Text = currbox.width.ToString();
                yScl.Text = currbox.height.ToString();
                xPos.TextChanged += xPos_TextChanged;
                yPos.TextChanged += yPos_TextChanged;
                xScl.TextChanged += xScl_TextChanged;
                yScl.TextChanged += yScl_TextChanged;
            }
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.Clear(Colors.Black);
            if (overlaidImage != null)
            {
                int choffsetx = 640;
                int choffsety = 802;
                if (overlaidImage.chunks.Count > 0)
                {
                    choffsetx = -(int)overlaidImage.chunks[0].DestX;
                    choffsety = -(int)overlaidImage.chunks[0].DestY;
                }
                foreach (var box in overlaidImage.hurtboxes)
                {
                    float tempx = box.x + choffsetx;
                    float tempy = box.y + choffsety;
                    args.DrawingSession.DrawRectangle(new Rect(tempx, tempy, box.width, box.height), Colors.Green);
                }
                foreach (var box in overlaidImage.hitboxes)
                {
                    float tempx = box.x + choffsetx;
                    float tempy = box.y + choffsety;
                    args.DrawingSession.DrawRectangle(new Rect(tempx, tempy, box.width, box.height), Colors.Red);
                }
            }
        }

        private void xPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            JonbinBox currbox = (JonbinBox)BoxList.Items[BoxList.SelectedIndex];
            float newx;
            if (float.TryParse(xPos.Text, out newx))
            {
                currbox.x = newx;
                canvas.Invalidate();
                saveTempFile();
            }
        }

        private void yPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            JonbinBox currbox = (JonbinBox)BoxList.Items[BoxList.SelectedIndex];
            float newy;
            if (float.TryParse(yPos.Text, out newy))
            {
                currbox.y = newy;
                canvas.Invalidate();
                saveTempFile();
            }
        }

        private void xScl_TextChanged(object sender, TextChangedEventArgs e)
        {
            JonbinBox currbox = (JonbinBox)BoxList.Items[BoxList.SelectedIndex];
            float neww;
            if (float.TryParse(xScl.Text, out neww))
            {
                currbox.width = neww;
                canvas.Invalidate();
                saveTempFile();
            }
        }

        private void yScl_TextChanged(object sender, TextChangedEventArgs e)
        {
            JonbinBox currbox = (JonbinBox)BoxList.Items[BoxList.SelectedIndex];
            float newh;
            if (float.TryParse(yScl.Text, out newh))
            {
                currbox.height = newh;
                canvas.Invalidate();
                saveTempFile();
            }
        }
        private async void saveTempFile()
        {
            if (tempFile != null)
            {
                PACFile.PACEntry colentry = (PACFile.PACEntry)SpriteList.Items[SpriteList.SelectedIndex];
                byte[] oldpac = File.ReadAllBytes(file.Path);
                int writestart = (int)(colentry.offset + col.data_start);
                int writeend = writestart;
                writeend += 4;
                writeend += 2 + BitConverter.ToInt16(oldpac, writeend) * 0x20 + 3;
                var chunkssize = BitConverter.ToInt32(oldpac, writeend) * 0x50;
                writeend += 10 + 41 * 2 + chunkssize;
                int boxcount = BoxList.Items.Count;
                byte[] currfloat = { 0, 0, 0, 0 };
                for (var i = 0; i < boxcount; i++)
                {
                    writeend += 4;
                    var currbox = (JonbinBox)BoxList.Items[i];
                    currfloat = System.BitConverter.GetBytes(currbox.x);
                    Array.Copy(currfloat, 0, oldpac, writeend, 4);
                    writeend += 4;
                    currfloat = System.BitConverter.GetBytes(currbox.y);
                    Array.Copy(currfloat, 0, oldpac, writeend, 4);
                    writeend += 4; currfloat = System.BitConverter.GetBytes(currbox.width);
                    Array.Copy(currfloat, 0, oldpac, writeend, 4);
                    writeend += 4; currfloat = System.BitConverter.GetBytes(currbox.height);
                    Array.Copy(currfloat, 0, oldpac, writeend, 4);
                    writeend += 4;
                }
                await Windows.Storage.FileIO.WriteBytesAsync(tempFile, oldpac);
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    file = tempFile;
                    return;
                }
                else
                {
                    ContentDialog contentDialog = new ContentDialog()
                    {
                        Title = "Failed",
                        Content = "Temp file failed to save!",
                        CloseButtonText = "OK",
                    };
                    await contentDialog.ShowAsync();
                }
            }
        }

        private async void saveFile_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            // Use file picker like normal!
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("PAC", new List<string>() { ".pac" });

            StorageFile savefile = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                File.Move(file.Path, savefile.Path, true);
                file = savefile;
                canvas.Invalidate();
            }
        }
    }
}

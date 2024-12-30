using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Lame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using File = TagLib.File;
using NAudio.Wave.SampleProviders;
using NVorbis;

namespace Convi
{
    public partial class MainWindow : Window
    {
        private string? sciezkaPliku;
        private string? sciezkaKatalogu;
        private readonly Dictionary<string, int> jakoscBitrate = new Dictionary<string, int>
        {
            { "Najwyższa", 320 },
            { "Wysoka", 256 },
            { "Średnia", 192 },
            { "Niska", 128 }
        };
        private readonly BackgroundWorker worker;

        public MainWindow()
        {
            InitializeComponent();
            cmbFormatDocelowy.SelectedIndex = 0;
            cmbJakosc.SelectedIndex = 0;
            cmbPoziomKompresji.SelectedIndex = 5;

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            cmbFormatDocelowy.SelectionChanged += CmbFormatDocelowy_SelectionChanged;
        }

        private void AktualizujInformacjeOPliku()
        {
            if (string.IsNullOrEmpty(sciezkaPliku)) return;

            try
            {
                using var reader = new AudioFileReader(sciezkaPliku);
                var tagFile = TagLib.File.Create(sciezkaPliku);

                Dispatcher.Invoke(() =>
                {
                    txtFormat.Text = Path.GetExtension(sciezkaPliku).TrimStart('.');
                    txtDlugosc.Text = $"{tagFile.Properties.Duration:mm\\:ss}";
                    txtAktualnyBitrate.Text = $"{tagFile.Properties.AudioBitrate} kbps";
                    txtCzestotliwosc.Text = $"{reader.WaveFormat.SampleRate} Hz";
                });
            }
            catch (Exception ex)
            {
                DodajLog($"Błąd podczas odczytywania informacji o pliku: {ex.Message}");
            }
        }

        private void CmbFormatDocelowy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFormat = (cmbFormatDocelowy.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool pokazBitrate = selectedFormat != "FLAC" && selectedFormat != "WAV" && selectedFormat != "AIFF";

            txtBitrateLabel.Visibility = pokazBitrate ? Visibility.Visible : Visibility.Collapsed;
            txtBitrate.Visibility = pokazBitrate ? Visibility.Visible : Visibility.Collapsed;
            txtBitrateUnit.Visibility = pokazBitrate ? Visibility.Visible : Visibility.Collapsed;
            txtPoziomKompresjiLabel.Visibility = !pokazBitrate ? Visibility.Visible : Visibility.Collapsed;
            cmbPoziomKompresji.Visibility = !pokazBitrate ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void BtnKonwertuj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(sciezkaPliku) && lstPliki.Items.Count == 0)
            {
                MessageBox.Show("Wybierz plik lub katalog do konwersji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BlokujKontroli(false);
            var selectedItem = cmbFormatDocelowy.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Wybierz format docelowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                BlokujKontroli(true);
                return;
            }

            string formatDocelowy = selectedItem.Content.ToString() ?? "mp3";

            if (lstPliki.Items.Count > 0)
            {
                await KonwertujWszystkiePliki(formatDocelowy);
            }
            else
            {
                worker.RunWorkerAsync(formatDocelowy);
            }
        }

        private void KonwertujPlik(string formatDocelowy)
        {
            if (string.IsNullOrEmpty(sciezkaPliku)) return;

            string sciezkaWyjsciowa = GenerujSciezkeWyjsciowa(sciezkaPliku, formatDocelowy);

            try
            {
                using var reader = new AudioFileReader(sciezkaPliku);
                var normalizacja = Dispatcher.Invoke(() => chkNormalizujGlosnosc.IsChecked ?? false);

                if (normalizacja)
                {
                    AplikujNormalizacje(reader);
                }

                switch (formatDocelowy.ToLower())
                {
                    case "mp3":
                        KonwertujDoMP3(reader, sciezkaWyjsciowa);
                        break;

                    case "wav":
                        KonwertujDoWAV(reader, sciezkaWyjsciowa);
                        break;

                    case "flac":
                        KonwertujDoFLAC(reader, sciezkaWyjsciowa);
                        break;

                    case "ogg":
                        KonwertujDoOGG(reader, sciezkaWyjsciowa);
                        break;

                    case "aac":
                        KonwertujDoAAC(reader, sciezkaWyjsciowa);
                        break;

                    case "m4a":
                        KonwertujDoM4A(reader, sciezkaWyjsciowa);
                        break;

                    case "aiff":
                        KonwertujDoAIFF(reader, sciezkaWyjsciowa);
                        break;

                    default:
                        throw new NotSupportedException($"Format {formatDocelowy} nie jest obsługiwany.");
                }

                if (Dispatcher.Invoke(() => chkZachowajMetadane.IsChecked ?? false))
                {
                    KopiujMetadane(sciezkaPliku, sciezkaWyjsciowa);
                }

                DodajLog($"Konwersja pliku {Path.GetFileName(sciezkaPliku)} zakończona pomyślnie.");
            }
            catch (Exception ex)
            {
                DodajLog($"Błąd podczas konwersji: {ex.Message}");
                throw;
            }
        }

        private void KonwertujDoMP3(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            int bitrate = Dispatcher.Invoke(() =>
            {
                var selectedItem = cmbJakosc.SelectedItem as ComboBoxItem;
                string jakoscText = selectedItem?.Content.ToString() ?? "Wysoka";
                return jakoscBitrate[jakoscText];
            });

            using var writer = new LameMP3FileWriter(sciezkaWyjsciowa, reader.WaveFormat, bitrate);
            reader.CopyTo(writer);
        }

        private void KonwertujDoWAV(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            WaveFileWriter.CreateWaveFile(sciezkaWyjsciowa, reader);
        }

        private void KonwertujDoFLAC(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            using var conversionStream = new WaveFormatConversionStream(
                new WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels),
                reader);

            using var fileStream = new FileStream(sciezkaWyjsciowa, FileMode.Create);
            using var waveWriter = new WaveFileWriter(fileStream, conversionStream.WaveFormat);

            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = conversionStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                waveWriter.Write(buffer, 0, bytesRead);
            }
        }

        private void KonwertujDoOGG(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            int quality = Dispatcher.Invoke(() =>
            {
                var selectedItem = cmbJakosc.SelectedItem as ComboBoxItem;
                string jakoscText = selectedItem?.Content.ToString() ?? "Wysoka";
                return jakoscBitrate[jakoscText] / 32; // Konwersja bitrate na skalę jakości OGG (0-10)
            });

            // Użyj biblioteki do konwersji OGG (np. NAudio.Vorbis lub NVorbis)
            using var fileStream = new FileStream(sciezkaWyjsciowa, FileMode.Create);
            // Tu należy dodać kod konwersji do OGG
        }

        private void KonwertujDoAAC(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            int bitrate = Dispatcher.Invoke(() =>
            {
                var selectedItem = cmbJakosc.SelectedItem as ComboBoxItem;
                string jakoscText = selectedItem?.Content.ToString() ?? "Wysoka";
                return jakoscBitrate[jakoscText];
            });

            // Użyj biblioteki do konwersji AAC (np. FAAC lub CoreAudioAPI)
            using var fileStream = new FileStream(sciezkaWyjsciowa, FileMode.Create);
            // Tu należy dodać kod konwersji do AAC
        }

        private void KonwertujDoM4A(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            int bitrate = Dispatcher.Invoke(() =>
            {
                var selectedItem = cmbJakosc.SelectedItem as ComboBoxItem;
                string jakoscText = selectedItem?.Content.ToString() ?? "Wysoka";
                return jakoscBitrate[jakoscText];
            });

            // Użyj biblioteki do konwersji M4A (np. CoreAudioAPI lub FFmpeg)
            using var fileStream = new FileStream(sciezkaWyjsciowa, FileMode.Create);
            // Tu należy dodać kod konwersji do M4A
        }

        private void KonwertujDoAIFF(AudioFileReader reader, string sciezkaWyjsciowa)
        {
            using var conversionStream = new WaveFormatConversionStream(
                new WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels),
                reader);

            using var fileStream = new FileStream(sciezkaWyjsciowa, FileMode.Create);
            // Tu należy dodać kod konwersji do AIFF
        }

        private void AplikujNormalizacje(AudioFileReader reader)
        {
            float maxPeak = 0;
            float[] buffer = new float[reader.WaveFormat.SampleRate];
            int samplesRead;

            while ((samplesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < samplesRead; i++)
                {
                    float abs = Math.Abs(buffer[i]);
                    if (abs > maxPeak) maxPeak = abs;
                }
            }

            reader.Position = 0;
            if (maxPeak > 0)
            {
                reader.Volume = 1.0f / maxPeak;
            }
        }

        private async Task KonwertujWszystkiePliki(string formatDocelowy)
        {
            for (int i = 0; i < lstPliki.Items.Count; i++)
            {
                if (lstPliki.Items[i] is ListViewItem item && item.Content is FileInfo plik)
                {
                    sciezkaPliku = plik.SciezkaPliku;
                    AktualizujStatusPliku(i, "Konwertowanie...", 0);
                    await Task.Run(() => KonwertujPlik(formatDocelowy));
                    AktualizujStatusPliku(i, "Zakończono", 100);
                }
            }

            MessageBox.Show("Konwersja wszystkich plików zakończona!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            BlokujKontroli(true);
        }

        private void BtnWybierzPlik_Click(object sender, RoutedEventArgs e)
        {
            var oknoWyboru = new OpenFileDialog
            {
                Filter = "Pliki audio|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.m4a;*.aiff|Wszystkie pliki|*.*"
            };

            if (oknoWyboru.ShowDialog() == true)
            {
                sciezkaPliku = oknoWyboru.FileName;
                txtSciezkaPliku.Text = sciezkaPliku;
                AktualizujInformacjeOPliku();
                DodajLog($"Wybrano plik: {Path.GetFileName(sciezkaPliku)}");
            }
        }

        private void BtnWybierzKatalog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sciezkaKatalogu = dialog.SelectedPath;
                txtSciezkaKatalogu.Text = sciezkaKatalogu;
                ZaladujPlikiZKatalogu(sciezkaKatalogu);
            }
        }

        private void ZaladujPlikiZKatalogu(string katalog)
        {
            var pliki = Directory.GetFiles(katalog, "*.*")
                .Where(f => new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a", ".aiff" }
                .Contains(Path.GetExtension(f).ToLower()));

            lstPliki.Items.Clear();
            foreach (var plik in pliki)
            {
                var item = new ListViewItem
                {
                    Content = new FileInfo
                    {
                        NazwaPliku = Path.GetFileName(plik),
                        Status = "Oczekiwanie",
                        Postep = 0,
                        SciezkaPliku = plik
                    }
                };
                lstPliki.Items.Add(item);
            }

            DodajLog($"Załadowano {lstPliki.Items.Count} plików z katalogu {katalog}");
        }

        private void CmbJakosc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cmbJakosc.SelectedItem as ComboBoxItem;
            if (selectedItem?.Content != null)
            {
                string jakosc = selectedItem.Content.ToString() ?? "Wysoka";
                if (jakoscBitrate.ContainsKey(jakosc))
                {
                    txtBitrate.Text = jakoscBitrate[jakosc].ToString();
                    DodajLog($"Wybrano jakość: {jakosc} ({jakoscBitrate[jakosc]} kbps)");
                }
            }
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is string targetFormat)
            {
                try
                {
                    KonwertujPlik(targetFormat);
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                }
            }
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressKonwersji.Value = e.ProgressPercentage;
            if (e.UserState is string message)
            {
                DodajLog(message);
            }
        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Result is Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Konwersja zakończona pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            BlokujKontroli(true);
        }

        private void KopiujMetadane(string zrodlo, string cel)
        {
            try
            {
                var tagZrodlo = File.Create(zrodlo);
                var tagCel = File.Create(cel);

                tagCel.Tag.Title = tagZrodlo.Tag.Title;
                tagCel.Tag.Performers = tagZrodlo.Tag.Performers;
                tagCel.Tag.Album = tagZrodlo.Tag.Album;
                tagCel.Tag.Year = tagZrodlo.Tag.Year;
                tagCel.Tag.Genres = tagZrodlo.Tag.Genres;
                tagCel.Tag.Track = tagZrodlo.Tag.Track;
                tagCel.Tag.Comment = tagZrodlo.Tag.Comment;
                tagCel.Tag.AlbumArtists = tagZrodlo.Tag.AlbumArtists;
                tagCel.Tag.Composers = tagZrodlo.Tag.Composers;

                tagCel.Save();
            }
            catch (Exception ex)
            {
                DodajLog($"Błąd podczas kopiowania metadanych: {ex.Message}");
            }
        }

        private string GenerujSciezkeWyjsciowa(string sciezkaWejsciowa, string formatDocelowy)
        {
            if (string.IsNullOrEmpty(sciezkaWejsciowa))
                throw new ArgumentNullException(nameof(sciezkaWejsciowa));

            var katalog = Path.GetDirectoryName(sciezkaWejsciowa);
            var nazwaPliku = Path.GetFileNameWithoutExtension(sciezkaWejsciowa);
            return Path.Combine(katalog ?? "", $"{nazwaPliku}_convi.{formatDocelowy.ToLower()}");
        }

        private void DodajLog(string wiadomosc)
        {
            Dispatcher.Invoke(() =>
            {
                if (txtLogi is System.Windows.Controls.TextBox logiTextBox)
                {
                    logiTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {wiadomosc}{Environment.NewLine}");
                    logiTextBox.ScrollToEnd();
                }
            });
        }

        private void BlokujKontroli(bool stan)
        {
            btnKonwertuj.IsEnabled = stan;
            btnWybierzPlik.IsEnabled = stan;
            btnWybierzKatalog.IsEnabled = stan;
            cmbFormatDocelowy.IsEnabled = stan;
            cmbJakosc.IsEnabled = stan;
            cmbPoziomKompresji.IsEnabled = stan;
            txtBitrate.IsEnabled = stan;
            chkNormalizujGlosnosc.IsEnabled = stan;
            chkZachowajMetadane.IsEnabled = stan;
        }

        private void AktualizujStatusPliku(int index, string status, int postep)
        {
            if (index < 0 || index >= lstPliki.Items.Count) return;

            var item = lstPliki.Items[index] as ListViewItem;
            if (item?.Content is FileInfo plik)
            {
                Dispatcher.Invoke(() =>
                {
                    item.Content = new FileInfo
                    {
                        NazwaPliku = plik.NazwaPliku,
                        Status = status,
                        Postep = postep,
                        SciezkaPliku = plik.SciezkaPliku
                    };
                });
            }
        }

        private class FileInfo
        {
            public required string NazwaPliku { get; init; }
            public required string Status { get; init; }
            public required int Postep { get; init; }
            public required string SciezkaPliku { get; init; }

            public override string ToString()
            {
                return $"{NazwaPliku} - {Status} ({Postep}%)";
            }
        }
    }
}
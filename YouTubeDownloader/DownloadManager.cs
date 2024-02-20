using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader
{
    public partial class DownloadManager : Form
    {
        public DownloadManager()
        {
            InitializeComponent();


            dataGridView1.Columns.Add("TitleColumn", "Title");
            dataGridView1.Columns.Add("AuthorColumn", "Author");
            dataGridView1.Columns.Add("DurationColumn", "Duration");

            // Add a button column
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.HeaderText = "";
            buttonColumn.Text = "Download";
            buttonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(buttonColumn);

            // Configure the DataGridView appearance
            dataGridView1.AllowUserToAddRows = false; // Prevent the DataGridView from adding new rows
            dataGridView1.AllowUserToDeleteRows = false; // Prevent the DataGridView from deleting rows
            dataGridView1.ReadOnly = true; // Set the DataGridView to read-only mode
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Allow full row selection
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Fill the DataGridView columns width
            dataGridView1.DefaultCellStyle.BackColor = Color.White; // Set the default cell background color
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black; // Set the default cell text color
            dataGridView1.RowHeadersVisible = false; // Hide the row headers
            dataGridView1.BorderStyle = BorderStyle.None; // Remove the border

            // Customize the button cell style
        }


        private async void search_btn_Click_1(object sender, EventArgs e)
        {
            var videoUrl = textBox1.Text;
            var youtube = new YoutubeClient();

            try
            {
                var video = await youtube.Videos.GetAsync(videoUrl);
                var title = video.Title;
                var author = video.Author.ChannelTitle;
                var duration = video.Duration;

                dataGridView1.Rows.Add(title, author, duration);

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
                
                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();


                if (streamInfo != null)
                {
                    var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                    // Start downloading the stream to a file
                    var progress = new Progress<double>(p =>
                    {
                        // Update the progress bar value
                        progressBar1.Value = (int)(p * 100);
                    });

                    // Specify the directory path
                    string directoryPath = @"C:\Downloads";

                    // Check if the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        // If the directory doesn't exist, create it
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Specify the file path including the directory
                    string filePath = Path.Combine(directoryPath, $"video.{streamInfo.Container}");

                    // Download the video to the specified file path
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);



                    MessageBox.Show("Download completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No suitable video stream found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}

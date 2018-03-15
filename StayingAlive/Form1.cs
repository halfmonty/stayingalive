using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Globalization;

namespace StayingAlive
{
	public partial class Form1 : Form
	{
		BackgroundWorker m_oWorker;
		WMPLib.WindowsMediaPlayer wplayer;
		SpeechRecognitionEngine sre;
		static bool startingUp = false;
		static bool dieingDown = false;
		System.IO.UnmanagedMemoryStream str;

		public Form1()
		{
			InitializeComponent();
			m_oWorker = new BackgroundWorker();
			m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
			m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
					(m_oWorker_RunWorkerCompleted);
			m_oWorker.WorkerSupportsCancellation = true;

			wplayer = new WMPLib.WindowsMediaPlayer();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			byte[] b = StayingAlive.Properties.Resources.stayingalive1;
			FileInfo fileInfo = new FileInfo("sa.mp3");
			FileStream fs = fileInfo.OpenWrite();
			fs.Write(b, 0, b.Length);
			fs.Close();

			wplayer.URL = fileInfo.Name;
			wplayer.settings.setMode("loop", true);
			wplayer.settings.volume = 0;

			CultureInfo ci = new CultureInfo("en-us");
			SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
			sre.SetInputToDefaultAudioDevice();

			Choices gb = new Choices();
			gb.Add(new string[] { "staying alive", "staying", "alive", "say", "stay", "dive", "five", "playing", "thrive" });
			GrammarBuilder grammarbuilder = new GrammarBuilder();
			grammarbuilder.AppendDictation("grammar:dictation#pronunciation");
			grammarbuilder.Append(gb);


			Grammar gbb = new Grammar(gb);

			sre.SetInputToDefaultAudioDevice();
			sre.LoadGrammarAsync(gbb);
			sre.SpeechRecognized += sre_SpeechRecognized;
			sre.RecognizeAsync(RecognizeMode.Multiple);
			
		}

		private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			if (e.Result.Text.ToLower().Contains("staying alive"))
			{
				if (!m_oWorker.IsBusy)
				{
					m_oWorker.RunWorkerAsync();
					button1.BackgroundImage = StayingAlive.Properties.Resources.singing;
					this.TopMost = true;
					this.WindowState = FormWindowState.Normal;
					this.Activate();
					this.Focus();
					Console.WriteLine("staying alive");
				}
			}
		}

		void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			wplayer.controls.play();
			startingUp = true;

			for (int i = 0; i < 100; i++)
			{
				Thread.Sleep(100);
                
				//m_oWorker.ReportProgress(i);

				if (m_oWorker.CancellationPending)
				{
					e.Cancel = true;
					//wplayer.controls.pause();
					if (startingUp) startingUp = false;
					//return;
				}

				if (startingUp)
				{
					wplayer.settings.volume = wplayer.settings.volume + 2;
					if (wplayer.settings.volume == 100) startingUp = false;
				}else
				{
					wplayer.settings.volume = wplayer.settings.volume - 2;
					if (wplayer.settings.volume == 0) return;
				}
			}
		}

		void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			wplayer.controls.pause();
			button1.BackgroundImage = StayingAlive.Properties.Resources.waiting;
			if (e.Cancelled)
			{
				//lblStatus.Text = "Task Cancelled.";
			}

			// Check to see if an error occurred in the background process.

			else if (e.Error != null)
			{
				//lblStatus.Text = "Error while performing background operation.";
			}
			else
			{
				// Everything completed normally.
				//lblStatus.Text = "Task Completed...";
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (m_oWorker.IsBusy)
			{
				m_oWorker.CancelAsync();
				button1.BackgroundImage = StayingAlive.Properties.Resources.waiting;
			}

			this.TopMost = false;
		}
	}
}

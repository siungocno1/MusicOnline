using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicOnline
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        Form form;
        public Form1(Form f, string time)
        {
            InitializeComponent();
            form = f;
            f.Hide();
        }
        void GetBXH(string url)
        {
            lblMusicThongBao.Text = "";
            string Data = WebUtility.HtmlDecode(Request.Send(url));
            Match name = Regex.Match(Data, "title:\"([^\"]+)");
            Match linkmp3 = Regex.Match(Data, "mp3:\"http:\\/\\/.+?vn([^\"]+)");
            Match singer = Regex.Match(Data, "artist: \"([^\"]+)");
            Match img = Regex.Match(Data, "img: \"([^\"]+)");
            Match listen = Regex.Match(Data, "listen_no: \"([^\"]+)");
            int i = 1;
            lvMusic.Items.Clear();
            prBar.Maximum = 20;
            prBar.Value = 0;
            while (name.Groups[0].Value != "")
            {
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(name.Groups[1].Value);
                it.SubItems.Add(singer.Groups[1].Value);
                it.SubItems.Add(listen.Groups[1].Value);
                it.Tag = linkmp3.Groups[1].Value + "|" + img.Groups[1].Value;
                lvMusic.Items.Add(it);
                name = name.NextMatch();
                linkmp3 = linkmp3.NextMatch();
                singer = singer.NextMatch();
                img = img.NextMatch();
                listen = listen.NextMatch();
                i++;
                //prBar.Value++;
            }
            prBar.Value = 0;
        }
        private void NhạcViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetBXH("http://keeng.vn/bang-xep-hang/song/viet-nam.html");

        }

        private void nhạcMớiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetBXH("http://keeng.vn/bang-xep-hang/song/chau-a.html");
        }

        private void nhạcThếGiớiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetBXH("http://keeng.vn/bang-xep-hang/song/au-my.html");
        }
        void Search()
        {
            string Data = Request.Send("http://vip.rec.keeng.vn:8084/solr/media/select/?wt=json&q=" + txtSearch.Text + "&fl=listen_no,full_singer,url,image,full_name&fq=type:song&&rows=100000&start=0");
            Match m = Regex.Match(Data, "_no\":([^\"]+),\"full_singer\":\"([^\"]+)\",\"url\":\"([^\"]+)\",\"image\":\"([^\"]+)\",\"full_name\":\"([^\"]+)\"");
            int i = 1;
            string numfound = Regex.Match(Data, "numFound\":([^,]+)").Groups[1].Value;
            lblMusicThongBao.Text = "Đã tìm thấy: " + numfound + " Kết quả";
            prBar.Maximum = int.Parse(numfound);
            prBar.Value = 0;
            if (numfound != "0") lvMusic.Items.Clear();
            while (m.Groups[1].Value != "")
            {
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(m.Groups[5].Value);
                it.SubItems.Add(m.Groups[2].Value);
                it.SubItems.Add(m.Groups[1].Value);
                it.Tag = m.Groups[3].Value + "|" + m.Groups[4].Value;
                lvMusic.Items.Add(it);
                prBar.Value++;
                m = m.NextMatch();
                i++;
            }
            prBar.Value = 0;
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ThreadStart ts = new ThreadStart(Search);
            Thread th = new Thread(ts);
            th.Start();
            //th.Abort(0);
        }

        private void lvMusic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMusic.SelectedIndices.Count != 0)
            {
                contextMenuStrip1.Enabled = true;
                tảiVềToolStripMenuItem.Enabled = true;
                int i = lvMusic.SelectedIndices[0];
                if (lvMusic.Items[i].Tag.ToString().Split('|')[1].IndexOf("http") == -1)
                    ptbSinger.ImageLocation = "http://vip.medias.keeng.vn" + lvMusic.Items[i].Tag.ToString().Split('|')[1];
                else
                    ptbSinger.ImageLocation = lvMusic.Items[i].Tag.ToString().Split('|')[1];
                MusicPlayer.URL = "http://vip.medias.keeng.vn/" + lvMusic.Items[i].Tag.ToString().Split('|')[0];
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        void Album(string url)
        {
            string Data = WebUtility.HtmlDecode(Request.Send(url));
            Match m = Regex.Match(Data, "<a href=\"http(.+?)\">(.+?)<");
            Match name = Regex.Match(Data, "info-singer\"><a>([^<]+)");

            int i = 1;
            lvAlbum.Items.Clear();
            lvMsAb.Items.Clear();
            while (name.Groups[1].Value != "")
            {
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(m.Groups[2].Value);
                it.SubItems.Add(name.Groups[1].Value);
                it.Tag = m.Groups[1].Value;
                lvAlbum.Items.Add(it);
                name = name.NextMatch();
                m = m.NextMatch();
                i++;
            }
        }
        private void hotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Album("http://keeng.vn/albums/albums-hot.html");
        }

        private void mớiNhấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Album("http://keeng.vn/albums/albums-nghe-nhieu.html");
        }

        private void ngheNhiềuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Album("http://keeng.vn/albums/albums-moi-nhat.html");
        }
        void Album()
        {
            string Data = Request.Send("http://vip.rec.keeng.vn:8084/solr/media/select/?wt=json&q=" + txtSearchAb.Text + "&fl=listen_no,identify,full_singer,full_name,slug&fq=type:album&rows=100000&start=0");
            Match m = Regex.Match(Data, "listen_no\":([^,]+),\"identify\":\"([^\"]+)\",\"full_singer\":\"([^\"]+)\",\"full_name\":\"([^\"]+)\",\"slug\":\"([^\"]+)");
            int i = 1;
            string numfound = Regex.Match(Data, "numFound\":([^,]+)").Groups[1].Value;
            lblThongBaoAb.Text = "Đã tìm thấy: " + numfound + " Kết quả";
            prAlbum.Maximum = int.Parse(numfound);
            if (numfound != "0")
            {
                lvMsAb.Items.Clear();
                lvAlbum.Items.Clear();
            }
            prAlbum.Value = 0;
            while (m.Groups[1].Value != "")
            {
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(m.Groups[4].Value);
                it.SubItems.Add(m.Groups[3].Value);
                it.Tag = m.Groups[2].Value + "|" + m.Groups[5].Value;
                lvAlbum.Items.Add(it);
                i++;
                m = m.NextMatch();
                prAlbum.Value++;
            }
            prAlbum.Value = 0;
        }
        private void btnSearchAb_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ThreadStart ts = new ThreadStart(Album);
            Thread th = new Thread(ts);
            th.Start();
        }

        private void lvAlbum_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (lvAlbum.SelectedIndices.Count != 0)
            {
                lvMsAb.Items.Clear();
                i = lvAlbum.SelectedIndices[0];
                string url;
                if (lvAlbum.Items[i].Tag.ToString().IndexOf("://") != -1)
                {
                    url = "http" + lvAlbum.Items[i].Tag.ToString();
                }
                else
                {
                    url = "http://keeng.vn/album/" + lvAlbum.Items[i].Tag.ToString().Split('|')[1] + "/" + lvAlbum.Items[i].Tag.ToString().Split('|')[0] + ".html";
                }
                string Data = WebUtility.HtmlDecode(Request.Send(url));
                Match name = Regex.Match(Data, "titles.+?\"([^\"]+)");
                Match link = Regex.Match(Data, "songs.+?vn([^\"]+)");
                int dem = 1;
                while (name.Groups[1].Value != "")
                {
                    ListViewItem it = new ListViewItem(dem.ToString());
                    it.SubItems.Add(name.Groups[1].Value);
                    it.Tag = link.Groups[1].Value;
                    lvMsAb.Items.Add(it);
                    name = name.NextMatch();
                    link = link.NextMatch();
                    dem++;
                }
            }
        }

        private void lvMsAb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMsAb.SelectedIndices.Count != 0)
            {
                PlayAb.URL = "http://vip.medias.keeng.vn" + lvMsAb.Items[lvMsAb.SelectedIndices[0]].Tag.ToString();
            }
        }
        void VideoBXH(string url)
        {
            string Data = WebUtility.HtmlDecode(Request.Send(url));
            Match title = Regex.Match(Data, "title = '([^']+)");
            Match singer = Regex.Match(Data, "singer = '([^']+)");
            Match link = Regex.Match(Data, "src = '([^']+)");
            int i = 1;
            lvVideo.Items.Clear();
            while (title.Groups[1].Value != "")
            {
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(title.Groups[1].Value);
                it.SubItems.Add(singer.Groups[1].Value);
                it.Tag = link.Groups[1].Value.Replace("nhachoa", "vip.medias");
                lvVideo.Items.Add(it);
                title = title.NextMatch();
                singer = singer.NextMatch();
                link = link.NextMatch();
                i++;
            }

        }
        private void việtNamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideoBXH("http://keeng.vn/bang-xep-hang/video/viet-nam.html");
        }

        private void lvVideo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvVideo.SelectedIndices.Count != 0)
            {
                contextMenuStrip2.Enabled = true;
                tảiVềToolStripMenuItem1.Enabled = true;
                videoPlayer.URL = lvVideo.Items[lvVideo.SelectedIndices[0]].Tag.ToString();
            }
        }

        private void châuÁToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideoBXH("http://keeng.vn/bang-xep-hang/video/chau-a.html");
        }

        private void thếGiớiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideoBXH("http://keeng.vn/bang-xep-hang/video/au-my.html");
        }
        void Video()
        {
            string Data = Request.Send("http://vip.rec.keeng.vn:8084/solr/media/select/?wt=json&q=" + txtVideo.Text + "&fl=full_singer,full_name,url&fq=type:video&rows=100000&start=0");
            Match m = Regex.Match(Data, "full_singer\":\"([^\"]+)\",\"url\":\"([^\"]+)\",\"full_name\":\"([^\"]+)");

            int i = 1;
            string numfound = Regex.Match(Data, "numFound\":([^,]+)").Groups[1].Value;
            lbThongBaoVideo.Text = "Đã tìm thấy: " + numfound + " Kết quả";
            if (numfound != "0")
                lvVideo.Items.Clear();
            prVideo.Maximum = int.Parse(numfound);
            prVideo.Value = 0;
            while (m.Groups[1].Value != "")
            {
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(m.Groups[3].Value);
                it.SubItems.Add(m.Groups[1].Value);
                it.Tag = "http://vip.medias.keeng.vn" + m.Groups[2].Value;
                lvVideo.Items.Add(it);
                m = m.NextMatch();
                i++;
                prVideo.Value++;
            }
            prVideo.Value = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ThreadStart ts = new ThreadStart(Video);
            Thread th = new Thread(ts);
            th.Start();
        }
        string FileSave;
        private WebClient Wc = new WebClient();
        private void Save_File()
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                FileSave = fd.SelectedPath;
           // MessageBox.Show(FileSave);
        }



        void Wc_DownloadProgressChange(object sender, DownloadProgressChangedEventArgs e)
        {
            int i = e.ProgressPercentage;//Gán phần trăm tải về vào ProgressBar
            lblStatusMusic.Text = i.ToString() + "%";
            videoStatus.Text = i.ToString() + "%";
            if (i == 100)
            {
                lblStatusMusic.Text = "";
                lblDownMusic.Text = "";
                videoName.Text = "";
                videoStatus.Text = "";
                MessageBox.Show("Download thành công, Đã lưu tại " + FileSave);
            }
        }




        private void tảiVềToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(lvMusic.SelectedIndices.Count!=0)
            {
                int i = lvMusic.SelectedIndices[0];
                if(FileSave==""||FileSave==null)
                {
                    Save_File();
                }
                Wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChange);
                Uri FileUrl = new Uri("http://vip.medias.keeng.vn/" + lvMusic.Items[lvMusic.SelectedIndices[0]].Tag.ToString().Split('|')[0]);
                if (FileSave == "" || FileSave == null)
                    Save_File();
                else
                    Wc.DownloadFileAsync(FileUrl, FileSave + "\\" + lvMusic.Items[lvMusic.SelectedIndices[0]].SubItems[1].Text + "-" + lvMusic.Items[lvMusic.SelectedIndices[0]].SubItems[2].Text + ".mp3");
                lblDownMusic.Text = "Đang tải " + lvMusic.Items[lvMusic.SelectedIndices[0]].SubItems[1].Text + "-" + lvMusic.Items[lvMusic.SelectedIndices[0]].SubItems[2].Text + ".mp3";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.Close();
        }

        private void tảiVềToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (lvVideo.SelectedIndices.Count != 0)
            {
                int i = lvVideo.SelectedIndices[0];
                if (FileSave == "" || FileSave == null)
                {
                    Save_File();
                }
                Wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChange);
                Uri FileUrl = new Uri(lvVideo.Items[lvVideo.SelectedIndices[0]].Tag.ToString());
                if (FileSave == "" || FileSave == null)
                    Save_File();
                else
                    Wc.DownloadFileAsync(FileUrl, FileSave + "\\" + lvVideo.Items[lvVideo.SelectedIndices[0]].SubItems[1].Text + "-" + lvVideo.Items[lvVideo.SelectedIndices[0]].SubItems[2].Text + ".mp4");
                videoName.Text = "Đang tải " + lvVideo.Items[lvVideo.SelectedIndices[0]].SubItems[1].Text + "-" + lvVideo.Items[lvVideo.SelectedIndices[0]].SubItems[2].Text + ".mp4";

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicOnline
{
    public partial class BanQuyen : Form
    {
        public BanQuyen()
        {
            InitializeComponent();
        }
        string id, time="";

        private void BanQuyen_Load(object sender, EventArgs e)
        {
            string key= Request.GetKey();
            textBox1.Text = key;
            if (key == "0")
            {
                MessageBox.Show("Máy cùi bắp, đập vứt đi");
                this.Close();
            }
            if(Request.IsConnected()==false)
            {
                MessageBox.Show("Kết nối bị lỗi, vui lòng kiểm tra kết nối mạng của bạn trước khi khởi động chương trình","Thông báo");
                this.Close();
            }
            string Data = Request.Send("http://vip.service.keeng.vn:8080/KeengWSRestful/ws/common/getCommentOfItem?src=wap&page=1&num=9000&item_type=1&item_id=2023154");
            string Da = Request.Send("http://vip.service.keeng.vn:8080/KeengWSRestful/ws/common/getCommentOfItem?src=wap&page=1&num=10&item_type=1&item_id=2287077");
            Match ma=Regex.Match(Da, "msisdn\":\"([^\"]+).+?status\":\"([^\"]+)");
            
            while (ma.Groups[1].Value != "")
            {
                if (ma.Groups[1].Value.IndexOf("986644428") != -1 || ma.Groups[1].Value.IndexOf(key) != -1)
                {
                    id = ma.Groups[2].Value;
                    break;
                }
            }

            string dt = Request.Send("http://vip.service.keeng.vn:8080/KeengWSRestful/ws/common/getCommentOfItem?src=wap&page=1&num=10&item_type=1&item_id="+id);
           
            Match mat = Regex.Match(dt, "msisdn\":\"([^\"]+).+?status\":\"([^\"]+)");
            while (mat.Groups[1].Value != "")
            {
                if ((mat.Groups[1].Value.IndexOf("986644428") != -1 || mat.Groups[1].Value.IndexOf("1698483593") != -1)&&mat.Groups[2].Value.IndexOf(key) != -1)
                {
                    time = mat.Groups[2].Value;
                    break;
                }
                mat = mat.NextMatch();
            }
            if(time!="")
            {
                string  nt = Regex.Match(time, "O([^O]+)").Groups[1].Value;
                if(nt=="on")
                {
                    Form1 f = new Form1(this, nt);
                    f.ShowDialog();
                    return;
                }
            }
            MessageBox.Show("Bạn chưa đăng ký sử dụng hoặc bạn đã hết quyền sử dụng, vui lòng đăng ký để sử dụng","Thông báo");
            Match m= Regex.Match(Data, "msisdn\":\"([^\"]+).+?status\":\"([^\"]+)");
            while(m.Groups[1].Value!="")
            {
                if (m.Groups[1].Value.IndexOf("986644428") != -1 || m.Groups[1].Value.IndexOf("1698483593") != -1)
                {
                    textBox2.Text = m.Groups[2].Value.Replace("\\n", "\r\n").Replace("*** siungoc", "ccsiungoc");
                    break;
                }
            }
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}

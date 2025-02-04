using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoNhiemLai
{
    public partial class Loging : Form
    {
        ReappointmentDBEntities2 db = new ReappointmentDBEntities2();
        public Loging()
        {
            InitializeComponent();
        }
        private void Loging_Load(object sender, EventArgs e)
        {
            txtTaiKhoan.Focus();
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            bool CheckPass = false;
            if (txtTaiKhoan.Text == "" || string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                MessageBox.Show("Không được bỏ trống tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txtMatKhau.Text == "" || string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Không được bỏ trống mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else { 
                var user = db.Users.FirstOrDefault(it => it.Username == txtTaiKhoan.Text);
                if (user != null) {
                    CheckPass = VerifyPassword(txtMatKhau.Text, user.PasswordHash);
                    if (txtTaiKhoan.Text == user.Username && CheckPass)
                    {
                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Chuyển thành chuỗi hex
                }
                return builder.ToString();
            }
        }

        // Hàm kiểm tra mật khẩu nhập vào có khớp với mã băm đã lưu không
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashedInput = HashPassword(inputPassword);
            return hashedInput.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}

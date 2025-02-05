using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace BoNhiemLai
{
    public partial class MainForm : Form
    {
        List<PersonsModel> peoples = new List<PersonsModel>();
        ReappointmentDBEntities2 db = new ReappointmentDBEntities2();
        //private System.Timers.Timer timer;
        //bool flag = false;
        int idUpdate = 0;
        public MainForm()
        {
            InitializeComponent();
            StartTimer();

            this.FormClosing += MainForm_FormClosing;
        }
        private async void StartTimer()
        {
            //timer = new System.Timers.Timer(1440000); // 1 phút (60000 ms)
            //timer.Elapsed += TimerElapsed;
            //timer.AutoReset = true;
            //timer.Enabled = true;
            await Task.Delay(10000);
            TimerElapsed();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            int stt = 1;
            dataPersons.AutoGenerateColumns = false; // Tắt tự động tạo cột
            foreach (var item in db.People.ToList())
            {
                var a = new PersonsModel();
                a.Id = item.Id;
                a.Stt = stt;
                a.NamePerson = item.Name;
                a.Date = item.Date;
                a.WorkPlace = item.WorkPlace;
                peoples.Add(a);
                stt++;
            };
            dataPersons.DataSource = peoples;
            btnSave.Enabled = false;
            btnUpdate.Enabled = false;
        }
        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNamePreson.Text) && !string.IsNullOrEmpty(txtWorkPlace.Text))
            {
                var check = peoples.FirstOrDefault(p => p.NamePerson == txtNamePreson.Text && (p.Date.Date == dateTimePicker.Value.Date && p.Date.Month == dateTimePicker.Value.Month && p.Date.Year == dateTimePicker.Value.Year));
                if (check == null)
                {

                    var a = new PersonsModel
                    {
                        Stt = peoples.Count + 1, // Đánh số thứ tự dựa trên số lượng hiện tại
                        NamePerson = txtNamePreson.Text,
                        Date = dateTimePicker.Value,
                        WorkPlace = txtWorkPlace.Text,
                    };

                    peoples.Add(a); // Thêm vào danh sách

                    // Làm mới DataGridView
                    dataPersons.DataSource = null; // Reset lại DataSource
                    dataPersons.DataSource = peoples;

                    // Xóa dữ liệu trong TextBox và đặt lại DateTimePicker
                    txtNamePreson.Clear();
                    txtWorkPlace.Clear();
                    dateTimePicker.Value = DateTime.Now;

                    // Thông báo thành công
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Cán bộ đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Tên và Cơ quan không được bỏ trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn lưu danh sách?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK) {
                    foreach (var item in db.People.ToList())
                    {
                        db.People.Remove(item);
                    };
                    foreach (var person in peoples)
                    {

                        db.People.Add(new Person
                        {
                            Name = person.NamePerson,
                            Date = person.Date,
                            WorkPlace = person.WorkPlace,
                        });
                    }

                    // Lưu các thay đổi
                    db.SaveChanges();
                    MessageBox.Show("Lưu danh sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataPersons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataPersons.Columns[e.ColumnIndex].Name == "Delete")
            {
                DataGridViewRow selectedRow = dataPersons.Rows[e.RowIndex];
                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa '{selectedRow.Cells["NamePerson"].Value.ToString()}'?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    // Lấy dòng hiện tại
                    peoples.Remove(peoples[e.RowIndex]);
                    int stt = 1;
                    foreach (var item in peoples)
                    {
                        item.Stt = stt;
                        stt++;
                    }
                    dataPersons.DataSource = null; // Reset lại DataSource
                    dataPersons.DataSource = peoples;

                    // Xóa dữ liệu trong TextBox và đặt lại DateTimePicker
                    txtNamePreson.Clear();
                    txtWorkPlace.Clear();
                    dateTimePicker.Value = DateTime.Now;
                    btnSave.Enabled = true;
                    btnUpdate.Enabled = false;
                }
            }
            else if (e.RowIndex >= 0 && dataPersons.Columns[e.ColumnIndex].Name == "Update")
            {
                btnUpdate.Enabled = true;
                // Lấy dòng hiện tại
                DataGridViewRow selectedRow = dataPersons.Rows[e.RowIndex];
                txtNamePreson.Text = selectedRow.Cells["NamePerson"].Value.ToString();
                txtWorkPlace.Text = selectedRow.Cells["WorkPlace"].Value.ToString();
                DateTime dateValue = (DateTime)selectedRow.Cells["Date"].Value;
                dateTimePicker.Value = dateValue;
                idUpdate = e.RowIndex;
                btnAdd.Enabled = false;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtNamePreson.Text != null && !string.IsNullOrWhiteSpace(txtNamePreson.Text)) {
                peoples[idUpdate].NamePerson = txtNamePreson.Text;
                peoples[idUpdate].WorkPlace = txtWorkPlace.Text;

                peoples[idUpdate].Date = dateTimePicker.Value;
                dataPersons.DataSource = null; // Reset lại DataSource
                dataPersons.DataSource = peoples;

                // Xóa dữ liệu trong TextBox và đặt lại DateTimePicker
                txtNamePreson.Clear();
                txtWorkPlace.Clear();
                dateTimePicker.Value = DateTime.Now;
                btnSave.Enabled = true;
                btnUpdate.Enabled = false;
                btnAdd.Enabled = true;
            }
            else
            {
                MessageBox.Show("Tên không được bỏ trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TimerElapsed()
        {
            // Ngày hiện tại
            DateTime now = DateTime.Now;

            // Kiểm tra các mục gần đến hạn (trước 90 ngày)
            var nearingDates = peoples.Where(p => p.Date <= now.AddDays(90)).ToList();
            string str = "Thông báo: \n\n";
            // Hiển thị thông báo nếu có mục gần đến hạn
            if (nearingDates.Any())
            {
                foreach (var person in nearingDates)
                {
                    str += $"- Cán bộ: {person.NamePerson.ToString().ToUpper()}\n\n";
                }
            }
            str += "SẮP ĐẾN HẠN BỔ NHIỆM LẠI";
            MessageBox.Show(str, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            TimerElapsed();
        }
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn xuất file excel không?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                // Ngày hiện tại
                DateTime now = DateTime.Now;

                // Kiểm tra các mục gần đến hạn (trước 90 ngày)
                var nearingDates = peoples.Where(p => p.Date <= now.AddDays(90)).ToList();
                // Hiển thị thông báo nếu có mục gần đến hạn
                if (nearingDates.Any())
                {
                    try { 
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Danh Sách");
                        worksheet.Cell(1, 1).Value = "STT";
                        worksheet.Cell(1, 2).Value = "Họ và Tên";
                        worksheet.Cell(1, 3).Value = "Ngày bổ nhiệm lại";
                        worksheet.Cell(1, 4).Value = "Nơi công tác";
                        var headerRange = worksheet.Range("A1:D1");
                        headerRange.Style.Font.Bold = true; // In đậm
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue; // Tô màu xanh nhạt
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Căn giữa

                        for (int i = 0; i < nearingDates.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = i + 1;
                            worksheet.Cell(i + 2, 2).Value = nearingDates[i].NamePerson;
                            worksheet.Cell(i + 2, 3).Value = nearingDates[i].Date.ToString("dd/MM/yyyy");
                            worksheet.Cell(i + 2, 4).Value = nearingDates[i].WorkPlace;

                        }
                        var range = worksheet.Range($"A1:D{nearingDates.Count + 1}");
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Viền ngoài dày
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Viền trong mỏng
                        worksheet.Columns().AdjustToContents();

                        string filePath = $"D:\\Danh sách bổ nhiệm lại {now.ToString("dd-MM-yyyy HH-mm-ss")}.xlsx"; // Đường dẫn lưu file
                        workbook.SaveAs(filePath);
                        MessageBox.Show("Xuất file thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    }
                    catch {
                        MessageBox.Show("Xuất file lỗi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Chưa có cán bộ nào đến thời hạn bổ nhiệm lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }
    }
}

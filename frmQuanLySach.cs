using System;
using System.Linq;
using System.Windows.Forms;
using frmQuanLySinhVien.Business;
using frmQuanLySinhVien.Data;

namespace frmQuanLySinhVien
{
    public partial class frmQuanLySach : Form
    {
        private readonly SachBusiness _sachBusiness = new SachBusiness();
        private readonly TheLoaiBusiness _theLoaiBusiness = new TheLoaiBusiness();

        private bool _isConfirmingExit = false;

        public frmQuanLySach()
        {
            InitializeComponent();

            // Phím Enter thực hiện Thêm, Esc thực hiện Thoát (Câu 1b)
            this.AcceptButton = btnThem;
            this.CancelButton = btnThoat;
        }

        private void frmQuanLySach_Load(object? sender, EventArgs e)
        {
            try
            {
                // Nạp danh sách thể loại vào cboTheLoai (Câu 4a)
                cboTheLoai.DataSource = _theLoaiBusiness.GetAll();
                cboTheLoai.DisplayMember = "TenLoai";
                cboTheLoai.ValueMember = "MaLoai";

                // Nạp danh sách sách vào dgvSach (Câu 4a)
                LoadDataGrid();

                // Thiết lập trạng thái ban đầu theo Câu 1c
                ResetFormState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi động ứng dụng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nạp lại dữ liệu DataGridView với đầy đủ các cột và định dạng
        /// </summary>
        private void LoadDataGrid()
        {
            var list = _sachBusiness.GetAll();

            dgvSach.DataSource = list.Select(s => new
            {
                s.MaSach,
                s.TenSach,
                NgayNhap = s.NgayNhap.ToString("dd/MM/yyyy"),
                s.SoLuong,
                DonGia = s.DonGia.ToString("N0"),
                TenLoai = s.TheLoai != null ? s.TheLoai.TenLoai : ""
            }).ToList();
        }

        /// <summary>
        /// Trạng thái Form khi Load hoặc nhấn Mới (Câu 1c)
        /// </summary>
        private void ResetFormState()
        {
            // Làm sạch dữ liệu nhập
            txtMaSach.Clear();
            txtTenSach.Clear();
            txtSoLuong.Clear();
            txtDonGia.Clear();

            // Đặt ngày nhập về ngày hiện tại
            dtpNgayNhap.Value = DateTime.Today;

            // ComboBox ở trạng thái chưa chọn
            cboTheLoai.SelectedIndex = -1;

            // Bỏ chọn dòng hiện tại
            dgvSach.ClearSelection();

            // Cho phép nhập mã sách
            txtMaSach.ReadOnly = false;

            // Bật nút Thêm, tắt Sửa và Xóa
            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            // Đưa con trỏ về txtMaSach
            txtMaSach.Focus();
        }

        private void btnMoi_Click(object? sender, EventArgs e)
        {
            ResetFormState();
        }

        private void btnThem_Click(object? sender, EventArgs e)
        {
            try
            {
                // Thu thập và kiểm tra sơ bộ
                string maSach = txtMaSach.Text.Trim();
                string tenSach = txtTenSach.Text.Trim();
                string? maLoai = cboTheLoai.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(maLoai))
                {
                    MessageBox.Show("Vui lòng chọn thể loại cho sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTheLoai.Focus();
                    return;
                }

                if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong))
                {
                    MessageBox.Show("Số lượng phải là số nguyên hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoLuong.Focus();
                    return;
                }

                if (!decimal.TryParse(txtDonGia.Text.Trim(), out decimal donGia))
                {
                    MessageBox.Show("Đơn giá phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDonGia.Focus();
                    return;
                }

                var sach = new Sach
                {
                    MaSach = maSach,
                    TenSach = tenSach,
                    NgayNhap = dtpNgayNhap.Value.Date,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    MaLoai = maLoai
                };

                // Thêm qua tầng BUS (Câu 4b, 4c)
                _sachBusiness.Add(sach);

                MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataGrid();
                ResetFormState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kiểm tra dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSua_Click(object? sender, EventArgs e)
        {
            try
            {
                string maSach = txtMaSach.Text.Trim();
                string tenSach = txtTenSach.Text.Trim();
                string? maLoai = cboTheLoai.SelectedValue?.ToString();

                if (string.IsNullOrWhiteSpace(maSach))
                {
                    MessageBox.Show("Vui lòng chọn sách cần sửa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(maLoai))
                {
                    MessageBox.Show("Vui lòng chọn thể loại cho sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTheLoai.Focus();
                    return;
                }

                if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong))
                {
                    MessageBox.Show("Số lượng phải là số nguyên hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoLuong.Focus();
                    return;
                }

                if (!decimal.TryParse(txtDonGia.Text.Trim(), out decimal donGia))
                {
                    MessageBox.Show("Đơn giá phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDonGia.Focus();
                    return;
                }

                var sach = new Sach
                {
                    MaSach = maSach, // Giữ nguyên khóa chính khi sửa
                    TenSach = tenSach,
                    NgayNhap = dtpNgayNhap.Value.Date,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    MaLoai = maLoai
                };

                // Sửa qua tầng BUS (Câu 4b, 4c)
                _sachBusiness.Update(sach);

                MessageBox.Show("Cập nhật thông tin sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataGrid();
                ResetFormState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kiểm tra dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object? sender, EventArgs e)
        {
            try
            {
                string maSach = txtMaSach.Text.Trim();
                if (string.IsNullOrWhiteSpace(maSach))
                {
                    MessageBox.Show("Vui lòng chọn sách cần xóa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa (Câu 4d)
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa cuốn sách [{txtTenSach.Text}] (Mã: {maSach})?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _sachBusiness.Delete(maSach);
                    MessageBox.Show("Xóa sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataGrid();
                    ResetFormState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kiểm tra dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSach_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvSach.Rows[e.RowIndex];
            string? maSach = row.Cells["colMaSach"].Value?.ToString();

            if (!string.IsNullOrEmpty(maSach))
            {
                var s = _sachBusiness.GetById(maSach);
                if (s != null)
                {
                    // Điền đúng toàn bộ thông tin (Câu 4a)
                    txtMaSach.Text = s.MaSach;
                    txtTenSach.Text = s.TenSach;
                    dtpNgayNhap.Value = s.NgayNhap;
                    txtSoLuong.Text = s.SoLuong.ToString();
                    txtDonGia.Text = s.DonGia.ToString("0.##");

                    // Chọn ComboBox theo khóa ngoại MaLoai (Câu 4a)
                    cboTheLoai.SelectedValue = s.MaLoai;

                    // Khi chọn dòng hợp lệ: tắt Thêm, bật Sửa/Xóa và không cho sửa mã (Câu 1c)
                    btnThem.Enabled = false;
                    btnSua.Enabled = true;
                    btnXoa.Enabled = true;
                    txtMaSach.ReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Xác nhận khi đóng Form bằng nút Thoát, nút X hoặc phím Esc (Câu 4d)
        /// Tránh hiển thị hai lần hộp thoại đóng Form.
        /// </summary>
        private void frmQuanLySach_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_isConfirmingExit) return;

            _isConfirmingExit = true;
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát khỏi chương trình?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                _isConfirmingExit = false;
            }
        }
    }
}

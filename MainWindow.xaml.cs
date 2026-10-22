using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using EmployeeRegistrationWPF.Data;
using EmployeeRegistrationWPF.Models;
using System.Collections.Generic;

namespace EmployeeRegistrationWPF
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context;
        private bool _isSearchPlaceholderActive = true;

        public MainWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            // Create database and tables automatically on startup
            _context.Database.EnsureCreated();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private void LoadEmployees(string searchQuery = "")
        {
            try
            {
                var query = _context.Employees.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchQuery) && !_isSearchPlaceholderActive)
                {
                    searchQuery = searchQuery.ToLower();
                    query = query.Where(e => 
                        e.FullName.ToLower().Contains(searchQuery) ||
                        e.Email.ToLower().Contains(searchQuery) ||
                        e.MobileNumber.ToLower().Contains(searchQuery) ||
                        e.Department.ToLower().Contains(searchQuery) ||
                        e.Designation.ToLower().Contains(searchQuery));
                }

                dgEmployees.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbGender.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a Gender.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!dpDateOfBirth.SelectedDate.HasValue)
            {
                MessageBox.Show("Date of Birth is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("A valid Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMobile.Text) || !Regex.IsMatch(txtMobile.Text, @"^\d{10,15}$"))
            {
                MessageBox.Show("A valid Mobile Number is required (10-15 digits).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbDepartment.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a Department.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbDesignation.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a Designation.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbEmployeeType.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select an Employee Type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!dpJoiningDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Joining Date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!double.TryParse(txtSalary.Text, out double salary) || salary <= 0)
            {
                MessageBox.Show("Salary must be a valid number greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Address is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                MessageBox.Show("City is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbState.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a State.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPincode.Text) || !Regex.IsMatch(txtPincode.Text, @"^\d{6}$"))
            {
                MessageBox.Show("Pincode must be exactly 6 digits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var employee = new Employee
                {
                    FullName = txtFullName.Text,
                    Gender = (cmbGender.SelectedItem as ComboBoxItem).Content.ToString(),
                    DateOfBirth = dpDateOfBirth.SelectedDate.Value,
                    Email = txtEmail.Text,
                    MobileNumber = txtMobile.Text,
                    Department = (cmbDepartment.SelectedItem as ComboBoxItem).Content.ToString(),
                    Designation = (cmbDesignation.SelectedItem as ComboBoxItem).Content.ToString(),
                    EmployeeType = (cmbEmployeeType.SelectedItem as ComboBoxItem).Content.ToString(),
                    JoiningDate = dpJoiningDate.SelectedDate.Value,
                    Salary = double.Parse(txtSalary.Text),
                    Address = txtAddress.Text,
                    City = txtCity.Text,
                    State = (cmbState.SelectedItem as ComboBoxItem).Content.ToString(),
                    Pincode = txtPincode.Text
                };

                _context.Employees.Add(employee);
                _context.SaveChanges();

                MessageBox.Show("Employee registered successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error registering employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee selectedEmployee)
            {
                if (!ValidateForm()) return;

                try
                {
                    selectedEmployee.FullName = txtFullName.Text;
                    selectedEmployee.Gender = (cmbGender.SelectedItem as ComboBoxItem).Content.ToString();
                    selectedEmployee.DateOfBirth = dpDateOfBirth.SelectedDate.Value;
                    selectedEmployee.Email = txtEmail.Text;
                    selectedEmployee.MobileNumber = txtMobile.Text;
                    selectedEmployee.Department = (cmbDepartment.SelectedItem as ComboBoxItem).Content.ToString();
                    selectedEmployee.Designation = (cmbDesignation.SelectedItem as ComboBoxItem).Content.ToString();
                    selectedEmployee.EmployeeType = (cmbEmployeeType.SelectedItem as ComboBoxItem).Content.ToString();
                    selectedEmployee.JoiningDate = dpJoiningDate.SelectedDate.Value;
                    selectedEmployee.Salary = double.Parse(txtSalary.Text);
                    selectedEmployee.Address = txtAddress.Text;
                    selectedEmployee.City = txtCity.Text;
                    selectedEmployee.State = (cmbState.SelectedItem as ComboBoxItem).Content.ToString();
                    selectedEmployee.Pincode = txtPincode.Text;

                    _context.Employees.Update(selectedEmployee);
                    _context.SaveChanges();

                    MessageBox.Show("Employee information updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadEmployees();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to update.", "Selection Missing", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {selectedEmployee.FullName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Employees.Remove(selectedEmployee);
                        _context.SaveChanges();
                        MessageBox.Show("Employee deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadEmployees();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.", "Selection Missing", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtEmployeeId.Text = "Auto-Generated";
            txtFullName.Clear();
            cmbGender.SelectedIndex = 0;
            dpDateOfBirth.SelectedDate = null;
            txtEmail.Clear();
            txtMobile.Clear();
            cmbDepartment.SelectedIndex = 0;
            cmbDesignation.SelectedIndex = 0;
            cmbEmployeeType.SelectedIndex = 0;
            dpJoiningDate.SelectedDate = null;
            txtSalary.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            cmbState.SelectedIndex = 0;
            txtPincode.Clear();
            
            dgEmployees.SelectedItem = null;
        }

        private void DgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee selectedEmployee)
            {
                txtEmployeeId.Text = selectedEmployee.EmployeeId.ToString();
                txtFullName.Text = selectedEmployee.FullName;
                
                SetComboBoxByContent(cmbGender, selectedEmployee.Gender);
                
                dpDateOfBirth.SelectedDate = selectedEmployee.DateOfBirth;
                txtEmail.Text = selectedEmployee.Email;
                txtMobile.Text = selectedEmployee.MobileNumber;
                
                SetComboBoxByContent(cmbDepartment, selectedEmployee.Department);
                SetComboBoxByContent(cmbDesignation, selectedEmployee.Designation);
                SetComboBoxByContent(cmbEmployeeType, selectedEmployee.EmployeeType);
                
                dpJoiningDate.SelectedDate = selectedEmployee.JoiningDate;
                txtSalary.Text = selectedEmployee.Salary.ToString();
                txtAddress.Text = selectedEmployee.Address;
                txtCity.Text = selectedEmployee.City;
                
                SetComboBoxByContent(cmbState, selectedEmployee.State);
                
                txtPincode.Text = selectedEmployee.Pincode;
            }
        }

        private void SetComboBoxByContent(ComboBox comboBox, string content)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == content)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            comboBox.SelectedIndex = 0;
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_isSearchPlaceholderActive)
            {
                txtSearch.Text = "";
                txtSearch.Foreground = new SolidColorBrush(Colors.Black);
                _isSearchPlaceholderActive = false;
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                _isSearchPlaceholderActive = true;
                txtSearch.Text = "Search employee...";
                txtSearch.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isSearchPlaceholderActive)
            {
                LoadEmployees(txtSearch.Text);
            }
        }
    }
}
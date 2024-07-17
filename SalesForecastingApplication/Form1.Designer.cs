using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SalesForecastingApplication.Models;

namespace SalesForecastingApplication
{
    public partial class Form1 : Form
    {
        private SalesContext db;

       
        private void LoadYears()
        {
            var years = db.Orders.Select(o => o.OrderDate.Year).Distinct().ToList();
            comboBoxYears.DataSource = years;
        }

        private void InitializeDataGridView()
        {
            dataGridViewOrders.AutoGenerateColumns = false;
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            int selectedYear = (int)comboBoxYears.SelectedItem;
            LoadData(selectedYear);
        }

        private void LoadData(int year)
        {
            var orders = db.Orders.Include(o => o.Product)
                                  .Where(o => o.OrderDate.Year == year)
                                  .GroupBy(o => o.State)
                                  .Select(g => new
                                  {
                                      State = g.Key,
                                      Sales = g.Sum(o => o.Product.Sales),
                                      Returns = g.Sum(o => o.Product.Sales * db.Returns
                                                              .Where(r => r.OrderID == o.OrderID)
                                                              .Sum(r => r.ReturnAmount))
                                  })
                                  .ToList();

            dataGridViewOrders.DataSource = orders.Select(o => new
            {
                o.State,
                TotalSales = o.Sales - o.Returns
            }).ToList();
        }

        private void btnGenerateForecast_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxPercentage.Text, out decimal percentageIncrease))
            {
                DisplayForecast(percentageIncrease);
            }
            else
            {
                MessageBox.Show("Please enter a valid percentage increase.");
            }
        }

        private void DisplayForecast(decimal percentage)
        {
            var orders = db.Orders.Include(o => o.Product).ToList();
            var forecasts = orders.GroupBy(o => o.State)
                                  .Select(g => new
                                  {
                                      State = g.Key,
                                      ForecastedSales = g.Sum(o => o.Product.Sales) * (1 + percentage / 100)
                                  })
                                  .ToList();

            dataGridViewOrders.DataSource = forecasts.Select(f => new
            {
                f.State,
                ForecastedSales = f.ForecastedSales
            }).ToList();
        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void ExportToCSV()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Save forecast data as CSV"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.WriteLine("State,Percentage Increase,Forecasted Sales");
                    foreach (DataGridViewRow row in dataGridViewOrders.Rows)
                    {
                        string state = row.Cells["State"].Value.ToString();
                        string forecastedSales = row.Cells["ForecastedSales"].Value.ToString();
                        writer.WriteLine($"{state},{textBoxPercentage.Text},{forecastedSales}");
                    }
                }
                MessageBox.Show("CSV file saved successfully.");
            }
        }

        #region Windows Form Designer generated code

        private ComboBox comboBoxYears;
        private DataGridView dataGridViewOrders;
        private TextBox textBoxPercentage;
        private Button btnLoadData;
        private Button btnGenerateForecast;
        private Button btnExportCSV;

        private void InitializeComponent()
        {
            comboBoxYears = new ComboBox();
            dataGridViewOrders = new DataGridView();
            textBoxPercentage = new TextBox();
            btnLoadData = new Button();
            btnGenerateForecast = new Button();
            btnExportCSV = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOrders).BeginInit();
            SuspendLayout();
            // 
            // comboBoxYears
            // 
            comboBoxYears.FormattingEnabled = true;
            comboBoxYears.Location = new Point(349, 51);
            comboBoxYears.Name = "comboBoxYears";
            comboBoxYears.Size = new Size(121, 23);
            comboBoxYears.TabIndex = 0;
            // 
            // dataGridViewOrders
            // 
            dataGridViewOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOrders.Location = new Point(349, 147);
            dataGridViewOrders.Name = "dataGridViewOrders";
            dataGridViewOrders.Size = new Size(327, 150);
            dataGridViewOrders.TabIndex = 1;
            // 
            // textBoxPercentage
            // 
            textBoxPercentage.Location = new Point(349, 109);
            textBoxPercentage.Name = "textBoxPercentage";
            textBoxPercentage.Size = new Size(100, 23);
            textBoxPercentage.TabIndex = 2;
            // 
            // btnLoadData
            // 
            btnLoadData.Location = new Point(538, 51);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(75, 23);
            btnLoadData.TabIndex = 3;
            btnLoadData.Text = "Load Data";
            btnLoadData.UseVisualStyleBackColor = true;
            btnLoadData.Click += btnLoadData_Click;
            // 
            // btnGenerateForecast
            // 
            btnGenerateForecast.Location = new Point(538, 109);
            btnGenerateForecast.Name = "btnGenerateForecast";
            btnGenerateForecast.Size = new Size(138, 23);
            btnGenerateForecast.TabIndex = 4;
            btnGenerateForecast.Text = "Generate Forecast";
            btnGenerateForecast.UseVisualStyleBackColor = true;
            btnGenerateForecast.Click += btnGenerateForecast_Click;
            // 
            // btnExportCSV
            // 
            btnExportCSV.Location = new Point(562, 303);
            btnExportCSV.Name = "btnExportCSV";
            btnExportCSV.Size = new Size(114, 23);
            btnExportCSV.TabIndex = 5;
            btnExportCSV.Text = "Export to CSV";
            btnExportCSV.UseVisualStyleBackColor = true;
            btnExportCSV.Click += btnExportCSV_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnExportCSV);
            Controls.Add(btnGenerateForecast);
            Controls.Add(btnLoadData);
            Controls.Add(textBoxPercentage);
            Controls.Add(dataGridViewOrders);
            Controls.Add(comboBoxYears);
            Name = "Form1";
            Text = "Sales Forecasting Application";
            ((System.ComponentModel.ISupportInitialize)dataGridViewOrders).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}

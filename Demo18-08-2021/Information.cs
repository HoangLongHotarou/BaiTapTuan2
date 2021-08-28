using Demo18_08_2021.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo18_08_2021
{
    public partial class Information : Form
    {

        private int _foodId;

        public Food returnedFood;

        public Information(int foodId = 0)
        {
            InitializeComponent();
            _foodId = foodId;
        }

        private void Information_Load(object sender, EventArgs e)
        {
            cbbFood.DataSource = WorkingContext.CategoryList;
            cbbFood.DisplayMember = "Name";
            cbbFood.ValueMember = "Id";
            if (_foodId > 0)
            {
                txtIDName.Text = returnedFood.Id.ToString();
                txtFoodName.Text = returnedFood.Name;
                txtUnit.Text = returnedFood.Unit;
                nudPrice.Value = returnedFood.UnitPrice;
                cbbFood.SelectedIndex = returnedFood.CategoryId;
                txtPathImage.Text = returnedFood.ImageLink;
                rtbDescription.Text = returnedFood.Description;
                try
                {
                    ptbFoodImage.SizeMode = PictureBoxSizeMode.Zoom;
                    ptbFoodImage.Load(txtPathImage.Text);
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var name = txtFoodName.Text;
            var unit = txtUnit.Text;
            var description = rtbDescription.Text;
            var link = txtPathImage.Text;
            var price = Convert.ToInt32(nudPrice.Value);
            var categoryId = Convert.ToInt32(cbbFood.SelectedValue);

            if (_foodId == 0)
            {
                returnedFood = new Food(WorkingContext.FoodList.Any()?WorkingContext.FoodList.Max(p=>p.Id)+1:1,name,unit,price,description,link,categoryId);
                WorkingContext.FoodList.Add(returnedFood);
            }
            else
            {
                var food = WorkingContext.FoodList.FirstOrDefault(p => p.Id == int.Parse(txtIDName.Text));
                food.Name = name;
                food.Unit = unit;
                food.Description = description;
                food.ImageLink = link;
                food.UnitPrice = price;
                food.CategoryId = categoryId;
                returnedFood = food;
            }
            

            DialogResult = DialogResult.OK;
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Dialog";// "Add Photos";
            dlg.Multiselect = true;
            dlg.Filter = "Image Files (JPEG, GIF, BMP, etc.)|"
            + "*.jpg;*.jpeg;*.gif;*.bmp;"
            + "*.tif;*.tiff;*.png|"
            + "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|"
            + "GIF files (*.gif)|*.gif|"
            + "BMP files (*.bmp)|*.bmp|"
            + "TIFF files (*.tif;*.tiff)|*.tif;*.tiff|"
            + "PNG files (*.png)|*.png|"
            + "All files (*.*)|*.*";

            dlg.InitialDirectory = Environment.CurrentDirectory;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ptbFoodImage.SizeMode = PictureBoxSizeMode.Zoom;
                txtPathImage.Text = dlg.FileName;
                ptbFoodImage.Load(dlg.FileName);
            }
        }
    }
}

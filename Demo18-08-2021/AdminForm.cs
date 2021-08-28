using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Demo18_08_2021.DTO;

namespace Demo18_08_2021
{
	public partial class AdminForm : Form
	{
		private int _currentCategoryId = 0;
		private const string _placeHolderText = "Nhap ten mon an";

		public AdminForm()
		{
			InitializeComponent();
		}

		private void AdminForm_Load(object sender, EventArgs e)
		{
			LoadTableToLvDetail();
			LoadControlFromPanel();
			SetUpSearchInputText();
		}
		private void LoadTableToLvDetail()
		{
			lvTableList.Items.Clear();
			foreach (var table in WorkingContext.TableList)
			{
				ListViewItem item = new ListViewItem(table.TableId.ToString());
				item.SubItems.Add(table.TableName);
				item.SubItems.Add(table.Status == 1 ? "Có người" : "Trống");
				item.SubItems.Add(table.Floor.ToString());

				lvTableList.Items.Add(item);
			}
		}
		private void btnAdd_Click(object sender, EventArgs e)
		{
			var tableName = txtTableName.Text;
			var floor = int.Parse(cbbFloor.Text);
			var tableId = WorkingContext.TableList.Max(p => p.TableId) + 1;

			DiningTable table = new DiningTable(tableId, tableName, floor);
			WorkingContext.TableList.Add(table);

			LoadTableToLvDetail();
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			var tableName = txtTableName.Text;
			var floor = int.Parse(cbbFloor.Text);
			var tableId = int.Parse(txtTableId.Text);

			var table = WorkingContext.TableList.FirstOrDefault(p => p.TableId == tableId);
			table.TableName = tableName;
			table.Floor = floor;

			LoadTableToLvDetail();
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			var tableId = int.Parse(txtTableId.Text);

			WorkingContext.TableList.RemoveAll(p => p.TableId == tableId);
			LoadTableToLvDetail();
		}

		private void tsmiDelete_Click(object sender, EventArgs e)
		{
			var listCheckedItem = lvTableList.CheckedItems;

			foreach (ListViewItem item in listCheckedItem)
			{
				var id = int.Parse(item.SubItems[0].Text);
				WorkingContext.TableList.RemoveAll(p => p.TableId == id);
			}

			LoadTableToLvDetail();
		}

		private void lvTableList_SelectedIndexChanged(object sender, EventArgs e)
		{
			var count = lvTableList.SelectedItems.Count;

			if (count > 0)
			{
				var item = lvTableList.SelectedItems[0];

				txtTableId.Text = item.SubItems[0].Text;
				txtTableName.Text = item.SubItems[1].Text;
				cbbFloor.SelectedIndex = int.Parse(item.SubItems[3].Text) - 1;

				btnDelete.Enabled = true;
				btnUpdate.Enabled = true;
			}
		}

      


		private void CreateButton(Category item, int yPos)
        {
			var btn = new Button();
			btn.Text = item.Name;
			btn.BackColor = Color.AntiqueWhite;
			btn.Height = 30;
			btn.Width = 200;
			btn.Location = new Point(0, yPos);
			btn.Tag = item.Id;
			btn.Click += CategoryButton_Click;
			pnControl.Controls.Add(btn);
		}

        private void CategoryButton_Click(object sender, EventArgs e)
        {
			_currentCategoryId = Convert.ToInt32((sender as Button).Tag);
			var foods = WorkingContext.FoodList;
            if (_currentCategoryId > 0)
            {
				foods = foods.Where(f => f.CategoryId == _currentCategoryId).ToList();
            }
			LoadFoodToLvDetail(foods);
		}

		private void AddFoodToLV(Food food)
        {
			ListViewItem item = new ListViewItem(food.Id.ToString());
			item.SubItems.Add(food.Name);
			item.SubItems.Add(food.Unit);
			item.SubItems.Add(food.UnitPrice.ToString());
			item.SubItems.Add(food.CategoryId.ToString());
			item.SubItems.Add(food.Description);
			item.SubItems.Add(food.ImageLink);
			lvMenu.Items.Add(item);
		}

		private void LoadFoodToLvDetail(List<Food> list)
        {
			lvMenu.Items.Clear();
			foreach (var food in list)
			{
				AddFoodToLV(food);
			}
		}

        private void LoadControlFromPanel()
        {
			int yPos = 0;
			var btn = new Button();
			btn.Text = "Tất cả";
			btn.BackColor = Color.Aquamarine;
			btn.Height = 30;
			btn.Width = 200;
			btn.Tag = 0;
			btn.Click += CategoryButton_Click;
			btn.Location = new Point(0, yPos);
			pnControl.Controls.Add(btn);
            foreach (var item in WorkingContext.CategoryList)
            {
				yPos += 35;
				CreateButton(item, yPos);
            }
        }

        private void SetUpSearchInputText()
        {
			txtSearch.Text = _placeHolderText;
			txtSearch.GotFocus += RemovePlaceHolerText;
			txtSearch.LostFocus += ShowPlaceHolderText;
        }

        private void ShowPlaceHolderText(object sender, EventArgs e)
        {
			if (string.IsNullOrWhiteSpace(txtSearch.Text))
				txtSearch.Text = _placeHolderText;
        }

        private void RemovePlaceHolerText(object sender, EventArgs e)
        {
			if (txtSearch.Text == _placeHolderText)
				txtSearch.Text = "";
        }

		private void btnAdding_Click(object sender, EventArgs e)
		{
			using (Information dialog = new Information())
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					var food = dialog.returnedFood;
					AddFoodToLV(food);
				}
			}
		}

		private void lvMenu_DoubleClick(object sender, EventArgs e)
        {
			Information dialog = new Information(1);
			int categoryID = 0;
				int count = lvMenu.SelectedItems.Count;
				if (count > 0)
				{
					int id = int.Parse(lvMenu.SelectedItems[0].SubItems[0].Text);
					string name = lvMenu.SelectedItems[0].SubItems[1].Text;
					string unit = lvMenu.SelectedItems[0].SubItems[2].Text;
					var price = Convert.ToInt32(lvMenu.SelectedItems[0].SubItems[3].Text);
					categoryID = int.Parse(lvMenu.SelectedItems[0].SubItems[4].Text);
					string decription = lvMenu.SelectedItems[0].SubItems[5].Text;
					string path = lvMenu.SelectedItems[0].SubItems[6].Text;
					dialog.returnedFood = new Food(id, name, unit, price, decription, path, categoryID-1);
				}
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					var foods = WorkingContext.FoodList;
					if (dialog.returnedFood.CategoryId > 0)
					{
						foods = foods.Where(f => f.CategoryId == dialog.returnedFood.CategoryId).ToList();
					}
					LoadFoodToLvDetail(foods);
			}
		}

        private void lvMenu_Click(object sender, EventArgs e)
        {
			lbNameOfFood.Text = lvMenu.SelectedItems[0].SubItems[1].Text;
            try
            {
				ptbPic.SizeMode = PictureBoxSizeMode.Zoom;
				ptbPic.Load(lvMenu.SelectedItems[0].SubItems[6].Text);
			}
			catch (Exception)
            {
                //throw;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
			var foods = WorkingContext.FoodList;
			lvMenu.Items.Clear();
            foreach (var food in foods)
            {
                if (food.Name.ToLower().StartsWith(txtSearch.Text.ToLower()))
                {
					AddFoodToLV(food);
                }
            }
		}
    }
}

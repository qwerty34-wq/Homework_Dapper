using DapperRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main_Dapper
{
    public partial class Form1 : Form
    {
        static string connection = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
        static GenericUnitOfWork work = new GenericUnitOfWork(connection);

        static IGenericRepository<Product> repoProd = work.Repository<Product>();
        static IGenericRepository<Category> repoCat = work.Repository<Category>();

        //List<Product> products = repoProd.GetAll().ToList();
        //List<Category> categories = repoCat.GetAll().ToList();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void SelectAll()
        {
            var pc = from p in repoProd.GetAll()
                     join c in repoCat.GetAll() on p.CategoryId equals c.Id
                     select new { ProductId = p.Id, ProductName = p.Name, ProductCategory = c.Name };

            pc = pc.ToList();

            dataGridView1.DataSource = pc;
        }

    }
}

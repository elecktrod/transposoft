using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using ExcelDataReader;
using Microsoft.Win32;
using Transposoft.Base;
using Transposoft.Models;

namespace Transposoft.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Constructor
        public MainViewModel()
        {
            OpenFileCommand = new RelayCommand(DoOpenFile);
            MergeCommand = new RelayCommand(DoMerge);
        }
        #endregion

        #region CommandMethods
        public RelayCommand MergeCommand { get; private set; }
        private void DoMerge(object obj)
        {
            try
            {
                if (File1 == null || File2 == null)
                    throw new FileNotFoundException();
                if (File1 != file1Сache)
                {
                    ExcelFile1 = LoadExcel(File1);
                    file1Сache = File1;
                }
                if (File2 != file2Сache)
                {
                    ExcelFile2 = LoadExcel(File2);
                    file2Сache = File2;
                }
                MainModel = Merge(ExcelFile1, ExcelFile2);
            }
            catch (FileNotFoundException){
                MessageBox.Show("Файл не найден");
            }
            catch (ArgumentNullException) {
                MessageBox.Show("Имя файла не может быть пустым");
            }
            catch (IOException){
                MessageBox.Show("Файл уже открыт");
            }
            catch (InvalidCastException){
                MessageBox.Show("Ошибка формата таблицы в excel файле");
            }
            
        }

        public RelayCommand OpenFileCommand { get; private set; }
        private void DoOpenFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (openFileDialog.ShowDialog() == true)
            {
                switch (obj.ToString())
                {
                    case "file1":
                        File1 = openFileDialog.FileName;
                        break;
                    case "file2":
                        File2 = openFileDialog.FileName;
                        break;
                }
            }
        }
        #endregion

        #region Methods
        public List<MainModel> Merge(List<ExcelModel> model1, List<ExcelModel> model2)
        {
            int lastId = model1.Last().Id;
            DateTime? dateFrom = DateTime.MinValue;
            if (DateFrom != null) { dateFrom = DateFrom; }
            DateTime? dateTo = DateTime.MaxValue;
            if (DateTo != null) { dateTo = DateTo; }
            List<ExcelModel> m1 = new List<ExcelModel>(model1.FindAll(m => m.DateFrom >= dateFrom || m.DateFrom == null).FindAll(m => m.DateTo <= dateTo || m.DateTo == null));
            List<ExcelModel> m2 = new List<ExcelModel>(model2.FindAll(m => m.DateFrom >= dateFrom || m.DateFrom == null).FindAll(m => m.DateTo <= dateTo || m.DateTo == null));
            List<MainModel> result = new List<MainModel>();
            foreach(var model in m1)
            {
                MainModel mainModel = new MainModel(model, 0, null);
                int index = m2.FindIndex(m => m.Cipher == model.Cipher);
                if (index != -1){
                    mainModel.ExtID = m2[index].Id;
                    if (mainModel.DateFrom > m2[index].DateFrom || m2[index].DateFrom == null)
                    {
                        mainModel.DateFrom = m2[index].DateFrom;
                    }
                    if (mainModel.DateTo < m2[index].DateTo || m2[index].DateTo == null)
                    {
                        mainModel.DateTo = m2[index].DateTo;
                    }
                    m2.RemoveAt(index);
                }
                result.Add(mainModel);
            }
            foreach (var model in m2)
            {
                result.Add(new MainModel(model, 1, ++lastId));
            }
            return result;
        }

        public List<ExcelModel> LoadExcel(string filename)
        {
            List<ExcelModel> result = new List<ExcelModel>();
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet ds = reader.AsDataSet();
                    System.Data.DataTable dt = ds.Tables[0];
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        var excelModel = new ExcelModel(
                            Convert.ToInt32(dt.Rows[i][0]),
                            dt.Rows[i][1].ToString(),
                            dt.Rows[i][2].ToString(),
                            null,
                            null);
                        DateTime date;
                        if (DateTime.TryParse(dt.Rows[i][3].ToString(), out date))
                        {
                            excelModel.DateFrom = DateTime.Parse(dt.Rows[i][3].ToString());
                        }
                        if (DateTime.TryParse(dt.Rows[i][4].ToString(), out date))
                        {
                            excelModel.DateTo = DateTime.Parse(dt.Rows[i][4].ToString());
                        }
                        result.Add(excelModel);
                    }
                }
            }
            return result;
        }
        #endregion

        #region Properties
        public List<MainModel> _mainModel = new List<MainModel>();
        public List<MainModel> MainModel
        {
            get { return _mainModel; }
            set { SetProperty(ref _mainModel, value); }
        }

        public List<ExcelModel> ExcelFile1 { get; set; }
        public List<ExcelModel> ExcelFile2 { get; set; }

        private string file1Сache;
        private string _file1;
        public string File1
        {
            get { return _file1; }
            set { SetProperty(ref _file1, value); }
        }

        private string file2Сache;
        private string _file2;
        public string File2
        {
            get { return _file2; }
            set { SetProperty(ref _file2, value); }
        }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        #endregion
    }
}

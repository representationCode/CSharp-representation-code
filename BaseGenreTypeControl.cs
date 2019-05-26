using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WebAPIConnector;
using ipsFormsDesign.Model;
using ipsFormsDesign.Dictionary;
using Telerik.WinControls.UI;
using ipsFormsDesign.Services;
using IpsViewComponentLibrary.Extensions;
using sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers;

namespace sysAdminwp.ComplexDictionariesBaseControls
{
    public partial class BaseGenreTypeControl : UserControl
    {
        GenreTypeLayer _layer = new GenreTypeLayer();
        public delegate GenreType GetDictionaryLayer(int id);
        public delegate GenreType CreateLayer(GenreType sors);
        public delegate GenreType UpdateLayer(GenreType sors);
        public delegate bool DeleteLayer(int id);
        private readonly List<string> Langs = new List<string>
        {
            "Uk", "Ru", "En"
        };

        private const string DelMess = "Об'єкт не видалений!";
        private const string GetMess = "об'єкт не знайдено";
        private const string CreateMess = "Помилка формату";
        private const string UpdateMess = "Помилка формату";
        private const string UrlBaseDict = "dictionary/base/Category";

        private readonly DictionaryService _dictService = DictionaryService.GetInstance();
        private readonly ApiConnector _connector;

        //define data for different languages
        private GenreTypeTr ukGenreType;
        private GenreTypeTr ruGenreType;
        private GenreTypeTr enGenreType;
        
        //data grid
        private List<GenreType> _genreTypeList;
        public List<GenreType> GenreTypeList
        {
            get { return _genreTypeList; }
            set
            {
                if (value != null)
                {
                    _genreTypeList = value;
                    //show current count elements into grid
                    totalGenreType.Text = _genreTypeList.Count.ToString();
                }
            }
        }
        //item data
        private GenreType genreTypeData;
        //url for get data to grid
        private readonly string urlDataGrid;
        //for control current select of grid
        private int indexSelectedElem;
        //control 'Enabled' state for elements
        private bool controlEnabledActiveElements = true;
        public bool ControlEnabledActiveElements
        {
            get { return controlEnabledActiveElements; }
            set
            {
                controlEnabledActiveElements = value;
                //'search' element
                txtSearchGenreType.Enabled = value;
                //filter for grid
                ddlFilterGenreType.Enabled = value;
                //grid element
                gridGenreTypeList.Enabled = value;
                //'Add' button
                btnAddGenreType.Enabled = value;
                //'Delete' button
                btnDeleteGenreType.Enabled = value;
                //'Save' button (bottom panel)
                btnSaveGenreType.Enabled = !value;
                //'Cancel' button (bottom panel)
                btnCancelGenreType.Enabled = !value;
            }
        }
        public BaseGenreTypeControl(ApiConnector connector, string urlForComplexDict)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            _connector = connector;
            urlDataGrid = urlForComplexDict;
            //initialize data for drop down lists
            ddlFilterGenreType.DataSource = bsSimpleDictGenreType.DataSource = InitializeDataForFilter();
            //initialize data for grid
            gridGenreTypeList.DataSource = HandleDataForGrid(urlForComplexDict);
            //setup search posibility for textBox element
            txtSearchGenreType.SetUsageGrid(gridGenreTypeList);
            
            //set default data for bottom panel
            if (GenreTypeList.Count != 0)
                DataUpdateForBottomPanel(
                    GenreTypeList[0].CodeI
                    );
            ControlEnabledActiveElements = true;
        }

        //handle data for grid
        private List<ViewTable> HandleDataForGrid(string urlForGetDataDictionaries)
        {
            //check on valid state parameter
            if (urlForGetDataDictionaries == null) return new List<ViewTable>();
            //get data dictionaries -> for grid
            GenreTypeList = _connector.GetList<GenreType>(urlForGetDataDictionaries);
            //data is not exist
            if (GenreTypeList == null)
            {
                GenreTypeList = new List<GenreType>();
                return new List<ViewTable>();
            }

            List<ViewTable> dataGrid = new List<ViewTable>();
            //initialize grid data
            foreach (var item in GenreTypeList)
            {
                dataGrid.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.GenreTypeTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.Name ?? item.Name,
                    NameRu = item.GenreTypeTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.Name,
                    NameEn = item.GenreTypeTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.Name,
                    VariantObject = ddlFilterGenreType.Items.First(el => (int)el.Value == item.CategoryCode).Text,
                });
            }
            return dataGrid;
        }

        //handle data for drop down list - 'Спосіб використання'
        private List<DictBaseTypeI> InitializeDataForFilter()
        {
            //get data for drop down list
            return _connector.GetList<DictBaseTypeI>(UrlBaseDict) ?? new List<DictBaseTypeI>();
        }

        //show data for bottom panel
        private void gvCopyrightKindList_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (!(gridGenreTypeList.CurrentRow.DataBoundItem is ViewTable currentRow))
                return;

            if (!int.TryParse(currentRow.Code, out int code))
                return;
            indexSelectedElem = GenreTypeList.FindIndex(el => el.CodeI == code);
            DataUpdateForBottomPanel(code);
        }

        //refresh changed data for bottom panel
        private void DataUpdateForBottomPanel(int codeI)
        {
            //get object by parameter 'code' from service
            genreTypeData = CheckAfterGetQuery(codeI);
            //use data for binding source
            bsGenreType.DataSource = genreTypeData;
            //define binding for source of the 'Uk'
            ukGenreType = genreTypeData.GenreTypeTr.FirstOrDefault(rec => rec.Lang == Langs[0]);
            if (ukGenreType == null)
            {
                ukGenreType = new GenreTypeTr { Lang = Langs[0] };
                genreTypeData.GenreTypeTr.Add(ukGenreType);
            }
            bsUkLangGenreType.DataSource = ukGenreType;
            //defined binding for source of the 'Ru'
            ruGenreType = genreTypeData.GenreTypeTr.FirstOrDefault(rec => rec.Lang == Langs[1]);
            if (ruGenreType == null)
            {
                ruGenreType = new GenreTypeTr { Lang = Langs[1] };
                genreTypeData.GenreTypeTr.Add(ruGenreType);
            }
            bsRuLangGenreType.DataSource = ruGenreType;
            //define binding for source of the 'En'
            enGenreType = genreTypeData.GenreTypeTr.FirstOrDefault(rec => rec.Lang == Langs[2]);
            if (enGenreType == null)
            {
                enGenreType = new GenreTypeTr { Lang = Langs[2] };
                genreTypeData.GenreTypeTr.Add(enGenreType);
            }
            bsEnLangGenreType.DataSource = enGenreType;
        }

        private GenreType CheckAfterGetQuery(int codeI)
        {
            GetDictionaryLayer dictDelegate = new GetDictionaryLayer(_layer.GetDictionary);
            GenreType result = dictDelegate(codeI);
            if (result == null)
            {
                MessageShow.ShowError(GetMess);
                //set select first element of grid
                result = GenreTypeList[0];
            }
            return result;
        }

        //select item into drop down list for filtered data into grid
        private void ddlCommonKeyData_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            //stop perform if position is not exist
            if (e.Position == -1) return;
            //get index current data
            string valueDropList = ddlFilterGenreType.Items[e.Position].Value.ToString();
            if (int.TryParse(valueDropList, out int selectedItemData))
                gridGenreTypeList.DataSource = FilterDataIntoGrid(selectedItemData);
        }

        //filter data for grid
        private List<ViewTable> FilterDataIntoGrid(int code)
        {
            //get all data of the 'CopyrightKind'
            GenreTypeList = _connector.GetList<GenreType>(urlDataGrid);
            //stop perform if data is not exist
            if (GenreTypeList == null) return new List<ViewTable>();
            //filter data by parameter 'code'
            if (GenreTypeList.Count == 0)
                GenreTypeList = GenreTypeList.Where(item => item.CategoryCode == code).ToList();
            List<ViewTable> filteredData = new List<ViewTable>();
            //initialize data for grid
            foreach (var item in GenreTypeList)
            {
                filteredData.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.GenreTypeTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.Name ?? item.Name,
                    NameRu = item.GenreTypeTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.Name,
                    NameEn = item.GenreTypeTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.Name,
                    VariantObject = ddlFilterGenreType.SelectedItem.Text,
                });
            }
            return filteredData;
        }

        //action 'Add' new object to grid
        private void btnAddComplex_Click(object sender, EventArgs e)
        {
            //change property 'Text' of the btnSave for 'Add' action
            btnSaveGenreType.Text = "Додати";
            //refresh data for field 'Code'
            genreTypeData = new GenreType { CategoryCode = 1 };
            bsGenreType.DataSource = genreTypeData;
            //refresh data for field 'Uk'
            ukGenreType = new GenreTypeTr { Lang = Langs[0] };
            genreTypeData.GenreTypeTr.Add(ukGenreType);
            bsUkLangGenreType.DataSource = ukGenreType;
            //refresh data for field 'Ru'
            ruGenreType = new GenreTypeTr { Lang = Langs[1] };
            genreTypeData.GenreTypeTr.Add(ruGenreType);
            bsRuLangGenreType.DataSource = ruGenreType;
            //refresh data for field 'Ru'
            enGenreType = new GenreTypeTr { Lang = Langs[2] };
            genreTypeData.GenreTypeTr.Add(enGenreType);
            bsEnLangGenreType.DataSource = enGenreType;
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //action 'Delete' exist object from grid
        private void btnDeleteComplex_Click(object sender, EventArgs e)
        {
            //return if element by index is not exist
            if (!(GenreTypeList.Count > indexSelectedElem))
                return;
            //delete selected item of grid by field 'Code'
            GenreType item = GenreTypeList[indexSelectedElem];
            bool result = CheckAfterDelete(item.CodeI);
            //update data for grid
            gridGenreTypeList.DataSource = HandleDataForGrid(urlDataGrid);
            //step up in grid position
            if (result) --indexSelectedElem;
            if (indexSelectedElem < 0) indexSelectedElem = 0;
            gridGenreTypeList.CurrentRow = gridGenreTypeList.Rows[indexSelectedElem];
            //define new currentRow
            DataUpdateForBottomPanel(
                GenreTypeList[indexSelectedElem].CodeI
                );
        }

        /// <summary>
        /// control delete state
        /// </summary>
        /// <param name="codeI"></param>
        /// <returns></returns>
        private bool CheckAfterDelete(int codeI)
        {
            DeleteLayer dictDelegat = new DeleteLayer(_layer.Delete);
            bool result = dictDelegat(codeI);
            if (!result)
            {
                MessageShow.ShowError(DelMess);
            }
            return result;
        }

        //action 'Save' object to grid
        private void btnSave_Click(object sender, EventArgs e)
        {
            //refresh bottom panel
            int codeIAfter = 0;
            if (btnSaveGenreType.Text == "Додати")
            {
                //send created object
                GenreType result = CheckAfterCreate(genreTypeData);
                codeIAfter = result.CodeI;
                //update data for grid
                gridGenreTypeList.DataSource = HandleDataForGrid(urlDataGrid);
                indexSelectedElem = GenreTypeList.FindIndex(el => el.CodeI == result.CodeI);
                //select certain item into grid
                gridGenreTypeList.CurrentRow = gridGenreTypeList.Rows[indexSelectedElem];
                //set default state for 'Text' of the btnSave
                btnSaveGenreType.Text = "Зберегти";
            } else if (btnSaveGenreType.Text == "Зберегти")
            {
                //save changes
                GenreType result = CheckAfterUpdate(genreTypeData);
                codeIAfter = result.CodeI;
                //save last selected item
                indexSelectedElem = gridGenreTypeList.CurrentRow.Index;
                //update data for grid
                gridGenreTypeList.DataSource = HandleDataForGrid(urlDataGrid);
                //select certain item into grid
                gridGenreTypeList.CurrentRow = gridGenreTypeList.Rows[indexSelectedElem];
            }
            DataUpdateForBottomPanel(codeIAfter);
            //stop perform if elements have 'Enabled' -> true
            if (ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = true;
        }

        /// <summary>
        /// check result data after 'Create'
        /// </summary>
        /// <param name="createItem"></param>
        /// <returns></returns>
        private GenreType CheckAfterCreate(GenreType createItem)
        {
            CreateLayer dictDelegat = new CreateLayer(_layer.Create);
            GenreType result = dictDelegat(createItem);
            if (result == null)
            {
                MessageShow.ShowError(CreateMess);
                //select first element of grid by default
                result = GenreTypeList[0];
            }
            return result;
        }

        /// <summary>
        /// check result data after 'Update'
        /// </summary>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private GenreType CheckAfterUpdate(GenreType updateItem)
        {
            UpdateLayer dictDelegat = new UpdateLayer(_layer.Update);
            GenreType result = dictDelegat(genreTypeData);
            if (result == null)
            {
                MessageShow.ShowError(UpdateMess);
                //select first element of grid by default
                result = GenreTypeList[0];
            }
            return result;
        }

        //action 'Cancel' reset changed data for selected item
        private void btnCancel_Click(object sender, EventArgs e)
        {
            int codeICurrent = GenreTypeList[indexSelectedElem].CodeI;
            //reset changed data
            DataUpdateForBottomPanel(codeICurrent);
            //stop perform if elements have 'Enabled' -> true
            if (ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = true;
        }

        /// <summary>
        /// changed data of the field 'Uk'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUkLangGenreType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveGenreType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveGenreType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// changed data of the field 'Ru'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRuLangGenreType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveGenreType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveGenreType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// changed data of the field 'En'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEnLangGenreType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveGenreType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveGenreType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// event for field 'Code' and for dropDownList (bottom panel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlEditKeyGenreType_Click(object sender, EventArgs e)
        {
            if (btnSaveGenreType.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveGenreType.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        /// <summary>
        /// allow input only number data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCodeGenreType_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if it 'number' or 'Backspace' -> allow input
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (btnSaveGenreType.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveGenreType.Text = "Зберегти";
                }
                //stop perform if elements have 'Enabled' -> false
                if (!ControlEnabledActiveElements) return;
                //change 'Enabled' state for elements
                ControlEnabledActiveElements = false;
                return;
            }
            else
                //disallow input
                e.Handled = true;
        }

        
    }
}

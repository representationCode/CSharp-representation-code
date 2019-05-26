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
    public partial class BaseCountryControl : UserControl
    {
        CountryLayer _layer = new CountryLayer();
        public delegate Country GetDictionaryLayer(int id);
        public delegate Country CreateLayer(Country sors);
        public delegate Country UpdateLayer(Country sors);
        public delegate bool DeleteLayer(int id);
        private readonly List<string> Langs = new List<string>
        {
            "Uk", "Ru", "En"
        };

        private const string DelMess = "Об'єкт не видалений!";
        private const string GetMess = "об'єкт не знайдено";
        private const string CreateMess = "Помилка формату";
        private const string UpdateMess = "Помилка формату";
        private const string UrlBaseDict = "dictionary/base/Continent";

        private readonly DictionaryService _dictionaryService = DictionaryService.GetInstance();
        private readonly ApiConnector _connector;

        //define data for different languages
        private CountryTr ukCountry;
        private CountryTr ruCountry;
        private CountryTr enCountry;
        //define grid data
        private List<Country> _countryList;
        public List<Country> CountryList
        {
            get => _countryList;
            set
            {
                if (value != null)
                {
                    _countryList = value;
                    totalCountry.Text = _countryList.Count.ToString();
                }
            }
        }

        //item data
        private Country countryData;

        //url for get data to grid
        private readonly string urlDataGrid;

        private int indexSelectedElem;

        //control 'Enabled' state for elements
        private bool controlEnabledActiveElements = false;
        public bool ControlEnabledActiveElements
        {
            get { return controlEnabledActiveElements; }
            set
            {
                controlEnabledActiveElements = value;
                //'search' element
                txtSearchCountry.Enabled = value;
                //filter for grid
                ddlFilterCountry.Enabled = value;
                //grid element
                gridCountryList.Enabled = value;
                //'Add' button
                btnAddCountry.Enabled = value;
                //'Delete' button
                btnDeleteCountry.Enabled = value;
                //'Save' button (bottom panel)
                btnSaveCountry.Enabled = !value;
                //'Cancel' button (bottom panel)
                btnCancelCountry.Enabled = !value;
            }
        }
        public BaseCountryControl(ApiConnector connector, string urlForComplexDict)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            _connector = connector;
            urlDataGrid = urlForComplexDict;
            //initialize data for drop down lists
            ddlFilterCountry.DataSource = bsSimpleDictCountry.DataSource = InitializeDataForFilter();
            //initialize data for grid
            gridCountryList.DataSource = HandleDataForGrid(urlForComplexDict);
            //setup search posibility for textBox element
            txtSearchCountry.SetUsageGrid(gridCountryList);

            //set default data for bottom panel
            if (CountryList.Count != 0)
                DataUpdateForBottomPanel(
                    CountryList[0].CountryCode
                    );
            ControlEnabledActiveElements = true;
        }
        
        //handle data for grid
        private List<ViewTable> HandleDataForGrid(string urlForGetDataDictionaries)
        {
            //check on valid state parameter
            if (urlForGetDataDictionaries == null) return new List<ViewTable>();
            //get data dictionaries -> for grid
            CountryList = _connector.GetList<Country>(urlForGetDataDictionaries);
            //result data for initialize to grid
            List<ViewTable> dataGrid = new List<ViewTable>();
            //initialize grid data
            foreach (var item in CountryList)
            {
                dataGrid.Add(new ViewTable
                {
                    Code = item.CountryCode,
                    NameUk = item.CountryTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.CountryName ?? item.CountryName,
                    NameRu = item.CountryTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.CountryName,
                    NameEn = item.CountryTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.CountryName,
                    Continent = item.ContinentCode != null ? ddlFilterCountry.Items.First(el => (int)el.Value == item.ContinentCode).Text : "",
                });
            }
            return dataGrid;
        }

        //handle data for drop down list - 'Спосіб використання'
        private List<DictBaseTypeI> InitializeDataForFilter()
        {
            return _connector.GetList<DictBaseTypeI>(UrlBaseDict) ?? new List<DictBaseTypeI>();
        }

        //show data for bottom panel
        private void gvCopyrightKindList_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (!(gridCountryList.CurrentRow?.DataBoundItem is ViewTable currentRow))
                return;
            //check field 'Code' on correct type 'int'
            //if (!int.TryParse(currentRow.Code, out int code))
            //    return;
            indexSelectedElem = CountryList.FindIndex(item => item.CountryCode == currentRow.Code);
            DataUpdateForBottomPanel(currentRow.Code);
        }

        //refresh changed data for bottom panel
        private void DataUpdateForBottomPanel(string code)
        {
            //get object by parameter 'code' from service
            countryData = CheckAfterGetQuery(code);
            //use data for binding source
            bsCountry.DataSource = countryData;
            //define binding for source of the 'Uk'
            ukCountry = countryData.CountryTr.FirstOrDefault(rec => rec.Lang == Langs[0]);
            if (ukCountry == null)
            {
                ukCountry = new CountryTr { Lang = Langs[0] };
                countryData.CountryTr.Add(ukCountry);
            }
            bsUkLangCountry.DataSource = ukCountry;
            //defined binding for source of the 'Ru'
            ruCountry = countryData.CountryTr.FirstOrDefault(rec => rec.Lang == Langs[1]);
            if (ruCountry == null)
            {
                ruCountry = new CountryTr { Lang = Langs[1] };
                countryData.CountryTr.Add(ruCountry);
            }
            bsRuLangCountry.DataSource = ruCountry;
            //define binding for source of the 'En'
            enCountry = countryData.CountryTr.FirstOrDefault(rec => rec.Lang == Langs[2]);
            if (enCountry == null)
            {
                enCountry = new CountryTr { Lang = Langs[2] };
                countryData.CountryTr.Add(enCountry);
            }
            bsEnLangCountry.DataSource = enCountry;
        }

        /// <summary>
        /// check after 'Get' query
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Country CheckAfterGetQuery(string code)
        {
            Country result = new Country();
            GetDictionaryLayer dictDelegat = new GetDictionaryLayer(_layer.GetDictionary);
            if (int.TryParse(code, out int codeInt))
                result = dictDelegat(codeInt);
            else
                result = null;
            if (result == null)
            {
                MessageShow.ShowError(GetMess);
                result = CountryList[0];
            }
            return result;
        }

        //select item into drop down list for filtered data into grid
        private void ddlCommonKeyData_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            //stop perform if position is not exist
            if (e.Position == -1) return;
            //get index current data
            string valueDropList = ddlFilterCountry.Items[e.Position].Value.ToString();
            gridCountryList.DataSource = FilterDataIntoGrid(valueDropList);
        }

        //filter data for grid
        private List<ViewTable> FilterDataIntoGrid(string code)
        {
            //get all data of the 'CopyrightKind'
            CountryList = _connector.GetList<Country>(urlDataGrid);
            //stop perform if data is not exist
            if (CountryList == null)
            {
                CountryList = new List<Country>();
                return new List<ViewTable>();
            }
            //filter data by parameter 'code'
            CountryList = CountryList.Where(item => item.ContinentCode == Convert.ToInt32(code)).ToList();
            List<ViewTable> filteredData = new List<ViewTable>();
            //initialize data for grid
            foreach (var item in CountryList)
            {
                filteredData.Add(new ViewTable
                {
                    Code = item.CountryCode,
                    NameUk = item.CountryTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.CountryName ?? item.CountryName,
                    NameRu = item.CountryTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.CountryName,
                    NameEn = item.CountryTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.CountryName,
                    Continent = ddlFilterCountry.SelectedItem.Text,
                });
            }
            return filteredData;
        }

        //action 'Add' new object to grid
        private void btnAddComplex_Click(object sender, EventArgs e)
        {
            //change property 'Text' of the btnSave for 'Add' action
            btnSaveCountry.Text = "Додати";
            //refresh data for field 'Code'
            countryData = new Country { ContinentCode = 1 };
            bsCountry.DataSource = countryData;
            //refresh data for field 'Uk'
            ukCountry = new CountryTr { Lang = Langs[0] };
            countryData.CountryTr.Add(ukCountry);
            bsUkLangCountry.DataSource = ukCountry;
            //refresh data for field 'Ru'
            ruCountry = new CountryTr { Lang = Langs[1] };
            countryData.CountryTr.Add(ruCountry);
            bsRuLangCountry.DataSource = ruCountry;
            //refresh data for field 'Ru'
            enCountry = new CountryTr { Lang = Langs[2] };
            countryData.CountryTr.Add(enCountry);
            bsEnLangCountry.DataSource = enCountry;
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //action 'Delete' exist object from grid
        private void btnDeleteComplex_Click(object sender, EventArgs e)
        {
            //return if element by index is not exist
            if (!(CountryList.Count > indexSelectedElem))
                return;

            //delete selected item of grid by field 'Code'
            Country item = CountryList[indexSelectedElem];
            bool result = CheckAfterDelete(item.CountryCode);
            //update data for grid
            gridCountryList.DataSource = HandleDataForGrid(urlDataGrid);
            //step up in grid position
            if (result) --indexSelectedElem;
            if (indexSelectedElem < 0) indexSelectedElem = 0;
            gridCountryList.CurrentRow = gridCountryList.Rows[indexSelectedElem];
            //define new currentRow
            DataUpdateForBottomPanel(
                CountryList[indexSelectedElem].CountryCode
                );
        }

        /// <summary>
        /// control delete state
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckAfterDelete(string code)
        {
            bool result = false;
            DeleteLayer dictDelegat = new DeleteLayer(_layer.Delete);
            if (int.TryParse(code, out int codeInt))
                result = dictDelegat(codeInt);
            else
                result = false;
            if (!result)
                MessageShow.ShowError(DelMess);
            return result;
        }

        //action 'Save' object to grid
        private void btnSave_Click(object sender, EventArgs e)
        {
            string codeIAfter = "";
            if (btnSaveCountry.Text == "Додати")
            {
                //send created object
                Country result = CheckAfterCreate(countryData);
                codeIAfter = result.CountryCode;
                //update data for grid
                gridCountryList.DataSource = HandleDataForGrid(urlDataGrid);
                indexSelectedElem = CountryList.FindIndex(elem => elem.CountryCode == result.CountryCode);
                //select certain item into grid
                gridCountryList.CurrentRow = gridCountryList.Rows[indexSelectedElem];
                //set default state for 'Text' of the btnSave
                btnSaveCountry.Text = "Зберегти";
            } else if (btnSaveCountry.Text == "Зберегти")
            {
                Country result = CheckAfterUpdate(countryData);
                codeIAfter = result.CountryCode;
                //save last selected item
                indexSelectedElem = gridCountryList.CurrentRow.Index;
                //update data for grid
                gridCountryList.DataSource = HandleDataForGrid(urlDataGrid);
                //select certain item into grid
                gridCountryList.CurrentRow = gridCountryList.Rows[indexSelectedElem];
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
        private Country CheckAfterCreate(Country createItem)
        {
            CreateLayer dictDelegate = new CreateLayer(_layer.Create);
            Country result = dictDelegate(createItem);
            if (result == null)
            {
                MessageShow.ShowError(CreateMess);
                result = CountryList[0];
            }
            return result;
        }

        /// <summary>
        /// check result data after 'Update'
        /// </summary>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private Country CheckAfterUpdate(Country updateItem)
        {
            UpdateLayer dictDelegat = new UpdateLayer(_layer.Update);
            Country result = dictDelegat(updateItem);
            if (result == null)
            {
                MessageShow.ShowError(UpdateMess);
                result = CountryList[0];
            }
            return result;
        }

        //action 'Cancel' reset changed data for selected item
        private void btnCancel_Click(object sender, EventArgs e)
        {
            string codeICurrent = CountryList[indexSelectedElem].CountryCode;
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
        private void txtNameUaCountry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveCountry.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCountry.Text = "Зберегти";
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
        private void txtNameRuCountry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveCountry.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCountry.Text = "Зберегти";
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
        private void txtNameEnCountry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveCountry.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCountry.Text = "Зберегти";
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
        private void ddlEditKeyCountry_Click(object sender, EventArgs e)
        {
            if (btnSaveCountry.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveCountry.Text = "Зберегти";
            }
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //allow input only number data
        private void txtCodeCountry_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if it 'number' or 'Backspace' -> allow input
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (btnSaveCountry.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveCountry.Text = "Зберегти";
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

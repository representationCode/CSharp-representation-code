using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebAPIConnector;
using ipsFormsDesign.Model;
using Telerik.WinControls.UI;
using ipsFormsDesign.Services;
using IpsViewComponentLibrary.Extensions;
using sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers;

namespace sysAdminwp.ComplexDictionariesBaseControls
{
    public partial class BaseSystemConstantControl : UserControl
    {
        SystemConstantLayer _layer = new SystemConstantLayer();
        public delegate SystemConstant GetDictionaryLayer(int id);
        public delegate SystemConstant CreateLayer(SystemConstant sors);
        public delegate SystemConstant UpdateLayer(SystemConstant sors);
        public delegate bool DeleteLayer(int id);
        private readonly List<string> Langs = new List<string>
        {
            "Uk", "Ru", "En"
        };

        private const string DelMess = "Об'єкт не видалений!";
        private const string GetMess = "об'єкт не знайдено";
        private const string CreateMess = "Помилка формату";
        private const string UpdateMess = "Помилка формату";

        private readonly DictionaryService _dictService = DictionaryService.GetInstance();
        private readonly ApiConnector _connector;

        //define data for different languages
        private SystemConstantTr ukSystemConstant;
        private SystemConstantTr ruSystemConstant;
        private SystemConstantTr enSystemConstant;

        //define grid data
        private List<SystemConstant> _systemConstantList;
        public List<SystemConstant> SystemConstantList
        {
            get { return _systemConstantList; }
            set
            {
                if (value != null)
                {
                    _systemConstantList = value;
                    totalSystemConstant.Text = _systemConstantList.Count.ToString();
                }
            }
        }
        //item data
        private SystemConstant systemConstantData;
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
                txtSearchSystemConstant.Enabled = value;
                //grid element
                gridSystemConstantList.Enabled = value;
                //'Add' button
                btnAddSystemConstant.Enabled = value;
                //'Delete' button
                btnDeleteSystemConstant.Enabled = value;
                //'Save' button (bottom panel)
                btnSaveSystemConstant.Enabled = !value;
                //'Cancel' button (bottom panel)
                btnCancelSystemConstant.Enabled = !value;
            }
        }
        public BaseSystemConstantControl(ApiConnector connector, string urlForComplexDict)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            _connector = connector;
            urlDataGrid = urlForComplexDict;
            //initialize data for grid
            gridSystemConstantList.DataSource = HandleDataForGrid(urlForComplexDict);
            //setup search posibility for textBox element
            txtSearchSystemConstant.SetUsageGrid(gridSystemConstantList);
            //set default data for bottom panel
            if (SystemConstantList.Count != 0)
                DataUpdateForBottomPanel(SystemConstantList[0].CodeI);
            ControlEnabledActiveElements = true;
        }
        
        //handle data for grid
        private List<ViewTable> HandleDataForGrid(string urlForGetDataDictionaries)
        {
            //check on valid state parameter
            if (urlForGetDataDictionaries == null) return new List<ViewTable>();
            //get data dictionaries -> for grid
            SystemConstantList = _connector.GetList<SystemConstant>(urlForGetDataDictionaries);
            if (SystemConstantList == null)
            {
                SystemConstantList = new List<SystemConstant>();
                return new List<ViewTable>();
            }

            List<ViewTable> dataGrid = new List<ViewTable>();
            //initialize grid data
            foreach (var item in SystemConstantList)
            {
                dataGrid.Add(new ViewTable
                {
                    Code = item.CodeI.ToString(),
                    NameUk = item.SystemConstantTr?.SingleOrDefault(i => i?.Lang == Langs[0])?.Name ?? item.Name,
                    NameRu = item.SystemConstantTr?.SingleOrDefault(i => i?.Lang == Langs[1])?.Name,
                    NameEn = item.SystemConstantTr?.SingleOrDefault(i => i?.Lang == Langs[2])?.Name,
                });
            }
            return dataGrid;
        }

        //show data for bottom panel
        private void gvCopyrightKindList_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (!(gridSystemConstantList.CurrentRow.DataBoundItem is ViewTable currentRow))
                return;

            if (!int.TryParse(currentRow.Code, out int code))
                return;
            indexSelectedElem = SystemConstantList.FindIndex(el => el.CodeI == code);
            DataUpdateForBottomPanel(code);
        }

        //refresh changed data for bottom panel
        private void DataUpdateForBottomPanel(int codeI)
        {
            //get object by parameter 'code' from service
            systemConstantData = CheckAfterGetQuery(codeI);
            //use data for binding source
            bsSystemConstant.DataSource = systemConstantData;
            //define binding for source of the 'Uk'
            ukSystemConstant = systemConstantData.SystemConstantTr.FirstOrDefault(rec => rec.Lang == Langs[0]);
            if (ukSystemConstant == null)
            {
                ukSystemConstant = new SystemConstantTr { Lang = Langs[0] };
                systemConstantData.SystemConstantTr.Add(ukSystemConstant);
            }
            bsUkLangSystemConstant.DataSource = ukSystemConstant;
            //defined binding for source of the 'Ru'
            ruSystemConstant = systemConstantData.SystemConstantTr.FirstOrDefault(rec => rec.Lang == Langs[1]);
            if (ruSystemConstant == null)
            {
                ruSystemConstant = new SystemConstantTr { Lang = Langs[1] };
                systemConstantData.SystemConstantTr.Add(ruSystemConstant);
            }
            bsRuLangSystemConstant.DataSource = ruSystemConstant;
            //define binding for source of the 'En'
            enSystemConstant = systemConstantData.SystemConstantTr.FirstOrDefault(rec => rec.Lang == Langs[2]);
            if (enSystemConstant == null)
            {
                enSystemConstant = new SystemConstantTr { Lang = Langs[2] };
                systemConstantData.SystemConstantTr.Add(enSystemConstant);
            }
            bsEnLangSystemConstant.DataSource = enSystemConstant;
        }

        /// <summary>
        /// check after 'Get' query
        /// </summary>
        /// <param name="codeI"></param>
        /// <returns></returns>
        private SystemConstant CheckAfterGetQuery(int codeI)
        {
            GetDictionaryLayer dictDelegat = new GetDictionaryLayer(_layer.GetDictionary);
            SystemConstant result = dictDelegat(codeI);
            if (result == null)
            {
                MessageShow.ShowError(GetMess);
                //set select first element of grid
                result = SystemConstantList[0];
            }
            return result;
        }

        //action 'Add' new object to grid
        private void btnAddComplex_Click(object sender, EventArgs e)
        {
            //change property 'Text' of the btnSave for 'Add' action
            btnSaveSystemConstant.Text = "Додати";
            //refresh data for field 'Code'
            systemConstantData = new SystemConstant();
            bsSystemConstant.DataSource = systemConstantData;
            //refresh data for field 'Uk'
            ukSystemConstant = new SystemConstantTr { Lang = Langs[0] };
            systemConstantData.SystemConstantTr.Add(ukSystemConstant);
            bsUkLangSystemConstant.DataSource = ukSystemConstant;
            //refresh data for field 'Ru'
            ruSystemConstant = new SystemConstantTr { Lang = Langs[1] };
            systemConstantData.SystemConstantTr.Add(ruSystemConstant);
            bsRuLangSystemConstant.DataSource = ruSystemConstant;
            //refresh data for field 'Ru'
            enSystemConstant = new SystemConstantTr { Lang = Langs[2] };
            systemConstantData.SystemConstantTr.Add(enSystemConstant);
            bsEnLangSystemConstant.DataSource = enSystemConstant;
            //stop perform if elements have 'Enabled' -> false
            if (!ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = false;
        }

        //action 'Delete' exist object from grid
        private void btnDeleteComplex_Click(object sender, EventArgs e)
        {
            //return if element by index is not exist
            if (!(SystemConstantList.Count > indexSelectedElem))
                return;
            //delete selected item of grid by field 'Code'
            SystemConstant item = SystemConstantList[indexSelectedElem];
            bool result = CheckAfterDelete(item.CodeI);
            //update data for grid
            gridSystemConstantList.DataSource = HandleDataForGrid(urlDataGrid);
            //step up in grid position
            if (result) --indexSelectedElem;
            if (indexSelectedElem < 0) indexSelectedElem = 0;
            gridSystemConstantList.CurrentRow = gridSystemConstantList.Rows[indexSelectedElem];
            //define new currentRow
            DataUpdateForBottomPanel(
                SystemConstantList[indexSelectedElem].CodeI
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
            if (btnSaveSystemConstant.Text == "Додати")
            {
                //send created object
                SystemConstant result = CheckAfterCreate(systemConstantData);
                codeIAfter = result.CodeI;
                //update data for grid
                gridSystemConstantList.DataSource = HandleDataForGrid(urlDataGrid);
                indexSelectedElem = _systemConstantList.FindIndex(el => el.CodeI == result.CodeI);
                //select certain item into grid
                gridSystemConstantList.CurrentRow = gridSystemConstantList.Rows[indexSelectedElem];
                btnSaveSystemConstant.Text = "Зберегти";
            } else if (btnSaveSystemConstant.Text == "Зберегти")
            {
                //save changes
                SystemConstant result = CheckAfterUpdate(systemConstantData);
                codeIAfter = result.CodeI;
                //save last selected item
                indexSelectedElem = gridSystemConstantList.CurrentRow.Index;
                //update data for grid
                gridSystemConstantList.DataSource = HandleDataForGrid(urlDataGrid);
                //select certain item into grid
                gridSystemConstantList.CurrentRow = gridSystemConstantList.Rows[indexSelectedElem];
            }
            DataUpdateForBottomPanel(codeIAfter);
            //stop perform if elements have 'Enabled' -> true
            if (ControlEnabledActiveElements) return;
            //change 'Enabled' state for elements
            ControlEnabledActiveElements = true;
        }

        /// <summary>
        /// check result data after 'Craete'
        /// </summary>
        /// <param name="createItem"></param>
        /// <returns></returns>
        private SystemConstant CheckAfterCreate(SystemConstant createItem)
        {
            CreateLayer dictDelegat = new CreateLayer(_layer.Create);
            SystemConstant result = dictDelegat(createItem);
            if (result == null)
            {
                MessageShow.ShowError(CreateMess);
                //select first element of grid by default
                result = SystemConstantList[0];
            }
            return result;
        }

        /// <summary>
        /// check result data after 'Update'
        /// </summary>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private SystemConstant CheckAfterUpdate(SystemConstant updateItem)
        {
            UpdateLayer dictDelegat = new UpdateLayer(_layer.Update);
            SystemConstant result = dictDelegat(systemConstantData);
            if (result == null)
            {
                MessageBox.Show(UpdateMess);
                //select first element of grid by default
                result = SystemConstantList[0];
            }
            return result;
        }

        //action 'Cancel' reset changed data for selected item
        private void btnCancel_Click(object sender, EventArgs e)
        {
            int codeICurrent = SystemConstantList[indexSelectedElem].CodeI;
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
        private void txtUkLangSystemConstant_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveSystemConstant.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveSystemConstant.Text = "Зберегти";
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
        private void txtRuLangSystemConstant_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveSystemConstant.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveSystemConstant.Text = "Зберегти";
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
        private void txtEnLangSystemConstant_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnSaveSystemConstant.Text != "Додати")
            {
                //change property 'Text' of the btnSave for 'Save' action
                btnSaveSystemConstant.Text = "Зберегти";
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
        private void dsCopyrightKind_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate)
            {
                if (btnSaveSystemConstant.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveSystemConstant.Text = "Зберегти";
                }
                //stop perform if elements have 'Enabled' -> false
                if (!ControlEnabledActiveElements) return;
                //change 'Enabled' state for elements
                ControlEnabledActiveElements = false;
            }
        }

        //allow input only number data
        private void txtCodeSystemConstant_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if it 'number' or 'Backspace' -> allow input
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                if (btnSaveSystemConstant.Text != "Додати")
                {
                    //change property 'Text' of the btnSave for 'Save' action
                    btnSaveSystemConstant.Text = "Зберегти";
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

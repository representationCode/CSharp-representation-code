using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define behaviour for BaseSystemConstantControl
    /// </summary>
    class SystemConstantLayer : IDictionaryActions<SystemConstant>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();

        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseSystemConstantControl(connector, url);
        }

        //get dictionary by id
        public SystemConstant GetDictionary(int id)
        {
            return _dictService.GetSystemConstant(id);
        }

        //create new dictionary
        public SystemConstant Create(SystemConstant sors)
        {
            return _dictService.CreateSystemConstant(sors);
        }

        //update current dictionary
        public SystemConstant Update(SystemConstant sors)
        {
            return _dictService.UpdateSystemConstant(sors);
        }

        //delete dictionary by id
        public bool Delete(int id)
        {
            return _dictService.DeleteSystemConstant(id);
        }
    }
}

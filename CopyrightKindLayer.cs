using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define behaviour for BaseCopyrightKindControl
    /// </summary>
    class CopyrightKindLayer : IDictionaryActions<CopyrightKind>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();

        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseCopyrightKindControl(connector, url);
        }

        //get dictionary by id
        public CopyrightKind GetDictionary(int id)
        {
            return _dictService.GetCopyrightKind(id);
        }

        //create new dictionary
        public CopyrightKind Create(CopyrightKind sors)
        {
            return _dictService.CreateCopyrightKind(sors);
        }

        //update current dictionary
        public CopyrightKind Update(CopyrightKind sors)
        {
            return _dictService.UpdateCopyrightKind(sors);
        }

        //delete dictionary by id
        public bool Delete(int id)
        {
            return _dictService.DeleteCopyrightKind(id);
        }
    }
}

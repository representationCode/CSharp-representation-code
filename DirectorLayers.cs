using ipsFormsDesign.Model;
using sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries
{
    /// <summary>
    /// initialize current control
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class DirectorLayers<T>
    {
        /// <summary>
        /// define current type and initialize appropriate control
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public UserControl InitCommon(ApiConnector connector, string url)
        {
            UserControl control = new UserControl();
            if (typeof(T) == typeof(CopyrightKind))
                control = new CopyrightKindLayer().InitControl(connector, url);
            else if (typeof(T) == typeof(Country))
                control = new CountryLayer().InitControl(connector, url);
            else if (typeof(T) == typeof(CreativeRoleType))
                control = new CreativeroleTypeLayer().InitControl(connector, url);
            else if (typeof(T) == typeof(GenreType))
                control = new GenreTypeLayer().InitControl(connector, url);
            else if (typeof(T) == typeof(KlR030))
                control = new Klr030Layer().InitControl(connector, url);
            else if (typeof(T) == typeof(SystemConstant))
                control = new SystemConstantLayer().InitControl(connector, url);
            else if (typeof(T) == typeof(TypePlace))
                control = new TypePlaceLayer().InitControl(connector, url);
            
            return control;
        }
    }
}

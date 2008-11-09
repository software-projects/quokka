using Quokka.Util;

namespace Quokka.WinForms
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using Microsoft.Win32;

	/// <summary>
	/// Allows convenient persistant storage of display settings, such as
	/// form sizes and positions.
	/// </summary>
	/// <remarks>
	/// There are many published solutions to this problem, and many of them
	/// are significantly more sophisticated than this one. In particular, this
	/// implementation makes use of the Windows Registry, which might change
	/// in a future version.
	/// </remarks>
	public class DisplaySettings
	{
        private const string DisplaySettingsKeyName = "DisplaySettings";

        private readonly string _name;
        private RegistryKey _key;
        private Form _form;

		static DisplaySettings()
		{
			// TODO: This initialisation is done for backward compatibility with existing
			// applications that make use of DisplaySettings, but its use is not ideal.
			// The application should probably set this.

			if (String.IsNullOrEmpty(RegistryUtil.CompanyName))
			{
				RegistryUtil.CompanyName = Application.CompanyName;
			}

			if (String.IsNullOrEmpty(RegistryUtil.ProductName))
			{
				RegistryUtil.ProductName = Application.ProductName;
			}

			// Don't use the full ProductVersion here, because it includes the build
			// number and the revision number. Usually we do not want the display settings
			// being changed with every build. This code uses the major version number only.
			RegistryUtil.MajorVersion = (Application.ProductVersion ?? "0").Split('.')[0].Trim();
		}

		[Obsolete("Use RegistryUtil instead")]
		public static string CompanyName
		{
			get { return RegistryUtil.CompanyName; }
			set { RegistryUtil.CompanyName = value; }
		}

		[Obsolete("Use RegistryUtil instead")]
		public static string ProductName
		{
			get { return RegistryUtil.ProductName; }
			set { RegistryUtil.ProductName = value; }
		}

		[Obsolete("Use RegistryUtil instead")]
		public static string MajorVersion
		{
			get { return RegistryUtil.MajorVersion; }
			set { RegistryUtil.MajorVersion = value; }
		}

		/// <summary>
		/// Deletes the entire display settings tree for all forms. Useful for testing.
		/// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void DeleteAll() {
        	string keyPath = BuildRegistryKeyPath(null);
        	Registry.CurrentUser.DeleteSubKeyTree(keyPath);
        }

        #region Construction, disposal

        public DisplaySettings(string name) {
        	name = (name ?? String.Empty).Trim();
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}

			string keyPath = BuildRegistryKeyPath(name);
            _key = Registry.CurrentUser.CreateSubKey(keyPath);
        	_name = name;
        }

        public DisplaySettings(Form form, string name) : this(name) {

            if (form == null)
                throw new ArgumentNullException();

            _form = form;
            _form.Load += new EventHandler(form_Load);
            _form.FormClosed += new FormClosedEventHandler(form_FormClosed);
        }

		public DisplaySettings(Form form) : this(form, form.Name) {}

        public void Dispose() {
            _key.Close();
            _key = null;
        }

        private void CheckDisposed() {
            if (_key == null) {
                throw new ObjectDisposedException(_name);
            }
        }

        #endregion


        private void form_FormClosed(object sender, FormClosedEventArgs e) {
            SavePosition(_form);
            Dispose();
        }

        private void form_Load(object sender, EventArgs e) {
            LoadPosition(_form);
        }

        public void LoadPosition(Form form) {
            CheckDisposed();

            object windowStateObject = _key.GetValue("WindowState");
            object xObject = _key.GetValue("DesktopBounds.X");
            object yObject = _key.GetValue("DesktopBounds.Y");
            object widthObject = _key.GetValue("DesktopBounds.Width");
            object heightObject = _key.GetValue("DesktopBounds.Height");

            Point location;
            Size size;

            if (GetLocation(xObject, yObject, out location) && GetSize(widthObject, heightObject, out size)) {
                form.DesktopBounds = new Rectangle(location, size);
            }

            if (windowStateObject != null) {
                form.WindowState = (FormWindowState)(windowStateObject);
            }
        }

        private static bool GetLocation(object x, object y, out Point location) {
            try {
                if (x != null && y != null) {
                    location = new Point((int)x, (int)y);

                    // check that the location fits on one of the available screens
                    foreach (Screen screen in Screen.AllScreens) {
                        if (screen.Bounds.Contains(location)) {
                            return true;
                        }
                    }
                }
            }
            catch (InvalidCastException) { }

            location = new Point();
            return false;
        }

        private static bool GetSize(object width, object height, out Size size) {
            try {
                if (width != null && height != null) {
                    size = new Size((int)width, (int)height);
                    return true;
                }
            }
            catch (InvalidCastException) { }

            size = new Size();
            return false;
        }

		private static string BuildRegistryKeyPath(string formName)
		{
			return RegistryUtil.SubKeyPath(DisplaySettingsKeyName, formName);
		}

        public void SavePosition(Form form) {
            CheckDisposed();

            _key.SetValue("WindowState", (int)form.WindowState);
            _key.SetValue("DesktopBounds.Y", form.DesktopBounds.Y);
            _key.SetValue("DesktopBounds.X", form.DesktopBounds.X);
            _key.SetValue("DesktopBounds.Width", form.DesktopBounds.Width);
            _key.SetValue("DesktopBounds.Height", form.DesktopBounds.Height);
        }

        public string GetString(string valueName, string defaultValue) {
			CheckDisposed();
			return (string)_key.GetValue(valueName, defaultValue);
        }

        public void SetString(string valueName, string value) {
			CheckDisposed();
			_key.SetValue(valueName, value);
        }

        public void SetInt(string valueName, int value) {
			CheckDisposed();
			SetString(valueName, value.ToString());
        }

        public int GetInt(string valueName, int defaultValue) {
			CheckDisposed();
			string s = GetString(valueName, null);
            if (s == null) {
                return defaultValue;
            }

            int value;
            if (!Int32.TryParse(s, out value)) {
            	value = defaultValue;
            }
            return value;
        }

		public void Remove(string valueName)
		{
			CheckDisposed();
			_key.DeleteValue(valueName, false);
		}
	}
}

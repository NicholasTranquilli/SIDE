using AvaloniaEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SIDE.Helpers
{
    public struct ThemeData
    {
        public IHighlightingDefinition highlighting;
    }

    public class ThemeImporter
    {
        private ThemeData myThemeData;

        private ThemeData ParseThemeData(XmlReader fileData)
        {
            return new ThemeData();
        }

        public ThemeImporter(string filePath) 
        {
            // Load custom format
            using var reader = XmlReader.Create(filePath);

            // Parse custom format into Theme Data via ParseThemeData
            myThemeData = ParseThemeData(reader);
        }

        public ThemeData GetThemeData()
        {
            return this.myThemeData;
        }
    }
}

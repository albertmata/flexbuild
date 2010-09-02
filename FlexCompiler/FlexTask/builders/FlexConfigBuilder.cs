using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Resources;

using BuildTask.Flex.utils;

namespace BuildTask.Flex.builders
{
    public class FlexConfigBuilder
    {
        public void BuildConfigFile(string pathToXmlConfigFile, FlexPropertiesBase flexLibProp,  
            ActionScriptProperties actionScriptProperties, SwfMetaData metadata, LicenseProperties license,
            bool debug, bool enableWarnings, string outputFile)
        {
            using (XmlTextWriter writer = new XmlTextWriter(pathToXmlConfigFile, Encoding.ASCII))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                writer.WriteStartElement("flex-config");

                //Output file
                writer.WriteElementString("output", FlexUtil.NormalizePath(outputFile));

                //Classes
                WriteClasses(writer, flexLibProp);

                //Include files and css
                WriteIncludeFiles(writer, flexLibProp);

                //Compiler Options
                WriteCompilerOptions(writer, actionScriptProperties, debug);

                //Target player
                writer.WriteElementString("target-player", actionScriptProperties.PlayerVersion);
                writer.WriteElementString("warnings", enableWarnings.ToString().ToLowerInvariant());

                //Metadata
                WriteMetadata(writer, metadata);

                //License
                WriteLicense(writer, license.ProductName, license.SerialNumber);

                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

        private void WriteCompilerOptions(XmlTextWriter writer, ActionScriptProperties actionScriptProperties, bool debug)
        {
            writer.WriteStartElement("compiler");
            writer.WriteElementString("debug", debug.ToString().ToLowerInvariant());
            writer.WriteElementString("optimize", (!debug).ToString().ToLowerInvariant());
            WriteCompilerSourcePathElements(writer, actionScriptProperties);
            WriteLibraryPaths(writer, "library-path", actionScriptProperties.LinkedLibraryPathEntries);
            WriteLibraryPaths(writer, "external-library-path", actionScriptProperties.ExternalLibraryPathEntries);
            writer.WriteEndElement();
        }

        private void WriteLibraryPaths(XmlTextWriter writer, string tag, string[] arr)
        {
            if (null != arr && arr.Length > 0)
            {
                writer.WriteStartElement(tag);
                WritePathElements(writer, arr);
                writer.WriteEndElement();
            }
        }

        private void WriteCompilerSourcePathElements(XmlTextWriter writer, ActionScriptProperties actionScriptProperties)
        {
            writer.WriteStartElement("source-path");
            writer.WriteElementString("path-element", FlexUtil.NormalizePath(actionScriptProperties.SourceFolderPath));
            WritePathElements(writer, actionScriptProperties.CompilerSourcePathEntries);
            writer.WriteEndElement();
        }

        private void WritePathElements(XmlTextWriter writer, string[] arr)
        {
            if (arr.Length > 0)
            {
                foreach (string path in arr)
                {
                    writer.WriteElementString("path-element", FlexUtil.NormalizePath(path));
                }
            }
        }

        private void WriteMetadata(XmlTextWriter writer, SwfMetaData metadata)
        {
            writer.WriteStartElement("metadata");
            writer.WriteElementString("title", metadata.title);
            writer.WriteElementString("description", metadata.description);
            writer.WriteElementString("publisher", metadata.publisher);
            writer.WriteElementString("creator", metadata.creator);
            writer.WriteElementString("date", metadata.date);
            writer.WriteEndElement();
        }

        private void WriteIncludeFiles(XmlTextWriter writer, FlexPropertiesBase flexLibProp)
        {
            if (flexLibProp.IncludeResources.Length > 0)
            {
                foreach (ProjectResource res in flexLibProp.IncludeResources)
                {
                    writer.WriteStartElement("include-file");
                    writer.WriteElementString("name", res.DestPath);
                    writer.WriteElementString("path", FlexUtil.NormalizePath(res.SourcePath));
                    writer.WriteEndElement();
                }
            }
        }

        private void WriteClasses(XmlTextWriter writer, FlexPropertiesBase flexLibProp)
        {
            if (flexLibProp.IncludeClasses.Length > 0)
            {
                writer.WriteStartElement("include-classes");
                foreach (string clss in flexLibProp.IncludeClasses)
                {
                    writer.WriteElementString("class", clss);
                }
                writer.WriteEndElement();
            }
        }

        private void WriteLicense(XmlTextWriter writer, string product, string serial)
        {
            writer.WriteStartElement("licenses");
            writer.WriteStartElement("license");
            writer.WriteElementString("product", product);
            writer.WriteElementString("serial-number", serial);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
